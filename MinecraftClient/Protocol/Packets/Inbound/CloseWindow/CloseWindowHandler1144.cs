using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.CloseWindow
{
    internal class CloseWindowHandler1144 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC1144;
        protected override int PacketId => 0x13;
        protected override InboundTypes PackageType => InboundTypes.CloseWindow;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var windowId = PacketUtils.readNextByte(packetData);
            handler.GetPlayer().WindowClosed(windowId);

            return null;
        }
    }
}