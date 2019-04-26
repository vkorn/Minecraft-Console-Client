namespace MinecraftClient.Protocol.Packets.Inbound.KeepAlive
{
    internal class KeepAliveHandler114Pre5 : KeepAliveHandler18W01A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;

        protected override int PacketId => 0x20;
    }
}