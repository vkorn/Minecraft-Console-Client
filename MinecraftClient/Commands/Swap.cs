using System;

namespace MinecraftClient.Commands
{
    public class Swap : Command
    {
        public override string CMDName => "Swap";
        public override string CMDDesc => "Swap <slot1> <slot2>: swapping two inventory slots";

        public override string Run(McTcpClient handler, string command)
        {
            if (!hasArg(command))
            {
                return CMDDesc;
            }

            var args = getArgs(command);

            if (2 != args.Length)
            {
                return "Wrong arguments count: " + CMDDesc;
            }

            try
            {
                var i1 = Convert.ToInt16(args[0]);
                var i2 = Convert.ToInt16(args[1]);
                return handler.GetPlayer().SwapItems(i1, i2) ? "Success" : "Failure";
            }
            catch (Exception ex)
            {
                return "Wrong arguments: " + ex.Message;
            }
        }
    }
}