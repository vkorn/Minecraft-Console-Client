namespace MinecraftClient.Protocol.Packets.Inbound.MultiBlockChange
{
    internal class MultiBlockChangeHandler112Pre5 : MultiBlockChangeHandler17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC112Pre5;
        protected override int PacketId => 0x10;
    }
}