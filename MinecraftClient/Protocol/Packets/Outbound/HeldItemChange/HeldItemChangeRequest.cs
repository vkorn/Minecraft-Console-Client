namespace MinecraftClient.Protocol.Packets.Outbound.HeldItemChange
{
    internal struct HeldItemChangeRequest : IOutboundRequest
    {
        public short SlotNum { get; set; }
    }
}