namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookOut19 : PlayerPositionAndLookOut18
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x0D;
    }
}