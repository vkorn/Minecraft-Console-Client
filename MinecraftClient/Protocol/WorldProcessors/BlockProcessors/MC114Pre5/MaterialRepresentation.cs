using System.Collections.Generic;

namespace MinecraftClient.Protocol.WorldProcessors.BlockProcessors.MC114Pre5
{
    internal struct MaterialRepresentation
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public Material Material { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}