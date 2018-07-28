﻿using CAPDEData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Common.CAPDEEnums;
using static Common.Backup;
using static Common.GoogleDrive;

namespace CAPDE
{
    public partial class Form1 : Form
    {
        Common common = new Common();
        int capacitadoId = (int)FiltroCapacitado.All;

        string filtro = String.Empty;

        public Form1()
        {
            InitializeComponent();

            ProcessInitial();
        }

        private void ProcessInitial()
        {
            VerifiRestoredDataBase();
            VerifiBackupSuccess();
            AtualizaPreencheInicial();

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "capdeRestore.mdf")))
            {
                Task_PreencherGrid();
            }
        }

        private void AtualizaPreencheInicial()
        {
            common.PreencheCombos_Pessoa(cmbRAJ, cmbCJ, cmbCidade, cmbCargo, cmbSetor, cmbCapacitacao);
            
            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> RAJ = context.RAJs.OrderByDescending(x=>x.NomeRaj == "TODOS").ThenBy(x=>x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                tscRAJ = common.PreencheCombo(tscRAJ, RAJ, "RajId", "NomeRaj");
            }
        }

        private void cidadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Cidade, (int)SizeForm_Cad.Cidade);
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.CJ, (int)SizeForm_Cad.CJ);
        }

        private void rAJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.RAJ, (int)SizeForm_Cad.RAJ);
        }

        private void setorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Setor, (int)SizeForm_Cad.Setor);
        }

        private void capacitacaoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Lote_Capacitar, (int)SizeForm_Cad.Lote);
        }

        private void pessoaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCadPessoa pessoa = new FormCadPessoa();
            if (pessoa.ShowDialog() == DialogResult.OK)
            {
                AtualizaPreencheInicial();
            }
        }

        private void cargoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Cargo, (int)SizeForm_Cad.Cargo);
        }

        private void turmaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Turma, (int)SizeForm_Cad.Turma);
        }

        private void sobreToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About sobre = new About();
            sobre.Show();
        }

        private void tsbRAJ_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.RAJ, (int)SizeForm_Cad.RAJ);
        }

        private void newFormCad(int formType, int heightForm)
        {
            FormCad cad = new FormCad(formType, heightForm);
            if (cad.ShowDialog() == DialogResult.OK)
            {
                AtualizaPreencheInicial();
            }
        }

        private void tsbCJ_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.CJ, (int)SizeForm_Cad.CJ);
        }

        private void tsbCidade_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Cidade, (int)SizeForm_Cad.Cidade);
        }

        private void tsbSetor_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Setor, (int)SizeForm_Cad.Setor);
        }

        private void tsbCargo_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Cargo, (int)SizeForm_Cad.Cargo);
        }

        private void tsbTurma_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Turma, (int)SizeForm_Cad.Turma);
        }

        private void tsbPessoa_Click(object sender, EventArgs e)
        {
            FormCadPessoa pessoa = new FormCadPessoa();
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
                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.CjId == (int)tscCJ.ComboBox.SelectedValue)
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

        private void btAtualizar_Click(object sender, EventArgs e)
        {
            string registro = dtGrid.CurrentRow.Cells[0].Value.ToString();

            using (capdeEntities context = new capdeEntities())
            {
                Pessoa pessoa = context.Pessoas.Where(x => x.Registro == registro).FirstOrDefault();

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

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

                context.SaveChanges();
            }

            AtualizaPreencheInicial();
        }

        private void cmbRAJ_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbRAJ.ValueMember != String.Empty && cmbRAJ.SelectedValue != null)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> CJ = context.CJs.Where(x => x.RajId == (int)cmbRAJ.SelectedValue && x.CjNome != "TODOS")
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
                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.CjId == (int)cmbCJ.SelectedValue && x.NomeCidade != "TODOS")
                        .OrderBy(X=>X.NomeCidade).Select(x => new { x.CidadeId, x.NomeCidade }).ToList();

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
            lblCapacitacao.Text = "Turma";

            cmbCapacitacao = common.LoadTurmaCombo(cmbCapacitacao);
        }

        private void radioPresencial_Click(object sender, EventArgs e)
        {
            cmbCapacitacao.Enabled = true;
            lblCapacitacao.Text = "RAJ";

            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> capacitado = context.RAJs.Where(x=>x.NomeRaj != "TODOS").OrderBy(X=>X.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
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

                    if (pessoa.Capacitacao.IsCapacitado)
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
                            lblCapacitacao.Text = "Turma";
                        }
                        else if (pessoa.Capacitacao.RajId != null)
                        {
                            radioPresencial.Checked = true;
                            radioPresencial.PerformClick();
                            cmbCapacitacao.SelectedValue = pessoa.Capacitacao.RajId;
                            cmbCapacitacao.Enabled = true;
                            lblCapacitacao.Text = "RAJ";
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
            if (radioButton1.Checked) relatorio += "de Capacitados ";
            else if (radioButton2.Checked) relatorio += "de Incapacitados ";

            if (tscCidade.Text != String.Empty && tscCidade.Text != "TODOS") relatorio += "Cidade de " + returnFirstMaiuscula(tscCidade.Text.ToLower());
            else if (tscCJ.Text != String.Empty && tscCJ.Text != "TODOS") relatorio += "da CJ de " + returnFirstMaiuscula(tscCJ.Text.ToLower());
            else if (tscRAJ.Text == "TODOS") relatorio += "Geral";
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

        private void radioEAD_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tscRAJ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tscRAJ.ComboBox.ValueMember != "" && tscRAJ.ComboBox.SelectedValue != null)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> CJ = context.CJs.Where(x => x.RajId == (int)tscRAJ.ComboBox.SelectedValue)
                        .OrderByDescending(x=>x.CjNome == "TODOS").ThenBy(x=>x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                    tscCJ = common.PreencheCombo(tscCJ, CJ, "CjId", "CjNome");
                }
            }
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            filtro = txtBusca.Text;
            Task_PreencherGrid();
        }

        private async void Task_PreencherGrid()
        {
            await FilterGrid();
            tsProgressBar.Maximum = Convert.ToInt32(tslValorTotal.Text);
        }

        private async Task FilterGrid()
        {
            await Task.Run(() =>
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> pessoas = context.Pessoas.ToList();

                    if (filtro != String.Empty)
                    {
                        pessoas = pessoas.Where(x => x.Nome.Contains(filtro) || x.Registro.Contains(filtro)).ToList();
                    }
                    if (capacitadoId == (int)FiltroCapacitado.Incapacitado || capacitadoId == (int)FiltroCapacitado.Capacitado)
                    {
                        pessoas = pessoas.Where(x => x.Capacitacao.IsCapacitado == Convert.ToBoolean(capacitadoId)).ToList();
                    }
                        
                    if (this.tscRAJ.ComboBox.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (tscRAJ.ComboBox.SelectedValue != null && tscRAJ.ComboBox.Text != "TODOS")
                            {
                                pessoas = pessoas.Where(x => x.Setor.Cidade.CJ.RajId == (int)tscRAJ.ComboBox.SelectedValue).ToList();
                            }
                        });
                    }

                    if (this.tscCJ.ComboBox.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (tscCJ.ComboBox.SelectedValue != null && tscCJ.ComboBox.Text != "TODOS")
                            {
                                pessoas = pessoas.Where(x => x.Setor.Cidade.CjId == (int)tscCJ.ComboBox.SelectedValue).ToList();
                            }   
                        });
                    }

                    if (this.tscCidade.ComboBox.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (tscCidade.ComboBox.SelectedValue != null && tscCidade.ComboBox.Text != "TODOS")
                            {
                                pessoas = pessoas.Where(x => x.Setor.CidadeId == (int)tscCidade.ComboBox.SelectedValue).ToList();
                            }
                        });
                    }

                    pessoas = pessoas.OrderBy(x => x.Nome).Select(x => new { x.Registro, x.Nome, x.Capacitacao.IsCapacitado }).ToList();
                    tslValorTotal.Text = pessoas.Count().ToString();

                    if (this.dtGrid.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            dtGrid.DataSource = pessoas;
                            dtGrid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dtGrid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dtGrid.Columns[2].Visible = false;
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
                    if ((bool)i.Cells[2].Value)
                    {
                        i.DefaultCellStyle.BackColor = Color.PaleGreen;
                        totalCapacitados++;
                    }
                }

                tslValorCapacitados.Text = totalCapacitados.ToString();
                tsProgressBar.ToolTipText = porcentagemCapacitados().ToString();
                tsProgressBar.Value = totalCapacitados;
            }
            catch(IndexOutOfRangeException f)
            {
                MessageBox.Show(String.Empty + f);
            }  
        }

        private float porcentagemCapacitados()
        {
            int total = Convert.ToInt32(tslValorTotal.Text);
            int capacitados = Convert.ToInt32(tslValorCapacitados.Text);

            return (float)(capacitados * 100) / total;
        }

        private void txtNome_KeyDown(object sender, KeyEventArgs e)
        {
            enterPressed_Atualizar(e);
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            enterPressed_Atualizar(e);
        }

        private void txtRegistro_KeyDown(object sender, KeyEventArgs e)
        {
            enterPressed_Atualizar(e);
        }

        private void txtOBS_KeyDown(object sender, KeyEventArgs e)
        {
            enterPressed_Atualizar(e);
        }

        private void enterPressed_Atualizar(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btAtualizar.PerformClick();
            }
        }

        private void dtGrid_MouseClick(object sender, MouseEventArgs e)
        {
            //if(e.Button == MouseButtons.Right)
            //{
            //    int rowIndex = dtGrid.HitTest(e.Location.X, e.Location.Y).RowIndex;

            //    if(rowIndex >= 0)
            //    {
            //        dtGrid.Rows[rowIndex].Selected = true;
            //        contextMenuStrip1.Show(dtGrid, new Point(e.Location.X, e.Location.Y));
            //    }
            //}
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

                        DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                        config.HasChanged = false;

                        context.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Houve um erro ao executar o backup. \nErro:" + ex, "Falha Backup", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }   
            }
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

        private void VerifiRestoredDataBase()
        {
            if(File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "capdeRestore.mdf")))
            {
                FixRestauredDatabase();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "capdeRestore.mdf")))
            {
                this.Close();
            }
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
            common.OpenFileDialog_RestoreLocalBackup("Backup File | *.bak");
        }
    }
}
