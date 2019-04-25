using System;

namespace MinecraftClient.Commands
{
    public abstract class PositionedCommand : Command
    {
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

            if (3 != args.Length)
            {
                return "Wrong arguments count: " + CMDDesc;
            }

            try
            {
                var x = Convert.ToDouble(args[0]);
                var y = Convert.ToDouble(args[1]);
                var z = Convert.ToDouble(args[2]);
                return Execute(handler, x, y, z);
            }
            catch (Exception ex)
            {
                return "Wrong arguments: " + ex.Message;
            }
        }

        internal abstract string Execute(McTcpClient handler, double x, double y, double z);
    }
}