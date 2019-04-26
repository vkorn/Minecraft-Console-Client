namespace MinecraftClient.Protocol.Packets.Outbound.KeepAlive
{
    internal class KeepAliveOut113Pre7 : KeepAliveOut113Pre4
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC113Pre7;
        protected override int PacketId => 0x0E;
    }
}