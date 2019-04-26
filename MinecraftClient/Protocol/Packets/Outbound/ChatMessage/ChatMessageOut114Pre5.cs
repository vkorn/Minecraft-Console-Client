namespace MinecraftClient.Protocol.Packets.Outbound.ChatMessage
{
    internal class ChatMessageOut114Pre5 : ChatMessageOut113Pre7
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x03;
    }
}