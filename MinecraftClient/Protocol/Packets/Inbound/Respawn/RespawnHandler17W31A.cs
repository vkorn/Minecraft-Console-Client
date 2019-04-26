namespace MinecraftClient.Protocol.Packets.Inbound.Respawn
{
    internal class RespawnHandler17W31A : RespawnHandler112Pre5
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W31A;
        protected override int PacketId => 0x35;
    }
}