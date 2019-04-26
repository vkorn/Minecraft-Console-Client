namespace MinecraftClient.Protocol.Packets.Inbound.JoinGame
{
    internal class JoinGameHandler17W31A : JoinGameHandler112Pre5
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC17W31A;


        protected override int PacketId => 0x24;
    }
}