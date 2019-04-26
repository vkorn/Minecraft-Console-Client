namespace MinecraftClient.Protocol.Packets.Inbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookHandler18W01A : PlayerPositionAndLookHandler17W45A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18W01A;
        protected override int PacketId => 0x31;
    }
}