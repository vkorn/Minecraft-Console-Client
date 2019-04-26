using System.IO;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors
{
    internal class RegistryProcessorDummy : RegistryProcessor
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.Zero;
        protected override string ResourceName => "";

        public override IItem GetItem(int id)
        {
            return null;
        }

        protected override void ProcessData(MemoryStream ms)
        {
            throw new System.NotImplementedException();
        }
    }
}