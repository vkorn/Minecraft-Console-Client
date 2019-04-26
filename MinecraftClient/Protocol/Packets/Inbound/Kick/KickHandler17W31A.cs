namespace MinecraftClient.Protocol.Packets.Inbound.Kick
{
    internal class KickHandler17W31A : KickHandler112Pre5
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W31A;
        protected override int PacketId => 0x1B;
    }
}