using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;

namespace MoBot.Structure.Windows
{
    [DefaultBindingProperty("LabelCaption")]
    public partial class SelectionList : UserControl
    {
        private object[] globalCollection;

        public SelectionList()
        {
            InitializeComponent();
            var menu = new ContextMenu(new[] { new MenuItem("Add..", AddClick), new MenuItem("Remove", RemoveClick) });
            collection.ContextMenu = menu;
        }


        public object[] Items
        {
            get => collection.Items.Cast<object>().ToArray();
            set
            {
                collection.Items.Clear();
                collection.Items.AddRange(value);
            }
        }

        public string LabelCaption
        {
            get => caption.Text;
            set => caption.Text = value;
        }

        public object[] GlobalCollection
        {
            get => globalCollection;
            set
            {
                globalCollection = value;
                if(globalCollection != null)
                    Array.Sort(globalCollection, (x, y) => string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal));
            }
        }

        private void SelectionList_Load(object sender, EventArgs e)
        {
        }

        private void AddClick(object sender, EventArgs args)
        {
            var input = new SelectionForm(globalCollection);
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