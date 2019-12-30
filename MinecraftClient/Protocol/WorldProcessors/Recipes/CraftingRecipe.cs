using System.Collections.Generic;
using MinecraftClient.Character;

namespace MinecraftClient.Protocol.WorldProcessors.Recipes
{
    public class CraftingRecipe
    {
        public string Id { get; set; }

        public ItemSlot Result { get; set; }
        public List<ItemSlot> Ingredients { get; set; }
        public bool IsShapeless { get; set; }
        public bool IsSmelting { get; set; }

        public CraftingRecipe()
        {
            Ingredients = new List<ItemSlot>();
        }

        public Dictionary<string, int> GetIngredientsCount()
        {
            var result = new Dictionary<string, int>();

            foreach (var itemSlot in Ingredients)
            {
                if (result.ContainsKey(itemSlot.Item.Name()))
                {
                    result[itemSlot.Item.Name()] += itemSlot.Count;
                }
                else
                {
                    result[itemSlot.Item.Name()] = itemSlot.Count;
                }
            }

            return result;
        }
    }
}