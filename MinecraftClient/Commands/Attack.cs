using System;

namespace MinecraftClient.Commands
{
    public class Attack : Command
    {
        public override string CMDName => "Attack";
        public override string CMDDesc => "attack <id>: attacking a mob";

        public override string Run(McTcpClient handler, string command)
        {
            if (!Settings.TerrainAndMovements)
            {
                return "Terrain processing is disabled";
            }

            if (!hasArg(command))
            {
                return CMDDesc;
            }

            var args = getArgs(command);

            if (1 != args.Length)
            {
                return "Wrong arguments count: " + CMDDesc;
            }

            try
            {
                var id = Convert.ToInt32(args[0]);
                return handler.GetPlayer().Attack(id) ? "Done" : "Failed to attack";
            }
            catch (Exception ex)
            {
                return "Wrong arguments: " + ex.Message;
            }
        }
    }
}