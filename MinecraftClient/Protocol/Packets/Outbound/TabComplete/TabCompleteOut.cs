using System.Collections.Generic;

namespace MinecraftClient.Protocol.Packets.Outbound.TabComplete
{
    internal class TabCompleteOut : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.Zero;
        protected override int PacketId => 0x14;
        protected override OutboundTypes PackageType => OutboundTypes.TabComplete;
        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            // We need to implement logic here, but I don't think it's used right now.
            return packetData;
        }
    }
}