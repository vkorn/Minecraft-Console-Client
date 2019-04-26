using MinecraftClient.Mapping;

namespace MinecraftClient.Protocol.Packets.Outbound.PlayerBlockPlacement
{
    internal struct PlayerBlockPlacementRequest : IOutboundRequest
    {
        public Hands Hand { get; set; }
        public Location Location { get; set; }
        public BlockFaces FaceVector { get; set; }
        public Location CursorPosition { get; set; }

        /// <summary>
        /// True for scaffolding only.
        /// </summary>
        public bool IsInsideBlock { get; set; }
    }
}