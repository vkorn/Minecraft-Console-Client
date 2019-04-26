using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.MultiBlockChange
{
    internal class MultiBlockChangeHandler18 : MultiBlockChangeHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18;

        protected override int ReadRecordsCount(List<byte> packetData)
        {
            return PacketUtils.readNextVarInt(packetData);
        }

        protected override void ReadAndUpdateBlock(IMinecraftComHandler handler, List<byte> packetData, int chunkX,
            int chunkZ)
        {
            var locationXz = PacketUtils.readNextByte(packetData);
            var blockY = (ushort) PacketUtils.readNextByte(packetData);
            var blockIdMeta = (ushort) PacketUtils.readNextVarInt(packetData);

            UpdateBlock(handler, chunkX, chunkZ, blockIdMeta, blockY, locationXz);
        }
    }
}