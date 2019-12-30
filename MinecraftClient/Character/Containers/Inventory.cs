using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.Packets;
using MinecraftClient.Protocol.Packets.Outbound;
using MinecraftClient.Protocol.Packets.Outbound.ClickWindow;
using MinecraftClient.Protocol.Packets.Outbound.HeldItemChange;
using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Character.Containers
{
    public class Inventory : BaseContainer
    {
        public short ActiveSlot { get; set; }

        protected override short MinSlot => 0;
        protected override short MaxSlot => 45;

        public Inventory(Inventory playerInventory, IMinecraftCom protocol, IMinecraftComHandler handler) : 
            base( playerInventory, protocol, handler)
        {
        }

        protected override List<WindowType> WinType => new List<WindowType> {WindowType.Generic_9x5};

        protected override void SendWindowClosePacket()
        {
        }
        
        public void PickActiveItem(short quickBarNumber)
        {
            if (quickBarNumber < 1 || quickBarNumber > 9)
            {
                quickBarNumber = 1;
            }

            Protocol.SendPacketOut(OutboundTypes.HeldItemChange, null,
                new HeldItemChangeRequest {SlotNum = quickBarNumber});
            ActiveSlot = quickBarNumber;
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

            Protocol.SendPacketOut(OutboundTypes.ClickWindow, null, new ClickWindowRequest
            {
                Item = itemFrom,
                SlotNum = from,
                WindowId = 0, // Inventory
            });

            Thread.Sleep(100);
            Protocol.SendPacketOut(OutboundTypes.ClickWindow, null, new ClickWindowRequest
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
            Protocol.SendPacketOut(OutboundTypes.ClickWindow, null, new ClickWindowRequest
            {
                Item = null,
                SlotNum = from,
                WindowId = 0, // Inventory
            });

            return true;
        }
        
        public bool HasItem(ref Hands hand)
        {
            switch (hand)
            {
                case Hands.Main:
                    return HasMainHandItem();
                case Hands.Offhand:
                    return HasOffHandItem();
                case Hands.Auto:
                {
                    if (HasMainHandItem())
                    {
                        hand = Hands.Main;
                        return true;
                    }

                    if (HasOffHandItem())
                    {
                        hand = Hands.Offhand;
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        private bool HasMainHandItem()
        {
            var itm = Inventory[(short) (InventoryConstants.QuickBarMin + ActiveSlot - 1)];
            return itm != null && itm.Item.CanPlace();
        }

        private bool HasOffHandItem()
        {
            var itm = Inventory[(short) InventoryConstants.OffHand];
            return itm != null && itm.Item.CanPlace();
        }
        
        private delegate bool SelectorDelegate(short slot);

        private Dictionary<short, ItemSlot> Selector(SelectorDelegate selector)
        {
            return Inventory.Where(x => x.Value != null && selector(x.Key))
                .OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public Dictionary<short, ItemSlot> QuickBar()
        {
            return Selector(x => x >= 36 && x <= 44);
        }

        public Dictionary<short, ItemSlot> InventoryOnly()
        {
            return Selector(x => x >= 9 && x < 36);
        }

        public Dictionary<short, ItemSlot> Equip()
        {
            return Selector(x => x >= 5 && x <= 8 || x == 45);
        }

        public override short GetFreeSlotsCount()
        {
            short num = 0;
            foreach (var itemSlot in Inventory.Where(x => x.Key >= 9 && x.Key < 45))
            {
                if (null == itemSlot.Value)
                {
                    num++;
                }
            }

            return num;
        }
    }
}