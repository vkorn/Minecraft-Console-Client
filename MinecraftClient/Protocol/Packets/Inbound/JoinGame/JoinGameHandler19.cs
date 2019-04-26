namespace MinecraftClient.Protocol.Packets.Inbound.JoinGame
{
    internal class JoinGameHandler19 : JoinGameHandler18
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;


        protected override int PacketId => 0x23;
    }
}