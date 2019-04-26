namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    internal abstract class Item : IItem
    {
        protected int Id { get; set; }
        protected string Name { get; set; }

        protected byte[] Nbt { get; set; }

        int IItem.Id()
        {
            return Id;
        }

        string IItem.Name()
        {
            return Name;
        }

        public void SetNbt(byte[] nbt)
        {
            Nbt = nbt;
        }

        byte[] IItem.Nbt()
        {
            return Nbt;
        }

        public abstract bool IsConsumable();
        public abstract bool CanHarm();
    }
}