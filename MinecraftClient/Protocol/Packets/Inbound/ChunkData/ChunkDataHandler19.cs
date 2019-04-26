using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.ChunkData
{
    internal class ChunkDataHandler19 : ChunkDataHandler18
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x20;

        protected override ushort ReadChunkMask(List<byte> packetData)
        {
            return (ushort) PacketUtils.readNextVarInt(packetData);
        }
    }
}