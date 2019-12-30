using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Inbound.CraftRecipe
{
    internal class CraftRecipeHandler1144 : InboundGamePacketHandler
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC1144;
        protected override int PacketId => 0x30;
        protected override InboundTypes PackageType => InboundTypes.CraftRecipeResponse;

        public override IInboundData Handle(IProtocol protocol, IMinecraftComHandler handler, List<byte> packetData)
        {
            var windowId = PacketUtils.readNextByte(packetData);
            var recipeId = PacketUtils.readNextString(packetData);
            handler.GetPlayer().Crafting.ConfirmLastRecipe(windowId, recipeId);
            return null;
        }
    }
}