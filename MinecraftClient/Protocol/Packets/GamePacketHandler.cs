namespace MinecraftClient.Protocol.Packets
{
    internal abstract class GamePacketHandler : IGamePacketHandler
    {
        protected abstract ProtocolVersions MinVersion { get; }
        protected abstract int PacketId { get; }

        ProtocolVersions IGamePacketHandler.MinVersion()
        {
            return MinVersion;
        }

        int IGamePacketHandler.PacketId()
        {
            return PacketId;
        }

        public abstract int PacketIntType();
    }
}