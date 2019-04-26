using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.TabComplete
{
    internal class TabCompleteOut17W46A : TabCompleteOut17W45A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W46A;
        protected override int PacketId => 0x04;
    }
}