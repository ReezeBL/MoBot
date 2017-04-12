using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MoBot.Core.Game.Items
{
    public class Item
    {
        private static readonly Dictionary<string, Item> Extension = new Dictionary<string, Item> { { "item.bread", new ItemFood() }, { "item.apple", new ItemFood() } };
        private static readonly Dictionary<int, Item> ItemRegistry = new Dictionary<int, Item>();

        public static IEnumerable<Item> Items => ItemRegistry.Values;

        public static void RegisterItem(string rawName, Item item)
        {
            Extension.Add(rawName, item);
        }


        public static void LoadItems()
        {
            var materials = new Dictionary<string, float>();

            var jsonFile = File.ReadAllText(Settings.MaterialsPath);
            dynamic materialDump = JsonConvert.DeserializeObject(jsonFile);
            foreach (var material in materialDump)
            {
                materials.Add((string)material.name, (float)material.effectiveness);
            }

            jsonFile = File.ReadAllText(Settings.ItemsPath);
            dynamic items = JsonConvert.DeserializeObject(jsonFile);
            foreach (var item in items)
            {

                Item reg;
                if (Extension.ContainsKey((string)item.rawname))
                {
                    reg = Extension[(string)item.rawname];
                    reg.Id = item.id;

                }
                else if (item.toolClass != null)
                {
                    reg = new ItemTool
                    {
                        ToolClasses = item.toolClass.ToObject<HashSet<string>>(),
                        ClassLevels = item.harvestLevel.ToObject<int[]>(),
                    };
                    if (!materials.TryGetValue((string)item.material, out ((ItemTool)reg).Effectivness))
                        Program.GetLogger().Error($"Unknown material: {item.material}");

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

        public static Item GetItem(int id)
        {
            return !ItemRegistry.TryGetValue(id, out Item result) ? null : result;
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
            return Id == -1 ? "" : $"{Name ?? RawName ?? ""} ({Id})";
        }
    }
}
