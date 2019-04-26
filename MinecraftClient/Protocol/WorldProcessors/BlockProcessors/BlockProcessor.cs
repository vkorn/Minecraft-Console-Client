namespace MinecraftClient.Protocol.WorldProcessors.BlockProcessors
{
    internal abstract class BlockProcessor : IBlockProcessor
    {
        protected abstract ProtocolVersions MinVersion { get; }

        ProtocolVersions IWorldProcessor.MinVersion()
        {
            return MinVersion;
        }

        public abstract IBlock CreateBlock(short blockId);

        public abstract IBlock CreateBlockFromMetadata(short type, byte metadata);
        public abstract IBlock CreateBlockFromIdMetadata(ushort typeAndMeta);
        public abstract IBlock CreateAirBlock();
    }
}