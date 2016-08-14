using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MoBot.Annotations;
using MoBot.Structure.Game.Items;

namespace MoBot.Structure.Game
{
    public class Container : INotifyPropertyChanged
    {
        private readonly ItemStack[] _items;
        private readonly ItemStack[] _inventory;
        private readonly object _monitor = new object();
        public ItemStack CursorItem { get; private set; }
        public readonly byte WindowId;
        private readonly int _capacity;

        public Container(int capacity)
        {
            _capacity = capacity;
            _items = new ItemStack[_capacity];
            _inventory = new ItemStack[36];
        }

        public Container(int capacity, Container inventory, byte windowId = 0)
        {
            _capacity = capacity;
            _items = new ItemStack[_capacity];
            _inventory = inventory._inventory;
            WindowId = windowId;
        }

        public ItemStack this[int n]
        {
            get
            {
                lock (_monitor)
                {
                    if (n == -1)
                        return CursorItem;
                    return n >= _capacity ? _inventory[n - _capacity] : _items[n];
                }
            }

            set
            {
                lock (_monitor)
                {
                    if (n == -1)
                        CursorItem = value;
                    else if (n >= _capacity)
                        _inventory[n - _capacity] = value;
                    else
                        _items[n] = value;

                    OnSlotChanged(n, value);
                }
            }
        }

        public IEnumerable<IndexedItem> IndexedInventory
        {
            get
            {
                int index = _capacity;
                lock (_monitor)
                {
                    return _inventory.Select(item => new IndexedItem {Item = item.Item, Slot = index++});
                }
            }
        }

        public IEnumerable<IndexedItem> IndexedContainer
        {
            get
            {
                int index = 0;
                lock (_monitor)
                {
                    return _items.Select(item => new IndexedItem {Item = item.Item, Slot = index++});
                }
            }
        }

        public IEnumerable<IndexedItem> Belt
        {
            get
            {
                int index = 0;
                lock (_monitor)
                {
                    return _inventory.Skip(27).Select(item => new IndexedItem {Item = item.Item, Slot = index++});
                }
            }
        }

        public int ContainerFreeSlot
        {
            get
            {
                lock (_monitor)
                {
                    for (int i = 0; i < _capacity; i++)
                        if (_items[i] == null || _items[i].Item.Id == -1)
                            return i;
                    return -1;
                }
            }
        }

        public int InventoryFreeSlot
        {
            get
            {
                lock (_monitor)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        if (_inventory[i] == null || _inventory[i].Item.Id == -1)
                            return _capacity + i;
                    }
                }
                return -1;
            }
        }

        public int Capacity => _capacity;

        public class IndexedItem
        {
            public Item Item;
            public int Slot;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            lock (_monitor)
            {
                for (int i = 0; i < _capacity; i++)
                    sb.Append($"{i} : {_items[i]} ");
                sb.AppendLine();

                for (var i = 0; i < 4; i++)
                {
                    for (var j = 0; j < 9; j++)
                        sb.Append($"{_capacity + i*9 + j} : {_inventory[i*9 + j]} ");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public void HandleClick(int slot)
        {
            var tmp = this[slot];
            this[slot] = CursorItem;
            CursorItem = tmp;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<object, int, ItemStack> SlotChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnSlotChanged(int n, ItemStack slot)
        {
            SlotChanged?.Invoke(this, n, slot);
        }
    }
}
