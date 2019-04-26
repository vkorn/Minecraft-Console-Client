namespace MinecraftClient.Protocol.Packets.Inbound.MultiBlockChange
{
    internal class MultiBlockChangeHandler17W31A : MultiBlockChangeHandler112Pre5
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W31A;
        protected override int PacketId => 0x0F;
    }
}