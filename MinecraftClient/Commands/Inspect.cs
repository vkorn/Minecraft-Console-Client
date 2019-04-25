namespace MinecraftClient.Commands
{
    public class Inspect : PositionedCommand
    {
        public override string CMDName => "Inspect";
        public override string CMDDesc => "inspect <X> <Y> <Z>: inspecting block at coordinates";

        internal override string Execute(McTcpClient handler, double x, double y, double z)
        {
            var block = handler.GetWorld().GetBlock(x, y, z);
            if (null == block)
            {
                return "No data for this block";
            }

            var props = "";
            foreach (var property in block.Properties())
            {
                props += $" {property.Key}: {property.Value}";
            }

            return $"Material: {block.Material()}" + (string.IsNullOrEmpty(props) ? "" : $"\nProperties:{props}");
        }
    }
}