namespace MinecraftClient.Protocol.Packets.Inbound.JoinGame
{
    internal class JoinGameHandler17W13A : JoinGameHandler191
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W13A;

        protected override int PacketId => 0x24;
    }
}