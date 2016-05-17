using MoBot.Structure.Game.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Protocol.Threading
{
    class AIThread : BaseThread
    {
        Type moduleType;
        public AIModule module
        {
            get; private set;
        }
        public AIThread(AIModule module)
        {
            this.moduleType = module.GetType();
            this.module = module;
            Thread thread = new Thread(() =>
            {
                while (Process)
                {
                    module.tick();
                    Thread.Sleep(50);
                }
            })
            {IsBackground = true };
            thread.Start();
        }

        public void SetProperty(String property_name, object val)
        {
            try {
                var Property = moduleType.GetProperty(property_name);
                Property.SetValue(module, Property.PropertyType.InvokeMember("Parse", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, new object[] { val }), null);
            }
            catch (Exception)
            {
                Program.GetLogger().Error($"Cant set property {property_name} in class {moduleType.Name}!");
            }
        }
    }
}
