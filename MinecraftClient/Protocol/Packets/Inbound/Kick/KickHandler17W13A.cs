namespace MinecraftClient.Protocol.Packets.Inbound.Kick
{
    internal class KickHandler17W13A : KickHandler19
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W13A;
        protected override int PacketId => 0x1A;
    }
}