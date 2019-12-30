using System;
using MinecraftClient.Mapping;

namespace MinecraftClient.ChatBots
{
    public class Craft : ChatBot
    {
        private enum Fsm
        {
            No,
            OpenFrom,
            GetFrom,
            CloseFrom,
            OpenTable,
            PickRecipe,
            Produce,
            OpenTo,
            PutTo,
            CloseTo,
        }

        private readonly Location _from;
        private readonly Location _to;
        private readonly Location _table;
        private readonly string _recipe;

        private Fsm _state;
        private long _lastStep;

        public Craft(Location from, Location to, Location table, string recipe)
        {
            if (Settings.AntiHunger_Enabled || Settings.MobFarm_Enabled)
            {
                throw new Exception("Craft can't be enabled with other active farms");
            }

            _from = from;
            _to = to;
            _table = table;
            _recipe = recipe;

            _state = Fsm.No;

            if (!_recipe.StartsWith("minecraft:"))
            {
                _recipe = "minecraft:" + _recipe;
            }
        }

        public override void Update()
        {
            if (!GetPlayer().IsLoaded)
            {
                return;
            }

            var now = DateTime.Now.Ticks;

            if (_state != Fsm.No && now - _lastStep < 0.5 * TimeSpan.TicksPerSecond)
            {
                return;
            }


            switch (_state)
            {
                case Fsm.No:
                {
                    OpenContainer(_from);
                    _state = Fsm.OpenFrom;
                }
                    break;
                case Fsm.OpenFrom:
                {
                    if (!GetPlayer().OpenedContainer.IsWindowOpened())
                    {
                        if (now - _lastStep > 5 * TimeSpan.TicksPerSecond)
                        {
                            _state = Fsm.No;
                        }

                        return;
                    }

                    ConsoleIO.WriteLineFormatted("Retrieving source items");

                    _state = Fsm.GetFrom;
                }
                    break;
                case Fsm.GetFrom:
                {
                    var slotsLeft = GetPlayer().OpenedContainer.GetTotalSlotsCount() -
                                    GetPlayer().OpenedContainer.GetFreeSlotsCount();
                    var freeInvSlots = GetPlayer().Inventory.GetFreeSlotsCount();

                    if (0 == freeInvSlots || 0 == slotsLeft)
                    {
                        _state = Fsm.CloseFrom;
                    }
                    else
                    {
                        GetPlayer().OpenedContainer.TakeFirstSlotOut();
                    }
                }
                    break;
                case Fsm.CloseFrom:
                {
                    CloseContainer();
                    _state = Fsm.OpenTable;
                }
                    break;
                case Fsm.OpenTable:
                {
                    if (!GetPlayer().Crafting.CanCraft(_recipe))
                    {
                        ConsoleIO.WriteLineFormatted("Can't craft recipe");
                        Console.Beep();
                        UnloadBot();
                        return;
                    }

                    OpenContainer(_table);
                    _state = Fsm.PickRecipe;
                }
                    break;
                case Fsm.PickRecipe:
                {
                    if (!GetPlayer().Crafting.IsWindowOpened())
                    {
                        return;
                    }

                    if (GetPlayer().Crafting.CanCraft(_recipe))
                    {
                        GetPlayer().Crafting.PickRecipe(_recipe, true);
                        _state = Fsm.Produce;
                    }
                    else
                    {
                        _state = Fsm.OpenTo;
                    }
                }
                    break;
                case Fsm.Produce:
                {
                    if (!GetPlayer().Crafting.IsLastRecipeConfirmed)
                    {
                        return;
                    }

                    GetPlayer().Crafting.TakeResults();

                    if (GetPlayer().Crafting.CanCraft(_recipe))
                    {
                        _state = Fsm.PickRecipe;
                    }
                    else
                    {
                        _state = Fsm.OpenTo;
                    }
                }
                    break;
                case Fsm.OpenTo:
                {
                    ConsoleIO.WriteLineFormatted("Storing produced items");
                    OpenContainer(_to);
                    _state = Fsm.PutTo;
                }
                    break;
                case Fsm.PutTo:
                {
                    if (0 == GetPlayer().Inventory.GetItemsCount(_recipe))
                    {
                        _state = Fsm.CloseTo;
                        break;
                    }

                    if (!GetPlayer().OpenedContainer.IsWindowOpened())
                    {
                        _state = Fsm.OpenTo;
                    }

                    if (0 == GetPlayer().OpenedContainer.GetFreeSlotsCount())
                    {
                        // Make sure container is really full: we're not breaking click
                        _state = Fsm.CloseTo;
                    }

                    GetPlayer().OpenedContainer.PutItemTo(_recipe);
                }
                    break;
                case Fsm.CloseTo:
                {
                    CloseContainer();
                    _state = Fsm.No;
                }
                    break;
            }

            _lastStep = now;
        }

        private void OpenContainer(Location location)
        {
            GetPlayer().PlaceBlock(location);
        }

        private void CloseContainer()
        {
            GetPlayer().Crafting.CloseWindow();
            GetPlayer().OpenedContainer.CloseWindow();
        }
    }
}