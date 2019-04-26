namespace MinecraftClient.Protocol.Packets.Inbound.Respawn
{
    internal class RespawnHandler113Pre7 : RespawnHandler18W01A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC113Pre7;
        protected override int PacketId => 0x38;
    }
}