namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    internal interface IRegistryProcessor: IWorldProcessor
    {
        IItem GetItem(int id);
    }
}