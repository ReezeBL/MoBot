using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MoBot.Annotations;
using MoBot.Structure.Game;
using Container = MoBot.Structure.Game.Container;

namespace MoBot.Structure.Windows
{
    public partial class ContainerGui : UserControl, INotifyPropertyChanged
    {
        private const int size = 30;
        private const int dSize = 31;
        private readonly Dictionary<object, int> _buttons = new Dictionary<object, int>();
        private readonly Dictionary<int, object> _revButtons = new Dictionary<int, object>();
        private Container _container;
        private ItemStack _selectedItem;

        public bool SelectMode { get; set; } = false;

        public ItemStack SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                OnPropertyChanged();
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
                    ApplyContainer();
                }
            }
        }

        private void ApplyContainer()
        {
            _buttons.Clear();
            _revButtons.Clear();
            Controls.Clear();
            if (_container == null) return;
            for (int i = 0; i < _container.Capacity; i++)
            {
                Button button = new Button
                {
                    Size = new Size(size, size),
                    Location = new Point(dSize * (i%9), dSize * (i / 9)),
                    FlatStyle = FlatStyle.Popup,
                    Text = _container[i]?.Item.Id.ToString(),
                    Font = new Font(FontFamily.GenericSansSerif, 6)
                };
                button.Click += SlotClick;
                Controls.Add(button);
                _buttons.Add(button, i);
                _revButtons.Add(i, button);
            }

            for (int i = 0; i < 36; i++)
            {
                Button button = new Button
                {
                    Size = new Size(size, size),
                    Location = new Point(dSize * (i % 9), dSize * (i / 9 + _container.Capacity / 9 + 1)),
                    FlatStyle = FlatStyle.Popup,
                    Text = _container[i]?.Item.Id.ToString(),
                    Font = new Font(FontFamily.GenericSansSerif, 6)
                };
                button.Click += SlotClick;
                Controls.Add(button);
                _buttons.Add(button, i + _container.Capacity);
                _revButtons.Add(i + _container.Capacity, button);
            }
            {
                Button button = new Button
                {
                    Size = new Size(size, size),
                    Location = new Point(0, dSize*11),
                    FlatStyle = FlatStyle.Popup,
                    Text = _container[-1]?.Item.Id.ToString(),
                    Font = new Font(FontFamily.GenericSansSerif, 6)
                };
                button.Click += SlotClick;
                Controls.Add(button);
                _buttons.Add(button, -1);
                _revButtons.Add(-1, button);
            }
            _container.PropertyChanged += ContainerOnPropertyChanged;
        }

        private void ContainerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            int slotNumber = int.Parse(propertyChangedEventArgs.PropertyName);
            var button = _revButtons[slotNumber] as Button;
            if (button != null) button.Text = _container[slotNumber].Item.Id.ToString();
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
