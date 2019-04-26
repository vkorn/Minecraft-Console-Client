namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPosition
{
    internal class PlayerPositionOut17W46A : PlayerPositionOut17W45A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W46A;
        protected override int PacketId => 0x0D;
    }
}