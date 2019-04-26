using System.Collections.Generic;

namespace MinecraftClient.Protocol.Packets.Inbound.Respawn
{
    internal class RespawnHandler114Pre5 : RespawnHandler113Pre7
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x3A;

        protected override byte ReadDifficulty(List<byte> packetData)
        {
            return 0;
        }
    }
}