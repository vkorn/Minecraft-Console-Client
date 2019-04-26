namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPosition
{
    internal class PlayerPositionOut19 : PlayerPositionOut18
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x0C;
    }
}