using System.Collections.Generic;

namespace MinecraftClient.Protocol.WorldProcessors.Recipes
{
    public interface IRecipeParser
    {
        void ReadRecipe(CraftingRecipe recipe, IMinecraftComHandler handler, List<byte> packetData);
    }
}