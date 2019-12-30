using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.Packets;

namespace MinecraftClient.Protocol.WorldProcessors.Recipes.MC1144
{
    internal class Cutting1144: RecipeParser
    {
        public override void ReadRecipe(CraftingRecipe recipe, IMinecraftComHandler handler, List<byte> packetData)
        {
            recipe.IsShapeless = false;
            recipe.IsSmelting = false;
                        
            PacketUtils.readNextString(packetData); // Group
            var noop = new NbtNoop(packetData);
            recipe.Ingredients.AddRange(ReadIngredient(handler, noop, packetData));
            recipe.Result = ReadSlot(handler, noop, packetData);
        }
    }
}