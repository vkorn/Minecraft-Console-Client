using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.HeldItemChange
{
    internal class HeldItemChangeOut114Pre5 : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x23;
        protected override OutboundTypes PackageType => OutboundTypes.HeldItemChange;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            return PacketUtils.getShort((short) (((HeldItemChangeRequest) data).SlotNum - 1));
        }
    }
}