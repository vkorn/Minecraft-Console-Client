using System.Collections.Generic;
using MinecraftClient.Protocol.Packets.Outbound;

namespace MinecraftClient.Protocol.Packets
{
    public interface IProtocol
    {
        bool SendPacketOut(OutboundTypes type, IEnumerable<byte> packetData, IOutboundRequest data);
        int Dimension();
    }
}