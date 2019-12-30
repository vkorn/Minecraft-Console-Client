namespace MinecraftClient.Commands
{
    public class Status : Command
    {
        public override string CMDName => "Status";
        public override string CMDDesc => "status: returns player status (health, food) as well as inventory";

        public override string Run(McTcpClient handler, string command)
        {
            var player = handler.GetPlayer();

            if (!player.IsLoaded)
            {
                return "Player's data is not available yet";
            }

            ConsoleIO.WriteLineFormatted($"Health: {GetColor((int) player.Health)}{(int) player.Health:00}", true,
                false);
            ConsoleIO.WriteLineFormatted($"Food:   {GetColor(player.Food)}{player.Food:00}", true, false);
            
            ConsoleIO.WriteLineFormatted("§6Quick bar:", true, false);
            foreach (var item in player.Inventory.QuickBar())
            {
                ConsoleIO.WriteLineFormatted(
                    $"{(item.Key - 35 == player.Inventory.ActiveSlot ? "*" : "")}{item.Key - 35} ({item.Key}): x{item.Value.Count:00} {item.Value.Item.Name()}",
                    true, false);
            }
            
            ConsoleIO.WriteLineFormatted("§6Inventory:", true, false);
            foreach (var item in player.Inventory.InventoryOnly())
            {
                ConsoleIO.WriteLineFormatted($"{item.Key}: x{item.Value.Count:00} {item.Value.Item.Name()}", true, false);
            }
            
            ConsoleIO.WriteLineFormatted("§6Equipped:", true, false);
            foreach (var item in player.Inventory.Equip())
            {
                ConsoleIO.WriteLineFormatted($"{item.Key}: x{item.Value.Count:00} {item.Value.Item.Name()}", true, false);
            }

            return "";
        }

        private static string GetColor(int value)
        {
            if (value >= 15)
            {
                return "§a";
            }

            if (value >= 10)
            {
                return "§b";
            }

            return value >= 5 ? "§e" : "§c";
        }
    }
}