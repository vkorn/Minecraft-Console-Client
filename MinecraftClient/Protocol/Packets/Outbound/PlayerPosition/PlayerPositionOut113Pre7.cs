namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPosition
{
    internal class PlayerPositionOut113Pre7 : PlayerPositionOut113Pre4
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC113Pre7;
        protected override int PacketId => 0x10;
    }
}