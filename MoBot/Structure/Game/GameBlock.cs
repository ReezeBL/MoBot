using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MoBot.Structure.Game
{
    public class GameBlock
    {
        private static readonly Dictionary<int, GameBlock> BlockRegistry = new Dictionary<int, GameBlock>();

        private class BlockInfo
        {
            public int id;
            public string name;
            public float hardness;
            public bool transparent;
            public string harvestTool;
        }

        public static IEnumerable<GameBlock> Blocks => BlockRegistry.Values;

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
                        GameBlock gameBlock = new GameBlock
                        {
                            Hardness = block.hardness > 0 ? block.hardness : 10000,
                            HarvestTool = block.harvestTool,
                            Id = block.id,
                            Name = block.name,
                            Transparent = block.transparent
                        };
                        BlockRegistry.Add(gameBlock.Id, gameBlock);
                    }
                }

                BlockRegistry.Add(-1, new GameBlock() {
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
          
        public static GameBlock GetBlock(int id)
        {
            GameBlock res;
            if (!BlockRegistry.TryGetValue(id, out res))
                res = new GameBlock { Id = id };
            return res;
        }
          
        public int Id;
        public string Name = "";
        public float Hardness = 10000f;
        public bool Transparent;
        public string HarvestTool;
    }
}
