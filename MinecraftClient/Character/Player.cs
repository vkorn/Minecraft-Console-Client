using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinecraftClient.Character.Containers;
using MinecraftClient.Mapping;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.Packets;
using MinecraftClient.Protocol.Packets.Outbound;
using MinecraftClient.Protocol.Packets.Outbound.ClickWindow;
using MinecraftClient.Protocol.Packets.Outbound.HeldItemChange;
using MinecraftClient.Protocol.Packets.Outbound.PlayerBlockPlacement;
using MinecraftClient.Protocol.Packets.Outbound.UseEntity;
using MinecraftClient.Protocol.WorldProcessors.Recipes;
using MinecraftClient.Protocol.WorldProcessors.RegistryProcessors;

namespace MinecraftClient.Character
{
    public enum InventoryConstants
    {
        QuickBarMin = 36,
        QuickBarMax = 44,
        OffHand = 45,
    }

    public class Player
    {
        private readonly IMinecraftCom _protocol;
        private readonly IMinecraftComHandler _handler;

        private const int AttackCd = 999;
        private long _lastAttack;

        public Radar Radar { get; }

        public Crafting Crafting { get; }

        public Container OpenedContainer { get; }

        public Inventory Inventory { get; }

        public float Health { get; set; }

        public int Food { get; set; }


        internal IRegistryProcessor RegistryProcessor { get; }

        public bool IsLoaded { get; private set; }

        public Player(int protocolVersion, IMinecraftCom protocol, IMinecraftComHandler handler)
        {
            RegistryProcessor = VersionsFactory.WorldProcessor<IRegistryProcessor>(protocolVersion);

            _protocol = protocol;
            _handler = handler;

            Radar = new Radar(protocol, handler);

            Inventory = new Inventory(null, protocol, handler);
            OpenedContainer = new Container(Inventory, protocol, handler);
            Crafting = new Crafting(Inventory, protocolVersion, protocol, handler);

            ConsoleIO.WriteLineFormatted("Loaded Registries processor:");
            ConsoleIO.WriteLine($"Version: {RegistryProcessor.MinVersion()}    " +
                                $"Implementation: {RegistryProcessor.GetType().Name}");
        }


        public void SetWindowItems(byte windowId, Dictionary<short, ItemSlot> inv)
        {
            if (0 == windowId)
            {
                Inventory.SetInventory(windowId, inv);
                IsLoaded = true;

                return;
            }

            OpenedContainer.SetInventory(windowId, inv);
            Crafting.SetInventory(windowId, inv);
        }

        public void SetSlot(byte windowId, short slotId, ItemSlot item)
        {
            Inventory.SetSlot(windowId, slotId, item);
            OpenedContainer.SetSlot(windowId, slotId, item);
            Crafting.SetSlot(windowId, slotId, item);
        }

        internal void Stop()
        {
            IsLoaded = false;
        }

        public void UseActiveItem()
        {
            _protocol.SendPacketOut(OutboundTypes.UseItem, null, null);
        }

        public void LookAt(Location loc)
        {
            _handler.UpdateLocation(_handler.GetCurrentLocation(), loc);
        }

        public bool PlaceBlock(Location loc, Hands hand = Hands.Main, BlockFaces faceVector = BlockFaces.Top)
        {
            if (!Settings.TerrainAndMovements)
            {
                return false;
            }

            var block = _handler.GetWorld().GetBlock(loc);
            if (block.IsEmpty() && !Inventory.HasItem(ref hand))
            {
                ConsoleIO.WriteLineFormatted($"Can't place block at {loc.X}:{loc.Y}:{loc.Z} : empty handed");
                return false;
            }

            if (!block.IsEmpty() && !block.CanUse())
            {
                ConsoleIO.WriteLineFormatted(
                    $"Can't use block at {loc.X}:{loc.Y}:{loc.Z} : material is {block.Material()}");
                return false;
            }

            LookAt(loc + new Location(0.5, 0.5, 0.5));
            return _protocol.SendPacketOut(OutboundTypes.PlayerBlockPlacement, null,
                new PlayerBlockPlacementRequest
                {
                    Hand = hand,
                    Location = loc,
                    CursorPosition = new Location(0.5, 0.5, 0.5),
                    FaceVector = faceVector,
                    IsInsideBlock = false,
                });
        }

        public bool Attack(IMob mob)
        {
            if (mob.Position().DistanceSquared(_handler.GetCurrentLocation()) > 16) // radius 4
            {
                return false;
            }

            var now = DateTime.Now.Ticks;
            if (now - _lastAttack < AttackCd * TimeSpan.TicksPerMillisecond)
            {
                return false;
            }

            _protocol.SendPacketOut(OutboundTypes.UseEntity, null, new UseEntityRequest
            {
                Type = UseEntityType.Attack,
                EntityId = mob.Id()
            });

            _lastAttack = now;
            return true;
        }

        public bool Attack(int id)
        {
            var mob = Radar.Get(id);

            if (null == mob)
            {
                return false;
            }


            return Attack(mob);
        }

        public void WindowOpened(byte windowId, WindowType type)
        {
            if (0 == windowId) // Inventory
            {
                return;
            }

            switch (type)
            {
                case WindowType.Crafting:
                    Crafting.WindowOpened(windowId, type);
                    break;
                case WindowType.Generic_9x3:
                case WindowType.Generic_9x6:
                case WindowType.Shulker_Box:
                    OpenedContainer.WindowOpened(windowId, type);
                    break;
            }
        }

        public void WindowClosed(byte windowId)
        {
            if (0 == windowId) // Inventory
            {
                return;
            }

            Crafting.WindowClosed(windowId);
            OpenedContainer?.WindowClosed(windowId);
        }
    }
}