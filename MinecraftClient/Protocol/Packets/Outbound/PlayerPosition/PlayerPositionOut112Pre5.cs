namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPosition
{
    internal class PlayerPositionOut112Pre5 : PlayerPositionOut17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC112Pre5;
        protected override int PacketId => 0x0E;
    }
}