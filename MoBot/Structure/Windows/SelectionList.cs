using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MoBot.Structure.Windows
{
    [DefaultBindingProperty("LabelCaption")]
    public partial class SelectionList : UserControl
    {
        private object[] _globalCollection;

        public SelectionList()
        {
            InitializeComponent();
        }


        public ListBox.ObjectCollection Items => collection.Items;

        public String LabelCaption
        {
            get { return caption.Text; }
            set { caption.Text = value; }
        }

        public void SetUp(object[] globalCollection, object[] selectedItems)
        {
            collection.Items.AddRange(selectedItems);

            Array.Sort(globalCollection, (x, y) => string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal));
            _globalCollection = globalCollection;

            var menu = new ContextMenu(new[] {new MenuItem("Add..", AddClick), new MenuItem("Remove", RemoveClick)});
            collection.ContextMenu = menu;
        }

        private void SelectionList_Load(object sender, EventArgs e)
        {
        }

        private void AddClick(object sender, EventArgs args)
        {
            var input = new SelectionForm(_globalCollection);
            input.ShowDialog();
            if (input.Item != null)
                collection.Items.Add(input.Item);
        }

        private void RemoveClick(object sender, EventArgs args)
        {
            collection.Items.Remove(collection.SelectedItem);
        }
    }
}