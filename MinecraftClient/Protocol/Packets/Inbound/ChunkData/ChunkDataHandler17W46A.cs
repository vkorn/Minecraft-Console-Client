namespace MinecraftClient.Protocol.Packets.Inbound.ChunkData
{
    internal class ChunkDataHandler17W46A : ChunkDataHandler17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W46A;
        protected override int PacketId => 0x22;
    }
}