namespace MinecraftClient.Protocol.Packets.Inbound.ChatMessage
{
    internal class ChatMessageHandler19 : ChatMessageHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x0F;
    }
}