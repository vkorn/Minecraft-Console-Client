using System;
using MinecraftClient.Protocol.WorldProcessors.DataConverters.Location;

namespace MinecraftClient.Protocol.WorldProcessors.DataConverters
{
    internal class DataHelpers
    {
        private static readonly Lazy<DataHelpers> Lazy = new Lazy<DataHelpers>(() => new DataHelpers());
        public static DataHelpers Instance => Lazy.Value;

        public ILocationConverter LocationConverter { get; private set; }

        private DataHelpers()
        {
        }

        public void Init(int protocolVersion)
        {
            LocationConverter = VersionsFactory.WorldProcessor<ILocationConverter>(protocolVersion);
            ConsoleIO.WriteLineFormatted("Loaded Location converter:");
            ConsoleIO.WriteLine($"Version: {LocationConverter.MinVersion()}    " +
                                $"Implementation: {LocationConverter.GetType().Name}");
        }
    }
}