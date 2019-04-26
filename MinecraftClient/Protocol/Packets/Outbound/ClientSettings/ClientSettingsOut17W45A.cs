namespace MinecraftClient.Protocol.Packets.Outbound.ClientSettings
{
    internal class ClientSettingsOut17W45A : ClientSettingsOut17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W45A;
        protected override int PacketId => 0x03;
    }
}