using System.Collections.Generic;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors.MC114Pre5
{
    internal class RegistryEntriesList
    {
        public Dictionary<string, RegistryEntry> Entries { get; set; }

        public RegistryEntriesList()
        {
            Entries = new Dictionary<string, RegistryEntry>();
        }
    }
}