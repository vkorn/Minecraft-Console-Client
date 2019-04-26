namespace MinecraftClient.Protocol.Packets.Outbound.ClientStatus
{
    internal class ClientStatusOut113Pre7 : ClientStatusOut17W45A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC113Pre7;
        protected override int PacketId => 0x03;
    }
}