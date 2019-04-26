namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookOut17W46A : PlayerPositionAndLookOut17W45A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W46A;
        protected override int PacketId => 0x0E;
    }
}