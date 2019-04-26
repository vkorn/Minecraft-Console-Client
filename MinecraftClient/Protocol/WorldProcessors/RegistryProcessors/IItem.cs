namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    public interface IItem
    {
        int Id();

        string Name();

        bool IsConsumable();

        bool CanHarm();

        void SetNbt(byte[] nbt);

        byte[] Nbt();
    }
}