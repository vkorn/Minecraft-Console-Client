namespace MinecraftClient.Protocol.Packets.Outbound.CloseWindow
{
    internal class CloseWindowRequest : IOutboundRequest
    {
        public byte WindowId { get; set; }
    }
}