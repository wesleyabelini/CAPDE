using CAPDEData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Common.Backup;
using static Common.CAPDEEnums;
using CAPDELogin;
using System.Diagnostics;

namespace CAPDE
{
    public partial class FormCAPDE : Form
    {
        Common.Common common = new Common.Common();
        Common.FormLoading formLoading = new Common.FormLoading();

        FileVersionInfo thisAssemblyVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        int capacitadoId = (int)FiltroCapacitado.All;

        bool isRequireAdmin = false;
        bool isAdmin = false;
        bool hasUserAdmin = false;
        bool hasUpdate = false;

        private string userName = String.Empty;
        private string logedUser = String.Empty;

        string filtro = String.Empty;

        public FormCAPDE()
        {
            InitializeComponent();

            Task ts = Task.Factory.StartNew(() =>
            {
                FormUpdate fUpdate = new FormUpdate();
                if (fUpdate.hasUpdate && fUpdate.ShowDialog() == DialogResult.OK) hasUpdate = fUpdate.hasUpdate;
            });

            ts.Wait();

            Task t = Task.Factory.StartNew(() => 
            {
                if (!hasUpdate) ProcessInitial();
                formLoading.DialogResult = DialogResult.Abort;
            });

            if(formLoading.ShowDialog() == DialogResult.Abort) formLoading.Close();
            t.Wait();
        }

