namespace MinecraftClient.Protocol.Packets.Inbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookHandler17W13A : PlayerPositionAndLookHandler19
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W13A;
        protected override int PacketId => 0x2F;
    }
}