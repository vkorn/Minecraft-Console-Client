using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.HeldItemChange
{
    internal class HeldItemChangeHandler114Pre5 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x3F;
        protected override InboundTypes PackageType => InboundTypes.HeldItemChange;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var id = PacketUtils.readNextByte(packetData);
            handler.GetPlayer().Inventory.ActiveSlot = (short) (id + 1);
            return null;
        }
    }
}