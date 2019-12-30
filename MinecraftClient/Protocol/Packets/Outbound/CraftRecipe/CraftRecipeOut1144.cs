using System.Collections.Generic;
using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.CraftRecipe
{
    internal class CraftRecipeOut1144 : OutboundGamePacket
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC1144;
        protected override int PacketId => 0x18;
        protected override OutboundTypes PackageType => OutboundTypes.CraftRecipe;

        public override IEnumerable<byte> TransformData(IEnumerable<byte> packetData, IOutboundRequest data)
        {
            return PacketUtils.concatBytes(
                new[] {((CraftRecipeRequest) data).WindowId},
                PacketUtils.getString(((CraftRecipeRequest) data).RecipeId),
                PacketUtils.getBool(((CraftRecipeRequest) data).MakeAll)
            );
        }
    }
}