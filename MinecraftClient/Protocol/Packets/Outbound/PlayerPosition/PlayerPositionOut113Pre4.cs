namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPosition
{
    internal class PlayerPositionOut113Pre4 : PlayerPositionOut17W46A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC113Pre4;
        protected override int PacketId => 0x0E;
    }
}