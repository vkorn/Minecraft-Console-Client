using System;
using System.IO;
using MinecraftClient.Mapping;

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

        public override IMob GetEntity(int type, int id, Guid uuid, Location position)
        {
            return null;
        }

        public override byte GetWindowTypeID(WindowType type)
        {
            return 0;
        }

        public override WindowType GetWindowType(byte windowTypeId)
        {
            return WindowType.Anvil;
        }

        protected override void ProcessData(MemoryStream ms)
        {
            throw new System.NotImplementedException();
        }
    }
}