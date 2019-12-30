using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.WorldProcessors.Recipes;

namespace MinecraftClient.Protocol.Packets.Inbound.DeclareRecipes
{
    internal class DeclareRecipesHandler1144 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC1144;
        protected override int PacketId => 0x5A;
        protected override InboundTypes PackageType => InboundTypes.DeclareRecipes;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var count = PacketUtils.readNextVarInt(packetData);

            var recipes = new List<CraftingRecipe>();

            for (var ii = 0; ii < count; ii++)
            {
                var recipe = new CraftingRecipe();
                var type = PacketUtils.readNextString(packetData);
                var id = PacketUtils.readNextString(packetData);
                recipe.Id = id;
                var parser = handler.GetPlayer().Crafting.RecipeProcessorFactory.GetParser(type);
                parser?.ReadRecipe(recipe, handler, packetData);
                recipes.Add(recipe);
            }

            handler.GetPlayer().Crafting.UpdateRecipes(recipes);

            return null;
        }
    }
}