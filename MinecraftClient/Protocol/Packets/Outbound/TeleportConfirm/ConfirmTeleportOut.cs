using System.Collections.Generic;

namespace MinecraftClient.Protocol.Packets.Outbound.TeleportConfirm
{
    internal class TeleportConfirmOut : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x00;
        protected override OutboundTypes PackageType => OutboundTypes.TeleportConfirm;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            return packetData;
        }
    }
}