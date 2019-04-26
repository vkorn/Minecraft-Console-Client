namespace MinecraftClient.Protocol.Packets.Inbound.Respawn
{
    internal class RespawnHandler19 : RespawnHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x33;
    }
}