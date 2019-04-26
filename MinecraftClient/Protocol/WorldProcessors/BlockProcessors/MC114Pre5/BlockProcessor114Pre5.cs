namespace MinecraftClient.Protocol.WorldProcessors.BlockProcessors.MC114Pre5
{
    internal class BlockProcessor114Pre5 : BlockProcessor
    {
        private readonly MaterialLoader114Pre5 _loader;
        private const short AirId = 0;

        public BlockProcessor114Pre5()
        {
            _loader = new MaterialLoader114Pre5(MinVersion);
        }


        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;

        public override IBlock CreateBlock(short blockId)
        {
            return new Block114Pre5(_loader.GetMaterial(blockId));
        }

        public override IBlock CreateBlockFromMetadata(short type, byte metadata)
        {
            throw new System.NotImplementedException();
        }

        public override IBlock CreateBlockFromIdMetadata(ushort typeAndMeta)
        {
            throw new System.NotImplementedException();
        }

        public override IBlock CreateAirBlock()
        {
            return new Block114Pre5(_loader.GetMaterial(AirId));
        }
    }
}