        private void ProcessInitial()
        {
            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "capdeRestore.mdf")))
                FixRestauredDatabase();
                
            isRequireAdmin = Form.ModifierKeys == Keys.Shift;
            if (isRequireAdmin) isAdmin = ShowLogin((int)TypeForm.Login, (int)SizeForm.Login);
            hasUserAdmin = (isAdmin) ? true : verifyUserAdmin_OnDatabase();

            VerifiBackupSuccess();

            AtualizaPreencheInicial();

            tslUser.Text = userName;

            if (isAdmin)
            {
                desaposentarToolStripMenuItem.Visible = true;
                IncluirtoolStripMenuItem.Visible = true;
                radioAposentado.Visible = true;
                radioExcluido.Visible = true;
                gerenciaToolStripMenuItem.Visible = true;
                this.Text += " Admin Mode";
            }

            Task_PreencherGrid();
        }

        private bool ShowLogin(int form, int heightForm)
        {
            FormLogin fLogin = new FormLogin(form, heightForm, String.Empty);
            fLogin.BringToFront();
            if (fLogin.ShowDialog() == DialogResult.Yes)
            {
                userName = fLogin.LogedUserName;
                logedUser = fLogin.LogedUser;

                return true;
            }

            userName = fLogin.LogedUserName;
            logedUser = fLogin.LogedUser;

            return false;
        }

        private bool verifyUserAdmin_OnDatabase()
        {
            try
            {
                using (capdeEntities context = new capdeEntities())
                {
                    Usuario user = context.Usuarios.Where(x => x.IsAdmin == true).FirstOrDefault();
                    if (user != null) return true;
                    else
                    {
                        formLoading.DialogResult = DialogResult.Abort;
                        ShowLogin((int)TypeForm.LoginAdmin, (int)SizeForm.Login_Cad);
                    }
                }
            }
            catch (TimeoutException ex) { common.MessageBox_TryConnection(ex.ToString()); }

            return false;
        }

        private void AtualizaPreencheInicial()
        {
            common.PreencheCombos_Pessoa(cmbRAJ, cmbCJ, cmbCidade, cmbCargo, cmbSetor, cmbCapacitacao);
            
            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> RAJ = context.RAJs.Where(x=>x.IsExcluido == false)
                    .OrderByDescending(x=>x.NomeRaj == StringBase.TODOS.ToString()).ThenBy(x=>x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                tscRAJ = common.PreencheCombo(tscRAJ, RAJ, "RajId", "NomeRaj");
            }
        }

        private void cidadeToolStripMenuItem_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Cidade, (int)SizeForm.Cidade); }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e) { this.Close(); }

        private void cJToolStripMenuItem_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.CJ, (int)SizeForm.CJ); }

        private void rAJToolStripMenuItem_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.RAJ, (int)SizeForm.RAJ); }

        private void setorToolStripMenuItem_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Setor, (int)SizeForm.Setor); }

        private void capacitacaoToolStripMenuItem_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Lote_Capacitar, (int)SizeForm.Lote); }

        private void pessoaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCadPessoa pessoa = new FormCadPessoa(isAdmin);
            if (pessoa.ShowDialog() == DialogResult.OK) AtualizaPreencheInicial();
        }

        private void cargoToolStripMenuItem_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Cargo, (int)SizeForm.Cargo); }

        private void turmaToolStripMenuItem_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Turma, (int)SizeForm.Turma); }

        private void sobreToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About sobre = new About();
            sobre.Show();
        }

        private void tsbRAJ_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.RAJ, (int)SizeForm.RAJ); }

        private void newFormCad(int formType, int heightForm)
        {
            FormCad cad = new FormCad(formType, heightForm, isAdmin);
            if (cad.ShowDialog() == DialogResult.OK) AtualizaPreencheInicial();
        }

        private void tsbCJ_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.CJ, (int)SizeForm.CJ); }

        private void tsbCidade_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Cidade, (int)SizeForm.Cidade); }

        private void tsbSetor_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Setor, (int)SizeForm.Setor); }

        private void tsbCargo_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Cargo, (int)SizeForm.Cargo); }

        private void tsbTurma_Click(object sender, EventArgs e) { newFormCad((int)TypeForm.Turma, (int)SizeForm.Turma); }

        private void tsbPessoa_Click(object sender, EventArgs e)
        {
            FormCadPessoa pessoa = new FormCadPessoa(isAdmin);
            if (pessoa.ShowDialog() == DialogResult.OK) AtualizaPreencheInicial();
        }

        private void tscCidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tscCidade.ComboBox.ValueMember != "") Task_PreencherGrid();
        }

        private void tscCJ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscCJ.ComboBox.ValueMember != "" && tscCJ.ComboBox.SelectedValue != null)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.CjId == (int)tscCJ.ComboBox.SelectedValue && x.IsExcluido == false)
                        .Select(x => new { x.CidadeId, x.NomeCidade }).ToList();
                    tscCidade = common.PreencheCombo(tscCidade, cidade, "CidadeId", "NomeCidade");
                }
            }
        }

        private void radioAll_Click(object sender, EventArgs e)
        {
            capacitadoId = (int)FiltroCapacitado.All;
            Task_PreencherGrid();
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            capacitadoId = (int)FiltroCapacitado.Capacitado;
            Task_PreencherGrid();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            capacitadoId = (int)FiltroCapacitado.Incapacitado;
            Task_PreencherGrid();
        }

        private void radioAposentado_Click(object sender, EventArgs e)
        {
            capacitadoId = (int)FiltroCapacitado.Aposentado;
            Task_PreencherGrid();
        }

        private void radioExcluido_Click(object sender, EventArgs e)
        {
            capacitadoId = (int)FiltroCapacitado.Excluido;
            Task_PreencherGrid();
        }

        private void btAtualizar_Click(object sender, EventArgs e)
        {
            string registro = dtGrid.CurrentRow.Cells[0].Value.ToString();

            using (capdeEntities context = new capdeEntities())
            {
                Pessoa pessoa = context.Pessoas.Where(x => x.Registro == registro).FirstOrDefault();

                if (ckCapacitado.Checked)
                {
                    pessoa.Capacitacao.DataInicio = Convert.ToDateTime(dateTInicio.Text);
                    pessoa.Capacitacao.DataFim = Convert.ToDateTime(dateTFim.Text);
                    pessoa.Capacitacao.IsCapacitado = true;

                    if (radioEAD.Checked && cmbCapacitacao.SelectedValue != null)
                    {
                        pessoa.Capacitacao.IsEAD = true;
                        pessoa.Capacitacao.TurmaId = (int)cmbCapacitacao.SelectedValue;
                    }
                    else if(radioPresencial.Checked && cmbCapacitacao.SelectedValue != null)
                    {
                        pessoa.Capacitacao.RajId = (int)cmbCapacitacao.SelectedValue;
                        pessoa.Capacitacao.IsEAD = false;
                    }
                    else
                    {
                        pessoa.Capacitacao.IsEAD = null;
                        pessoa.Capacitacao.TurmaId = null;
                        pessoa.Capacitacao.RajId = null;
                    }
                }
                else pessoa.Capacitacao.IsCapacitado = false;

                pessoa.Nome = txtNome.Text;
                pessoa.Registro = txtRegistro.Text;
                pessoa.EMail = txtEmail.Text;
                pessoa.CargoId = (int)cmbCargo.SelectedValue;
                pessoa.SetorId = (int)cmbSetor.SelectedValue;
                pessoa.Obs = txtOBS.Text;

                common.SaveChanges_Database(context, true);
            }

            AtualizaPreencheInicial();
        }

        private void cmbRAJ_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbRAJ.ValueMember != String.Empty && cmbRAJ.SelectedValue != null)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> CJ = context.CJs.Where(x => x.RajId == (int)cmbRAJ.SelectedValue && x.CjNome != StringBase.TODOS.ToString())
                        .OrderBy(x=>x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                    cmbCJ = common.PreencheCombo(cmbCJ, CJ, "CjId", "CjNome");
                }
            }
        }

        private void cmbCJ_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbCidade.ValueMember != String.Empty)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.CjId == (int)cmbCJ.SelectedValue && 
                    x.NomeCidade != StringBase.TODOS.ToString()).OrderBy(X=>X.NomeCidade).Select(x => new { x.CidadeId, x.NomeCidade }).ToList();

                    cmbCidade = common.PreencheCombo(cmbCidade, cidade, "CidadeId", "NomeCidade");
                }
            }
            else cmbCidade.DataSource = null;
        }

        private void cmbCidade_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbSetor.ValueMember!= String.Empty)
            {
                using(capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> setor = context.Setors.Where(x => x.CidadeId == (int)cmbCidade.SelectedValue).OrderBy(x=>x.NomeSetor)
                        .Select(x => new { x.SetorId, x.NomeSetor }).ToList();

                    cmbSetor = common.PreencheCombo(cmbSetor, setor, "SetorId", "NomeSetor");
                }
            }
        }

        private void ckCapacitado_CheckedChanged(object sender, EventArgs e)
        {
            if (ckCapacitado.Checked)
            {
                panelCapacitado.Enabled = true;
                cmbCapacitacao = common.LoadTurmaCombo(cmbCapacitacao);
            }
            else panelCapacitado.Enabled = false;
        }

        private void radioEAD_Click(object sender, EventArgs e)
        {
            cmbCapacitacao.Enabled = true;
            lblCapacitacao.Text = TypeForm.Turma.ToString();

            cmbCapacitacao = common.LoadTurmaCombo(cmbCapacitacao);
        }

        private void radioPresencial_Click(object sender, EventArgs e)
        {
            cmbCapacitacao.Enabled = true;
            lblCapacitacao.Text = TypeForm.RAJ.ToString();

            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> capacitado = context.RAJs.Where(x=>x.NomeRaj != StringBase.TODOS.ToString() && x.IsExcluido == false)
                    .OrderBy(X=>X.NomeRaj).Select(x => new { x.RajId, x.NomeRaj }).ToList();
                cmbCapacitacao = common.PreencheCombo(cmbCapacitacao, capacitado, "RajId", "NomeRaj");
            }
        }

        private void radioOutros_Click(object sender, EventArgs e)
        {
            cmbCapacitacao.Enabled = false;
            lblCapacitacao.Text = String.Empty;
        }

        private void tscCidade_DropDownClosed(object sender, EventArgs e)
        {
            if (tscCidade.ComboBox.SelectedValue != null) Task_PreencherGrid();
        }

        private void dtGrid_SelectionChanged(object sender, EventArgs e)
        {
            string registro = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();

            using (capdeEntities context = new capdeEntities())
            {
                Pessoa pessoa = context.Pessoas.Where(x => x.Registro == registro).FirstOrDefault();

                if(pessoa != null)
                {
                    txtNome.Text = pessoa.Nome;
                    txtEmail.Text = pessoa.EMail;
                    txtRegistro.Text = pessoa.Registro;
                    cmbCargo.SelectedValue = pessoa.CargoId;
                    cmbRAJ.SelectedValue = pessoa.Setor.Cidade.CJ.RajId;
                    cmbCJ.SelectedValue = pessoa.Setor.Cidade.CjId;
                    cmbCidade.SelectedValue = pessoa.Setor.CidadeId;
                    cmbSetor.SelectedValue = pessoa.SetorId;
                    txtOBS.Text = pessoa.Obs;

                    if ((bool)pessoa.IsExcluido) gBoxPessoa.BackColor = Color.MistyRose;
                    else if((bool)pessoa.IsAposentado) gBoxPessoa.BackColor = Color.Gray;
                    else if (pessoa.Capacitacao.IsCapacitado)
                    {
                        gBoxPessoa.BackColor = Color.PaleGreen;
                        ckCapacitado.Checked = true;

                        dateTInicio.Value = (pessoa.Capacitacao.DataInicio.HasValue) ? (DateTime)pessoa.Capacitacao.DataInicio: DateTime.Now;
                        dateTFim.Value = (pessoa.Capacitacao.DataFim.HasValue) ? (DateTime)pessoa.Capacitacao.DataFim : DateTime.Now;

                        if (pessoa.Capacitacao.IsEAD == true)
                        {
                            radioEAD.Checked = (pessoa.Capacitacao.IsEAD == true) ? true : false;
                            radioEAD.PerformClick();
                            cmbCapacitacao.SelectedValue = (pessoa.Capacitacao.TurmaId != null) ? pessoa.Capacitacao.TurmaId : -1;
                            lblCapacitacao.Text = TypeForm.Turma.ToString();
                        }
                        else if (pessoa.Capacitacao.RajId != null)
                        {
                            radioPresencial.Checked = true;
                            radioPresencial.PerformClick();
                            cmbCapacitacao.SelectedValue = pessoa.Capacitacao.RajId;
                            cmbCapacitacao.Enabled = true;
                            lblCapacitacao.Text = TypeForm.RAJ.ToString();
                        }
                        else
                        {
                            radioOutros.Checked = true;
                            cmbCapacitacao.Enabled = false;
                        }
                    }
                    else
                    {
                        ckCapacitado.Checked = false;
                        gBoxPessoa.BackColor = Color.MistyRose;
                    }
                }
            }
        }

        private void tsbRelatorio_Click(object sender, EventArgs e)
        {
            FormRelatorio relatorio = new FormRelatorio(dtGrid, returnRelatorioName());
            relatorio.Show();
        }

        private void cadastroToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormRelatorio relatorio = new FormRelatorio(dtGrid, returnRelatorioName());
            relatorio.Show();
        }

        private string returnRelatorioName()
        {
            string relatorio = "Relatório ";
            if (radioCapacitado.Checked) relatorio += "de Capacitados ";
            else if (radioIncapacitado.Checked) relatorio += "de Incapacitados ";

            if (tscCidade.Text != String.Empty && tscCidade.Text != StringBase.TODOS.ToString()) relatorio += 
                    "Cidade de " + returnFirstMaiuscula(tscCidade.Text.ToLower());
            else if (tscCJ.Text != String.Empty && tscCJ.Text != StringBase.TODOS.ToString()) relatorio += 
                    "da CJ de " + returnFirstMaiuscula(tscCJ.Text.ToLower());
            else if (tscRAJ.Text == StringBase.TODOS.ToString()) relatorio += "Geral";
            else if (tscRAJ.Text != String.Empty) relatorio += "da RAJ de " + returnFirstMaiuscula(tscRAJ.Text.ToLower());

            return relatorio;
        }

        private string returnFirstMaiuscula(string text)
        {
            string returText = String.Empty;
            int cont = 0;
            foreach(char i in text)
            {
                if (String.IsNullOrWhiteSpace(i.ToString()))
                    cont = -1;
                if (cont == 0) returText += (i.ToString()).ToUpper();
                else returText += i.ToString();
                cont++;
            }

            return returText;
        }

        private void tscRAJ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tscRAJ.ComboBox.ValueMember != "" && tscRAJ.ComboBox.SelectedValue != null)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> CJ = context.CJs.Where(x => x.RajId == (int)tscRAJ.ComboBox.SelectedValue && x.IsExcluido == false)
                        .OrderByDescending(x=>x.CjNome == StringBase.TODOS.ToString()).ThenBy(x=>x.CjNome)
                        .Select(x => new { x.CjId, x.CjNome }).ToList();
                    tscCJ = common.PreencheCombo(tscCJ, CJ, "CjId", "CjNome");
                }
            }
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            filtro = txtBusca.Text;
            Task_PreencherGrid();
        }

        private async void Task_PreencherGrid() { await FilterGrid(); }

        private async Task FilterGrid()
        {
            await Task.Run(() =>
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> pessoas = context.Pessoas.ToList();

                    if (filtro != String.Empty) pessoas = pessoas.Where(x => x.Nome.Contains(filtro) || x.Registro.Contains(filtro)).ToList();

                    if (capacitadoId == (int)FiltroCapacitado.Incapacitado || capacitadoId == (int)FiltroCapacitado.Capacitado)
                        pessoas = pessoas.Where(x => x.Capacitacao.IsCapacitado == Convert.ToBoolean(capacitadoId)).ToList();
                    else if (capacitadoId == (int)FiltroCapacitado.Aposentado) pessoas = pessoas.Where(x => x.IsAposentado == true).ToList();
                    else if (capacitadoId == (int)FiltroCapacitado.Excluido) pessoas = pessoas.Where(x => x.IsExcluido == true).ToList();

                    if (this.tscRAJ.ComboBox.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (tscRAJ.ComboBox.SelectedValue != null && tscRAJ.ComboBox.Text != StringBase.TODOS.ToString())
                                pessoas = pessoas.Where(x => x.Setor.Cidade.CJ.RajId == (int)tscRAJ.ComboBox.SelectedValue).ToList();
                        });
                    }

                    if (this.tscCJ.ComboBox.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (tscCJ.ComboBox.SelectedValue != null && tscCJ.ComboBox.Text != StringBase.TODOS.ToString())
                                pessoas = pessoas.Where(x => x.Setor.Cidade.CjId == (int)tscCJ.ComboBox.SelectedValue).ToList();
                        });
                    }

                    if (this.tscCidade.ComboBox.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (tscCidade.ComboBox.SelectedValue != null && tscCidade.ComboBox.Text != StringBase.TODOS.ToString())
                                pessoas = pessoas.Where(x => x.Setor.CidadeId == (int)tscCidade.ComboBox.SelectedValue).ToList();
                        });
                    }

                    if(isAdmin) pessoas = pessoas.OrderBy(x => x.Nome)
                        .Select(x => new { x.Registro, x.Nome, x.Capacitacao.IsCapacitado, x.IsAposentado, x.IsExcluido }).ToList();
                    else pessoas = pessoas.Where(x=>x.IsExcluido == false && x.IsAposentado == false).OrderBy(x => x.Nome)
                        .Select(x => new { x.Registro, x.Nome, x.Capacitacao.IsCapacitado, x.IsAposentado, x.IsExcluido }).ToList();

                    tslValorTotal.Text = pessoas.Count().ToString();

                    if (this.dtGrid.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            dtGrid.DataSource = pessoas;
                            dtGrid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dtGrid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dtGrid.Columns[2].Visible = false;
                            dtGrid.Columns[3].Visible = false;
                            dtGrid.Columns[4].Visible = false;
                        });
                    }
                }
            });
        }

        private void dtGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int totalCapacitados = 0;

            try
            {
                foreach(DataGridViewRow i in (sender as DataGridView).Rows)
                {
                    bool isExcluido = (bool)i.Cells[4].Value;
                    bool isAposentado = (bool)i.Cells[3].Value;
                    bool isCapacitado = (bool)i.Cells[2].Value;

                    if (isExcluido) i.DefaultCellStyle.BackColor = Color.Tomato;
                    else if(isAposentado) i.DefaultCellStyle.BackColor = Color.Gray;
                    else if (isCapacitado)
                    {
                        i.DefaultCellStyle.BackColor = Color.PaleGreen;
                        totalCapacitados++;
                    }
                }

                tslValorCapacitados.Text = totalCapacitados.ToString();
            }
            catch(IndexOutOfRangeException f) { MessageBox.Show(String.Empty + f); } 
        }

        private float porcentagemCapacitados()
        {
            int total = Convert.ToInt32(tslValorTotal.Text);
            int capacitados = Convert.ToInt32(tslValorCapacitados.Text);

            return (float)(capacitados * 100) / total;
        }

        private void txtNome_KeyDown(object sender, KeyEventArgs e) { enterPressed_Atualizar(e); }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e) { enterPressed_Atualizar(e); }

        private void txtRegistro_KeyDown(object sender, KeyEventArgs e) { enterPressed_Atualizar(e); }

        private void txtOBS_KeyDown(object sender, KeyEventArgs e) { enterPressed_Atualizar(e); }

        private void enterPressed_Atualizar(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btAtualizar.PerformClick();
        }

        private void dtGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = dtGrid.HitTest(e.Location.X, e.Location.Y).RowIndex;

                if (rowIndex >= 0)
                {
                    dtGrid.Rows[rowIndex].Selected = true;
                    contextMenuStrip1.Show(dtGrid, new Point(e.Location.X, e.Location.Y));
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();

            using(capdeEntities context = new capdeEntities())
            {
                bool hasChanged = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1)
                    .Select(x=>x.HasChanged).First();

                if (hasChanged)
                {
                    try
                    {
                        CreateLocalBackup();
                        common.SaveChanges_Database(context, false);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Houve um erro ao executar o backup. \nErro:" + ex, "Falha Backup", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }   
            }

            if(hasUpdate) Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\copy.bat");
        }

        private void VerifiBackupSuccess()
        {
            bool hasChanged = false;

            using(capdeEntities context = new capdeEntities())
            {
                hasChanged = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).Select(x => x.HasChanged).First();

                if (hasChanged) MessageBox.Show("O backup não foi realizado na sessão anterior. Favor, contate o administrador.",
                     "Falha backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (hasUpdate || File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "capdeRestore.mdf")))
                this.Close();
        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Backup File | *.bak";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(dialog.FileName))
                {
                    CreateLocalBackup(dialog.FileName);
                    MessageBox.Show("Backup realizado com sucesso.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("Não foi possível realizar o backup no local informado.", "Backup",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileBak = common.OpenFileDialog("Backup File | *.bak");
            if (!String.IsNullOrEmpty(fileBak)) RestoreLocalBackup(fileBak);
        }

        private void aposentadoToolStripMenuItem_Click(object sender, EventArgs e) { AposentarServidor(StatusServidor.Aposentar.ToString(), true); }

        private void desaposentarToolStripMenuItem_Click(object sender, EventArgs e) { AposentarServidor(StatusServidor.Desaposentar.ToString(), false); }

        private void ExcluirtoolStripMenuItem_Click(object sender, EventArgs e) { ExcluirServidor(StatusServidor.Excluir.ToString(), true); }

        private void IncluirtoolStripMenuItem_Click(object sender, EventArgs e) { ExcluirServidor(StatusServidor.Incluir.ToString(), false); }

        private void AposentarServidor(string statusServidor, bool status)
        {
            if (MessageBox.Show("Deseja " + statusServidor + " o servidor " + txtNome.Text + "?", statusServidor, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    Pessoa pessoa = context.Pessoas.Where(x => x.Registro == txtRegistro.Text && x.Nome == txtNome.Text).First();
                    pessoa.IsAposentado = status;

                    common.SaveChanges_Database(context, true);
                    Task_PreencherGrid();
                }
            }
        }

        private void ExcluirServidor(string statusServidor, bool status)
        {
            MessageBoxIcon msgIcon;
            string msg = String.Empty;
            if (isAdmin)
            {
                msgIcon = MessageBoxIcon.Warning;
                msg = "Deseja " + statusServidor + " o servidor " + txtNome.Text + "Permanentemente?";
            }
            else
            {
                msgIcon = MessageBoxIcon.Question;
                msg = "Deseja " + statusServidor + " o servidor " + txtNome.Text + "?";
            }

            if (MessageBox.Show(msg, statusServidor, MessageBoxButtons.YesNo, msgIcon) == DialogResult.Yes)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    Pessoa pessoa = context.Pessoas.Where(x => x.Registro == txtRegistro.Text && x.Nome == txtNome.Text).First();

                    if (isAdmin) context.Pessoas.Remove(pessoa);
                    else pessoa.IsExcluido = status;

                    common.SaveChanges_Database(context, true);
                    Task_PreencherGrid();
                }
            }
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormManageUser mUser = new FormManageUser();
            mUser.Show();
        }

        private void tslUser_Click(object sender, EventArgs e)
        {
            FormLogin fLogin = new FormLogin((int)TypeForm.ChangeLogin, (int)SizeForm.Login_Cad, logedUser);
            fLogin.Show();
        }

        private void servidoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormLoteServidores loteServidores = new FormLoteServidores();
            loteServidores.Show();
        }
    }
}
