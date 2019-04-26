namespace MinecraftClient.Protocol.Packets.Inbound.Kick
{
    internal class KickHandler19 : KickHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x1A;
    }
}