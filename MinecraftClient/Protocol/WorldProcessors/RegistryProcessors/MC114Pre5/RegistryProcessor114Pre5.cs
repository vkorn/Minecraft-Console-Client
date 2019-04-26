using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNet.Globbing;
using Newtonsoft.Json;

namespace MinecraftClient.Protocol.WorldProcessors.RegistryProcessors.MC114Pre5
{
    internal class RegistryProcessor114Pre5 : RegistryProcessor
    {
        protected Dictionary<int, Item> _items;

        protected override ProtocolVersions MinVersion => ProtocolVersions.MC114Pre5;
        protected override string ResourceName => "registries.json.zip";

        public override IItem GetItem(int id)
        {
            return _items.TryGetValue(id, out var item) ? item : null;
        }

        protected override void ProcessData(MemoryStream ms)
        {
            var res = JsonConvert.DeserializeObject<Dictionary<string, RegistryEntriesList>>(
                Encoding.UTF8.GetString(ms.ToArray()));

            ProcessItems(res);
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
    }
}