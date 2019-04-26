using System.Collections.Generic;

namespace MinecraftClient.Protocol.WorldProcessors.BlockProcessors.MC114Pre5
{
    internal class Block114Pre5 : IBlock
    {
        private readonly MaterialRepresentation _mat;

        public Block114Pre5(MaterialRepresentation mat)
        {
            _mat = mat;
        }


        public bool CanHarmPlayers()
        {
            return _mat.Material == Material.CanHarm || _mat.Material == Material.Lava;
        }

        public bool IsSolid()
        {
            return _mat.Material == Material.Solid || _mat.Material == Material.Walkable ||
                   _mat.Material == Material.Undestroyable ||
                   _mat.Material == Material.Ore || _mat.Material == Material.HasInterface ||
                   _mat.Material == Material.CanUse ||
                   _mat.Material == Material.Bed;
        }

        public bool IsLiquid()
        {
            return _mat.Material == Material.Water || _mat.Material == Material.Lava;
        }

        string IBlock.Material()
        {
            return _mat.Name;
        }

        public string Property(string name)
        {
            return _mat.Properties.TryGetValue(name.ToLowerInvariant(), out var val) ? val : "";
        }

        public int Id()
        {
            return _mat.Id;
        }

        public Dictionary<string, string> Properties()
        {
            return _mat.Properties;
        }
    }
}