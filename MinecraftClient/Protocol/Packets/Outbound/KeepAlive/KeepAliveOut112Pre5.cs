namespace MinecraftClient.Protocol.Packets.Outbound.KeepAlive
{
    internal class KeepAliveOut112Pre5 : KeepAliveOut17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC112Pre5;
        protected override int PacketId => 0x0B;
    }
}