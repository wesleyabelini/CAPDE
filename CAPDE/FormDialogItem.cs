using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPDE
{
    public partial class FormDialogItem : Form
    {
        public string registro = String.Empty;

        public FormDialogItem(IEnumerable<dynamic> data)
        {
            InitializeComponent();
            listBox1.DataSource = data;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null) registro = listBox1.SelectedItem.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
