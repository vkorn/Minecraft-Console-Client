using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using MinecraftClient.Crypto;
using MinecraftClient.Proxy;
using System.Security.Cryptography;
using MinecraftClient.Protocol.Handlers.Forge;
using MinecraftClient.Mapping;
using MinecraftClient.Protocol.Packets;
using MinecraftClient.Protocol.Packets.Inbound;
using MinecraftClient.Protocol.Packets.Inbound.ChunkData;
using MinecraftClient.Protocol.Packets.Inbound.JoinGame;
using MinecraftClient.Protocol.Packets.Outbound;
using MinecraftClient.Protocol.Packets.Outbound.ChatMessage;
using MinecraftClient.Protocol.Packets.Outbound.ClientSettings;
using MinecraftClient.Protocol.Packets.Outbound.HeldItemChange;
using MinecraftClient.Protocol.Packets.Outbound.PlayerPosition;
using MinecraftClient.Protocol.Packets.Outbound.PlayerPositionAndLook;
using MinecraftClient.Protocol.Packets.Outbound.PluginMessage;
using MinecraftClient.Protocol.WorldProcessors.ChunkProcessors;
using MinecraftClient.Protocol.WorldProcessors.DataConverters;

namespace MinecraftClient.Protocol.Handlers
{
    /// <summary>
    /// Implementation for Minecraft 1.7.X+ Protocols
    /// </summary>
    class Protocol18Handler : IMinecraftCom, IProtocol
    {
        private int compression_treshold = 0;
        private bool autocomplete_received = false;
        private int autocomplete_transaction_id = 0;
        private readonly List<string> autocomplete_result = new List<string>();
        private bool login_phase = true;
        private bool encrypted = false;
        private int protocolversion;

        // Server forge info -- may be null.
        private ForgeInfo forgeInfo;
        private FMLHandshakeClientState fmlHandshakeState = FMLHandshakeClientState.START;

        IMinecraftComHandler handler;
        Thread netRead;
        private Thread _chunkProcessing;
        private ConcurrentQueue<List<byte>> _chunkData;
        IAesStream s;
        TcpClient c;

        int currentDimension;

        private Dictionary<int, IInboundGamePacketHandler> _inboundHandlers;
        private Dictionary<OutboundTypes, IOutboundGamePacket> _outboundPackets;
        private IChunkProcessor _chunkProcessor;

        public Protocol18Handler(TcpClient client, int protocolVersion, IMinecraftComHandler handler,
            ForgeInfo forgeInfo)
        {
            ConsoleIO.SetAutoCompleteEngine(this);
            ChatParser.InitTranslations();
            c = client;
            protocolversion = protocolVersion;
            this.handler = handler;
            this.forgeInfo = forgeInfo;
            initHandlers();
        }

        private void initHandlers()
        {
            _inboundHandlers = VersionsFactory.InboundHandlers(protocolversion);
            ConsoleIO.WriteLineFormatted("Loaded inbound handlers:");
            foreach (var inboundGamePacketHandler in _inboundHandlers)
            {
                ConsoleIO.WriteLine($"Type: {inboundGamePacketHandler.Value.Type()}    " +
                                    $"Implementation: {inboundGamePacketHandler.Value.GetType().Name}    " +
                                    $"Packet: 0x{inboundGamePacketHandler.Key:X2}");
            }

            _outboundPackets = VersionsFactory.OutboundHandlers(protocolversion);

            ConsoleIO.WriteLineFormatted("Loaded outbound packets:");
            foreach (var outboundPacket in _outboundPackets)
            {
                ConsoleIO.WriteLine($"Type: {outboundPacket.Value.Type()}    " +
                                    $"Implementation: {outboundPacket.Value.GetType().Name}    " +
                                    $"Packet: 0x{outboundPacket.Value.PacketId():X2}");
            }

            _chunkProcessor = VersionsFactory.WorldProcessor<IChunkProcessor>(protocolversion);
            ConsoleIO.WriteLineFormatted("Loaded Chunk processor:");
            ConsoleIO.WriteLine($"Version: {_chunkProcessor.MinVersion()}    " +
                                $"Implementation: {_chunkProcessor.GetType().Name}");

            ConsoleIO.WriteLineFormatted("Loaded Block processor:");
            ConsoleIO.WriteLine($"Version: {handler.GetWorld().BlockProcessor.MinVersion()}    " +
                                $"Implementation: {handler.GetWorld().BlockProcessor.GetType().Name}");

            DataHelpers.Instance.Init(protocolversion);
        }

        private Protocol18Handler(TcpClient client)
        {
            c = client;
        }

        private void ProcessChunksData()
        {
            IInboundGamePacketHandler chunk = _inboundHandlers.First(x => x.Value.Type() == InboundTypes.ChunkData)
                .Value;

            while (true)
            {
                Thread.Sleep(1);
                if (_chunkData.TryDequeue(out var p))
                {
                    var data = chunk.Handle(this, handler, p);
                    if (null == data)
                        break;
                    _chunkProcessor.Process(handler, (ChunkDataResult) data);
                }
            }
        }


        /// <summary>
        /// Separate thread. Network reading loop.
        /// </summary>
        private void Updater()
        {
            try
            {
                do
                {
                    Thread.Sleep(100);
                } while (Update());
            }
            catch (System.IO.IOException)
            {
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }

            handler.OnConnectionLost(ChatBot.DisconnectReason.ConnectionLost, "");
        }

