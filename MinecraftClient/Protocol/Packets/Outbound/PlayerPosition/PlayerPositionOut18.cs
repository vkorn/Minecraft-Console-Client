namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPosition
{
    internal class PlayerPositionOut18 : PlayerPositionOut
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18;

        protected override byte[] GetExtraY(PlayerPositionRequest data)
        {
            return new byte[0];
        }
    }
}