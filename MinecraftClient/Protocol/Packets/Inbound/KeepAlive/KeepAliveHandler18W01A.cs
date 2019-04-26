namespace MinecraftClient.Protocol.Packets.Inbound.KeepAlive
{
    internal class KeepAliveHandler18W01A : KeepAliveHandler17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18W01A;

        protected override int PacketId => 0x21;
    }
}