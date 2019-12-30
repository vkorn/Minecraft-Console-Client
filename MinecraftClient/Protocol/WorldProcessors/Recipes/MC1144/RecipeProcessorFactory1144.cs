using System.Collections.Generic;

namespace MinecraftClient.Protocol.WorldProcessors.Recipes.MC1144
{
    internal class RecipeProcessorFactory1144 : RecipeProcessorFactory
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC1144;

        protected override Dictionary<string, IRecipeParser> GetParsers()
        {
            var smelting = new Smelting1144();

            return new Dictionary<string, IRecipeParser>
            {
                {"minecraft:stonecutting", new Cutting1144()},
                {"minecraft:crafting_shapeless", new Shapeless1144()},
                {"minecraft:crafting_shaped", new Shaped1144()},
                {"minecraft:smelting", smelting},
                {"minecraft:blasting", smelting},
                {"minecraft:smoking", smelting},
                {"minecraft:campfire_cooking", smelting},
            };
        }
    }
}