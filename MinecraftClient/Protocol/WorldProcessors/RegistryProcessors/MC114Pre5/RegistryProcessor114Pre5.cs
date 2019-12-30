using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotNet.Globbing;
using MinecraftClient.Mapping;
using Newtonsoft.Json;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors.MC114Pre5
{
    internal class RegistryProcessor114Pre5 : RegistryProcessor
    {
        protected Dictionary<int, Item> _items;
        protected Dictionary<int, MobEntry> _entities;
        protected Dictionary<WindowType, byte> _windows;

        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override string ResourceName => "registries.json.zip";

        public override IItem GetItem(int id)
        {
            return _items.TryGetValue(id, out var item) ? item : null;
        }

        public override IMob GetEntity(int type, int id, Guid uuid, Location position)
        {
            if (!_entities.TryGetValue(type, out var entity))
            {
                return null;
            }

            return new Mob(entity, id, uuid, position);
        }

        public override byte GetWindowTypeID(WindowType type)
        {
            if (!_windows.TryGetValue(type, out var id))
            {
                return 0;
            }

            return id;
        }

        public override WindowType GetWindowType(byte windowTypeId)
        {
            return _windows.FirstOrDefault(x => x.Value == windowTypeId).Key;
        }

        protected override void ProcessData(MemoryStream ms)
        {
            var res = JsonConvert.DeserializeObject<Dictionary<string, RegistryEntriesList>>(
                Encoding.UTF8.GetString(ms.ToArray()));

            ProcessItems(res);
            ProcessEntities(res);
            ProcessWindows(res);
        }

        protected virtual void ProcessWindows(Dictionary<string, RegistryEntriesList> entries)
        {
            _windows = new Dictionary<WindowType, byte>();

            if (!entries.TryGetValue("minecraft:menu", out var menus))
            {
                return;
            }

            foreach (var menu in menus.Entries)
            {
                var name = menu.Key.Substring("minecraft:".Length);
                if (!Enum.TryParse(name, true, out WindowType type))
                {
                    continue;
                }

                if (_windows.ContainsKey(type))
                {
                    continue;
                }

                _windows.Add(type, (byte) menu.Value.Id);
            }
        }

        protected virtual void ProcessEntities(Dictionary<string, RegistryEntriesList> entries)
        {
            _entities = new Dictionary<int, MobEntry>();
            if (!entries.TryGetValue("minecraft:entity_type", out var entities))
            {
                return;
            }

            foreach (var entity in entities.Entries)
            {
                var name = entity.Key.Substring("minecraft:".Length);

                foreach (var mobAttribute in _mobAttributes)
                {
                    var found = false;

                    foreach (var attr in mobAttribute.Value)
                    {
                        if (string.Equals(attr, name))
                        {
                            found = true;
                            _entities.Add(entity.Value.Id, new MobEntry
                            {
                                Name = name,
                                Id = entity.Value.Id,
                                Type = mobAttribute.Key,
                            });

                            break;
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                }
            }
        }

        protected virtual void ProcessItems(Dictionary<string, RegistryEntriesList> entries)
        {
            _items = new Dictionary<int, Item>();

            if (!entries.TryGetValue("minecraft:item", out var items))
            {
                return;
            }

            var globs = new Dictionary<ItemAttributes, List<Glob>>();
            foreach (var attribute in _itemAttributes)
            {
                globs[attribute.Key] = new List<Glob>();
                foreach (var subs in attribute.Value)
                {
                    globs[attribute.Key].Add(Glob.Parse(subs));
                }
            }


            foreach (var item in items.Entries)
            {
                var name = item.Key.Substring("minecraft:".Length);

                var attributes = ItemAttributes.Unknown;
                foreach (var g in globs)
                {
                    foreach (var glob in g.Value)
                    {
                        if (glob.IsMatch(name))
                        {
                            attributes |= g.Key;
                            break;
                        }
                    }
                }

                _items[item.Value.Id] = new Item114Pre5(attributes, item.Value.Id, name);
            }
        }

        private static readonly Dictionary<ItemAttributes, List<string>> _itemAttributes =
            new Dictionary<ItemAttributes, List<string>>
            {
                {
                    ItemAttributes.Consumable, new List<string>
                    {
                        "*apple", "baked_*", "beetroot", "beetroot_soup", "bread", "carrot", "chorus_fruit", "cooked_*",
                        "cookie", "dried_kelp", "melon_slice", "*_stew", "*potato", "pufferfish", "pumpkin_pie", "beef",
                        "chicken", "cod", "mutton", "porkchop", "rabbit", "salmon", "rotten_flesh", "spider_eye",
                        "tropical_fish", "sweet_berries"
                    }
                },
                {
                    ItemAttributes.CanHarm, new List<string>
                    {
                        "rotten_flesh", "pufferfish", "spider_eye", "suspicious_stew"
                    }
                }
            };

        private static readonly Dictionary<MobTypes, List<string>> _mobAttributes =
            new Dictionary<MobTypes, List<string>>
            {
                {
                    MobTypes.Entity, new List<string>
                    {
                        "armor_stand", "arrow", "experience_orb", "item", "item_frame"
                    }
                },
                {
                    MobTypes.Peaceful, new List<string>
                    {
                        "bat", "cat", "chicken", "cod", "cow", "dolphin", "fox", "panda", "parrot", "pig",
                        "pufferfish", "polar_bear", "rabbit", "salmon", "sheep", "silverfish", "snow_golem", "squid",
                        "trader_llama", "tropical_fish", "turtle", "villager", "iron_golem", "wandering_trader",
                        "wolf"
                    }
                },
                {
                    MobTypes.Mob, new List<string>
                    {
                        "blaze", "cave_spider", "creeper", "drowned", "elder_guardian", "enderman",
                        "endermite", "evoker", "ghast", "guardian", "husk", "illusioner", "magma_cube",
                        "minecart", "zombie_pigman", "shulker", "skeleton", "slime", "spider", "stray",
                        "vindicator", "pillager", "wither", "wither_skeleton", "zombie", "zombie_villager",
                        "phantom", "ravager"
                    }
                },
                {
                    MobTypes.Bobber, new List<string>
                    {
                        "fishing_bobber"
                    }
                },
                {
                    MobTypes.Player, new List<string>
                    {
                        "player",
                    }
                },
                {
                    MobTypes.CanRide, new List<string>
                    {
                        "boat", "minecart"
                    }
                },
                {
                    MobTypes.Horseish, new List<string>
                    {
                        "llama", "mule", "horse", "donkey"
                    }
                }
            };
    }
}