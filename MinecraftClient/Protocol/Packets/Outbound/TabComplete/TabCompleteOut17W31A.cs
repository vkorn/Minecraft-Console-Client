using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.TabComplete
{
    internal class TabCompleteOut17W31A : TabCompleteOut17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W31A;
        protected override int PacketId => 0x01;
    }
}