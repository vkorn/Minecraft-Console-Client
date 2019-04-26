using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.TabComplete
{
    internal class TabCompleteOut17W45A : TabCompleteOut17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W45A;
        protected override int PacketId => -1;
    }
}