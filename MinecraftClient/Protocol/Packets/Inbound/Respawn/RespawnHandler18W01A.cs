namespace MinecraftClient.Protocol.Packets.Inbound.Respawn
{
    internal class RespawnHandler18W01A : RespawnHandler17W45A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18W01A;
        protected override int PacketId => 0x37;
    }
}