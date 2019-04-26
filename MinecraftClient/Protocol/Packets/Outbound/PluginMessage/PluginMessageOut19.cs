using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.PluginMessage
{
    internal class PluginMessageOut19 : PluginMessageOut18
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC19;
        protected override int PacketId => 0x09;
    }
}