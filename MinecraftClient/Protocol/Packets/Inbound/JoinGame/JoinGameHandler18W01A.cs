namespace MinecraftClient.Protocol.Packets.Inbound.JoinGame
{
    internal class JoinGameHandler18W01A : JoinGameHandler17W31A
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC18W01A;


        protected override int PacketId => 0x25;
    }
}