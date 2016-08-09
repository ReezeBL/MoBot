using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MoBot.Structure.Game.Items;

namespace MoBot.Structure.Game
{
    public class Container
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

        public int ContainerFreeSlot
        {
            get
            {
                for(int i=0;i<_capacity;i++)
                    if (_items[i] == null || _items[i].Item.Id == -1)
                        return i;
                return -1;
            }
        }

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
    }
}
