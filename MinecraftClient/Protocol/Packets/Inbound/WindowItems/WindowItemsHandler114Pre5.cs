using System.Collections.Generic;
using MinecraftClient.Character;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.WindowItems
{
    internal class WindowItemsHandler114Pre5 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x14;
        protected override InboundTypes PackageType => InboundTypes.WindowItems;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var windowId = PacketUtils.readNextByte(packetData);
            var slotsCount = PacketUtils.readNextShort(packetData);
            var noop = new NbtNoop(packetData);
            var inventory = new Dictionary<short, ItemSlot>();
            for (short ii = 0; ii < slotsCount; ii++)
            {
                var isPresent = PacketUtils.readNextBool(packetData);
                if (!isPresent)
                {
                    inventory[ii] = null;
                    continue;
                }

                var itemId = PacketUtils.readNextVarInt(packetData);
                var itemsCount = PacketUtils.readNextByte(packetData);
                inventory[ii] = new ItemSlot
                {
                    Item = handler.GetPlayer().RegistryProcessor.GetItem(itemId),
                    Count = itemsCount
                };
                
                noop.SkipTag();
                inventory[ii].Item.SetNbt(noop.SkippedData.ToArray());
                noop.Reset();
            }

            handler.GetPlayer().SetWindowItems(windowId, inventory);

            return null;
        }
    }
}