using System;
using System.Linq;
using MinecraftClient.Inventory;
using MinecraftClient.Protocol;

namespace MinecraftClient.ChatBots
{
    public class AntiHunger : ChatBot
    {
        private enum Fsm
        {
            No,
            Pick,
            Eat,
            UnPick,
        }


        public override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;

        private readonly bool _watchHealth;
        private readonly int _threshold;
        private long _lastMeal;
        private Fsm _state;
        private short _prevActive;
        private long _lastStep;

        public AntiHunger(bool watchHealth, int threshold)
        {
            _watchHealth = watchHealth;
            _threshold = threshold;
        }

        public override void Update()
        {
            if (!GetPlayer().IsLoaded)
            {
                return;
            }

            var now = DateTime.Now.Ticks;

            if (_state != Fsm.No && now - _lastStep < 2 * TimeSpan.TicksPerSecond)
            {
                return;
            }

            switch (_state)
            {
                case Fsm.No:
                {
                    if (now - _lastMeal < 10 * TimeSpan.TicksPerSecond)
                    {
                        // too early 
                        return;
                    }

                    if ((!(GetPlayer().Health < _threshold) || !_watchHealth) && GetPlayer().Food >= _threshold)
                    {
                        // bad condition
                        return;
                    }

                    ItemSlot foundFood = null;
                    short slot = 0;

                    foreach (var itemSlot in GetPlayer().Inventory.Where(x => null != x.Value))
                    {
                        if (itemSlot.Value.Item.IsConsumable() && !itemSlot.Value.Item.CanHarm())
                        {
                            foundFood = itemSlot.Value;
                            slot = itemSlot.Key;
                        }
                    }

                    if (null == foundFood)
                    {
                        ConsoleIO.WriteLineFormatted("No food found, stopping AntiHunger");
                        Console.Beep();
                        UnloadBot();
                        return;
                    }

                    ConsoleIO.WriteLineFormatted($"Eating {foundFood.Item.Name()}");
                    _prevActive = GetPlayer().ActiveSlot;
                    GetPlayer().SwapItems(slot, 44);

                    _state = Fsm.Pick;
                    _lastStep = now;
                }
                    break;
                case Fsm.Pick:
                {
                    GetPlayer().PickActiveItem(9);
                    _state = Fsm.Eat;
                    _lastStep = now;
                }
                    break;
                case Fsm.Eat:
                {
                    GetPlayer().UseActiveItem();
                    _state = Fsm.UnPick;
                    _lastStep = now;
                }
                    break;
                case Fsm.UnPick:
                {
                    GetPlayer().PickActiveItem(_prevActive);
                    _lastMeal = DateTime.Now.Ticks;
                    _state = Fsm.No;
                }
                    break;
            }
        }
    }
}