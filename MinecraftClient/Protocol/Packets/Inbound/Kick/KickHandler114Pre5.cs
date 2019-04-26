namespace MinecraftClient.Protocol.Packets.Inbound.Kick
{
    internal class KickHandler114Pre5 : KickHandler17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x1A;
    }
}