using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.WorldProcessors.DataConverters;

namespace MinecraftClient.Protocol.Packets.Inbound.BlockChange
{
    internal class BlockChangeHandler18 : BlockChangeHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            if (!Settings.TerrainAndMovements)
            {
                return null;
            }

            var val = (long) PacketUtils.readNextULong(packetData);
            var blockIdMeta = (ushort) PacketUtils.readNextVarInt(packetData);

            handler.GetWorld().SetBlock(DataHelpers.Instance.LocationConverter.LongToLocation(val),
                handler.GetWorld().BlockProcessor.CreateBlockFromIdMetadata(blockIdMeta));
            return null;
        }
    }
}