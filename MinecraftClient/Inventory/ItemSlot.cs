using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Inventory
{
    public class ItemSlot
    {
        public IItem Item { get; set; }
        public short Count { get; set; }
    }
}