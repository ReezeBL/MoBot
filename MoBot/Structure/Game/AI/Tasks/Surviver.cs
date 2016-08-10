using System;
using System.Collections;
using MoBot.Structure.Game.AI.Pathfinding;
using MoBot.Structure.Game.World;
using NLog;
using TreeSharp;
using Action = TreeSharp.Action;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Surviver : Task
    {
        public int MinFoodDanger = 5;
        public double MinHealthDanger = 5;

        public string TeleportHome;
        public string TeleportCheckpoint;
        public string CreateCheckpoint;
        public string RemoveCheckpoint;

        public Location Chest = new Location(-4533, 65, -1318);
        public bool Store;

        private IEnumerator _routine;
        private Logger _logger = Program.GetLogger();

        private bool IsHungry(object context)
        {
            return GameController.Player.Food <= MinFoodDanger;
        }

        private bool InventoryIsFool(object context)
        {
            var res =  GameController.Player.CurrentContainer.InventoryFreeSlot == -1 || Store;

            if (!res) return false;

            Store = false;
            _routine = Routine();
            return true;
        }

        private void Feed()
        {
            
        }

        private RunStatus RunRoutine(object context)
        {
            if(_routine == null || !_routine.MoveNext())
                return RunStatus.Success;
            return RunStatus.Running;
        }

        private IEnumerator Routine()
        {
            _logger.Info("Perform homerun");
            ActionManager.SendChatMessage(RemoveCheckpoint);

            var ss = WaitForSeconds(3500);
            while (ss.MoveNext()) yield return ss.Current;

            ActionManager.SendChatMessage(CreateCheckpoint);

            ss = WaitForSeconds(3500);
            while (ss.MoveNext()) yield return ss.Current;

            ActionManager.SendChatMessage(TeleportHome);

            ss = WaitForSeconds(1000);
            while (ss.MoveNext()) yield return ss.Current;

            ActionManager.RightClick(Chest.X, Chest.Y, Chest.Z);

            ss = WaitForSeconds(1500);
            while (ss.MoveNext()) yield return ss.Current;

            foreach (var slot in GameController.Player.CurrentContainer.IndexedInventory)
            {
                if (Settings.KeepItems.Contains(slot.Item.Id) || slot.Item.Id == -1) continue;
                int freeSlot = GameController.Player.CurrentContainer.ContainerFreeSlot;
                if (freeSlot == -1) break;
                foreach (var p in ActionManager.ExchangeInventorySlots(slot.Slot, freeSlot))
                    yield return p;

                yield return null;
                yield return null;
            }

            ActionManager.CloseWindow();

            ss = WaitForSeconds(500);
            while (ss.MoveNext()) yield return ss.Current;

            ActionManager.SendChatMessage(TeleportCheckpoint);

            ss = WaitForSeconds(1500);
            while (ss.MoveNext()) yield return ss.Current;
        }

        private bool IsDead(object context)
        {
            return Math.Abs(GameController.Player.Health) < 1;
        }

        private void Ressurect()
        {
            ActionManager.Respawn();
        }

        public Surviver()
        {
            _root = new PrioritySelector(new Decorator(IsDead, new Action(o => Ressurect())), new Decorator(IsHungry, new Action(o => Feed())), new Decorator(InventoryIsFool, new Action(RunRoutine)));
        }

        public void GenerateStrings()
        {
            string name = GameController.Player.Name;
            TeleportCheckpoint = $"/warp blpoint{name}";
            TeleportHome = $"/warp {Settings.HomeWarp}";
            CreateCheckpoint = $"/warp pcreate blpoint{name}";
            RemoveCheckpoint = $"/warp remove blpoint{name}";
        }

        public override int GetPriority()
        {
            return 0;
        }
    }
}
