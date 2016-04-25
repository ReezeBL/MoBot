using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI
{
    abstract class AIModule
    {
        protected GameController mainAIController;
        protected AIHandler aiHandler;
        public void SetMainAIController(AIHandler aiHandler)
        {
            mainAIController = aiHandler.controller;
            this.aiHandler = aiHandler;
        }
        public abstract void tick();
    }
}
