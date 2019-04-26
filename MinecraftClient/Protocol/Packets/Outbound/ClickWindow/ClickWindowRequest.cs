using MinecraftClient.Character;

namespace MinecraftClient.Protocol.Packets.Outbound.ClickWindow
{
    internal struct ClickWindowRequest : IOutboundRequest
    {
        public byte WindowId { get; set; }
        public short SlotNum { get; set; }
        public ItemSlot Item { get; set; }
    }
}