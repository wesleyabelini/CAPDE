using CAPDEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Common
{
    public partial class FormLogs : Form
    {
        public FormLogs()
        {
            InitializeComponent();

            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> logs = context.Logs.ToList();
                dgvLogs.DataSource = logs;
            }

            dgvLogs.Columns["LogId"].Visible = false;
            dgvLogs.Columns["Data"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvLogs.Columns["Usuario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvLogs.Columns["Maquina"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvLogs.Columns["Version"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvLogs.Columns["MethodTitle"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvLogs.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void dgvLogs_SelectionChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = dgvLogs.CurrentRow.Cells["Message"].Value.ToString();
        }
    }
}
