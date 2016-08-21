using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MoBot.Structure.Game.Items
{
    public class Item
    {
        protected static readonly Dictionary<string, Item> Extension = new Dictionary<string, Item> { { "minecraft:bread", new ItemFood() }, { "minecraft:apple", new ItemFood() } };
        private static readonly Dictionary<int, Item> ItemRegistry = new Dictionary<int, Item>();

        public static IEnumerable<Item> Items => ItemRegistry.Values;

        public static void LoadItems()
        {
            var materials = new Dictionary<string, float>();

            using (var file = File.OpenText(Settings.MaterialsPath))
            using (var reader = new JsonTextReader(file))
            {
                var deserializer = JsonSerializer.Create();
                var materialDump = deserializer.Deserialize<MaterialInfo[]>(reader);
                foreach (var material in materialDump)
                {
                    materials.Add(material.name, material.effectiveness);
                }
            }

            using (var file = File.OpenText(Settings.ItemsPath))
            using (var reader = new JsonTextReader(file))
            {
                var deserializer = JsonSerializer.Create();
                var items = deserializer.Deserialize<ItemInfo[]>(reader);
                foreach (var item in items)
                {
                    
                    Item reg;
                    if (Extension.ContainsKey(item.rawname))
                    {
                        reg = Extension[item.rawname];
                        reg.Id = item.id;

                    }
                    else if (item.toolClass != null)
                    {
                        reg = new ItemTool
                        {
                            ToolClasses = new HashSet<string>(item.toolClass),
                            ClassLevels = item.harvestLevel,
                            Effectivness = materials[item.material]
                        };
                    }
                    else
                    {
                        reg = new Item();
                    }

                    reg.Id = item.id;
                    reg.Name = item.name;
                    reg.RawName = item.rawname;
                    ItemRegistry.Add(reg.Id, reg);
                }
            }
        }

        public static Item GetItem(int id)
        {
            Item result;
            if (!ItemRegistry.TryGetValue(id, out result))
                result = new Item {Id = id};
            return result;
        }

        private class MaterialInfo
        {
            public string name;
            public float effectiveness;
            public float damage;
        }

        private class ItemInfo
        {
            public int id;
            public string name;
            public string rawname;
            public string material;
            public float effectivness;
            public string[] toolClass;
            public int[] harvestLevel;
        }

        public string Name;
        public string RawName;
        public int Id;

        public virtual bool CanHarvest(Block block)
        {
            return false;
        }

        public virtual float GetItemStrength(ItemStack stack, Block block)
        {
            return 1.0f;
        }

        public override string ToString()
        {
            return Name ?? RawName ?? "";
        }
    }
}
