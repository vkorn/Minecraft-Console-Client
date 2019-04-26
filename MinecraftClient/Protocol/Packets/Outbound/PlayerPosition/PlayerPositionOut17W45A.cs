namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPosition
{
    internal class PlayerPositionOut17W45A : PlayerPositionOut17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W45A;
        protected override int PacketId => 0x0C;
    }
}