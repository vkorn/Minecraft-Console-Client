namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookOut18 : PlayerPositionAndLookOut
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18;

        protected override byte[] GetExtraY(PlayerPositionAndLookRequest data)
        {
            return new byte[0];
        }
    }
}