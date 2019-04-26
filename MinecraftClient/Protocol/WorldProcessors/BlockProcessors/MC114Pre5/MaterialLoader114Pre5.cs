using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNet.Globbing;
using Newtonsoft.Json;

namespace MinecraftClient.Protocol.WorldProcessors.BlockProcessors.MC114Pre5
{
    internal class MaterialLoader114Pre5 : MaterialLoader
    {
        protected override string ResourceName => "blocks.json.zip";

        private Dictionary<int, MaterialRepresentation> _materials;

        public MaterialRepresentation GetMaterial(int id)
        {
            if (!_materials.TryGetValue(id, out var mat))
            {
                mat = new MaterialRepresentation
                {
                    Id = id,
                    Name = "unknown",
                    Properties = new Dictionary<string, string>(),
                    Material = Material.Unknown,
                };
            }

            return mat;
        }

        protected override void ProcessData(MemoryStream ms)
        {
            _materials = new Dictionary<int, MaterialRepresentation>();

            var res = JsonConvert.DeserializeObject<Dictionary<string, JsonBlockObject>>(
                Encoding.UTF8.GetString(ms.ToArray()));

            var globs = new Dictionary<Material, List<Glob>>();

            foreach (var rule in Rules())
            {
                globs.Add(rule.Key, new List<Glob>());

                foreach (var g in rule.Value)
                {
                    globs[rule.Key].Add(Glob.Parse(g));
                }
            }

            foreach (var jsonBlockObject in res)
            {
                foreach (var jsonBlockObjectState in jsonBlockObject.Value.States)
                {
                    var matR = new MaterialRepresentation
                    {
                        Material = Material.Unknown,
                        Name = jsonBlockObject.Key.Substring("minecraft:".Length),
                        Properties = jsonBlockObjectState.Properties
                    };

                    var found = false;

                    foreach (var g in globs)
                    {
                        if (found) break;

                        foreach (var glob in g.Value)
                        {
                            if (glob.IsMatch(matR.Name))
                            {
                                found = true;
                                matR.Material = g.Key;
                                break;
                            }
                        }
                    }

                    _materials.Add(jsonBlockObjectState.Id, matR);
                }
            }
        }

        private Dictionary<Material, List<string>> Rules()
        {
            return new Dictionary<Material, List<string>>
            {
                {
                    Material.Air, new List<string> {"air", "void_air", "cave_air"}
                },
                {
                    Material.Solid, new List<string>
                    {
                        "stone", "*granite", "*diorite", "*andesite",
                        "*dirt", "podzol", "*cobblestone", "*planks", "*sand", "gravel",
                        "*_log", "*_wood", "*_leaves", "*sponge", "*glass", "*sandstone",
                        "*piston*", "fern", "*bush", "*_wool", "*_block", "bricks", "tnt",
                        "bookshelf", "obsidian", "spawner", "*_ice", "clay", "jukebox",
                        "*_pumpkin", "netherrack", "glowstone", "jack_o_lantern", "*_bricks",
                        "infested_*", "melon", "end_stone", "redstone_lamp", "*anvil", "*_pillar",
                        "*terracotta*", "*concrete*"
                    }
                },
                {
                    Material.Walkable, new List<string>
                    {
                        "*sapling", "*rail", "cobweb", "dandelion", "poppy", "blue_orchid",
                        "allium", "azure_bluet", "*_tulip", "oxeye_daisy", "cornflower",
                        "lily_of_the_valley", "*_mushroom", "*_torch", "*_stairs", "redstone_wire",
                        "wheat", "*_pressure_plate", "snow", "sugar_cane",
                        "*_portal", "*_stem", "vine", "lily_pad", "nether_wart", "potted_*",
                        "daylight_detector", "hopper", "*slab", "*carpet", "*grass", "*coral_fan", "*coral_wall_fan"
                    }
                },
                {
                    Material.NonWalkable, new List<string>
                    {
                        "*_sign", "*_door", "ladder", "*_fence", "*glass_pane", "iron_bars", "*_gate",
                        "*_skull", "*_head", "*_banner", "*_wall"
                    }
                },
                {
                    Material.Undestroyable, new List<string>
                    {
                        "bedrock", "end_portal_frame"
                    }
                },
                {
                    Material.Water, new List<string> {"water"}
                },
                {
                    Material.Lava, new List<string> {"lava"}
                },
                {
                    Material.Ore, new List<string> {"*_ore"}
                },
                {
                    Material.HasInterface, new List<string>
                    {
                        "dispenser", "chest", "crafting_table",
                        "furnace", "enchanting_table", "brewing_stand",
                        "dropper", "*shulker_box", "loom", "barrel", "smoker", "blast_furnace",
                        "cartography_table", "fletching_table", "grindstone", "lectern",
                        "smithing_table", "stonecutter"
                    }
                },
                {
                    Material.CanUse, new List<string>
                    {
                        "lever", "*_button", "cake", "repeater", "*_trapdoor", "cauldron", "comparator", "bell",
                        "composter"
                    }
                },
                {
                    Material.Bed, new List<string> {"*_bed"}
                },
                {
                    Material.CanHarm, new List<string>
                    {
                        "wither_rose", "fire", "cactus", "campfire",
                    }
                }
            };
        }

        public MaterialLoader114Pre5(ProtocolVersions minVersion) : base(minVersion)
        {
        }
    }
}