using System.Collections.Generic;
using MinecraftClient.Mapping;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.EntityRelativeMove
{
    internal class EntityRelativeMoveHandler : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x28;
        protected override InboundTypes PackageType => InboundTypes.EntityRelativeMove;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var id = PacketUtils.readNextVarInt(packetData);
            var x = PacketUtils.readNextShort(packetData);
            var y = PacketUtils.readNextShort(packetData);
            var z = PacketUtils.readNextShort(packetData);

            handler.GetPlayer().Radar.UpdatePosition(id, new Location(x, y, z), true);
            return null;
        }
    }
}