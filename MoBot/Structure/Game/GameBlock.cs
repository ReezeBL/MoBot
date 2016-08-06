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

        public static void LoadBlocks()
        {
            try
            {
                using (var file = File.OpenText("Settings/blocks.json"))
                using (var reader = new JsonTextReader(file))
                {
                    dynamic loadedData = JToken.ReadFrom(reader);
                    foreach (var data in loadedData)
                    {
                        if (data.id == null) continue;
                        var block = new GameBlock
                        {
                            Id = (int) data.id,
                            Name = (string) data.name,
                            Hardness = (float) (data.hardness.Value ?? float.MaxValue),
                            Transparent = (bool) data.transparent
                        };
                        BlockRegistry.Add(block.Id, block);
                    }
                }
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
        public float Hardness = -1.0f;
        public bool Transparent;
    }
}
