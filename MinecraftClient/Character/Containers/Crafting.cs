using System.Collections.Generic;
using System.Linq;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.Packets.Outbound;
using MinecraftClient.Protocol.Packets.Outbound.ClickWindow;
using MinecraftClient.Protocol.Packets.Outbound.CloseWindow;
using MinecraftClient.Protocol.Packets.Outbound.CraftRecipe;
using MinecraftClient.Protocol.WorldProcessors.Recipes;
using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Character.Containers
{
    public class Crafting : BaseContainer
    {
        protected override short MinSlot => 0;
        protected override short MaxSlot => 9;
        
        private List<CraftingRecipe> _recipes;
        private const int ResultSlot = 0;

        internal IRecipeProcessorFactory RecipeProcessorFactory { get; }

        public bool IsLastRecipeConfirmed { get; private set; }
        private string _lastRecipe;

        public Crafting(Inventory playerInventory, int protocolVersion, IMinecraftCom protocol,
            IMinecraftComHandler handler) :
            base(playerInventory, protocol, handler)
        {
            _recipes = new List<CraftingRecipe>();
            RecipeProcessorFactory = VersionsFactory.WorldProcessor<IRecipeProcessorFactory>(protocolVersion);
            ConsoleIO.WriteLineFormatted("Loaded Recipes factory:");
            ConsoleIO.WriteLine($"Version: {RecipeProcessorFactory.MinVersion()}    " +
                                $"Implementation: {RecipeProcessorFactory.GetType().Name}");
        }

        protected override List<WindowType> WinType => new List<WindowType> {WindowType.Crafting};

        protected override void SendWindowClosePacket()
        {
            Protocol.SendPacketOut(OutboundTypes.CloseWindow, null, new CloseWindowRequest {WindowId = WindowId});
        }

        public void UpdateRecipes(List<CraftingRecipe> recipes)
        {
            _recipes = recipes;
            ConsoleIO.WriteLine($"Loaded {_recipes.Count} new recipes");
        }

        public bool CanCraft(string recipeId)
        {
            var target = _recipes.FirstOrDefault(x => x.Id == recipeId);
            if (null == target)
            {
                return false;
            }

            var ingredients = target.GetIngredientsCount();
            foreach (var ingredient in ingredients)
            {
                if (PlayerInventory.GetItemsCount(ingredient.Key) < ingredient.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public void PickRecipe(string recipeId, bool makeAll)
        {
            IsLastRecipeConfirmed = false;
            _lastRecipe = recipeId;

            Protocol.SendPacketOut(OutboundTypes.CraftRecipe, null, new CraftRecipeRequest
            {
                MakeAll = makeAll,
                RecipeId = recipeId,
                WindowId = WindowId,
            });
        }

        public void TakeResults()
        {
            // if (null == Inventory[ResultSlot])
            // {
            //     return;
            // }

            var req = new ClickWindowRequest
            {
                Item = Inventory[ResultSlot],
                ShiftPressed = true,
                SlotNum = ResultSlot,
                WindowId = WindowId,
            };

            Protocol.SendPacketOut(OutboundTypes.ClickWindow, null, req);
        }

        public void ConfirmLastRecipe(byte windowId, string recipeId)
        {
            if (WindowId != windowId || _lastRecipe != recipeId)
            {
                return;
            }

            IsLastRecipeConfirmed = true;
        }

        public override void SetInventory(byte windowId, Dictionary<short, ItemSlot> inv)
        {
            if (windowId != WindowId)
            {
                return;
            }
            
            base.SetInventory(windowId, inv);

            var item = Inventory?[ResultSlot];
            if (item != null && item.Item.Name() == _lastRecipe)
            {
                IsLastRecipeConfirmed = true;
            }
        }

        public override void SetSlot(byte windowId, short itemSlot, ItemSlot item)
        {
            if (windowId != WindowId)
            {
                return;
            }
            
            base.SetSlot(windowId, itemSlot, item);
            
            if (item != null && itemSlot == ResultSlot)
            {
                IsLastRecipeConfirmed = true;
            }
        }
    }
}