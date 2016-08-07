using System;
using System.IO;
using System.Xml.Serialization;

namespace MoBot.Settings
{
    [Serializable]
    public sealed class SettingsClass
    {
        private SettingsClass() { }

        static SettingsClass()
        {
            Instance = Deserialize() as SettingsClass ?? new SettingsClass();
        }

        public static object Deserialize()
        {
            XmlSerializer ser = new XmlSerializer(typeof(SettingsClass));
            try
            {
                using (TextReader reader = new StreamReader(Path))
                {
                    return ser.Deserialize(reader) as SettingsClass;
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
            XmlSerializer ser = new XmlSerializer(typeof(SettingsClass));
            using (TextWriter writer = new StreamWriter(Path))
            {
                ser.Serialize(writer, Instance);
            }
        }

        private const string Path = "Settings/Settings.xml";
        private static readonly SettingsClass Instance;

        
        public string _serverIp = "";
        public string _userName = "";

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
    }
}
