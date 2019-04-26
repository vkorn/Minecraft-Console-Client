namespace MinecraftClient.Protocol.Packets.Outbound.KeepAlive
{
    internal class KeepAliveOut19 : KeepAliveOut
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x0B;
    }
}