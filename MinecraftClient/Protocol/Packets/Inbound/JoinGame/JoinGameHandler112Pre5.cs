namespace MinecraftClient.Protocol.Packets.Inbound.JoinGame
{
    internal class JoinGameHandler112Pre5 : JoinGameHandler17W13A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC112Pre5;

        protected override int PacketId => 0x23;
    }
}