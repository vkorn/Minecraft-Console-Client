namespace MinecraftClient.Protocol.Packets.Inbound.Respawn
{
    internal class RespawnHandler17W45A : RespawnHandler17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W45A;
        protected override int PacketId => 0x36;
    }
}