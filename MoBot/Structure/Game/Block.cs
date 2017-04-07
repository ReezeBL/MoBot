using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using Newtonsoft.Json;

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

        public static void WriteBlocksToDb(DbConnection db)
        {
            using (var command = db.CreateCommand())
            {
                command.CommandText = @"DROP TABLE IF EXISTS [blocks]";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE [blocks] (
                    [id] integer PRIMARY KEY NOT NULL,
                    [name] char(100),
                    [raw_name] char(100),
                    [hardness] real ,
                    [transparent] integer,
                    [harvest_tool] char(100)
                    );";
                command.ExecuteNonQuery();
            }

            using (var transaction = db.BeginTransaction())
            {
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO [blocks] ([id], [name], [raw_name], [hardness], [transparent], [harvest_tool])
                                        VALUES (@id, @name, @raw_name, @hardness, @transparent, @harvest_tool)";

                    cmd.AddParameter("@id");
                    cmd.AddParameter("@name");
                    cmd.AddParameter("@raw_name");
                    cmd.AddParameter("@hardness");
                    cmd.AddParameter("@transparent");
                    cmd.AddParameter("@harvest_tool");

                    foreach (var block in Blocks)
                    {
                        cmd.Parameters["@id"].Value = block.Id;
                        cmd.Parameters["@name"].Value = block.Name;
                        cmd.Parameters["@raw_name"].Value = block.RawName;
                        cmd.Parameters["@hardness"].Value = block.Hardness;
                        cmd.Parameters["@transparent"].Value = block.Transparent;
                        cmd.Parameters["@harvest_tool"].Value = block.HarvestTool;

                        cmd.ExecuteNonQuery();
                    }
                  
                }
                transaction.Commit();
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
