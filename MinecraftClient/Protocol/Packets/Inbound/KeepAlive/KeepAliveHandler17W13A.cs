namespace MinecraftClient.Protocol.Packets.Inbound.KeepAlive
{
    internal class KeepAliveHandler17W13A : KeepAliveHandler19
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W13A;

        protected override int PacketId => 0x20;
    }
}