using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Character
{
    public class ItemSlot
    {
        public IItem Item { get; set; }
        public short Count { get; set; }
    }
}