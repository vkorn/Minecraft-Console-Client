using System.Collections.Generic;

namespace MinecraftClient.Protocol.WorldProcessors.Recipes
{
    internal abstract class RecipeProcessorFactory : IRecipeProcessorFactory
    {
        protected abstract ProtocolVersions MinVersion { get; }

        ProtocolVersions IWorldProcessor.MinVersion()
        {
            return MinVersion;
        }

        private readonly Dictionary<string, IRecipeParser> _parsers;

        protected RecipeProcessorFactory()
        {
            _parsers = GetParsers();
        }

        protected abstract Dictionary<string, IRecipeParser> GetParsers();

        public IRecipeParser GetParser(string type)
        {
            return !_parsers.ContainsKey(type) ? null : _parsers[type];
        }
    }
}