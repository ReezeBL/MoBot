using System;
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
            keepItemsSettings.GlobalCollection = Item.Items.ToArray<object>();
            keepItemsSettings.Items = Settings.KeepItems.Select(Item.GetItem).ToArray<object>();
            autoDiggerSettings.GlobalCollection = Block.Blocks.ToArray<object>();
            autoDiggerSettings.Items = Settings.IntrestedBlocks.Select(Block.GetBlock).ToArray<object>();
            serverTextbox.Text = Settings.ServerIp;
            reconnectCheckBox.Checked = Settings.AutoReconnect;
            homeWarpText.Text = Settings.HomeWarp;
            returnWarpText.Text = Settings.BackWarp;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            ApplyChanges();

            Settings.Serialize();
            Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            Close();
        }

        private void ApplyChanges()
        {
            Settings.KeepItems.Clear();
            foreach (var item in keepItemsSettings.Items)
                Settings.KeepItems.Add(((Item)item).Id);

            Settings.IntrestedBlocks.Clear();
            foreach (var item in autoDiggerSettings.Items)
                Settings.IntrestedBlocks.Add(((Block)item).Id);

            Settings.ServerIp = serverTextbox.Text;
            Settings.AutoReconnect = reconnectCheckBox.Checked;
            Settings.HomeWarp = homeWarpText.Text;
            Settings.BackWarp = returnWarpText.Text;
        }

    }
}
