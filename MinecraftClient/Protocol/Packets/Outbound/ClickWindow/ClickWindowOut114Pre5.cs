using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.ClickWindow
{
    /// <summary>
    /// Items manipulations https://wiki.vg/Protocol#Click_Window
    /// </summary>
    internal class ClickWindowOut114Pre5 : OutboundGamePacket
    {
        private readonly Dictionary<byte, short> _actionNumbers;

        public ClickWindowOut114Pre5()
        {
            _actionNumbers = new Dictionary<byte, short>();
        }

        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x09;
        protected override OutboundTypes PackageType => OutboundTypes.ClickWindow;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            // TODO: Do we need all modes? Now only left clicks are supported in order to provide SwapItems functionality

            return PacketUtils.concatBytes(
                new[] {((ClickWindowRequest) data).WindowId},
                PacketUtils.getShort(((ClickWindowRequest) data).SlotNum),
                new byte[] {0}, // Left click
                PacketUtils.getShort(GetNextActionNumber(((ClickWindowRequest) data).WindowId)), // Action number
                ((ClickWindowRequest) data).ShiftPressed ? PacketUtils.getVarInt(1) : PacketUtils.getVarInt(0),
                // Item data
                null == ((ClickWindowRequest) data).Item
                    ? PacketUtils.getBool(false)
                    : PacketUtils.concatBytes(
                        PacketUtils.getBool(true),
                        PacketUtils.getVarInt(((ClickWindowRequest) data).Item.Item.Id()),
                        new[] {(byte) ((ClickWindowRequest) data).Item.Count},
                        ((ClickWindowRequest) data).Item.Item.Nbt())
            );

            // TODO: some straight up bs happening with custom NBT, packets are identical but server rejects them
        }

        private short GetNextActionNumber(byte windowId)
        {
            if (!_actionNumbers.TryGetValue(windowId, out _))
            {
                _actionNumbers[windowId] = 0;
            }

            _actionNumbers[windowId]++;
            return _actionNumbers[windowId];
        }
    }
}