using System.Collections.Generic;

namespace MinecraftClient.Protocol.Packets.Outbound.ConfirmTransaction
{
    internal class ConfirmTransactionOut114Pre5 : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x07;
        protected override OutboundTypes PackageType => OutboundTypes.ConfirmTransaction;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            return packetData;
        }
    }
}