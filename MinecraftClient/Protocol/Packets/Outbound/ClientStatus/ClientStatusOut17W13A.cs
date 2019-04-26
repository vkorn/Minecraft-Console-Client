namespace MinecraftClient.Protocol.Packets.Outbound.ClientStatus
{
    internal class ClientStatusOut17W13A : ClientStatusOut19
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W13A;
        protected override int PacketId => 0x04;
    }
}