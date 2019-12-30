using System.Collections.Generic;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.Packets.Outbound;
using MinecraftClient.Protocol.Packets.Outbound.ClickWindow;
using MinecraftClient.Protocol.Packets.Outbound.CloseWindow;
using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Character.Containers
{
    public class Container : BaseContainer
    {
        protected override short MinSlot => 0;

        protected override short MaxSlot
        {
            get
            {
                switch (Type)
                {
                    case WindowType.Shulker_Box:
                    case WindowType.Generic_9x3:
                        return 26;
                    default:
                        return 53;
                }
            }
        }

        public Container(Inventory playerInventory, IMinecraftCom protocol, IMinecraftComHandler handler) : 
            base(playerInventory, protocol, handler)
        {
        }

        protected override List<WindowType> WinType => new List<WindowType>
        {
            WindowType.Generic_9x3,
            WindowType.Generic_9x6,
            WindowType.Shulker_Box,
        };

        protected override void SendWindowClosePacket()
        {
            Protocol.SendPacketOut(OutboundTypes.CloseWindow, null, new CloseWindowRequest {WindowId = WindowId});
        }

        public short TakeItemsOut(short slotsAmount)
        {
            var itemsLeft = slotsAmount;
            var itemsToTake = new Dictionary<short, ItemSlot>();

            foreach (var itemSlot in Inventory)
            {
                if (0 == itemsLeft)
                {
                    break;
                }

                if (null == itemSlot.Value)
                {
                    continue;
                }

                itemsToTake[itemSlot.Key] = itemSlot.Value;
                itemsLeft--;
            }

            foreach (var itemSlot in itemsToTake)
            {
                var req = new ClickWindowRequest
                {
                    Item = itemSlot.Value,
                    ShiftPressed = true,
                    SlotNum = itemSlot.Key,
                    WindowId = WindowId,
                };

                Protocol.SendPacketOut(OutboundTypes.ClickWindow, null, req);
            }

            return (short) (slotsAmount - itemsLeft);
        }

        public void TakeFirstSlotOut()
        {
            if (!IsItemClickProcessed)
            {
                return;
            }

            foreach (var itemSlot in Inventory)
            {
                if (null == itemSlot.Value)
                {
                    continue;
                }

                var req = new ClickWindowRequest
                {
                    Item = itemSlot.Value,
                    ShiftPressed = true,
                    SlotNum = itemSlot.Key,
                    WindowId = WindowId,
                };
                
                IsItemClickProcessed = false;
                Protocol.SendPacketOut(OutboundTypes.ClickWindow, null, req);
            }
        }
    }
}