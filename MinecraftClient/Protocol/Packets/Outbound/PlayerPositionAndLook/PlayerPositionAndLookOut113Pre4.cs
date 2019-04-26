namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookOut113Pre4 : PlayerPositionAndLookOut17W46A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC113Pre4;
        protected override int PacketId => 0x0F;
    }
}