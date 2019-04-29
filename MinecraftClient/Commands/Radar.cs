namespace MinecraftClient.Commands
{
    public class Radar : Command
    {
        public override string CMDName => "Radar";
        public override string CMDDesc => "radar: shows surrounding entities";

        public override string Run(McTcpClient handler, string command)
        {
            var me = handler.GetCurrentLocation();
            foreach (var mob in handler.GetPlayer().Radar.Mobs)
            {
                ConsoleIO.WriteLineFormatted(
                    $"{mob.Key} ({mob.Value.Uuid()}): {mob.Value.Name()} at {mob.Value.Position().ToString()} " +
                    $"({(int)mob.Value.Position().Distance(me)} blocks)", true, false);
            }

            return "Done";
        }
    }
}