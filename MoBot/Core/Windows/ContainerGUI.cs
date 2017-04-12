using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MoBot.Core.Game;
using static System.String;

namespace MoBot.Core.Windows
{
    public partial class ContainerGui : UserControl, INotifyPropertyChanged
    {
        private const int SlotSize = 30;
        private const int DSize = 31;
        private readonly Dictionary<object, int> buttons = new Dictionary<object, int>();
        private readonly Dictionary<int, object> revButtons = new Dictionary<int, object>();
        private readonly ToolTip toolTip = new ToolTip();
        private Game.Container container;
        private ItemStack selectedItem;

        public bool SelectMode { get; set; } = false;

        public ItemStack SelectedItem
        {
            get => selectedItem;
            set
            {
                if (Equals(value, selectedItem)) return;
                selectedItem = value;
                OnPropertyChanged();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x2000000;
                return cp;
            }
        }

        public new Game.Container Container
        {
            get => container;
            set
            {
                container = value;
                if(InvokeRequired)
                    Invoke(new Action(ApplyContainer));
                else
                {
                    SuspendLayout();
                    ApplyContainer();
                    ResumeLayout();
                }
            }
        }

        private void ApplyContainer()
        {
            buttons.Clear();
            revButtons.Clear();
            Controls.Clear();
            if (container == null) return;
            for (var i = 0; i < container.Capacity; i++)
            {
                CreateButton(i, i%9, i/9);
            }

            for (var i = 0; i < 36; i++)
            {
                CreateButton(i + container.Capacity, i%9, i/9 + container.Capacity/9 + 1);
            }

            container.SlotChanged += ContainerOnPropertyChanged;
        }

        private void CreateButton(int i, int x, int y)
        {
            var button = new Button
            {
                Size = new Size(SlotSize, SlotSize),
                Location = new Point(DSize * x, DSize * y),
                FlatStyle = FlatStyle.Popup,
                Font = new Font(FontFamily.GenericSansSerif, 6)
            };
            button.Click += SlotClick;

            var itemToolTip = container[i]?.Item?.ToString() ?? Empty;
            toolTip.SetToolTip(button, itemToolTip);
            button.BackColor = itemToolTip == Empty ? Color.Transparent : Color.LawnGreen;

            Controls.Add(button);
            buttons.Add(button, i);
            revButtons.Add(i, button);
        }

        private void ContainerOnPropertyChanged(object sender, int n, ItemStack slot)
        {
            if (!sender.Equals(container))
                return;
            if (InvokeRequired)
            {
                Invoke(new Action<object, int, ItemStack>(ContainerOnPropertyChanged), sender, n, slot);
            }
            else
            {
                var button = revButtons[n] as Button;
                if (button != null)
                {
                    var itemToolTip = slot?.Item?.ToString() ?? Empty;
                    toolTip.SetToolTip(button, itemToolTip);
                    button.BackColor = itemToolTip == Empty ? Color.Transparent : Color.LawnGreen;
                }
            }
        }

        public ContainerGui()
        {
            InitializeComponent();
        }

        private void SlotClick(object sender, EventArgs args)
        {
            if (SelectMode)
            {
                SelectedItem = container[buttons[sender]];
            }
            else
            {
                ActionManager.ClickInventorySlot(buttons[sender]);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
