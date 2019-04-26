namespace MinecraftClient.Commands
{
    public class Use : Command
    {
        public override string CMDName => "Use";
        public override string CMDDesc => "use: using current active item (always with the main hand)";

        public override string Run(McTcpClient handler, string command)
        {
            handler.GetPlayer().UseActiveItem();
            return "Done";
        }
    }
}