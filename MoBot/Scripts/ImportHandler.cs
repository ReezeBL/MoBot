using System;

namespace MoBot.Scripts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MoBotExtension : Attribute
    {
        public readonly string Id;
        public readonly string Version;
        public readonly bool RemoteOnly;
        public MoBotExtension(string id, string version, bool remoteOnly = false)
        {
            Id = id;
            Version = version;
            RemoteOnly = remoteOnly;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Initialisation : Attribute
    {
        
    }
}
