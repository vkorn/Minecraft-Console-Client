using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.Packets.Outbound.HeldItemChange;

namespace MinecraftClient.Protocol.Packets.Outbound.CloseWindow
{
    internal class CloseWindowOut1144 : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC1144;
        protected override int PacketId => 0x0A;
        protected override OutboundTypes PackageType => OutboundTypes.CloseWindow;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            return new[] {((CloseWindowRequest) data).WindowId};
        }
    }
}