using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.UpdateHealth
{
    internal class UpdateHealthHandler114Pre5 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x48;
        protected override InboundTypes PackageType => InboundTypes.UpdateHealth;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            handler.GetPlayer().Health = PacketUtils.readNextFloat(packetData);
            handler.GetPlayer().Food = PacketUtils.readNextVarInt(packetData);

            return null;
        }
    }
}