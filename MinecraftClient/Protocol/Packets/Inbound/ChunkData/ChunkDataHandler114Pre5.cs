using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.ChunkData
{
    internal class ChunkDataHandler114Pre5 : ChunkDataHandler17W46A
    {
        protected override int MinVersion => PacketUtils.MC114pre5Version;
        protected override int PacketId => 0x21;

        protected override void SkipHeightMap(List<byte> packetData)
        {
            PacketUtils.readNextByte(packetData); // TAG_Compound
            var nameLength = PacketUtils.readNextShort(packetData);
            PacketUtils.readData(nameLength, packetData); // Compound name

            var tag = PacketUtils.readNextByte(packetData); // Array

            while (tag != 0)
            {
                nameLength = PacketUtils.readNextShort(packetData);
                PacketUtils.readData(nameLength, packetData); // Array name
                var itemsCount = PacketUtils.readNextInt(packetData);
                PacketUtils.readData(8 * itemsCount, packetData);

                tag = PacketUtils.readNextByte(packetData);
            }
        }
    }
}