using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.EntityStatus
{
    internal class EntityStatusHandler : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x1B;
        protected override InboundTypes PackageType => InboundTypes.EntityStatus;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            // We are interested only in death 
            var id = PacketUtils.readNextInt(packetData);
            var status = PacketUtils.readNextByte(packetData);
            if (3 == status)
            {
                handler.GetPlayer().Radar.Remove(id);
            }

            return null;
        }
    }
}