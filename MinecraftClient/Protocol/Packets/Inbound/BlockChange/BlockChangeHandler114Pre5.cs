using MinecraftClient.Mapping;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.BlockChange
{
    internal class BlockChangeHandler114Pre5 : BlockChangeHandler17W31A
    {
        protected override int MinVersion => PacketUtils.MC114pre5Version;
        protected override int PacketId => 0x0B;

        protected override Location LocationFromLong(long val)
        {
            var x = (int) (val >> 38);
            var y = (int) (val >> 26 & 0xFFF);
            var z = (int) ((val << 38 >> 38) >> 12);
            if (x >= 33554432)
                x -= 67108864;
            if (y >= 2048)
                y -= 4096;
            if (z >= 33554432)
                z -= 67108864;
            return new Location(x, y, z);
        }
    }
}