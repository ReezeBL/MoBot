using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using Newtonsoft.Json;

namespace MoBot.Core.Game
{
    public class Block
    {
        private static readonly Dictionary<int, Block> BlockRegistry = new Dictionary<int, Block>();

        public static IEnumerable<Block> Blocks => BlockRegistry.Values;

        public static void LoadBlocks()
        {
            try
            {
                var jsonFile = File.ReadAllText(Settings.BlocksPath);
                dynamic data = JsonConvert.DeserializeObject(jsonFile);
                foreach (var block in data)
                {
                    var gameBlock = new Block
                    {
                        Hardness = block.hardness,
                        HarvestTool = block.harvestTool,
                        Id = block.id,
                        Name = block.name,
                        Transparent = block.transparent,
                        RawName = block.rawname
                    };

                    if (block.name == "Вода")
                        Water.Add(gameBlock.Id);

                    BlockRegistry.Add(gameBlock.Id, gameBlock);
                }
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
            return Id == -1 ? "" : $"{Name ?? RawName ?? ""} ({Id})";
        }
    }
}
