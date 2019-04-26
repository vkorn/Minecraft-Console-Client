namespace MinecraftClient.Protocol.WorldProcessors.DataConverters.Location
{
    internal interface ILocationConverter: IWorldProcessor
    {
        long LocationToLong(Mapping.Location location);
        Mapping.Location LongToLocation(long val);
    }
}