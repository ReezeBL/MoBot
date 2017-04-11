using System;

namespace MoBot.Scripts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MoBotExtension : Attribute
    {
        public readonly string Id;
        public readonly string Version;

        public MoBotExtension(string id, string version)
        {
            Id = id;
            Version = version;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Initialisation : Attribute
    {
        
    }
}
