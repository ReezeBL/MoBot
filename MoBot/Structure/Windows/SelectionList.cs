using System;
using System.Linq;
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
            var menu = new ContextMenu(new[] { new MenuItem("Add..", AddClick), new MenuItem("Remove", RemoveClick) });
            collection.ContextMenu = menu;
        }


        public object[] Items
        {
            get
            {
                return collection.Items.Cast<object>().ToArray();
            }
            set
            {
                collection.Items.Clear();
                collection.Items.AddRange(value);
            }
        }

        public string LabelCaption
        {
            get { return caption.Text; }
            set { caption.Text = value; }
        }

        public object[] GlobalCollection
        {
            get { return _globalCollection; }
            set
            {
                _globalCollection = value;
                if(_globalCollection != null)
                    Array.Sort(_globalCollection, (x, y) => string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal));
            }
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