using System.Collections.Generic;
using MinecraftClient.Mapping;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.SpawnMob
{
    internal class SpawnMobHandler : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x03;
        protected override InboundTypes PackageType => InboundTypes.SpawnMob;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var id = PacketUtils.readNextVarInt(packetData);
            var uuid = PacketUtils.readNextUUID(packetData);
            var type = PacketUtils.readNextVarInt(packetData);
            var x = PacketUtils.readNextDouble(packetData);
            var y = PacketUtils.readNextDouble(packetData);
            var z = PacketUtils.readNextDouble(packetData);
            // Skipping rest of the packets as it's not essential

            var entity = handler.GetPlayer().RegistryProcessor.GetEntity(type, id, uuid, new Location(x, y, z));
            handler.GetPlayer().Radar.Spawn(entity);

            return null;
        }
    }
}