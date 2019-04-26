using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;
using MinecraftClient.Protocol.Packets.Outbound;

namespace MinecraftClient.Protocol.Packets.Inbound.ConfirmTransaction
{
    internal class ConfirmTransactionHandler114Pre5 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x12;
        protected override InboundTypes PackageType => InboundTypes.ConfirmTransaction;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var cp = new byte[packetData.Count];
            packetData.CopyTo(cp);
            PacketUtils.readNextByte(packetData);
            PacketUtils.readNextShort(packetData);
            var accepted = PacketUtils.readNextBool(packetData);
            if (accepted)
            {
                return null;
            }

            ConsoleIO.WriteLineFormatted("§cServer rejected the transaction");
            protocol.SendPacketOut(OutboundTypes.ConfirmTransaction, cp, null);
            return null;
        }
    }
}