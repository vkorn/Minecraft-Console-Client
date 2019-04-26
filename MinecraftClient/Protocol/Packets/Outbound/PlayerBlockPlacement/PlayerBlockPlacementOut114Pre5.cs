using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.WorldProcessors.DataConverters;

namespace MinecraftClient.Protocol.Packets.Outbound.PlayerBlockPlacement
{
    internal class PlayerBlockPlacementOut114Pre5 : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x2C;
        protected override OutboundTypes PackageType => OutboundTypes.PlayerBlockPlacement;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            return PacketUtils.concatBytes(
                PacketUtils.getVarInt((int) ((PlayerBlockPlacementRequest) data).Hand),
                PacketUtils.getLong(
                    DataHelpers.Instance.LocationConverter.LocationToLong(((PlayerBlockPlacementRequest) data)
                        .Location)),
                PacketUtils.getVarInt((int) ((PlayerBlockPlacementRequest) data).FaceVector),
                PacketUtils.getFloat((float) ((PlayerBlockPlacementRequest) data).CursorPosition.X),
                PacketUtils.getFloat((float) ((PlayerBlockPlacementRequest) data).CursorPosition.Y),
                PacketUtils.getFloat((float) ((PlayerBlockPlacementRequest) data).CursorPosition.Z),
                PacketUtils.getBool(((PlayerBlockPlacementRequest) data).IsInsideBlock)
            );
        }
    }
}