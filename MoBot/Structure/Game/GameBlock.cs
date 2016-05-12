using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game
{
    class GameBlock
    {
        public static Dictionary<int, GameBlock> blockRegistry = new Dictionary<int, GameBlock>();

        public static void loadBlocks()
        {
            using (var file = File.OpenText("Settings/blocks.json"))
            using (var reader = new JsonTextReader(file))
            {
                dynamic loadedData = Newtonsoft.Json.Linq.JToken.ReadFrom(reader);
                foreach (var data in loadedData)
                {
                    if (data.id != null)
                    {
                        var block = new GameBlock { id = (int)data.id, name = (string)data.name, hardness = (float)(data.hardness.Value ?? float.MaxValue), transparent = (bool)data.transparent };
                        blockRegistry.Add(block.id, block);
                    }
                }
            }
        }
            

        public int id;
        public String name;
        public String displayName;
        public float hardness;
        public int stackSize;
        public bool diggable;
        public bool transparent;
    }
}
