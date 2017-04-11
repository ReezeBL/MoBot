using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MoBot.Structure.Game;
using static System.String;
using Container = MoBot.Structure.Game.Container;

namespace MoBot.Structure.Windows
{
    public partial class ContainerGui : UserControl, INotifyPropertyChanged
    {
        private const int SlotSize = 30;
        private const int DSize = 31;
        private readonly Dictionary<object, int> _buttons = new Dictionary<object, int>();
        private readonly Dictionary<int, object> _revButtons = new Dictionary<int, object>();
        private readonly ToolTip _toolTip = new ToolTip();
        private Container _container;
        private ItemStack _selectedItem;

        public bool SelectMode { get; set; } = false;

        public ItemStack SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
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

        public new Container Container
        {
            get { return _container; }
            set
            {
                _container = value;
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
            _buttons.Clear();
            _revButtons.Clear();
            Controls.Clear();
            if (_container == null) return;
            for (var i = 0; i < _container.Capacity; i++)
            {
                CreateButton(i, i%9, i/9);
            }

            for (var i = 0; i < 36; i++)
            {
                CreateButton(i + _container.Capacity, i%9, i/9 + _container.Capacity/9 + 1);
            }

            _container.SlotChanged += ContainerOnPropertyChanged;
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

            var toolTip = _container[i]?.Item?.ToString() ?? Empty;
            _toolTip.SetToolTip(button, toolTip);
            button.BackColor = toolTip == Empty ? Color.Transparent : Color.LawnGreen;

            Controls.Add(button);
            _buttons.Add(button, i);
            _revButtons.Add(i, button);
        }

        private void ContainerOnPropertyChanged(object sender, int n, ItemStack slot)
        {
            if (!sender.Equals(_container))
                return;
            if (InvokeRequired)
            {
                Invoke(new Action<object, int, ItemStack>(ContainerOnPropertyChanged), sender, n, slot);
            }
            else
            {
                var button = _revButtons[n] as Button;
                if (button != null)
                {
                    var toolTip = slot?.Item?.ToString() ?? Empty;
                    _toolTip.SetToolTip(button, toolTip);
                    button.BackColor = toolTip == Empty ? Color.Transparent : Color.LawnGreen;
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
                SelectedItem = _container[_buttons[sender]];
            }
            else
            {
                ActionManager.ClickInventorySlot(_buttons[sender]);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
