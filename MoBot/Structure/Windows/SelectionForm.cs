using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoBot.Structure
{
    public partial class SelectionForm : Form
    {
        private readonly object[] _items;
        public object Item;
        public SelectionForm(object[] selectableItems, bool sortByName = false)
        {
            if (sortByName)
                Array.Sort(selectableItems, (x,y) => string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal));
            _items = selectableItems;
            InitializeComponent();
        }

        private void SelectionForm_Load(object sender, EventArgs e)
        {
            selectionList.Items.AddRange(_items);
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            Item = selectionList.SelectedItem;
            Close();
        }
    }
}
