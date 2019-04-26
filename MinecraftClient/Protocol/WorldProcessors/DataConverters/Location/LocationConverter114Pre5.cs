namespace MinecraftClient.Protocol.WorldProcessors.DataConverters.Location
{
    internal class LocationConverter114Pre5 : LocationConverter
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;

        public override long LocationToLong(Mapping.Location location)
        {
            return (((long) location.X & 0x3FFFFFF) << 38) | (((long) location.Z & 0x3FFFFFF) << 12) |
                   ((long) location.Y & 0xFFF);
        }

        public override Mapping.Location LongToLocation(long val)
        {
            var x = (int) (val >> 38);
            var y = (int) (val & 0xFFF);
            var z = (int) (val << 26 >> 38);
            if (x >= 33554432)
                x -= 67108864;
            if (y >= 2048)
                y -= 4096;
            if (z >= 33554432)
                z -= 67108864;
            return new Mapping.Location(x, y, z);
        }
    }
}