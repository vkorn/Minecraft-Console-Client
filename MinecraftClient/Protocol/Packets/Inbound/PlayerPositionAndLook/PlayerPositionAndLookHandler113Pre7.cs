namespace MinecraftClient.Protocol.Packets.Inbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookHandler113Pre7 : PlayerPositionAndLookHandler18W01A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC113Pre7;
        protected override int PacketId => 0x32;
    }
}