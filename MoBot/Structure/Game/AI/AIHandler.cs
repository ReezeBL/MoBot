using MoBot.Protocol.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI
{
    class AIHandler
    {
        public GameController controller { get; private set; }
        private NLog.Logger log = Program.getLogger();
        public Dictionary<String, AIThread> moduleList { get; private set; } = new Dictionary<String, AIThread>();

        public AIHandler(GameController controller)
        {
            this.controller = controller;
        }

        public void RegisterInternalModule(String name)
        {
            try
            {
                AIModule module = Activator.CreateInstance(null,$"MoBot.Structure.Game.AI.Modules.{name}").Unwrap() as AIModule;
                module.SetMainAIController(this);
                moduleList.Add(name, new AIThread(module));
            }
            catch (Exception)
            {
                log.Error($"Cannot load Module with name: {name}");
            }
        }

        public void RegisterModule(Type type)
        {
            try
            {
                AIModule module = Activator.CreateInstance(type) as AIModule;
                module.SetMainAIController(this);
                moduleList.Add(type.Name, new AIThread(module));
            }
            catch (Exception)
            {
                log.Error($"Cannot load Module with name: {type.Name}");
            }
        }

        public void UnregisterModule(String name)
        {
            try
            {
                AIThread thread = moduleList[name];
                thread.Stop();
                moduleList.Remove(name);
            }
            catch (Exception)
            {
                log.Error($"Cannot unload Module with name: {name}");
            }
        }
    }
}
