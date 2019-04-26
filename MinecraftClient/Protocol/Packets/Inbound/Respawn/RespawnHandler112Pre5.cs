namespace MinecraftClient.Protocol.Packets.Inbound.Respawn
{
    internal class RespawnHandler112Pre5 : RespawnHandler17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC112Pre5;
        protected override int PacketId => 0x34;
    }
}