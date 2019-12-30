using System;

namespace MinecraftClient.Commands
{
    public class Hold : Command
    {
        public override string CMDName => "Hold";

        public override string CMDDesc =>
            "hold <number>: select an item in a quick bar where <number> is between 1 and 9";

        public override string Run(McTcpClient handler, string command)
        {
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
                var num = Convert.ToInt16(args[0]);
                if (num < 1 || num > 9)
                {
                    return "Invalid number: " + CMDDesc;
                }

                handler.GetPlayer().Inventory.PickActiveItem(num);
                return "Done";
            }
            catch (Exception ex)
            {
                return "Wrong arguments: " + ex.Message;
            }
        }
    }
}