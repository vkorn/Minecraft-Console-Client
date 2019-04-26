using System;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    [Flags]
    internal enum ItemAttributes
    {
        Unknown = 0,
        Consumable = 1,
        CanHarm = 2,
    }
}