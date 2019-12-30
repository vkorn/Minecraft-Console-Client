using System.Collections.Generic;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.Packets.Outbound;
using MinecraftClient.Protocol.Packets.Outbound.ClickWindow;
using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Character.Containers
{
    public abstract class BaseContainer
    {
        public byte WindowId { get; protected set; }
        protected readonly IMinecraftCom Protocol;
        protected readonly IMinecraftComHandler Handler;
        protected abstract short MinSlot { get; }
        protected abstract short MaxSlot { get; }
        protected WindowType Type;

        protected abstract List<WindowType> WinType { get; }
        protected Inventory PlayerInventory { get; }

        protected bool IsItemClickProcessed { get; set; }

        protected BaseContainer(Inventory playerInventory, IMinecraftCom protocol, IMinecraftComHandler handler)
        {
            Inventory = new Dictionary<short, ItemSlot>();
            Protocol = protocol;
            Handler = handler;
            WindowId = 0;
            PlayerInventory = playerInventory;
        }

        public virtual void WindowOpened(byte windowId, WindowType type)
        {
            if (!WinType.Contains(type))
            {
                return;
            }

            Type = type;
            WindowId = windowId;
        }

        public virtual void WindowClosed(byte windowId)
        {
            if (!IsWindowOpened())
            {
                return;
            }

            if (WindowId == windowId)
            {
                WindowId = 0;
            }
        }

        public virtual bool IsWindowOpened()
        {
            return 0 != WindowId;
        }

        public virtual void CloseWindow()
        {
            if (!IsWindowOpened())
            {
                return;
            }

            SendWindowClosePacket();
            WindowId = 0;
        }

        protected abstract void SendWindowClosePacket();

        public Dictionary<short, ItemSlot> Inventory { get; protected set; }
        protected Dictionary<short, ItemSlot> FullInventory;

        public virtual void SetSlot(byte windowId, short itemSlot, ItemSlot item)
        {
            if (windowId != WindowId)
            {
                return;
            }

            IsItemClickProcessed = true;

            if (itemSlot > MaxSlot)
            {
                PlayerInventory.SetSlot(0, (short) (itemSlot - MaxSlot + 8), item);
            }
            else
            {
                Inventory[itemSlot] = item;
            }
        }

        public virtual void SetInventory(byte windowId, Dictionary<short, ItemSlot> inv)
        {

            if (windowId != WindowId)
            {
                return;
            }

            IsItemClickProcessed = true;
            FullInventory = inv;

            if (windowId != 0)
            {
                Inventory = new Dictionary<short, ItemSlot>();
            }

            var playerInv = new Dictionary<short, ItemSlot>();
            foreach (var itemSlot in inv)
            {
                if (itemSlot.Key < MinSlot)
                {
                    continue;
                }

                if (itemSlot.Key > MaxSlot)
                {
                    playerInv[(short) (itemSlot.Key - MaxSlot + 8)] = itemSlot.Value;
                }
                else
                {
                    Inventory[itemSlot.Key] = itemSlot.Value;
                }
            }

            if (0 != WindowId)
            {
                PlayerInventory.SetInventory(0, playerInv);
            }
        }

        public virtual short GetFreeSlotsCount()
        {
            short num = 0;
            foreach (var itemSlot in Inventory)
            {
                if (null == itemSlot.Value)
                {
                    num++;
                }
            }

            return num;
        }

        public short GetTotalSlotsCount()
        {
            return (short) Inventory.Count;
        }

        public int GetItemsCount(string itemId)
        {
            var count = 0;

            foreach (var itemSlot in Inventory)
            {
                if (null == itemSlot.Value)
                {
                    continue;
                }

                if (itemSlot.Value.Item.SameId(itemId))
                {
                    count += itemSlot.Value.Count;
                }
            }

            return count;
        }

        public void PutItemTo(string itemId)
        {
            if (!IsItemClickProcessed)
            {
                return;
            }

            foreach (var itemSlot in FullInventory)
            {
                if (null == itemSlot.Value || itemSlot.Key <= MaxSlot)
                {
                    continue;
                }

                if (itemSlot.Value.Item.SameId(itemId))
                {
                    var req = new ClickWindowRequest
                    {
                        Item = itemSlot.Value,
                        ShiftPressed = true,
                        SlotNum = itemSlot.Key,
                        WindowId = WindowId,
                    };

                    IsItemClickProcessed = false;
                    Protocol.SendPacketOut(OutboundTypes.ClickWindow, null, req);

                    return;
                }
            }
        }
    }
}