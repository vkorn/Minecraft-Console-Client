using MinecraftClient.Protocol.Handlers;

namespace MinecraftClient.Protocol.Packets.Outbound.PlayerPositionAndLook
{
    internal class PlayerPositionAndLookOut114pre5 : PlayerPositionAndLookOut113Pre7
    {
        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override int PacketId => 0x12;
    }
}