namespace MinecraftClient.Protocol.Packets.Outbound.CraftRecipe
{
    internal class CraftRecipeRequest : IOutboundRequest
    {
        public byte WindowId { get; set; }
        public string RecipeId { get; set; }
        public bool MakeAll { get; set; }
    }
}