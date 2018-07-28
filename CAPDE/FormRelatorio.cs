using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAPDE
{
    public partial class FormRelatorio : Form
    {
        DataGridView datagrid = new DataGridView();
        string nomeRelatorio = String.Empty;

        public FormRelatorio(DataGridView data, string _nameRelatorio)
        {
            InitializeComponent();
            datagrid = data;
            nomeRelatorio = _nameRelatorio;
        }

        private void FormRelatorio_Load(object sender, EventArgs e)
        {
            ReportDataSource rds = new ReportDataSource("DataSet1", datagrid.DataSource);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.ReportEmbeddedResource = "CAPDE.Report1.rdlc";

            ReportParameter tituloRelatorio = new ReportParameter("TituloRelatorio", "CAPDE");
            ReportParameter subtituloRelatorio = new ReportParameter("SubtituloRelatorio", nomeRelatorio);
            reportViewer1.LocalReport.SetParameters(tituloRelatorio);
            reportViewer1.LocalReport.SetParameters(subtituloRelatorio);

            this.reportViewer1.RefreshReport();
        }
    }
}
