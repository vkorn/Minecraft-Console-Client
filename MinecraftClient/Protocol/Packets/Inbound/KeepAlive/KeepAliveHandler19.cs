namespace MinecraftClient.Protocol.Packets.Inbound.KeepAlive
{
    internal class KeepAliveHandler19 : KeepAliveHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;

        protected override int PacketId => 0x1F;
    }
}