        /// <summary>
        /// Read data from the network. Should be called on a separate thread.
        /// </summary>
        /// <returns>FALSE if an error occured, TRUE otherwise.</returns>
        private bool Update()
        {
            handler.OnUpdate();
            if (c.Client == null || !c.Connected)
            {
                return false;
            }

            try
            {
                while (c.Client.Available > 0)
                {
                    int packetID = 0;
                    List<byte> packetData = new List<byte>();
                    readNextPacket(ref packetID, packetData);

                    try
                    {
                        HandleIncomingPacket(packetID, new List<byte>(packetData));
                    }
                    catch (Exception e)
                    {
                        ConsoleIO.WriteLineFormatted(
                            $"Failed to process packet 0x{packetID:X2}: {e.Message}");
                        ConsoleIO.WriteLine(e.ToString());
                        throw;
                    }
                }
            }
            catch (SocketException)
            {
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read the next packet from the network
        /// </summary>
        /// <param name="packetID">will contain packet ID</param>
        /// <param name="packetData">will contain raw packet Data</param>
        private void readNextPacket(ref int packetID, List<byte> packetData)
        {
            packetData.Clear();
            int size = readNextVarIntRAW(); //Packet size
            packetData.AddRange(readDataRAW(size)); //Packet contents

            //Handle packet decompression
            if (protocolversion >= (int) ProtocolVersions.MC18
                && compression_treshold > 0)
            {
                int sizeUncompressed = PacketUtils.readNextVarInt(packetData);
                if (sizeUncompressed != 0) // != 0 means compressed, let's decompress
                {
                    byte[] toDecompress = packetData.ToArray();
                    byte[] uncompressed = ZlibUtils.Decompress(toDecompress, sizeUncompressed);
                    packetData.Clear();
                    packetData.AddRange(uncompressed);
                }
            }

            packetID = PacketUtils.readNextVarInt(packetData); //Packet ID
        }

        public int Dimension()
        {
            return currentDimension;
        }

        public bool SendPacketOut(OutboundTypes type, IEnumerable<byte> packetData, IOutboundRequest data)
        {
            if (!_outboundPackets.TryGetValue(type, out var packet))
            {
                ConsoleIO.WriteLineFormatted($"Packet {type.ToString()} is not supported in this version");
                return false;
            }

            if (null == packetData)
            {
                packetData = new byte[] {0};
            }

            try
            {
                SendPacket(packet.PacketId(), packet.TransformData(packetData, data));
            }
            catch (SocketException)
            {
                return false;
            }
            catch (System.IO.IOException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handle the given packet
        /// </summary>
        /// <param name="packetId">Packet ID</param>
        /// <param name="packetData">Packet contents</param>
        /// <returns>TRUE if the packet was processed, FALSE if ignored or unknown</returns>
        private bool HandleIncomingPacket(int packetId, List<byte> packetData)
        {
            if (login_phase)
            {
                switch (packetId) //Packet IDs are different while logging in
                {
                    case 0x03:
                        if (protocolversion >= (int) ProtocolVersions.MC18)
                            compression_treshold = PacketUtils.readNextVarInt(packetData);
                        break;
                    default:
                        return false; //Ignored packet
                }

                return true;
            }

            if (!_inboundHandlers.TryGetValue(packetId, out var pHandler))
            {
                return false;
            }

            if (pHandler.Type() == InboundTypes.ChunkData)
            {
                _chunkData.Enqueue(packetData);
                return true;
            }

            var data = pHandler.Handle(this, handler, packetData);
            switch (pHandler.Type())
            {
                case InboundTypes.JoinGame:
                {
                    currentDimension = ((JoinGameResult) data).Dimension;
                }
                    break;
            }

            return true;
        }


//        /// <summary>
//        /// Handle the given packet
//        /// </summary>
//        /// <param name="packetID">Packet ID</param>
//        /// <param name="packetData">Packet contents</param>
//        /// <returns>TRUE if the packet was processed, FALSE if ignored or unknown</returns>
//        private bool handlePacket(int packetID, List<byte> packetData)
//        {
//            if (login_phase)
//            {
//                switch (packetID) //Packet IDs are different while logging in
//                {
//                    case 0x03:
//                        if (protocolversion >= PacketUtils.MC18Version)
//                            compression_treshold = PacketUtils.readNextVarInt(packetData);
//                        break;
//                    default:
//                        return false; //Ignored packet
//                }
//            }
//
//            // Regular in-game packets
//            switch (getPacketIncomingType(packetID, protocolversion))
//            {
//                case PacketIncomingType.KeepAlive:
//                    SendPacket(PacketOutgoingType.KeepAlive, packetData);
//                    break;
//                case PacketIncomingType.JoinGame:
////                    var h = PacketFactory<IJoinGameHandler>.Instantiate(protocolversion);
////                    var res = h.Handle(handler, packetData);
////                    currentDimension = res.Dimension;
//                    break;
//                case PacketIncomingType.ChatMessage:
//                    string message = PacketUtils.readNextString(packetData);
//                    try
//                    {
//                        //Hide system messages or xp bar messages?
//                        byte messageType = PacketUtils.readNextByte(packetData);
//                        if ((messageType == 1 && !Settings.DisplaySystemMessages)
//                            || (messageType == 2 && !Settings.DisplayXPBarMessages))
//                            break;
//                    }
//                    catch (ArgumentOutOfRangeException)
//                    {
//                        /* No message type */
//                    }
//
//                    handler.OnTextReceived(message, true);
//                    break;
//                case PacketIncomingType.Respawn:
//                    this.currentDimension = PacketUtils.readNextInt(packetData);
//                    PacketUtils.readNextByte(packetData);
//                    PacketUtils.readNextByte(packetData);
//                    PacketUtils.readNextString(packetData);
//                    break;
//                case PacketIncomingType.PlayerPositionAndLook:
//                    if (Settings.TerrainAndMovements)
//                    {
//                        double x = PacketUtils.readNextDouble(packetData);
//                        double y = PacketUtils.readNextDouble(packetData);
//                        double z = PacketUtils.readNextDouble(packetData);
//                        float yaw = PacketUtils.readNextFloat(packetData);
//                        float pitch = PacketUtils.readNextFloat(packetData);
//                        byte locMask = PacketUtils.readNextByte(packetData);
//
//                        if (protocolversion >= PacketUtils.MC18Version)
//                        {
//                            Location location = handler.GetCurrentLocation();
//                            location.X = (locMask & 1 << 0) != 0 ? location.X + x : x;
//                            location.Y = (locMask & 1 << 1) != 0 ? location.Y + y : y;
//                            location.Z = (locMask & 1 << 2) != 0 ? location.Z + z : z;
//                            handler.UpdateLocation(location, yaw, pitch);
//                        }
//                        else handler.UpdateLocation(new Location(x, y, z), yaw, pitch);
//                    }
//
//                    if (protocolversion >= PacketUtils.MC19Version)
//                    {
//                        int teleportID = PacketUtils.readNextVarInt(packetData);
//                        // Teleport confirm packet
//                        SendPacket(PacketOutgoingType.TeleportConfirm, PacketUtils.getVarInt(teleportID));
//                    }
//
//                    break;
//                case PacketIncomingType.ChunkData:
//                    if (Settings.TerrainAndMovements)
//                    {
//                        int chunkX = PacketUtils.readNextInt(packetData);
//                        int chunkZ = PacketUtils.readNextInt(packetData);
//                        bool chunksContinuous = PacketUtils.readNextBool(packetData);
//                        ushort chunkMask = protocolversion >= PacketUtils.MC19Version
//                            ? (ushort) PacketUtils.readNextVarInt(packetData)
//                            : PacketUtils.readNextUShort(packetData);
//                        if (protocolversion < PacketUtils.MC18Version)
//                        {
//                            ushort addBitmap = PacketUtils.readNextUShort(packetData);
//                            int compressedDataSize = PacketUtils.readNextInt(packetData);
//                            byte[] compressed = PacketUtils.readData(compressedDataSize, packetData);
//                            byte[] decompressed = ZlibUtils.Decompress(compressed);
//                            ProcessChunkColumnData(chunkX, chunkZ, chunkMask, addBitmap, currentDimension == 0,
//                                chunksContinuous, new List<byte>(decompressed));
//                        }
//                        else
//                        {
//                            int dataSize = PacketUtils.readNextVarInt(packetData);
//                            ProcessChunkColumnData(chunkX, chunkZ, chunkMask, 0, false, chunksContinuous, packetData);
//                        }
//                    }
//
//                    break;
//                case PacketIncomingType.MultiBlockChange:
//                    if (Settings.TerrainAndMovements)
//                    {
//                        int chunkX = PacketUtils.readNextInt(packetData);
//                        int chunkZ = PacketUtils.readNextInt(packetData);
//                        int recordCount = protocolversion < PacketUtils.MC18Version
//                            ? (int) PacketUtils.readNextShort(packetData)
//                            : PacketUtils.readNextVarInt(packetData);
//
//                        for (int i = 0; i < recordCount; i++)
//                        {
//                            byte locationXZ;
//                            ushort blockIdMeta;
//                            int blockY;
//
//                            if (protocolversion < PacketUtils.MC18Version)
//                            {
//                                blockIdMeta = PacketUtils.readNextUShort(packetData);
//                                blockY = (ushort) PacketUtils.readNextByte(packetData);
//                                locationXZ = PacketUtils.readNextByte(packetData);
//                            }
//                            else
//                            {
//                                locationXZ = PacketUtils.readNextByte(packetData);
//                                blockY = (ushort) PacketUtils.readNextByte(packetData);
//                                blockIdMeta = (ushort) PacketUtils.readNextVarInt(packetData);
//                            }
//
//                            int blockX = locationXZ >> 4;
//                            int blockZ = locationXZ & 0x0F;
//                            Block block = new Block(blockIdMeta);
//                            handler.GetWorld().SetBlock(new Location(chunkX, chunkZ, blockX, blockY, blockZ), block);
//                        }
//                    }
//
//                    break;
//                case PacketIncomingType.BlockChange:
//                    if (Settings.TerrainAndMovements)
//                        if (protocolversion < PacketUtils.MC18Version)
//                        {
//                            int blockX = PacketUtils.readNextInt(packetData);
//                            int blockY = PacketUtils.readNextByte(packetData);
//                            int blockZ = PacketUtils.readNextInt(packetData);
//                            short blockId = (short) PacketUtils.readNextVarInt(packetData);
//                            byte blockMeta = PacketUtils.readNextByte(packetData);
//                            handler.GetWorld().SetBlock(new Location(blockX, blockY, blockZ),
//                                new Block(blockId, blockMeta));
//                        }
////                        else
////                            handler.GetWorld().SetBlock(Location.FromLong(PacketUtils.readNextULong(packetData)),
////                                new Block((ushort) PacketUtils.readNextVarInt(packetData)));
//
//                    break;
//                case PacketIncomingType.MapChunkBulk:
//                    if (protocolversion < PacketUtils.MC19Version && Settings.TerrainAndMovements)
//                    {
//                        int chunkCount;
//                        bool hasSkyLight;
//                        List<byte> chunkData = packetData;
//
//                        //Read global fields
//                        if (protocolversion < PacketUtils.MC18Version)
//                        {
//                            chunkCount = PacketUtils.readNextShort(packetData);
//                            int compressedDataSize = PacketUtils.readNextInt(packetData);
//                            hasSkyLight = PacketUtils.readNextBool(packetData);
//                            byte[] compressed = PacketUtils.readData(compressedDataSize, packetData);
//                            byte[] decompressed = ZlibUtils.Decompress(compressed);
//                            chunkData = new List<byte>(decompressed);
//                        }
//                        else
//                        {
//                            hasSkyLight = PacketUtils.readNextBool(packetData);
//                            chunkCount = PacketUtils.readNextVarInt(packetData);
//                        }
//
//                        //Read chunk records
//                        int[] chunkXs = new int[chunkCount];
//                        int[] chunkZs = new int[chunkCount];
//                        ushort[] chunkMasks = new ushort[chunkCount];
//                        ushort[] addBitmaps = new ushort[chunkCount];
//                        for (int chunkColumnNo = 0; chunkColumnNo < chunkCount; chunkColumnNo++)
//                        {
//                            chunkXs[chunkColumnNo] = PacketUtils.readNextInt(packetData);
//                            chunkZs[chunkColumnNo] = PacketUtils.readNextInt(packetData);
//                            chunkMasks[chunkColumnNo] = PacketUtils.readNextUShort(packetData);
//                            addBitmaps[chunkColumnNo] = protocolversion < PacketUtils.MC18Version
//                                ? PacketUtils.readNextUShort(packetData)
//                                : (ushort) 0;
//                        }
//
//                        //Process chunk records
//                        for (int chunkColumnNo = 0; chunkColumnNo < chunkCount; chunkColumnNo++)
//                            ProcessChunkColumnData(chunkXs[chunkColumnNo], chunkZs[chunkColumnNo],
//                                chunkMasks[chunkColumnNo], addBitmaps[chunkColumnNo], hasSkyLight, true, chunkData);
//                    }
//
//                    break;
//                case PacketIncomingType.UnloadChunk:
//                    if (protocolversion >= PacketUtils.MC19Version && Settings.TerrainAndMovements)
//                    {
//                        int chunkX = PacketUtils.readNextInt(packetData);
//                        int chunkZ = PacketUtils.readNextInt(packetData);
//                        handler.GetWorld()[chunkX, chunkZ] = null;
//                    }
//
//                    break;
//                case PacketIncomingType.PlayerListUpdate:
//                    if (protocolversion >= PacketUtils.MC18Version)
//                    {
//                        int action = PacketUtils.readNextVarInt(packetData);
//                        int numActions = PacketUtils.readNextVarInt(packetData);
//                        for (int i = 0; i < numActions; i++)
//                        {
//                            Guid uuid = PacketUtils.readNextUUID(packetData);
//                            switch (action)
//                            {
//                                case 0x00: //Player Join
//                                    string name = PacketUtils.readNextString(packetData);
//                                    int propNum = PacketUtils.readNextVarInt(packetData);
//                                    for (int p = 0; p < propNum; p++)
//                                    {
//                                        string key = PacketUtils.readNextString(packetData);
//                                        string val = PacketUtils.readNextString(packetData);
//                                        if (PacketUtils.readNextBool(packetData))
//                                            PacketUtils.readNextString(packetData);
//                                    }
//
//                                    PacketUtils.readNextVarInt(packetData);
//                                    PacketUtils.readNextVarInt(packetData);
//                                    if (PacketUtils.readNextBool(packetData))
//                                        PacketUtils.readNextString(packetData);
//                                    handler.OnPlayerJoin(uuid, name);
//                                    break;
//                                case 0x01: //Update gamemode
//                                case 0x02: //Update latency
//                                    PacketUtils.readNextVarInt(packetData);
//                                    break;
//                                case 0x03: //Update display name
//                                    if (PacketUtils.readNextBool(packetData))
//                                        PacketUtils.readNextString(packetData);
//                                    break;
//                                case 0x04: //Player Leave
//                                    handler.OnPlayerLeave(uuid);
//                                    break;
//                                default:
//                                    //Unknown player list item type
//                                    break;
//                            }
//                        }
//                    }
//                    else //MC 1.7.X does not provide UUID in tab-list updates
//                    {
//                        string name = PacketUtils.readNextString(packetData);
//                        bool online = PacketUtils.readNextBool(packetData);
//                        short ping = PacketUtils.readNextShort(packetData);
//                        Guid FakeUUID =
//                            new Guid(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(name)).Take(16).ToArray());
//                        if (online)
//                            handler.OnPlayerJoin(FakeUUID, name);
//                        else handler.OnPlayerLeave(FakeUUID);
//                    }
//
//                    break;
//                case PacketIncomingType.TabCompleteResult:
//                    if (protocolversion >= PacketUtils.MC17w46aVersion)
//                    {
//                        autocomplete_transaction_id = PacketUtils.readNextVarInt(packetData);
//                    }
//
//                    if (protocolversion >= PacketUtils.MC17w47aVersion)
//                    {
//                        // Start of the text to replace - currently unused
//                        PacketUtils.readNextVarInt(packetData);
//                    }
//
//                    if (protocolversion >= PacketUtils.MC18w06aVersion)
//                    {
//                        // Length of the text to replace - currently unused
//                        PacketUtils.readNextVarInt(packetData);
//                    }
//
//                    int autocomplete_count = PacketUtils.readNextVarInt(packetData);
//                    autocomplete_result.Clear();
//                    for (int i = 0; i < autocomplete_count; i++)
//                        autocomplete_result.Add(PacketUtils.readNextString(packetData));
//                    autocomplete_received = true;
//
//                    // In protocolversion >= MC18w06aVersion there is additional data if the match is a tooltip
//                    // Don't worry about skipping remaining data since there is no useful for us (yet)
//                    break;
//                case PacketIncomingType.PluginMessage:
//                    String channel = PacketUtils.readNextString(packetData);
//                    if (protocolversion < PacketUtils.MC18Version)
//                    {
//                        if (forgeInfo == null)
//                        {
//                            // 1.7 and lower prefix plugin channel packets with the length.
//                            // We can skip it, though.
//                            PacketUtils.readNextShort(packetData);
//                        }
//                        else
//                        {
//                            // Forge does something even weirder with the length.
//                            PacketUtils.readNextVarShort(packetData);
//                        }
//                    }
//
//                    // The remaining data in the array is the entire payload of the packet.
//                    handler.OnPluginChannelMessage(channel, packetData.ToArray());
//
//                    #region Forge Login
//
//                    if (forgeInfo != null && fmlHandshakeState != FMLHandshakeClientState.DONE)
//                    {
//                        if (channel == "FML|HS")
//                        {
//                            FMLHandshakeDiscriminator discriminator =
//                                (FMLHandshakeDiscriminator) PacketUtils.readNextByte(packetData);
//
//                            if (discriminator == FMLHandshakeDiscriminator.HandshakeReset)
//                            {
//                                fmlHandshakeState = FMLHandshakeClientState.START;
//                                return true;
//                            }
//
//                            switch (fmlHandshakeState)
//                            {
//                                case FMLHandshakeClientState.START:
//                                    if (discriminator != FMLHandshakeDiscriminator.ServerHello)
//                                        return false;
//
//                                    // Send the plugin channel registration.
//                                    // REGISTER is somewhat special in that it doesn't actually include length information,
//                                    // and is also \0-separated.
//                                    // Also, yes, "FML" is there twice.  Don't ask me why, but that's the way forge does it.
//                                    string[] channels = {"FML|HS", "FML", "FML|MP", "FML", "FORGE"};
//                                    SendPluginChannelPacket("REGISTER",
//                                        Encoding.UTF8.GetBytes(string.Join("\0", channels)));
//
//                                    byte fmlProtocolVersion = PacketUtils.readNextByte(packetData);
//
//                                    if (Settings.DebugMessages)
//                                        ConsoleIO.WriteLineFormatted(
//                                            "§8Forge protocol version : " + fmlProtocolVersion);
//
//                                    if (fmlProtocolVersion >= 1)
//                                        this.currentDimension = PacketUtils.readNextInt(packetData);
//
//                                    // Tell the server we're running the same version.
//                                    SendForgeHandshakePacket(FMLHandshakeDiscriminator.ClientHello,
//                                        new byte[] {fmlProtocolVersion});
//
//                                    // Then tell the server that we're running the same mods.
//                                    if (Settings.DebugMessages)
//                                        ConsoleIO.WriteLineFormatted("§8Sending falsified mod list to server...");
//                                    byte[][] mods = new byte[forgeInfo.Mods.Count][];
//                                    for (int i = 0; i < forgeInfo.Mods.Count; i++)
//                                    {
//                                        ForgeInfo.ForgeMod mod = forgeInfo.Mods[i];
//                                        mods[i] = PacketUtils.concatBytes(PacketUtils.getString(mod.ModID),
//                                            PacketUtils.getString(mod.Version));
//                                    }
//
//                                    SendForgeHandshakePacket(FMLHandshakeDiscriminator.ModList,
//                                        PacketUtils.concatBytes(PacketUtils.getVarInt(forgeInfo.Mods.Count),
//                                            PacketUtils.concatBytes(mods)));
//
//                                    fmlHandshakeState = FMLHandshakeClientState.WAITINGSERVERDATA;
//
//                                    return true;
//                                case FMLHandshakeClientState.WAITINGSERVERDATA:
//                                    if (discriminator != FMLHandshakeDiscriminator.ModList)
//                                        return false;
//
//                                    Thread.Sleep(2000);
//
//                                    if (Settings.DebugMessages)
//                                        ConsoleIO.WriteLineFormatted("§8Accepting server mod list...");
//                                    // Tell the server that yes, we are OK with the mods it has
//                                    // even though we don't actually care what mods it has.
//
//                                    SendForgeHandshakePacket(FMLHandshakeDiscriminator.HandshakeAck,
//                                        new byte[] {(byte) FMLHandshakeClientState.WAITINGSERVERDATA});
//
//                                    fmlHandshakeState = FMLHandshakeClientState.WAITINGSERVERCOMPLETE;
//                                    return false;
//                                case FMLHandshakeClientState.WAITINGSERVERCOMPLETE:
//                                    // The server now will tell us a bunch of registry information.
//                                    // We need to read it all, though, until it says that there is no more.
//                                    if (discriminator != FMLHandshakeDiscriminator.RegistryData)
//                                        return false;
//
//                                    if (protocolversion < PacketUtils.MC18Version)
//                                    {
//                                        // 1.7.10 and below have one registry
//                                        // with blocks and items.
//                                        int registrySize = PacketUtils.readNextVarInt(packetData);
//
//                                        if (Settings.DebugMessages)
//                                            ConsoleIO.WriteLineFormatted(
//                                                "§8Received registry with " + registrySize + " entries");
//
//                                        fmlHandshakeState = FMLHandshakeClientState.PENDINGCOMPLETE;
//                                    }
//                                    else
//                                    {
//                                        // 1.8+ has more than one registry.
//
//                                        bool hasNextRegistry = PacketUtils.readNextBool(packetData);
//                                        string registryName = PacketUtils.readNextString(packetData);
//                                        int registrySize = PacketUtils.readNextVarInt(packetData);
//                                        if (Settings.DebugMessages)
//                                            ConsoleIO.WriteLineFormatted(
//                                                "§8Received registry " + registryName + " with " + registrySize +
//                                                " entries");
//                                        if (!hasNextRegistry)
//                                            fmlHandshakeState = FMLHandshakeClientState.PENDINGCOMPLETE;
//                                    }
//
//                                    return false;
//                                case FMLHandshakeClientState.PENDINGCOMPLETE:
//                                    // The server will ask us to accept the registries.
//                                    // Just say yes.
//                                    if (discriminator != FMLHandshakeDiscriminator.HandshakeAck)
//                                        return false;
//                                    if (Settings.DebugMessages)
//                                        ConsoleIO.WriteLineFormatted("§8Accepting server registries...");
//                                    SendForgeHandshakePacket(FMLHandshakeDiscriminator.HandshakeAck,
//                                        new byte[] {(byte) FMLHandshakeClientState.PENDINGCOMPLETE});
//                                    fmlHandshakeState = FMLHandshakeClientState.COMPLETE;
//
//                                    return true;
//                                case FMLHandshakeClientState.COMPLETE:
//                                    // One final "OK".  On the actual forge source, a packet is sent from
//                                    // the client to the client saying that the connection was complete, but
//                                    // we don't need to do that.
//
//                                    SendForgeHandshakePacket(FMLHandshakeDiscriminator.HandshakeAck,
//                                        new byte[] {(byte) FMLHandshakeClientState.COMPLETE});
//                                    if (Settings.DebugMessages)
//                                        ConsoleIO.WriteLine("Forge server connection complete!");
//                                    fmlHandshakeState = FMLHandshakeClientState.DONE;
//                                    return true;
//                            }
//                        }
//                    }
//
//                    #endregion
//
//                    return false;
//                case PacketIncomingType.KickPacket:
//                    handler.OnConnectionLost(ChatBot.DisconnectReason.InGameKick,
//                        ChatParser.ParseText(PacketUtils.readNextString(packetData)));
//                    return false;
//                case PacketIncomingType.NetworkCompressionTreshold:
//                    if (protocolversion >= PacketUtils.MC18Version && protocolversion < PacketUtils.MC19Version)
//                        compression_treshold = PacketUtils.readNextVarInt(packetData);
//                    break;
//                case PacketIncomingType.ResourcePackSend:
//                    string url = PacketUtils.readNextString(packetData);
//                    string hash = PacketUtils.readNextString(packetData);
//                    //Send back "accepted" and "successfully loaded" responses for plugins making use of resource pack mandatory
//                    byte[] responseHeader = new byte[0];
//                    if (protocolversion < PacketUtils.MC110Version
//                    ) //MC 1.10 does not include resource pack hash in responses
//                        responseHeader = PacketUtils.concatBytes(PacketUtils.getVarInt(hash.Length),
//                            Encoding.UTF8.GetBytes(hash));
//                    SendPacket(PacketOutgoingType.ResourcePackStatus,
//                        PacketUtils.concatBytes(responseHeader, PacketUtils.getVarInt(3))); //Accepted pack
//                    SendPacket(PacketOutgoingType.ResourcePackStatus,
//                        PacketUtils.concatBytes(responseHeader, PacketUtils.getVarInt(0))); //Successfully loaded
//                    break;
//                default:
//                    return false; //Ignored packet
//            }
//
//            return true; //Packet processed
//        }


        /// <summary>
        /// Start the updating thread. Should be called after login success.
        /// </summary>
        private void StartUpdating()
        {
            _chunkData = new ConcurrentQueue<List<byte>>();
            _chunkProcessing = new Thread(ProcessChunksData) {Name = "ChunkDataProcessing"};
            _chunkProcessing.Start();

            netRead = new Thread(Updater) {Name = "ProtocolPacketHandler"};
            netRead.Start();
        }

        /// <summary>
        /// Disconnect from the server, cancel network reading.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (netRead != null)
                {
                    netRead.Abort();
                    c.Close();
                }

                _chunkProcessing?.Abort();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Read some data directly from the network
        /// </summary>
        /// <param name="offset">Amount of bytes to read</param>
        /// <returns>The data read from the network as an array</returns>
        private byte[] readDataRAW(int offset)
        {
            if (offset > 0)
            {
                try
                {
                    byte[] cache = new byte[offset];
                    Receive(cache, 0, offset, SocketFlags.None);
                    return cache;
                }
                catch (OutOfMemoryException)
                {
                }
            }

            return new byte[] { };
        }

        /// <summary>
        /// Read an integer from the network
        /// </summary>
        /// <returns>The integer</returns>
        private int readNextVarIntRAW()
        {
            int i = 0;
            int j = 0;
            int k = 0;
            byte[] tmp = new byte[1];
            while (true)
            {
                Receive(tmp, 0, 1, SocketFlags.None);
                k = tmp[0];
                i |= (k & 0x7F) << j++ * 7;
                if (j > 5) throw new OverflowException("VarInt too big");
                if ((k & 0x80) != 128) break;
            }

            return i;
        }

        /// <summary>
        /// C-like atoi function for parsing an int from string
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <returns>Int parsed</returns>
        private static int atoi(string str)
        {
            return int.Parse(new string(str.Trim().TakeWhile(char.IsDigit).ToArray()));
        }

        /// <summary>
        /// Network reading method. Read bytes from the socket or encrypted socket.
        /// </summary>
        private void Receive(byte[] buffer, int start, int offset, SocketFlags f)
        {
            int read = 0;
            while (read < offset)
            {
                if (encrypted)
                {
                    read += s.Read(buffer, start + read, offset - read);
                }
                else read += c.Client.Receive(buffer, start + read, offset - read, f);
            }
        }

        /// <summary>
        /// Send a forge plugin channel packet ("FML|HS").  Compression and encryption will be handled automatically
        /// </summary>
        /// <param name="discriminator">Discriminator to use.</param>
        /// <param name="data">packet Data</param>
        private void SendForgeHandshakePacket(FMLHandshakeDiscriminator discriminator, byte[] data)
        {
            SendPluginChannelPacket("FML|HS", PacketUtils.concatBytes(new[] {(byte) discriminator}, data));
        }

        /// <summary>
        /// Send a packet to the server.  Compression and encryption will be handled automatically.
        /// </summary>
        /// <param name="packetID">packet ID</param>
        /// <param name="packetData">packet Data</param>
        private void SendPacket(int packetID, IEnumerable<byte> packetData)
        {
            //The inner packet
            byte[] the_packet = PacketUtils.concatBytes(PacketUtils.getVarInt(packetID), packetData.ToArray());

            if (compression_treshold > 0) //Compression enabled?
            {
                if (the_packet.Length >= compression_treshold) //Packet long enough for compressing?
                {
                    byte[] compressed_packet = ZlibUtils.Compress(the_packet);
                    the_packet = PacketUtils.concatBytes(PacketUtils.getVarInt(the_packet.Length), compressed_packet);
                }
                else
                {
                    byte[] uncompressed_length = PacketUtils.getVarInt(0); //Not compressed (short packet)
                    the_packet = PacketUtils.concatBytes(uncompressed_length, the_packet);
                }
            }

            SendRAW(PacketUtils.concatBytes(PacketUtils.getVarInt(the_packet.Length), the_packet));
        }

        /// <summary>
        /// Send raw data to the server. Encryption will be handled automatically.
        /// </summary>
        /// <param name="buffer">data to send</param>
        private void SendRAW(byte[] buffer)
        {
            if (encrypted)
            {
                s.Write(buffer, 0, buffer.Length);
            }
            else c.Client.Send(buffer);
        }

        /// <summary>
        /// Do the Minecraft login.
        /// </summary>
        /// <returns>True if login successful</returns>
        public bool Login()
        {
            byte[] protocol_version = PacketUtils.getVarInt(protocolversion);
            string server_address = handler.GetServerHost() + (forgeInfo != null ? "\0FML\0" : "");
            byte[] server_port = BitConverter.GetBytes((ushort) handler.GetServerPort());
            Array.Reverse(server_port);
            byte[] next_state = PacketUtils.getVarInt(2);
            byte[] handshake_packet = PacketUtils.concatBytes(protocol_version, PacketUtils.getString(server_address),
                server_port, next_state);

            SendPacket(0x00, handshake_packet);

            byte[] login_packet = PacketUtils.getString(handler.GetUsername());

            SendPacket(0x00, login_packet);

            int packetID = -1;
            List<byte> packetData = new List<byte>();
            while (true)
            {
                readNextPacket(ref packetID, packetData);
                if (packetID == 0x00) //Login rejected
                {
                    handler.OnConnectionLost(ChatBot.DisconnectReason.LoginRejected,
                        ChatParser.ParseText(PacketUtils.readNextString(packetData)));
                    return false;
                }
                else if (packetID == 0x01) //Encryption request
                {
                    string serverID = PacketUtils.readNextString(packetData);
                    byte[] Serverkey = PacketUtils.readNextByteArray(protocolversion, packetData);
                    byte[] token = PacketUtils.readNextByteArray(protocolversion, packetData);
                    return StartEncryption(handler.GetUserUUID(), handler.GetSessionID(), token, serverID, Serverkey);
                }
                else
                {
                    if (packetID == 0x02) //Login successful
                    {
                        ConsoleIO.WriteLineFormatted("§8Server is in offline mode.");
                        login_phase = false;

                        if (forgeInfo != null)
                        {
                            // Do the forge handshake.
                            if (!CompleteForgeHandshake())
                            {
                                return false;
                            }
                        }

                        StartUpdating();
                        return true; //No need to check session or start encryption
                    }

                    HandleIncomingPacket(packetID, packetData);
                }
            }
        }

        /// <summary>
        /// Completes the Minecraft Forge handshake.
        /// </summary>
        /// <returns>Whether the handshake was successful.</returns>
        private bool CompleteForgeHandshake()
        {
            int packetID = -1;
            List<byte> packetData = new List<byte>();

            while (fmlHandshakeState != FMLHandshakeClientState.DONE)
            {
                readNextPacket(ref packetID, packetData);

                if (packetID == 0x40) // Disconnect
                {
                    handler.OnConnectionLost(ChatBot.DisconnectReason.LoginRejected,
                        ChatParser.ParseText(PacketUtils.readNextString(packetData)));
                    return false;
                }

                HandleIncomingPacket(packetID, packetData);
            }

            return true;
        }

        /// <summary>
        /// Start network encryption. Automatically called by Login() if the server requests encryption.
        /// </summary>
        /// <returns>True if encryption was successful</returns>
        private bool StartEncryption(string uuid, string sessionId, byte[] token, string serverIdHash, byte[] serverKey)
        {
            RSACryptoServiceProvider rsaService =
                CryptoHandler.DecodeRSAPublicKey(serverKey);
            byte[] secretKey = CryptoHandler.GenerateAESPrivateKey();

            if (Settings.DebugMessages)
                ConsoleIO.WriteLineFormatted("§8Crypto keys & hash generated.");

            if (serverIdHash != "-")
            {
                Console.WriteLine("Checking Session...");
                if (!ProtocolHandler.SessionCheck(uuid, sessionId,
                    CryptoHandler.getServerHash(serverIdHash, serverKey, secretKey)))
                {
                    handler.OnConnectionLost(ChatBot.DisconnectReason.LoginRejected, "Failed to check session.");
                    return false;
                }
            }

            //Encrypt the data
            byte[] key_enc = PacketUtils.getArray(protocolversion, rsaService.Encrypt(secretKey, false));
            byte[] token_enc = PacketUtils.getArray(protocolversion, rsaService.Encrypt(token, false));

            //Encryption Response packet
            SendPacket(0x01, PacketUtils.concatBytes(key_enc, token_enc));

            //Start client-side encryption
            s = CryptoHandler.getAesStream(c.GetStream(), secretKey);
            encrypted = true;

            //Process the next packet
            int packetID = -1;
            List<byte> packetData = new List<byte>();
            while (true)
            {
                readNextPacket(ref packetID, packetData);
                if (packetID == 0x00) //Login rejected
                {
                    handler.OnConnectionLost(ChatBot.DisconnectReason.LoginRejected,
                        ChatParser.ParseText(PacketUtils.readNextString(packetData)));
                    return false;
                }
                else
                {
                    if (packetID == 0x02) //Login successful
                    {
                        login_phase = false;

                        if (forgeInfo != null)
                        {
                            // Do the forge handshake.
                            if (!CompleteForgeHandshake())
                            {
                                return false;
                            }
                        }

                        StartUpdating();
                        return true;
                    }

                    HandleIncomingPacket(packetID, packetData);
                }
            }
        }

        /// <summary>
        /// Get max length for chat messages
        /// </summary>
        /// <returns>Max length, in characters</returns>
        public int GetMaxChatMessageLength()
        {
            return protocolversion >= (int) ProtocolVersions.MC111
                ? 256
                : 100;
        }

        /// <summary>
        /// Send a chat message to the server
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>True if properly sent</returns>
        public bool SendChatMessage(string message)
        {
            if (String.IsNullOrEmpty(message))
                return true;

            return SendPacketOut(OutboundTypes.ChatMessage, null,
                new ChatMessageRequest {Message = message});
        }

        /// <summary>
        /// Send a respawn packet to the server
        /// </summary>
        /// <returns>True if properly sent</returns>
        public bool SendRespawnPacket()
        {
            return SendPacketOut(OutboundTypes.ClientStatus, null, null);
        }

        /// <summary>
        /// Tell the server what client is being used to connect to the server
        /// </summary>
        /// <param name="brandInfo">Client string describing the client</param>
        /// <returns>True if brand info was successfully sent</returns>
        public bool SendBrandInfo(string brandInfo)
        {
            if (String.IsNullOrEmpty(brandInfo))
                return false;
            // Plugin channels were significantly changed between Minecraft 1.12 and 1.13
            // https://wiki.vg/index.php?title=Pre-release_protocol&oldid=14132#Plugin_Channels
            if (protocolversion >= (int) ProtocolVersions.MC113)
            {
                return SendPluginChannelPacket("minecraft:brand", PacketUtils.getString(brandInfo));
            }
            else
            {
                return SendPluginChannelPacket("MC|Brand", PacketUtils.getString(brandInfo));
            }
        }

        /// <summary>
        /// Inform the server of the client's Minecraft settings
        /// </summary>
        /// <param name="language">Client language eg en_US</param>
        /// <param name="viewDistance">View distance, in chunks</param>
        /// <param name="difficulty">Game difficulty (client-side...)</param>
        /// <param name="chatMode">Chat mode (allows muting yourself)</param>
        /// <param name="chatColors">Show chat colors</param>
        /// <param name="skinParts">Show skin layers</param>
        /// <param name="mainHand">1.9+ main hand</param>
        /// <returns>True if client settings were successfully sent</returns>
        public bool SendClientSettings(string language, byte viewDistance, byte difficulty, byte chatMode,
            bool chatColors, byte skinParts, byte mainHand)
        {
            var req = new ClientSettingsRequest
            {
                Language = language,
                Difficulty = difficulty,
                ChatMode = chatMode,
                MainHand = mainHand,
                SkinParts = skinParts,
                ChatColors = chatColors,
                ViewDistance = viewDistance,
            };

            return SendPacketOut(OutboundTypes.ClientSettings, null, req);
        }

        /// <summary>
        /// Send a location update to the server
        /// </summary>
        /// <param name="location">The new location of the player</param>
        /// <param name="onGround">True if the player is on the ground</param>
        /// <param name="yaw">Optional new yaw for updating player look</param>
        /// <param name="pitch">Optional new pitch for updating player look</param>
        /// <returns>True if the location update was successfully sent</returns>
        public bool SendLocationUpdate(Location location, bool onGround, float? yaw = null, float? pitch = null)
        {
            if (!Settings.TerrainAndMovements)
            {
                return false;
            }

            if (yaw.HasValue && pitch.HasValue)
            {
                return SendPacketOut(OutboundTypes.PlayerPositionAndLook, null,
                    new PlayerPositionAndLookRequest
                    {
                        Location = location,
                        IsOnGround = onGround,
                        Yaw = yaw.Value,
                        Pitch = pitch.Value,
                    });
            }

            return SendPacketOut(OutboundTypes.PlayerPosition, null,
                new PlayerPositionRequest
                {
                    Location = location,
                    IsOnGround = onGround,
                });
        }

        /// <summary>
        /// Send a plugin channel packet (0x17) to the server, compression and encryption will be handled automatically
        /// </summary>
        /// <param name="channel">Channel to send packet on</param>
        /// <param name="data">packet Data</param>
        public bool SendPluginChannelPacket(string channel, byte[] data)
        {
            return SendPacketOut(OutboundTypes.PluginMessage, data, new PluginMessageRequest {Channel = channel});
        }
        
        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                c.Close();
            }
            catch (SocketException)
            {
            }
            catch (System.IO.IOException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Autocomplete text while typing username or command
        /// </summary>
        /// <param name="BehindCursor">Text behind cursor</param>
        /// <returns>Completed text</returns>
        IEnumerable<string> IAutoComplete.AutoComplete(string BehindCursor)
        {
            if (String.IsNullOrEmpty(BehindCursor))
                return new string[] { };

            byte[] transaction_id = PacketUtils.getVarInt(autocomplete_transaction_id);
            byte[] assume_command = new byte[] {0x00};
            byte[] has_position = new byte[] {0x00};

            byte[] tabcomplete_packet = new byte[] { };

            if (protocolversion >= (int) ProtocolVersions.MC18)
            {
                if (protocolversion >= (int) ProtocolVersions.MC17W46A)
                {
                    tabcomplete_packet = PacketUtils.concatBytes(tabcomplete_packet, transaction_id);
                    tabcomplete_packet =
                        PacketUtils.concatBytes(tabcomplete_packet, PacketUtils.getString(BehindCursor));
                }
                else
                {
                    tabcomplete_packet =
                        PacketUtils.concatBytes(tabcomplete_packet, PacketUtils.getString(BehindCursor));

                    if (protocolversion >= (int) ProtocolVersions.MC19)
                    {
                        tabcomplete_packet = PacketUtils.concatBytes(tabcomplete_packet, assume_command);
                    }

                    tabcomplete_packet = PacketUtils.concatBytes(tabcomplete_packet, has_position);
                }
            }
            else
            {
                tabcomplete_packet = PacketUtils.concatBytes(PacketUtils.getString(BehindCursor));
            }

            autocomplete_received = false;
            autocomplete_result.Clear();
            autocomplete_result.Add(BehindCursor);
            SendPacketOut(OutboundTypes.TabComplete, tabcomplete_packet, null);

            int wait_left = 50; //do not wait more than 5 seconds (50 * 100 ms)
            while (wait_left > 0 && !autocomplete_received)
            {
                System.Threading.Thread.Sleep(100);
                wait_left--;
            }

            if (autocomplete_result.Count > 0)
                ConsoleIO.WriteLineFormatted("§8" + String.Join(" ", autocomplete_result), false);
            return autocomplete_result;
        }

        /// <summary>
        /// Ping a Minecraft server to get information about the server
        /// </summary>
        /// <returns>True if ping was successful</returns>
        public static bool doPing(string host, int port, ref int protocolversion, ref ForgeInfo forgeInfo)
        {
            string version = "";
            TcpClient tcp = ProxyHandler.newTcpClient(host, port);
            tcp.ReceiveBufferSize = 1024 * 1024;

            byte[] packet_id = PacketUtils.getVarInt(0);
            byte[] protocol_version = PacketUtils.getVarInt(-1);
            byte[] server_port = BitConverter.GetBytes((ushort) port);
            Array.Reverse(server_port);
            byte[] next_state = PacketUtils.getVarInt(1);
            byte[] packet = PacketUtils.concatBytes(packet_id, protocol_version, PacketUtils.getString(host),
                server_port, next_state);
            byte[] tosend = PacketUtils.concatBytes(PacketUtils.getVarInt(packet.Length), packet);

            tcp.Client.Send(tosend, SocketFlags.None);

            byte[] status_request = PacketUtils.getVarInt(0);
            byte[] request_packet =
                PacketUtils.concatBytes(PacketUtils.getVarInt(status_request.Length), status_request);

            tcp.Client.Send(request_packet, SocketFlags.None);

            Protocol18Handler ComTmp = new Protocol18Handler(tcp);
            int packetLength = ComTmp.readNextVarIntRAW();
            if (packetLength > 0) //Read Response length
            {
                List<byte> packetData = new List<byte>(ComTmp.readDataRAW(packetLength));
                if (PacketUtils.readNextVarInt(packetData) == 0x00) //Read Packet ID
                {
                    string result = PacketUtils.readNextString(packetData); //Get the Json data

                    if (!String.IsNullOrEmpty(result) && result.StartsWith("{") && result.EndsWith("}"))
                    {
                        Json.JSONData jsonData = Json.ParseJson(result);
                        if (jsonData.Type == Json.JSONData.DataType.Object &&
                            jsonData.Properties.ContainsKey("version"))
                        {
                            Json.JSONData versionData = jsonData.Properties["version"];

                            //Retrieve display name of the Minecraft version
                            if (versionData.Properties.ContainsKey("name"))
                                version = versionData.Properties["name"].StringValue;

                            //Retrieve protocol version number for handling this server
                            if (versionData.Properties.ContainsKey("protocol"))
                                protocolversion = atoi(versionData.Properties["protocol"].StringValue);

                            //Automatic fix for BungeeCord 1.8 reporting itself as 1.7...
                            if (protocolversion < 47 && version.Split(' ', '/').Contains("1.8"))
                                protocolversion = ProtocolHandler.MCVer2ProtocolVersion("1.8.0");

                            // Check for forge on the server.
                            if (jsonData.Properties.ContainsKey("modinfo") &&
                                jsonData.Properties["modinfo"].Type == Json.JSONData.DataType.Object)
                            {
                                Json.JSONData modData = jsonData.Properties["modinfo"];
                                if (modData.Properties.ContainsKey("type") &&
                                    modData.Properties["type"].StringValue == "FML")
                                {
                                    forgeInfo = new ForgeInfo(modData);

                                    if (forgeInfo.Mods.Any())
                                    {
                                        if (Settings.DebugMessages)
                                        {
                                            ConsoleIO.WriteLineFormatted("§8Server is running Forge. Mod list:");
                                            foreach (ForgeInfo.ForgeMod mod in forgeInfo.Mods)
                                            {
                                                ConsoleIO.WriteLineFormatted("§8  " + mod.ToString());
                                            }
                                        }
                                        else ConsoleIO.WriteLineFormatted("§8Server is running Forge.");
                                    }
                                    else forgeInfo = null;
                                }
                            }

                            ConsoleIO.WriteLineFormatted("§8Server version : " + version + " (protocol v" +
                                                         protocolversion +
                                                         (forgeInfo != null ? ", with Forge)." : ")."));

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}