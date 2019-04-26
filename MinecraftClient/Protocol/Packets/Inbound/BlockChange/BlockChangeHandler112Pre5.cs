namespace MinecraftClient.Protocol.Packets.Inbound.BlockChange
{
    internal class BlockChangeHandler112Pre5 : BlockChangeHandler17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC112Pre5;
        protected override int PacketId => 0x10;
    }
}