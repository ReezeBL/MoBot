using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MoBot.Structure.Game
{
    public class Block
    {
        private static readonly Dictionary<int, Block> BlockRegistry = new Dictionary<int, Block>();

        private class BlockInfo
        {
            public int id;
            public string name;
            public string rawname;
            public float hardness;
            public bool transparent;
            public string harvestTool;
        }

        public static IEnumerable<Block> Blocks => BlockRegistry.Values;

        public static void LoadBlocks()
        {
            try
            {
                using (var file = File.OpenText(Settings.BlocksPath))
                using (var reader = new JsonTextReader(file))
                {
                    var deserializer = JsonSerializer.Create();
                    var blocks = deserializer.Deserialize<BlockInfo[]>(reader);
                    foreach (var block in blocks)
                    {
                        var gameBlock = new Block
                        {
                            Hardness = block.hardness,
                            HarvestTool = block.harvestTool,
                            Id = block.id,
                            Name = block.name,
                            Transparent = block.transparent,
                            RawName =  block.rawname
                        };

                        if (block.name == "Вода")
                            Water.Add(block.id);

                        BlockRegistry.Add(gameBlock.Id, gameBlock);
                    }
                }

                BlockRegistry.Add(-1, new Block
                {
                    Hardness = 0,
                    Id = -1,
                    Name = "air",
                    Transparent = true
                });
            }
            catch (FileNotFoundException exception)
            {
                Program.GetLogger().Warn($"Cant find {exception.FileName} file!");   
            }
        }
        
        public static HashSet<int> Water { get; } = new HashSet<int>();

        public static Block GetBlock(int id)
        {
            Block res;
            if (!BlockRegistry.TryGetValue(id, out res))
                res = new Block { Id = id };
            return res;
        }
          
        public int Id;
        public string Name;
        public string RawName;
        public float Hardness = -1f;
        public bool Transparent;
        public string HarvestTool;

        public override string ToString()
        {
            return Name ?? RawName ?? "";
        }
    }
}
