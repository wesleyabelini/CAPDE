using CAPDEData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static Common.CAPDEEnums;

namespace CAPDE
{
    public partial class FormLoteServidores : Form
    {
        Common.Common common = new Common.Common();
        
        string matricula = String.Empty;
        string nome = String.Empty;
        string email = String.Empty;
        string obs = String.Empty;

        Cargo cargo = new Cargo();
        RAJ raj = new RAJ();
        CJ cj = new CJ();
        Cidade cidade = new Cidade();
        Setor setor = new Setor();

        public FormLoteServidores()
        {
            InitializeComponent();

            string excelFile = common.OpenFileDialog("Excel | *.xls");
            if (!String.IsNullOrEmpty(excelFile)) PreencheGrid(excelFile);
        }

        private void PreencheGrid(string fileLocation)
        {
            string cmd = @"SELECT * FROM [Plan1$]";

            OleDbConnection conn = new System.Data.OleDb.OleDbConnection(("provider=Microsoft.Jet.OLEDB.4.0; " + ("data source=" + fileLocation + "; " + "Extended Properties=Excel 8.0;")));
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd, conn);

            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adapter.Fill(ds);
                ds.Tables[0].Columns.Add("Falhas");
                
                VerifyRegistros(ds.Tables[0]);

                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                ExcludeNullableRow();
                if (VerifyCorrectData()) btnCadastrar.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("" + ex, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            using (capdeEntities context = new capdeEntities())
            {
                foreach(DataGridViewRow row in dataGridView1.Rows)
                {
                    string registro = row.Cells[0].Value.ToString();
                    string nome = row.Cells[1].Value.ToString();
                    string email = row.Cells[2].Value.ToString();
                    string obs = row.Cells[8].Value.ToString();
                    string nomeCargo = row.Cells[3].Value.ToString();
                    string nomeSetor = row.Cells[7].Value.ToString();

                    Pessoa verifyPessoa = context.Pessoas.Where(x => x.Registro == registro).FirstOrDefault();

                    if(verifyPessoa == null)
                    {
                        Capacitacao capacitacao = new Capacitacao();
                        capacitacao.IsCapacitado = false;

                        Pessoa pessoa = new Pessoa
                        {
                            Registro = registro,
                            Nome = nome.ToUpper(),
                            EMail = email,
                            Obs = obs,
                            SetorId = context.Setors.Where(x => x.NomeSetor == nomeSetor).Select(x => x.SetorId).First(),
                            IsAposentado = false,
                            IsExcluido = false,
                            Capacitacao = capacitacao,
                            CargoId = context.Cargoes.Where(x => x.NomeCargo == nomeCargo).Select(x => x.CargoId).First(),
                        };

                        context.Capacitacaos.Add(capacitacao);
                        context.Pessoas.Add(pessoa);
                        common.SaveChanges_Database(context, true);
                    }
                }
            }

            this.Close();
        }

