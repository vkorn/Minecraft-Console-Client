namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookOut17W13A : PlayerPositionAndLookOut19
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W13A;
        protected override int PacketId => 0x0E;
    }
}