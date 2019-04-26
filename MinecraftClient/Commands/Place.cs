using MinecraftClient.Mapping;

namespace MinecraftClient.Commands
{
    public class Place : PositionedCommand
    {
        public override string CMDName => "place";

        public override string CMDDesc =>
            "place <x> <y> <z>: placing currently held block or activating an item at coordinates";

        internal override string Execute(McTcpClient handler, double x, double y, double z)
        {
            handler.GetPlayer().PlaceBlock(new Location(x, y, z));
            return "Done";
        }
    }
}