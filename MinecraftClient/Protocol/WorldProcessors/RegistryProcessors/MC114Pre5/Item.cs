namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors.MC114Pre5
{
    internal class Item114Pre5 : Item
    {
        protected virtual ItemAttributes Attributes { get; }

        public Item114Pre5(ItemAttributes attributes, int id, string name)
        {
            Id = id;
            Name = name;
            Attributes = attributes;
        }

        public override bool IsConsumable()
        {
            return Attributes.HasFlag(ItemAttributes.Consumable);
        }

        public override bool CanHarm()
        {
            return Attributes.HasFlag(ItemAttributes.CanHarm);
        }
    }
}