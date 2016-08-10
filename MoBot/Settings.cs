using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MoBot
{
    [Serializable]
    public sealed class Settings
    {
        private Settings() { }

        static Settings()
        {
            Instance = Deserialize() as Settings ?? new Settings();
        }

        public static object Deserialize()
        {
            JsonSerializer serializer = JsonSerializer.CreateDefault();
            try
            {
                using (TextReader stream = new StreamReader(Path))
                using (JsonTextReader reader = new JsonTextReader(stream) )
                {
                    return serializer.Deserialize<Settings>(reader);
                }
            }
            catch (Exception)
            {
                Program.GetLogger().Warn("There is no settings file. Created a new one");
                return null;
            }

        }

        public static void Serialize()
        {
            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {Formatting = Formatting.Indented});
            using (TextWriter stream = new StreamWriter(Path))
            using (JsonTextWriter writer = new JsonTextWriter(stream))
            {
                serializer.Serialize(writer, Instance);
            }
        }

        private const string Path = "Settings/Settings.json";
        public const string BlocksPath = "Settings/blocks.json";
        public const string ItemsPath = "Settings/items.json";
        public const string ScriptsPath = "Scripts";
        public const string MaterialsPath = "Settings/materials.json";
        public const string UserIdsPath = "Settings/UserIDS.xml";

        private static readonly Settings Instance;

        
        public string _serverIp = "";
        public string _userName = "";
        public string _homewarp = "";
        public HashSet<int> _intrestedBlocks = new HashSet<int>();
        public HashSet<int> _keepItemIds = new HashSet<int> ();
        public int _scanRange = 32;
        public bool _autoReconnect;

        public static string ServerIp
        {
            get { return Instance._serverIp; }
            set { Instance._serverIp = value; }
        }

        public static string UserName
        {
            get { return Instance._userName; }
            set { Instance._userName = value; }
        }

        public static HashSet<int> IntrestedBlocks
        {
            get { return Instance._intrestedBlocks; }
            set { Instance._intrestedBlocks = value; }
        }

        public static int ScanRange
        {
            get { return Instance._scanRange; }
            set { Instance._scanRange = value; }
        }

        public static HashSet<int> KeepItems
        {
            get { return Instance._keepItemIds; }
            set { Instance._keepItemIds = value; }
        }

        public static bool AutoReconnect
        {
            get { return Instance._autoReconnect; }
            set { Instance._autoReconnect = value; }
        }

        public static string HomeWarp
        {
            get { return Instance._homewarp; }
            set { Instance._homewarp = value; }
        }
    }
}
