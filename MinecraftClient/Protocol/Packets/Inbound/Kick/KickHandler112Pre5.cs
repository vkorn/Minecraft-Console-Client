namespace MinecraftClient.Protocol.Packets.Inbound.Kick
{
    internal class KickHandler112Pre5 : KickHandler17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC112Pre5;
        protected override int PacketId => 0x1A;
    }
}