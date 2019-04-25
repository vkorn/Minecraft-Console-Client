using System.Collections.Generic;

namespace MinecraftClient.Protocol.WorldProcessors.BlockProcessors
{
    public interface IBlock
    {
        bool CanHarmPlayers();
        bool IsSolid();
        bool IsLiquid();

        string Material();

        string Property(string name);

        int Id();

        Dictionary<string, string> Properties();
    }
}