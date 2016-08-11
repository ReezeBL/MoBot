using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MoBot.Structure.Game;
using MoBot.Structure.Game.Items;

namespace MoBot.Structure.Windows
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            keepItemsSettings.SetUp(Item.Items.ToArray<object>(), Settings.KeepItems.Select(Item.GetItem).ToArray<object>());
            autoDiggerSettings.SetUp(Block.Blocks.ToArray<object>(), Settings.IntrestedBlocks.Select(Block.GetBlock).ToArray<object>());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Settings.KeepItems = new HashSet<int>();
            foreach (var item in keepItemsSettings.Items)
                Settings.KeepItems.Add(((Item) item).Id);

            Settings.IntrestedBlocks = new HashSet<int>();
            foreach (var item in autoDiggerSettings.Items)
                Settings.IntrestedBlocks.Add(((Block)item).Id);

            Settings.Serialize();
            Close();
        }
    }
}
