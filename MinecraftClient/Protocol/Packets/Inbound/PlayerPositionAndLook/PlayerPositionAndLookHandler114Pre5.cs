namespace MinecraftClient.Protocol.Packets.Inbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookHandler114Pre5 : PlayerPositionAndLookHandler113Pre7
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x35;
    }
}