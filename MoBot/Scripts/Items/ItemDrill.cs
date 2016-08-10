using System.Collections.Generic;
using System.Diagnostics;
using MoBot.Structure.Game;
using MoBot.Structure.Game.Items;

namespace MoBot.Scripts.Items
{
    public class ItemAdvancedDrill : ItemTool
    {
        private const float NormalPower = 35f;
        private const float LowPower = 16f;
        private const float UltraLowPower = 10f;
        private const float BigHolesPower = 16f;

        private const float EnergyPerOperation = 200;
        private const float EnergyPerLowOperation = 80;
        private const float EnergyPerUltraLowOperation = 50;

        private enum Mode
        {
            Normal,
            LowPower,
            UltraLowPower,
            BigHoles
        }

        private readonly Dictionary<Mode, float> effectivness = new Dictionary<Mode, float>
        {
            {Mode.Normal, NormalPower},
            {Mode.LowPower, LowPower},
            {Mode.BigHoles, BigHolesPower},
            {Mode.UltraLowPower, UltraLowPower}
        };

        private readonly Dictionary<Mode, float> energyCosts = new Dictionary<Mode, float>
        {
            {Mode.Normal, EnergyPerOperation },
            {Mode.BigHoles, EnergyPerOperation },
            {Mode.LowPower, EnergyPerLowOperation },
            {Mode.UltraLowPower, EnergyPerUltraLowOperation }
        };

        public int GetToolMode(ItemStack stack)
        {
            var nbtTag = stack.NbtRoot.Get("toolMode");
            if (nbtTag != null) return nbtTag.IntValue;
            return 0;
        }

        public double GetItemCharge(ItemStack stack)
        {
            var nbtTag = stack.NbtRoot.Get("charge");
            if (nbtTag != null) return nbtTag.DoubleValue;
            return 0;
        }

        public override float GetItemStrength(ItemStack stack, GameBlock block)
        {
            if (!CanHarvest(block))
                return 1.0f;
            Mode mode = (Mode) GetToolMode(stack);
            if (energyCosts[mode] > GetItemCharge(stack))
                return 1.0f;
            return effectivness[mode];
        }

        public ItemAdvancedDrill()
        {
            ToolClasses = new HashSet<string> {"shovel", "pickaxe"};
        }

        [ImportHandler.PreInit]
        public static void Import()
        {
            Extension.Add("GraviSuite:advDDrill", new ItemAdvancedDrill());
        }
    }
}
