using System.Collections.Generic;

namespace MinecraftClient.Protocol.WorldProcessors.BlockProcessors._114Pre5
{
    internal class JsonBlockObjectState
    {
        public Dictionary<string, string> Properties { get; set; }
        public int Id { get; set; }

        public JsonBlockObjectState()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}