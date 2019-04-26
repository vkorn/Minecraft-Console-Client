namespace MinecraftClient.Protocol.Packets.Outbound.KeepAlive
{
    internal class KeepAliveOut17W46A : KeepAliveOut17W45A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W46A;
        protected override int PacketId => 0x0B;
    }
}