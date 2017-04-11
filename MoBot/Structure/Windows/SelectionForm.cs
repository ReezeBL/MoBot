using System;
using System.Windows.Forms;

namespace MoBot.Structure
{
    public partial class SelectionForm : Form
    {
        private readonly object[] items;
        public object Item;
        public SelectionForm(object[] selectableItems, bool sortByName = false)
        {
            if (sortByName)
                Array.Sort(selectableItems, (x,y) => string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal));
            items = selectableItems;
            InitializeComponent();
        }

        private void SelectionForm_Load(object sender, EventArgs e)
        {
            selectionList.Items.AddRange(items);
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            Item = selectionList.SelectedItem;
            Close();
        }
    }
}
