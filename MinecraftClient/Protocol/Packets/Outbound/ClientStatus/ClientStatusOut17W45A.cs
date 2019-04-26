using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.ClientStatus
{
    internal class ClientStatusOut17W45A : ClientStatusOut17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W45A;
        protected override int PacketId => 0x02;
    }
}