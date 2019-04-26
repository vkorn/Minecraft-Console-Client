namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPosition
{
    internal class PlayerPositionOut17W13A : PlayerPositionOut19
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W13A;
        protected override int PacketId => 0x0D;
    }
}