        private DataTable VerifyRegistros(DataTable table)
        {
            using (capdeEntities context = new capdeEntities())
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        matricula = row[0].ToString();
                        nome = row[1].ToString();
                        email = row[2].ToString();
                        obs = row[8].ToString();

                        Pessoa pessoa = context.Pessoas.Where(x => x.Registro == matricula).FirstOrDefault();

                        string rajValue = row[4].ToString();
                        string cjValue = row[5].ToString();
                        string cidadeValue = row[6].ToString();
                        string setorValue = row[7].ToString();

                        raj = context.RAJs.Where(x => x.NomeRaj == rajValue).FirstOrDefault();

                        if (raj != null) cj = context.CJs.Where(x => x.RajId == raj.RajId && x.CjNome == cjValue.ToUpper()).FirstOrDefault();
                        if (cj != null && cj.CjNome != null) cidade = context.Cidades.Where(x => x.CjId == cj.CjId && x.NomeCidade == cidadeValue.ToUpper()).FirstOrDefault();
                        if (cidade != null && cidade.NomeCidade != null) setor = context.Setors.Where(x => x.CidadeId == cidade.CidadeId && x.NomeSetor == setorValue.ToUpper())
                                .FirstOrDefault();

                        string nomeCargo = row[3].ToString();
                        cargo = context.Cargoes.Where(x => x.NomeCargo == nomeCargo).FirstOrDefault();

                        if (String.IsNullOrEmpty(matricula)) row[9] = "Matrícula não pode ser nula";
                        else if (pessoa != null) row[9] = "Matrícula existente. Registro não será casastrado";
                        else if (String.IsNullOrEmpty(nome)) row[9] = "Nome não pode ser nulo";
                        else if (String.IsNullOrEmpty(rajValue)) row[9] = "RAJ não encontrada";
                        else if (cj == null) row[9] = "CJ não encontrada";
                        else if (cidade == null) row[9] = "Cidade não encontrada";
                        else if (setor == null) row[9] = "Setor não encontrado";
                        else if (cargo == null) row[9] = "Cargo não pode ser nulo";
                    }
                }
            }

            btnVerificar.Enabled = true;

            return table;
        }

        private bool ExcludeNullableRow()
        {
            int i = 0;
            while(i < dataGridView1.RowCount)
            {
                if (String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[0].Value.ToString()) && 
                    String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[1].Value.ToString()) && 
                    String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[2].Value.ToString()) && 
                    String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[3].Value.ToString()) &&
                    String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[4].Value.ToString()) && 
                    String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[5].Value.ToString()) &&
                    String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[6].Value.ToString()) && 
                    String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[7].Value.ToString()) &&
                    String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[8].Value.ToString())) dataGridView1.Rows.RemoveAt(i);
                else i++;
            }
            

            return false;
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(dataGridView1.CurrentRow != null && e.KeyCode == Keys.Delete) dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                if (!String.IsNullOrEmpty(row.Cells[9].Value.ToString())) row.DefaultCellStyle.BackColor = Color.MistyRose;
            }
        }

        private void btnVerificar_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[9].Value = String.Empty;
                row.DefaultCellStyle.BackColor = Color.White;
            }

            VerifyRegistros((DataTable)dataGridView1.DataSource);

            if (VerifyCorrectData()) btnCadastrar.Enabled = true;
        }

        private bool VerifyCorrectData()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!String.IsNullOrEmpty(row.Cells[9].Value.ToString())) return false;
            }

            return true;
        }

        private void FormLoteServidores_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            btnCadastrar.Enabled = false;
        }

        private void consultarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IEnumerable<dynamic> data = null;

            using(capdeEntities context = new capdeEntities())
            {
                string nomeRAJ = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                string nomeCJ = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                string nomeCidade = dataGridView1.CurrentRow.Cells[6].Value.ToString();

                if (dataGridView1.CurrentCell.ColumnIndex == 4) data = context.RAJs.Where(x=>x.NomeRaj != StringBase.TODOS.ToString())
                        .Select(x => x.NomeRaj).ToList();
                else if (dataGridView1.CurrentCell.ColumnIndex == 5)
                    data = context.CJs.Where(x => x.RAJ.NomeRaj == nomeRAJ && x.CjNome != StringBase.TODOS.ToString()).Select(x => x.CjNome).ToList();
                else if (dataGridView1.CurrentCell.ColumnIndex == 6) data = context.Cidades
                        .Where(x => x.CJ.CjNome == nomeCJ && x.NomeCidade != StringBase.TODOS.ToString()).Select(x => x.NomeCidade).ToList();
                else if (dataGridView1.CurrentCell.ColumnIndex == 7) data = context.Setors.Where(x => x.Cidade.NomeCidade == nomeCidade)
                        .Select(x => x.NomeSetor).ToList();
                else if (dataGridView1.CurrentCell.ColumnIndex == 3) data = context.Cargoes.Select(x => x.NomeCargo).ToList();
            }

            using(FormDialogItem dialogItem = new FormDialogItem(data))
            {
                if (dialogItem.ShowDialog() == DialogResult.OK) dataGridView1.CurrentCell.Value = dialogItem.registro;
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right && dataGridView1.CurrentCell != null) contextMenuStrip1.Show(dataGridView1, e.X, e.Y);
        }

        private void btnExelFile_Click(object sender, EventArgs e)
        {
            string CAPDEExcelFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CAPDE.xls");

            string file = common.SaveFileDrialog("Excel | *.xls");
            if (!String.IsNullOrEmpty(file))
            {
                File.Copy(CAPDEExcelFile, file);
                MessageBox.Show("O arquivo Lote - Servidores foi transferido com sucesso", "Lote - Servidor", MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
        }
    }
}
