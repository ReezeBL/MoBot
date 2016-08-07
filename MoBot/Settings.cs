using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            try
            {
                using (TextReader reader = new StreamReader(Path))
                {
                    return ser.Deserialize(reader) as Settings;
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
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            using (TextWriter writer = new StreamWriter(Path))
            {
                ser.Serialize(writer, Instance);
            }
        }

        private const string Path = "Settings/Settings.xml";
        private static readonly Settings Instance;

        
        public string _serverIp = "";
        public string _userName = "";
        public HashSet<int> _intrestedBlocks = new HashSet<int> {14,15,16,56};
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
    }
}
