namespace MinecraftClient.Protocol.WorldProcessors.DataConverters.Location
{
    abstract class LocationConverter : ILocationConverter
    {
        protected abstract ProtocolVersions MinVersion { get; }

        ProtocolVersions IWorldProcessor.MinVersion()
        {
            return MinVersion;
        }

        public abstract long LocationToLong(Mapping.Location location);
        public abstract Mapping.Location LongToLocation(long val);
    }
}