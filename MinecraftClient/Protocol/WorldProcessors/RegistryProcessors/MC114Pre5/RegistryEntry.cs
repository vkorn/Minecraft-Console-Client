using Newtonsoft.Json;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors.MC114Pre5
{
    internal class RegistryEntry
    {
        [JsonProperty("protocol_id")] public int Id { get; set; }
    }
}