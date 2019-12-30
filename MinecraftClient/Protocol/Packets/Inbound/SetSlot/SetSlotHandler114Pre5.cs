using System.Collections.Generic;
using MinecraftClient.Character;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.SetSlot
{
    internal class SetSlotHandler114Pre5 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x16;
        protected override InboundTypes PackageType => InboundTypes.SetSlot;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var windowId = PacketUtils.readNextByte(packetData);
            var slotId = PacketUtils.readNextShort(packetData);
            if (-1 == slotId && -1 == windowId)
            {
                return null;
            }

            var isPresent = PacketUtils.readNextBool(packetData);
            if (!isPresent)
            {
                handler.GetPlayer().SetSlot(windowId, slotId, null);
                return null;
            }

            var itemId = PacketUtils.readNextVarInt(packetData);
            var itemsCount = PacketUtils.readNextByte(packetData);
            var item = new ItemSlot
            {
                Item = handler.GetPlayer().RegistryProcessor.GetItem(itemId),
                Count = itemsCount
            };

            var noop = new NbtNoop(packetData);
            noop.SkipTag();
            item.Item.SetNbt(noop.SkippedData.ToArray());
            handler.GetPlayer().SetSlot(windowId, slotId, item);
            return null;
        }
    }
}