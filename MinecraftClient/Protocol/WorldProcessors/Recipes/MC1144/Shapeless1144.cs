using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.Packets;

namespace MinecraftClient.Protocol.WorldProcessors.Recipes.MC1144
{
    internal class Shapeless1144 : RecipeParser
    {
        public override void ReadRecipe(CraftingRecipe recipe, IMinecraftComHandler handler, List<byte> packetData)
        {
            recipe.IsShapeless = true;
            recipe.IsSmelting = false;

            PacketUtils.readNextString(packetData); // Group
            var ingCount = PacketUtils.readNextVarInt(packetData);
            var noop = new NbtNoop(packetData);
            for (var jj = 0; jj < ingCount; jj++)
            {
                recipe.Ingredients.AddRange(ReadIngredient(handler, noop, packetData));
            }

            recipe.Result = ReadSlot(handler, noop, packetData);
        }
    }
}