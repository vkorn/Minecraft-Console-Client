namespace MinecraftClient.Protocol.Packets.Inbound.KeepAlive
{
    internal class KeepAliveHandler112Pre5 : KeepAliveHandler17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC112Pre5;

        protected override int PacketId => 0x1F;
    }
}