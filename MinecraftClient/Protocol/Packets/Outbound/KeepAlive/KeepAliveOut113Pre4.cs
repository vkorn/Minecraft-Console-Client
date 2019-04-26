namespace MinecraftClient.Protocol.Packets.Outbound.KeepAlive
{
    internal class KeepAliveOut113Pre4 : KeepAliveOut17W46A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC113Pre4;
        protected override int PacketId => 0x0C;
    }
}