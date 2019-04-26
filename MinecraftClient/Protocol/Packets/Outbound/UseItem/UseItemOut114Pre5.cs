using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.UseItem
{
    internal class UseItemOut114Pre5 : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x2D;
        protected override OutboundTypes PackageType => OutboundTypes.UseItem;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            // TODO: Do we want to support offhand?
            // 0: main hand, 1: off hand

            return PacketUtils.getVarInt(0);
        }
    }
}