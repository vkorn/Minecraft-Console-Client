using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.Packets.Outbound;
using MinecraftClient.Protocol.Packets.Outbound.ClickWindow;
using MinecraftClient.Protocol.Packets.Outbound.HeldItemChange;
using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Inventory
{
    public class Player
    {
        private readonly IMinecraftCom _protocol;

        public float Health { get; set; }
        public int Food { get; set; }

        public short ActiveSlot { get; set; }

        public Dictionary<short, ItemSlot> Inventory { get; private set; }

        internal IRegistryProcessor RegistryProcessor { get; }

        public bool IsLoaded { get; private set; }

        public Player(int protocolVersion, IMinecraftCom protocol)
        {
            Inventory = new Dictionary<short, ItemSlot>();
            RegistryProcessor = VersionsFactory.WorldProcessor<IRegistryProcessor>(protocolVersion);
            _protocol = protocol;

            ConsoleIO.WriteLineFormatted("Loaded Registries processor:");
            ConsoleIO.WriteLine($"Version: {RegistryProcessor.MinVersion()}    " +
                                $"Implementation: {RegistryProcessor.GetType().Name}");
        }

        private delegate bool SelectorDelegate(short slot);

        private Dictionary<short, ItemSlot> Selector(SelectorDelegate selector)
        {
            return Inventory.Where(x => x.Value != null && selector(x.Key))
                .OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public Dictionary<short, ItemSlot> QuickBar()
        {
            return Selector(x => x >= 36);
        }

        public Dictionary<short, ItemSlot> InventoryOnly()
        {
            return Selector(x => x >= 9 && x < 36);
        }

        public Dictionary<short, ItemSlot> Equip()
        {
            return Selector(x => x >= 5 && x <= 8 || x == 45);
        }

        public void SetInventory(Dictionary<short, ItemSlot> inv)
        {
            Inventory = inv;
            IsLoaded = true;
        }

        internal void Stop()
        {
            IsLoaded = false;
        }

        public void PickActiveItem(short quickBarNumber)
        {
            if (quickBarNumber < 1 || quickBarNumber > 9)
            {
                quickBarNumber = 1;
            }

            _protocol.SendPacketOut(OutboundTypes.HeldItemChange, null,
                new HeldItemChangeRequest {SlotNum = quickBarNumber});
            ActiveSlot = quickBarNumber;
        }

        public void UseActiveItem()
        {
            _protocol.SendPacketOut(OutboundTypes.UseItem, null, null);
        }

        public bool SwapItems(short from, short to)
        {
            if (from < 0)
            {
                from = 0;
            }

            if (to < 0)
            {
                to = 0;
            }

            if (from == to)
            {
                return true;
            }

            // TODO: add another windows

            var itemFrom = Inventory[from];
            var itemTo = Inventory[to];

            if (null == itemFrom && null == itemTo)
            {
                return false;
            }

            Inventory[to] = itemFrom;
            Inventory[from] = itemTo;

            if (null == itemFrom)
            {
                var tmp = to;
                to = from;
                from = tmp;

                itemFrom = itemTo;
                itemTo = null;
            }

            _protocol.SendPacketOut(OutboundTypes.ClickWindow, null, new ClickWindowRequest
            {
                Item = itemFrom,
                SlotNum = from,
                WindowId = 0, // Inventory
            });

            Thread.Sleep(100);
            _protocol.SendPacketOut(OutboundTypes.ClickWindow, null, new ClickWindowRequest
            {
                Item = itemTo,
                SlotNum = to,
                WindowId = 0, // Inventory
            });

            if (null == itemTo)
            {
                return true;
            }

            Thread.Sleep(100);
            _protocol.SendPacketOut(OutboundTypes.ClickWindow, null, new ClickWindowRequest
            {
                Item = null,
                SlotNum = from,
                WindowId = 0, // Inventory
            });

            return true;
        }
    }
}