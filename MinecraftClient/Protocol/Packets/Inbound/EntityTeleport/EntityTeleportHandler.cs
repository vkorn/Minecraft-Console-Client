using System.Collections.Generic;
using MinecraftClient.Mapping;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.EntityTeleport
{
    internal class EntityTeleportHandler : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x56;
        protected override InboundTypes PackageType => InboundTypes.EntityTeleport;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var id = PacketUtils.readNextVarInt(packetData);
            var x = PacketUtils.readNextDouble(packetData);
            var y = PacketUtils.readNextDouble(packetData);
            var z = PacketUtils.readNextDouble(packetData);

            handler.GetPlayer().Radar.UpdatePosition(id, new Location(x, y, z), false);
            return null;
        }
    }
}