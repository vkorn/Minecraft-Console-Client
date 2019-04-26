using System.Collections.Generic;

namespace MinecraftClient.Protocol.Packets.Inbound.ChunkData
{
    internal class ChunkDataHandler114Pre5 : ChunkDataHandler17W46A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x21;

        protected override void SkipHeightMap(List<byte> packetData)
        {
            new NbtNoop(packetData).SkipTag();
        }
    }
}