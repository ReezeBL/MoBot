using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NLog;

#pragma warning disable 169

namespace MoBot
{
    [Serializable]
    public sealed class Settings
    {
        private const string Path = "Settings/Settings.json";
        public const string BlocksPath = "Settings/blocks.json";
        public const string ItemsPath = "Settings/items.json";
        public const string EntitiesPath = "Settings/entities.json";
        public const string ModsPath = "Settings/mods.json";
        public const string ScriptsPath = "Scripts";
        public const string MaterialsPath = "Settings/materials.json";
        public const string UserIdsPath = "Settings/UserIDS.xml";

        private static readonly Settings Instance;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        [JsonRequired] private readonly Dictionary<string, SettingsWrapper> nameBindings =
            new Dictionary<string, SettingsWrapper>();
        [JsonRequired] private readonly Dictionary<string, string> compiledModules = new Dictionary<string, string>();

        private SettingsWrapper currentSettings;

        static Settings()
        {
            Instance = Deserialize() as Settings ?? new Settings();
        }

        private Settings()
        {
        }


        public static Dictionary<string, string> CompiledModules => Instance.compiledModules;

        public static string ServerIp
        {
            get => Instance.currentSettings.ServerIp;
            set => Instance.currentSettings.ServerIp = value;
        }

        public static HashSet<int> IntrestedBlocks
        {
            get => Instance.currentSettings.IntrestedBlocks;
            set => Instance.currentSettings.IntrestedBlocks = value;
        }

        public static int ScanRange
        {
            get => Instance.currentSettings.ScanRange;
            set => Instance.currentSettings.ScanRange = value;
        }

        public static HashSet<int> KeepItems
        {
            get => Instance.currentSettings.KeepItemIds;
            set => Instance.currentSettings.KeepItemIds = value;
        }

        public static bool AutoReconnect
        {
            get => Instance.currentSettings.AutoReconnect;
            set => Instance.currentSettings.AutoReconnect = value;
        }

        public static string HomeWarp
        {
            get => Instance.currentSettings.HomeWarp;
            set => Instance.currentSettings.HomeWarp = value;
        }

        public static string BackWarp
        {
            get => Instance.currentSettings.BackWarp;
            set => Instance.currentSettings.BackWarp = value;
        }

        public static Dictionary<string, string> Users { get; } = new Dictionary<string, string>();

        public static object Deserialize()
        {
            var serializer = JsonSerializer.CreateDefault();
            try
            {
                using (TextReader stream = new StreamReader(Path))
                using (var reader = new JsonTextReader(stream))
                {
                    return serializer.Deserialize<Settings>(reader);
                }
            }
            catch (Exception)
            {
                Logger.Warn("There is no settings file. Created a new one");
                return null;
            }
        }

        public static void Load(string username)
        {
            if (!Instance.nameBindings.TryGetValue(username, out Instance.currentSettings))
                Instance.currentSettings = Instance.nameBindings[username] = new SettingsWrapper();
        }

        public static void Serialize()
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings {Formatting = Formatting.Indented});
            using (TextWriter stream = new StreamWriter(Path))
            using (var writer = new JsonTextWriter(stream))
            {
                serializer.Serialize(writer, Instance);
            }
        }

        [JsonObject(MemberSerialization.Fields)]
        private class SettingsWrapper
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public bool AutoReconnect;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public string BackWarp = "";
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public string HomeWarp = "";
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public HashSet<int> IntrestedBlocks = new HashSet<int>();
            public HashSet<int> KeepItemIds = new HashSet<int>();
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public int ScanRange = 32;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public string ServerIp = "";
            public string UserName = "";
        }
    }
}