namespace MinecraftClient.Protocol.Packets
{
    internal interface IGamePacketHandler
    {
        ProtocolVersions MinVersion();

        int PacketId();

        int PacketIntType();
    }
}