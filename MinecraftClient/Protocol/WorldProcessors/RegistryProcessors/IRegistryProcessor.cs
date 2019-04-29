using System;
using MinecraftClient.Mapping;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    internal interface IRegistryProcessor: IWorldProcessor
    {
        IItem GetItem(int id);
        IMob GetEntity(int type, int id, Guid uuid, Location position);
    }
}