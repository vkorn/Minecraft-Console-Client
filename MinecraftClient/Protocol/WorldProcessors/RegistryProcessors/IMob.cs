using System;
using MinecraftClient.Mapping;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    public interface IMob
    {
        int Id();
        Guid Uuid();
        string Name();
        Location Position();
        void UpdatePosition(Location delta, bool isRelative);
        MobTypes Type();
    }
}