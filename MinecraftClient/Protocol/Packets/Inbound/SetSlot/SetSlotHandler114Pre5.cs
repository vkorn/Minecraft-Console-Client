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
            PacketUtils.readNextByte(packetData); // Always inventory
            var slotId = PacketUtils.readNextShort(packetData);
            var isPresent = PacketUtils.readNextBool(packetData);
            if (!isPresent)
            {
                handler.GetPlayer().Inventory[slotId] = null;
                return null;
            }

            var itemId = PacketUtils.readNextVarInt(packetData);
            var itemsCount = PacketUtils.readNextByte(packetData);
            handler.GetPlayer().Inventory[slotId] = new ItemSlot
            {
                Item = handler.GetPlayer().RegistryProcessor.GetItem(itemId),
                Count = itemsCount
            };

            var noop = new NbtNoop(packetData);
            noop.SkipTag();
            handler.GetPlayer().Inventory[slotId].Item.SetNbt(noop.SkippedData.ToArray());

            return null;
        }
    }
}