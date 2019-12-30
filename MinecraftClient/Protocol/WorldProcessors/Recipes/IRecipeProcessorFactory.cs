namespace MinecraftClient.Protocol.WorldProcessors.Recipes
{
    internal interface IRecipeProcessorFactory : IWorldProcessor
    {
        IRecipeParser GetParser(string type);
    }
}