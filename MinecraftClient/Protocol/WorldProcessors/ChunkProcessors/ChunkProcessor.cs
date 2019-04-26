using MinecraftClient.Protocol.Packets.Inbound.ChunkData;

namespace MinecraftClient.Protocol.WorldProcessors.ChunkProcessors
{
    internal abstract class ChunkProcessor: IChunkProcessor
    {   
        protected abstract ProtocolVersions MinVersion { get; } 
        
        ProtocolVersions IWorldProcessor.MinVersion()
        {
            return MinVersion;
        }

        public abstract void Process(IMinecraftComHandler handler, ChunkDataResult data);
    }
}