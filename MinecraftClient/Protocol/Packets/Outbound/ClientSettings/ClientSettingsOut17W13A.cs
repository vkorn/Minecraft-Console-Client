namespace MinecraftClient.Protocol.Packets.Outbound.ClientSettings
{
    internal class ClientSettingsOut17W13A : ClientSettingsOut19
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W13A;
        protected override int PacketId => 0x05;
    }
}