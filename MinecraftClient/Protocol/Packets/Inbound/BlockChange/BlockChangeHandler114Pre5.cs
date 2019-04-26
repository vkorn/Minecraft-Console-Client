using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.WorldProcessors.DataConverters;

namespace MinecraftClient.Protocol.Packets.Inbound.BlockChange
{
    internal class BlockChangeHandler114Pre5 : BlockChangeHandler17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x0B;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            if (!Settings.TerrainAndMovements)
            {
                return null;
            }

            var location = (long) PacketUtils.readNextULong(packetData);
            var blockId = (short) PacketUtils.readNextVarInt(packetData);

            handler.GetWorld().SetBlock(DataHelpers.Instance.LocationConverter.LongToLocation(location),
                handler.GetWorld().BlockProcessor.CreateBlock(blockId));
            return null;
        }
    }
}