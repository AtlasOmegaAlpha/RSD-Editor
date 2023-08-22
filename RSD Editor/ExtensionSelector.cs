using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSD_Editor
{
    public partial class ExtensionSelector : Form
    {
        public string selectedExtension = ".mae";
        public ExtensionSelector()
        {
            InitializeComponent();
        }

        private void ExtensionSelector_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            object selectedItem = comboBox1.SelectedItem;
            if (selectedItem == null)
                return;

            selectedExtension = selectedItem.ToString();
            Close();

            DialogResult = DialogResult.OK;
        }
    }
}
