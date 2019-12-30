using System.Collections.Generic;
using MinecraftClient.Character;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.Packets;

namespace MinecraftClient.Protocol.WorldProcessors.Recipes
{
    internal abstract class RecipeParser : IRecipeParser
    {
        public abstract void ReadRecipe(CraftingRecipe recipe, IMinecraftComHandler handler, List<byte> packetData);

        protected virtual ItemSlot ReadSlot(IMinecraftComHandler handler, NbtNoop noop, List<byte> packetData)
        {
            var isPresent = PacketUtils.readNextBool(packetData);
            if (!isPresent)
            {
                return null;
            }

            var itemId = PacketUtils.readNextVarInt(packetData);
            var itemsCount = PacketUtils.readNextByte(packetData);

            var slot = new ItemSlot
            {
                Item = handler.GetPlayer().RegistryProcessor.GetItem(itemId),
                Count = itemsCount
            };

            noop.SkipTag();
            slot.Item.SetNbt(noop.SkippedData.ToArray());
            noop.Reset();

            return slot;
        }

        protected virtual IEnumerable<ItemSlot> ReadIngredient(IMinecraftComHandler handler, NbtNoop noop,
            List<byte> packetData)
        {
            var result = new List<ItemSlot>();

            var count = PacketUtils.readNextVarInt(packetData);
            for (var ii = 0; ii < count; ii++)
            {
                var slot = ReadSlot(handler, noop, packetData);
                if (null == slot)
                {
                    continue;
                }

                result.Add(slot);
            }

            return result;
        }
    }
}