namespace MinecraftClient.Protocol.Packets.Outbound.ClientStatus
{
    internal class ClientStatusOut17W31A : ClientStatusOut17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W31A;
        protected override int PacketId => 0x03;
    }
}