using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MoBot.Core.Game.Items;

namespace MoBot.Core.Game
{
    public class Container : INotifyPropertyChanged
    {
        private readonly ItemStack[] items;
        private readonly ItemStack[] inventory;
        private readonly object _monitor = new object();
        public ItemStack CursorItem { get; private set; }
        public readonly byte WindowId;
        private readonly int _capacity;

        public Container(int capacity)
        {
            _capacity = capacity;
            items = new ItemStack[_capacity];
            inventory = new ItemStack[36];
        }

        public Container(int capacity, byte windowId = 0)
        {
            _capacity = capacity;
            items = new ItemStack[_capacity];
            inventory = new ItemStack[36];
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
                    return n >= _capacity ? inventory[n - _capacity] : items[n];
                }
            }

            set
            {
                lock (_monitor)
                {
                    if (n == -1)
                        CursorItem = value;
                    else if (n >= _capacity)
                        inventory[n - _capacity] = value;
                    else
                        items[n] = value;
                }
                OnSlotChanged(n, value);
            }
        }

        public IEnumerable<IndexedItem> IndexedInventory
        {
            get
            {
                var index = _capacity;
                lock (_monitor)
                {
                    return inventory.Select(item => new IndexedItem {Item = item?.Item, Slot = index++});
                }
            }
        }

        public IEnumerable<IndexedItem> IndexedContainer
        {
            get
            {
                var index = 0;
                lock (_monitor)
                {
                    return items.Select(item => new IndexedItem {Item = item?.Item, Slot = index++});
                }
            }
        }

        public IEnumerable<IndexedItem> Belt
        {
            get
            {
                var index = 0;
                lock (_monitor)
                {
                    return inventory.Skip(27).Select(item => new IndexedItem {Item = item?.Item, Slot = index++});
                }
            }
        }

        public int ContainerFreeSlot
        {
            get
            {
                lock (_monitor)
                {
                    for (var i = 0; i < _capacity; i++)
                        if (items[i] == null || items[i].Item.Id == -1)
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
                    for (var i = 0; i < 36; i++)
                    {
                        if (inventory[i] == null || inventory[i].Item.Id == -1)
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
            var sb = new StringBuilder();

            lock (_monitor)
            {
                for (var i = 0; i < _capacity; i++)
                    sb.Append($"{i} : {items[i]} ");
                sb.AppendLine();

                for (var i = 0; i < 4; i++)
                {
                    for (var j = 0; j < 9; j++)
                        sb.Append($"{_capacity + i*9 + j} : {inventory[i*9 + j]} ");
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
