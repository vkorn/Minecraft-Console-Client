using System.Collections.Generic;
using MinecraftClient.Mapping;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.UseEntity
{
    internal struct UseEntityRequest : IOutboundRequest
    {
        public int EntityId { get; set; }
        public UseEntityType Type { get; set; }
        public Hands Hand { get; set; }
        public Location UseAt { get; set; }
    }

    internal enum UseEntityType
    {
        Interact = 0,
        Attack = 1,
        InteractAt = 2,
    }

    internal class UseEntityOut : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x0E;
        protected override OutboundTypes PackageType => OutboundTypes.UseEntity;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            return PacketUtils.concatBytes(
                PacketUtils.getVarInt(((UseEntityRequest) data).EntityId),
                PacketUtils.getVarInt((int) ((UseEntityRequest) data).Type),
                ((UseEntityRequest) data).Type == UseEntityType.InteractAt
                    ? PacketUtils.concatBytes(
                        PacketUtils.getFloat((float) ((UseEntityRequest) data).UseAt.X),
                        PacketUtils.getFloat((float) ((UseEntityRequest) data).UseAt.Y),
                        PacketUtils.getFloat((float) ((UseEntityRequest) data).UseAt.Z),
                        PacketUtils.getVarInt((int) ((UseEntityRequest) data).Hand)
                    )
                    : new byte[0]
            );
        }
    }
}