using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.OpenWindow
{
    internal class OpenWindowHandler1144 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC1144;

        protected override int PacketId => 0x2E;

        protected override InboundTypes PackageType => InboundTypes.OpenWindow;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var windowId = PacketUtils.readNextVarInt(packetData);
            var windowTypeId = PacketUtils.readNextVarInt(packetData);
            var type = handler.GetPlayer().RegistryProcessor.GetWindowType((byte) windowTypeId);

            handler.GetPlayer().WindowOpened((byte) windowId, type);
            return null;
        }
    }
}