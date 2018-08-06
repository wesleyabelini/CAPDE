using CAPDEData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Common.CAPDEEnums;

namespace CAPDE
{
    public partial class FormCad : Form
    {
        Common.Common common = new Common.Common();
        int formAtual = 0;
        bool isAdmin = false;
        bool registerInserted = false;

        bool isEditing = false;
        int? idEditing = null;

        public FormCad(int typeForm, int heightForm, bool _isAdmin)
        {
            InitializeComponent();

            this.Height = heightForm;
            isAdmin = _isAdmin;
            formAtual = typeForm;
            condicaoInicial(typeForm);
            if (isAdmin) incluirToolStripMenuItem.Visible = true;
        }

        private void condicaoInicial(int entradaCadastro)
        {
            if (entradaCadastro == (int)TypeForm.RAJ || entradaCadastro == (int)TypeForm.Turma || 
                entradaCadastro == (int)TypeForm.Cargo) CadastroRAJ_Form(entradaCadastro);
            else if (entradaCadastro == (int)TypeForm.CJ) CadastroCJ_Form(entradaCadastro);
            else if (entradaCadastro == (int)TypeForm.Cidade) CadastroCidade_Form();
            else if (entradaCadastro == (int)TypeForm.Setor || entradaCadastro == (int)TypeForm.Lote_Capacitar) CadastroSetor_Form(entradaCadastro);
        }

        private void CadastroRAJ_Form(int formtypeParameters)
        {
            this.Text = "Cadastro " + Enum.GetName(typeof(TypeForm),formtypeParameters);
            label1.Text = Enum.GetName(typeof(TypeForm), formtypeParameters);

            label2 = (Label)common.MudarStatusVisible(label2, false, String.Empty);
            label3 = (Label)common.MudarStatusVisible(label3, false, String.Empty);
            label4 = (Label)common.MudarStatusVisible(label4, false, String.Empty);
            comboBox1 = (ComboBox)common.MudarStatusVisible(comboBox1, false, String.Empty);
            comboBox2 = (ComboBox)common.MudarStatusVisible(comboBox2, false, String.Empty);
            comboBox3 = (ComboBox)common.MudarStatusVisible(comboBox3, false, String.Empty);

            IEnumerable<dynamic> formCad = null;

            using(capdeEntities context = new capdeEntities())
            {
                if (formtypeParameters == (int)TypeForm.RAJ)
                {
                    if (isAdmin) formCad = context.RAJs.Where(x => x.NomeRaj != StringBase.TODOS.ToString())
                         .Select(x => new { x.RajId, x.NomeRaj, x.IsExcluido }).ToList();
                    else formCad = context.RAJs.Where(x => x.NomeRaj != StringBase.TODOS.ToString() && x.IsExcluido == false)
                        .Select(x => new { x.RajId, x.NomeRaj, x.IsExcluido }).ToList();
                }
                else if (formtypeParameters == (int)TypeForm.Turma)
                {
                    if (isAdmin) formCad = context.Turmas.Where(x => x.NomeTurma != StringBase.TODOS.ToString())
                         .Select(x => new { x.TurmaId, x.NomeTurma, x.IsExcluido }).ToList();
                    else formCad = context.Turmas.Where(x => x.NomeTurma != StringBase.TODOS.ToString() && x.IsExcluido == false)
                        .Select(x => new { x.TurmaId, x.NomeTurma, x.IsExcluido }).ToList();
                }
                else if (formtypeParameters == (int)TypeForm.Cargo)
                {
                    if(isAdmin) formCad = context.Cargoes.Where(x => x.NomeCargo != StringBase.TODOS.ToString())
                        .Select(x => new { x.CargoId, x.NomeCargo, x.IsExcluido }).ToList();
                    else formCad = context.Cargoes.Where(x => x.NomeCargo != StringBase.TODOS.ToString() && x.IsExcluido == false)
                        .Select(x => new { x.CargoId, x.NomeCargo, x.IsExcluido }).ToList();
                }
            }

            dataGridView1.DataSource = formCad;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].HeaderText = TypeForm.RAJ.ToString();
            dataGridView1.Columns[2].Visible = false;
        }

        private void CadastroCJ_Form(int formtypeParameters)
        {
            this.Text = "Cadastro " + Enum.GetName(typeof(TypeForm), formtypeParameters);
            label1.Text = Enum.GetName(typeof(TypeForm), formtypeParameters);

            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> raj = context.RAJs.Where(x => x.NomeRaj != StringBase.TODOS.ToString()).OrderBy(x => x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                common.PreencheCombo(comboBox1, raj, "RajId", "NomeRaj");
                comboBox1.SelectedIndex = -1;
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
                if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;

                label2 = (Label)common.MudarStatusVisible(label2, true, TypeForm.RAJ.ToString());
            }

            label3 = (Label)common.MudarStatusVisible(label3, false, String.Empty);
            label4 = (Label)common.MudarStatusVisible(label4, false, String.Empty);

            comboBox1 = (ComboBox)common.MudarStatusVisible(comboBox1, true, String.Empty);

            comboBox2 = (ComboBox)common.MudarStatusVisible(comboBox2, false, String.Empty);
            comboBox3 = (ComboBox)common.MudarStatusVisible(comboBox3, false, String.Empty);
        }

        private void CadastroCidade_Form()
        {
            this.Text = "Cadastro Cidade";
            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> raj = context.RAJs.Where(x=>x.NomeRaj != StringBase.TODOS.ToString()).OrderBy(x=>x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                common.PreencheCombo(comboBox2, raj, "RajId", "NomeRaj");
                IEnumerable<dynamic> cj = context.CJs.Where(x => x.CjNome != StringBase.TODOS.ToString() && x.RajId == (int)comboBox2.SelectedValue)
                    .OrderBy(x=>x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                common.PreencheCombo(comboBox1, cj, "CjId", "CjNome");
                comboBox1.SelectedIndex = -1;
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
                if(comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            }

            label1 = (Label)common.MudarStatusVisible(label1, true, TypeForm.Cidade.ToString());
            label2 = (Label)common.MudarStatusVisible(label2, true, TypeForm.CJ.ToString());
            label3 = (Label)common.MudarStatusVisible(label3, true, TypeForm.RAJ.ToString());
            label4 = (Label)common.MudarStatusVisible(label4, false, string.Empty);

            comboBox1 = (ComboBox)common.MudarStatusVisible(comboBox1, true, String.Empty);
            comboBox2 = (ComboBox)common.MudarStatusVisible(comboBox2, true, String.Empty);
            comboBox3 = (ComboBox)common.MudarStatusVisible(comboBox3, false, String.Empty);
        }

        private void CadastroSetor_Form(int formtypeParameters)
        {
            if (TypeForm.Lote_Capacitar.ToString() == Enum.GetName(typeof(TypeForm), formtypeParameters)) this.Text = "Lote - Capacitação";
            else this.Text = "Cadastro Setor";

            using (capdeEntities context = new capdeEntities())
            {
                if(formAtual == (int)TypeForm.Setor)
                {
                    IEnumerable<dynamic> raj = context.RAJs.Where(x => x.NomeRaj != StringBase.TODOS.ToString()).OrderBy(x => x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                    common.PreencheCombo(comboBox3, raj, "RajId", "NomeRaj");

                    IEnumerable<dynamic> cj = context.CJs.Where(x => x.CjNome != StringBase.TODOS.ToString() && x.RajId == (int)comboBox3.SelectedValue)
                        .OrderBy(x => x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                    common.PreencheCombo(comboBox2, cj, "CjId", "CjNome");

                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.NomeCidade != StringBase.TODOS.ToString() && x.CjId == (int)comboBox2.SelectedValue)
                        .OrderBy(x => x.NomeCidade).Select(x => new { x.CidadeId, x.NomeCidade }).ToList();
                    common.PreencheCombo(comboBox1, cidade, "CidadeId", "NomeCidade");

                    label1 = (Label)common.MudarStatusVisible(label1, true, TypeForm.Setor.ToString());
                    label2 = (Label)common.MudarStatusVisible(label2, true, TypeForm.Cidade.ToString());
                    label3 = (Label)common.MudarStatusVisible(label3, true, TypeForm.CJ.ToString());
                    label4 = (Label)common.MudarStatusVisible(label4, true, TypeForm.RAJ.ToString());

                    comboBox1 = (ComboBox)common.MudarStatusVisible(comboBox1, true, String.Empty);
                    comboBox2 = (ComboBox)common.MudarStatusVisible(comboBox2, true, String.Empty);
                    comboBox3 = (ComboBox)common.MudarStatusVisible(comboBox3, true, String.Empty);
                }
                else if (formAtual == (int)TypeForm.Lote_Capacitar)
                {
                    IEnumerable<dynamic> lote;

                    if (radioEAD.Checked)
                    {
                        lote = context.Turmas.OrderBy(x => x.NomeTurma).Select(x => new { x.TurmaId, x.NomeTurma }).ToList();
                        comboBox1.Enabled = true;
                        common.PreencheCombo(comboBox1, lote, "TurmaId", "NomeTurma");
                        label2 = (Label)common.MudarStatusVisible(label2, true, TypeForm.Turma.ToString());
                        label3 = (Label)common.MudarStatusVisible(label3, true, "Data 1");
                        label4 = (Label)common.MudarStatusVisible(label4, true, "Data 2");
                    }
                    else if (radioPresencial.Checked)
                    {
                        lote = context.RAJs.Where(x => x.NomeRaj != StringBase.TODOS.ToString()).OrderBy(x => x.NomeRaj)
                            .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                        comboBox1.Enabled = true;
                        common.PreencheCombo(comboBox1, lote, "RajId", "NomeRaj");
                        label2 = (Label)common.MudarStatusVisible(label2, true, TypeForm.RAJ.ToString());
                    }
                    else if (radioOutro.Checked)
                    {
                        comboBox1.Enabled = false;
                        label2 = (Label)common.MudarStatusVisible(label2, false, String.Empty);
                    }

                    label1 = (Label)common.MudarStatusVisible(label1, true, "Lote");
                    dtPicker1 = (DateTimePicker)common.MudarStatusVisible(dtPicker1, true, String.Empty);
                    dtPicker2 = (DateTimePicker)common.MudarStatusVisible(dtPicker2, true, String.Empty);
                    btnArquivo = (Button)common.MudarStatusVisible(btnArquivo, true, String.Empty);
                    radioEAD = (RadioButton)common.MudarStatusVisible(radioEAD, true, String.Empty);
                    radioPresencial = (RadioButton)common.MudarStatusVisible(radioPresencial, true, String.Empty);
                    radioOutro = (RadioButton)common.MudarStatusVisible(radioOutro, true, String.Empty);
                }
            }   
        }

        private void btCadastrar_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != String.Empty)
            {
                bool isFirst = false;
                string reference = textBox1.Text;

                foreach(Char c in reference)
                {
                    if (String.IsNullOrWhiteSpace(c.ToString()) && !isFirst) textBox1.Text = textBox1.Text.Remove(0, 1);
                    else isFirst = true;
                }

                if (formAtual == (int)TypeForm.RAJ) CadastroRAJ();
                else if (formAtual == (int)TypeForm.Turma) CadastroTurma();
                else if (formAtual == (int)TypeForm.Cargo) CadastroCargo();
                else if (formAtual == (int)TypeForm.CJ) CadastroCJ();
                else if (formAtual == (int)TypeForm.Cidade) CadastroCidade();
                else if (formAtual == (int)TypeForm.Setor) CadastroSetor();
                else if (formAtual == (int)TypeForm.Lote_Capacitar) CadastroLote();

                if (formAtual != (int)TypeForm.Lote_Capacitar)
                {
                    textBox1.Clear();
                    condicaoInicial(formAtual);
                }

                registerInserted = true;
            }
            else
            {
                MessageBox.Show("Para realizar o cadastro, o respectivo compo não deve estar em branco", "Falha Cadastro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CadastroRAJ()
        {
            using(capdeEntities context = new capdeEntities())
            {
                RAJ rajVerif = context.RAJs.Where(x => x.NomeRaj == textBox1.Text).FirstOrDefault();
                if (isEditing)
                {
                    RAJ raj = context.RAJs.Where(x => x.RajId == idEditing).FirstOrDefault();
                    raj.NomeRaj = textBox1.Text;
                    raj.IsExcluido = false;
                }
                else if(rajVerif == null)
                {
                    RAJ raj = new RAJ
                    {
                        NomeRaj = textBox1.Text,
                        RajIdent = context.RAJs.Count() + 1,
                        IsExcluido = false,
                    };

                    CJ cj = new CJ
                    {
                        RajId = raj.RajId,
                        CjIdent = context.CJs.Count() + 1,
                        CjNome = StringBase.TODOS.ToString(),
                        IsExcluido = false,
                    };

                    Cidade cidade = new Cidade
                    {
                        CjId = cj.CjId,
                        NomeCidade = StringBase.TODOS.ToString(),
                        IsExcluido = false,
                    };

                    context.RAJs.Add(raj);
                    context.CJs.Add(cj);
                    context.Cidades.Add(cidade);
                }

                if (isEditing || rajVerif == null)
                {
                    DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                    config.HasChanged = true;

                    context.SaveChanges();

                    OutEditing();
                }
                else MessageBox_AlreadyCadastrado(textBox1, TypeForm.RAJ.ToString());
            };
        }

        private void CadastroTurma()
        {
            using (capdeEntities context = new capdeEntities())
            {
                Turma verifTurma = context.Turmas.Where(x => x.NomeTurma == textBox1.Text).FirstOrDefault();
                if (isEditing)
                {
                    Turma turma = context.Turmas.Where(x => x.TurmaId == idEditing).FirstOrDefault();
                    turma.NomeTurma = textBox1.Text;
                    turma.IsExcluido = false;
                }
                else if(verifTurma == null)
                {
                    Turma turma = new Turma
                    {
                        NomeTurma = textBox1.Text,
                        IsExcluido = false,
                    };

                    context.Turmas.Add(turma);
                }

                if (isEditing || verifTurma == null)
                {
                    DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                    config.HasChanged = true;

                    context.SaveChanges();
                    OutEditing();
                }
                else MessageBox_AlreadyCadastrado(textBox1, TypeForm.Turma.ToString());
            };
        }

        private void CadastroCargo()
        {
            using (capdeEntities context = new capdeEntities())
            {
                Cargo verifCargo = context.Cargoes.Where(x => x.NomeCargo == textBox1.Text).FirstOrDefault();
                if (isEditing)
                {
                    Cargo cargo = context.Cargoes.Where(x => x.CargoId == idEditing).FirstOrDefault();
                    cargo.NomeCargo = textBox1.Text;
                    cargo.IsExcluido = false;
                }
                else if(verifCargo == null)
                {
                    Cargo cargo = new Cargo
                    {
                        NomeCargo = textBox1.Text,
                        IsExcluido = false,
                    };

                    context.Cargoes.Add(cargo);
                }

                if (isEditing || verifCargo == null)
                {
                    DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                    config.HasChanged = true;

                    context.SaveChanges();
                    OutEditing();
                }
                else MessageBox_AlreadyCadastrado(textBox1, TypeForm.Cargo.ToString());
            };
        }

        private void CadastroCJ()
        {
            using (capdeEntities context = new capdeEntities())
            {
                CJ verifCJ = context.CJs.Where(x => x.CjNome == textBox1.Text).FirstOrDefault();
                if (isEditing)
                {
                    CJ cj = context.CJs.Where(x => x.CjId == idEditing).FirstOrDefault();
                    cj.CjNome = textBox1.Text;
                    cj.IsExcluido = false;
                }
                else if(verifCJ == null)
                {
                    CJ cj = new CJ
                    {
                        RajId = (int)comboBox1.SelectedValue,
                        CjIdent = context.CJs.Count() + 1,
                        CjNome = textBox1.Text,
                        IsExcluido = false,
                    };

                    Cidade cidade = new Cidade
                    {
                        CjId = cj.CjId,
                        NomeCidade = StringBase.TODOS.ToString(),
                        IsExcluido = false,
                    };

                    context.CJs.Add(cj);
                    context.Cidades.Add(cidade);
                }

                if (isEditing || verifCJ == null)
                {
                    DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                    config.HasChanged = true;

                    context.SaveChanges();
                    OutEditing();
                }
                else MessageBox_AlreadyCadastrado(textBox1, TypeForm.CJ.ToString());
            };
        }

        private void CadastroCidade()
        {
            using (capdeEntities context = new capdeEntities())
            {
                Cidade verifCidade = context.Cidades.Where(x => x.NomeCidade == textBox1.Text).FirstOrDefault();
                if (isEditing)
                {
                    Cidade cidade = context.Cidades.Where(x => x.CidadeId == idEditing).FirstOrDefault();
                    cidade.NomeCidade = textBox1.Text;
                    cidade.IsExcluido = false;
                }
                else if(verifCidade == null)
                {
                    Cidade cidade = new Cidade
                    {
                        CjId = (int)comboBox1.SelectedValue,
                        NomeCidade = textBox1.Text,
                        IsExcluido = false,
                    };

                    context.Cidades.Add(cidade);
                }

                if (isEditing || verifCidade == null)
                {
                    DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                    config.HasChanged = true;

                    context.SaveChanges();
                    OutEditing();
                }
                else MessageBox_AlreadyCadastrado(textBox1, TypeForm.Cidade.ToString());
            };
        }

        private void CadastroSetor()
        {
            using (capdeEntities context = new capdeEntities())
            {
                Setor verifSetor = context.Setors.Where(x => x.NomeSetor == textBox1.Text).FirstOrDefault();
                if (isEditing)
                {
                    Setor setor = context.Setors.Where(x => x.SetorId == idEditing).FirstOrDefault();
                    setor.NomeSetor = textBox1.Text;
                    setor.IsExcluido = false;
                }
                else if(verifSetor == null)
                {
                    Setor setor = new Setor
                    {
                        CidadeId = (int)comboBox1.SelectedValue,
                        NomeSetor = textBox1.Text,
                        IsExcluido = false,
                    };

                    context.Setors.Add(setor);
                }

                if (isEditing || verifSetor == null)
                {
                    DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                    config.HasChanged = true;

                    context.SaveChanges();
                    OutEditing();
                }
                else MessageBox_AlreadyCadastrado(textBox1, TypeForm.Setor.ToString());
            };
        }

        private void CadastroLote()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            if (textBox1.Text != String.Empty && dtPicker1.Value != DateTime.Now)
            {
                string[] registros = File.ReadAllLines(textBox1.Text);
                tspProgressBar.Maximum = registros.Length;

                worker.RunWorkerAsync();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.Clear();
            condicaoInicial(formAtual);
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            tspProgressBar.Value = 0;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tspProgressBar.Value = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] registros = File.ReadAllLines(textBox1.Text);

            using (capdeEntities context = new capdeEntities())
            {
                for (int i = 0; i < registros.Length; i++)
                {
                    string idRegistro = normalizeString(registros[i]);
                    Pessoa pessoa = context.Pessoas.Where(x => x.Registro == idRegistro).FirstOrDefault();

                    if (pessoa != null)
                    {
                        pessoa.Capacitacao.IsCapacitado = true;
                        pessoa.Capacitacao.DataInicio = dtPicker1.Value;
                        pessoa.Capacitacao.DataFim = dtPicker2.Value;

                        if (radioEAD.Checked && radioEAD.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                pessoa.Capacitacao.IsEAD = true;
                                pessoa.Capacitacao.TurmaId = (int)comboBox1.SelectedValue;
                            });
                        }
                        else if (radioPresencial.Checked && comboBox1.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                pessoa.Capacitacao.RajId = (int)comboBox1.SelectedValue;
                            });
                        }

                        context.SaveChanges();
                    }

                    (sender as BackgroundWorker).ReportProgress(i);
                }

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

                context.SaveChanges();
            }
        }

        private string normalizeString(string texto)
        {
            string retorno = String.Empty;

            for(int i=0; i < texto.Length; i++)
            {
                if (Convert.ToChar(texto[i]).ToString() != " ")
                {
                    retorno += Convert.ToChar(texto[i]).ToString();
                }
            }

            return retorno;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(comboBox3.ValueMember))
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> cj = context.CJs.Where(x => x.RajId == (int)comboBox3.SelectedValue && x.CjNome != StringBase.TODOS.ToString())
                        .OrderBy(x=>x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                    common.PreencheCombo(comboBox2, cj, "CjId", "CjNome");
                }
            } 
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (capdeEntities context = new capdeEntities())
            {
                if(!String.IsNullOrEmpty(comboBox2.ValueMember))
                {
                    if (formAtual == (int)TypeForm.Setor)
                    {
                        IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.CjId == (int)comboBox2.SelectedValue && x.NomeCidade != StringBase.TODOS.ToString())
                            .OrderBy(x=>x.NomeCidade).Select(x => new { x.CidadeId, x.NomeCidade }).ToList();
                        common.PreencheCombo(comboBox1, cidade, "CidadeId", "NomeCidade");
                    }
                    else if (formAtual == (int)TypeForm.Cidade)
                    {
                        IEnumerable<dynamic> cj = context.CJs.Where(x=>x.RajId == (int)comboBox2.SelectedValue && x.CjNome != StringBase.TODOS.ToString())
                            .OrderBy(x=>x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                        common.PreencheCombo(comboBox1, cj, "CjId", "CjNome");
                    }
                }
            }
        }

        private void FormCad_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (registerInserted) this.DialogResult = DialogResult.OK;
            else this.DialogResult = DialogResult.No;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) btCadastrar.PerformClick();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox1.Text = String.Empty;
                textBox1.Select(0, 0);
            }
        }

        private void btnArquivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text Document | *.txt";

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
                string[] list = File.ReadAllLines(dialog.FileName);
                dataGridView1.Columns.Add("Matricula", "Matrícula");
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                foreach (string i in list)
                {
                    dataGridView1.Rows.Add(i);
                }
            }
        }

        private void radioEAD_CheckedChanged(object sender, EventArgs e)
        {
            using(capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> turma = context.Turmas.OrderBy(x => x.NomeTurma).Select(x => new { x.TurmaId, x.NomeTurma }).ToList();
                comboBox1.Enabled = true;
                comboBox1 = common.PreencheCombo(comboBox1, turma, "TurmaId", "NomeTurma");

                label2 = (Label)common.MudarStatusVisible(label2, true, TypeForm.Turma.ToString());
            }
        }

        private void radioPresencial_CheckedChanged(object sender, EventArgs e)
        {
            using(capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> raj = context.RAJs.Where(x=>x.NomeRaj != StringBase.TODOS.ToString()).OrderBy(x => x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                comboBox1.Enabled = true;
                comboBox1 = common.PreencheCombo(comboBox1, raj, "RajId", "NomeRaj");

                label2 = (Label)common.MudarStatusVisible(label2, true, TypeForm.RAJ.ToString());
            }
        }

        private void radioOutro_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            label2 = (Label)common.MudarStatusVisible(label2, false, String.Empty);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(comboBox1.ValueMember) && formAtual != (int)TypeForm.Lote_Capacitar)
            {
                IEnumerable<dynamic> formEnum = null;
                string headerText = String.Empty;

                using (capdeEntities context = new capdeEntities())
                {
                    if (formAtual == (int)TypeForm.CJ)
                    {
                        headerText = TypeForm.CJ.ToString();
                        if (isAdmin) formEnum = context.CJs.Where(x => x.CjNome != StringBase.TODOS.ToString() && x.RajId == (int)comboBox1.SelectedValue)
                            .Select(x => new { x.CjId, x.CjNome, x.IsExcluido }).ToList();
                        else formEnum = context.CJs.Where(x => x.CjNome != StringBase.TODOS.ToString() && x.RajId == (int)comboBox1.SelectedValue &&
                        x.IsExcluido == false).Select(x => new { x.CjId, x.CjNome, x.IsExcluido }).ToList();
                    }
                    else if (formAtual == (int)TypeForm.Cidade)
                    {
                        headerText = TypeForm.Cidade.ToString();
                        if (isAdmin) formEnum = context.Cidades.Where(x => x.NomeCidade != StringBase.TODOS.ToString() &&
                         x.CjId == (int)comboBox1.SelectedValue).Select(x => new { x.CidadeId, x.NomeCidade, x.IsExcluido }).ToList();
                        else formEnum = context.Cidades.Where(x => x.NomeCidade != StringBase.TODOS.ToString() && x.IsExcluido == false &&
                        x.CjId == (int)comboBox1.SelectedValue).Select(x => new { x.CidadeId, x.NomeCidade, x.IsExcluido }).ToList();
                    }
                    else if (formAtual == (int)TypeForm.Setor)
                    {
                        headerText = TypeForm.Setor.ToString();
                        if(isAdmin) formEnum = context.Setors.Where(x => x.CidadeId == (int)comboBox1.SelectedValue)
                            .Select(x => new { x.SetorId, x.NomeSetor, x.IsExcluido }).ToList();
                        else formEnum = context.Setors.Where(x => x.CidadeId == (int)comboBox1.SelectedValue && x.IsExcluido == false)
                            .Select(x => new { x.SetorId, x.NomeSetor, x.IsExcluido }).ToList();
                    }
                }

                dataGridView1.DataSource = formEnum;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].HeaderText = headerText;
                dataGridView1.Columns[2].Visible = false;
            } 
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if(formAtual != (int)TypeForm.Lote_Capacitar && dataGridView1.CurrentRow != null && 
                dataGridView1.CurrentRow.Cells[1].Value.ToString() != StringBase.TODOS.ToString())
            {
                idEditing = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                textBox1.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                btCadastrar.Text = "Atualizar";
                isEditing = true;
            }
        }

        private void OutEditing()
        {
            btCadastrar.Text = "Cadastrar";
            idEditing = null;
            isEditing = false;
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGridView1.ClearSelection();
                int rowIndex = dataGridView1.HitTest(e.Location.X, e.Location.Y).RowIndex;

                if (rowIndex >= 0)
                {
                    dataGridView1.Rows[rowIndex].Selected = true;
                    idEditing = (int)dataGridView1.CurrentRow.Cells[0].Value;
                    contextMenuStrip1.Show(dataGridView1, new Point(e.Location.X, e.Location.Y));
                }
            }
        }

        private void excluirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IncluirExcluirRegistro("Excluir", true);
        }

        private void incluirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IncluirExcluirRegistro("Incluir", false);
        }

        private void IncluirExcluirRegistro(string acao, bool isExcluido)
        {
            using (capdeEntities context = new capdeEntities())
            {
                Object objToDel = new Object();

                if (formAtual == (int)TypeForm.RAJ) objToDel = context.RAJs.Where(x => x.RajId == idEditing).FirstOrDefault();
                else if (formAtual == (int)TypeForm.CJ) objToDel = context.CJs.Where(x => x.CjId == idEditing).FirstOrDefault();
                else if (formAtual == (int)TypeForm.Cidade) objToDel = context.Cidades.Where(x => x.CidadeId == idEditing).FirstOrDefault();
                else if (formAtual == (int)TypeForm.Setor) objToDel = context.Setors.Where(x => x.SetorId == idEditing).FirstOrDefault();
                else if (formAtual == (int)TypeForm.Cargo) objToDel = context.Cargoes.Where(x => x.CargoId == idEditing).FirstOrDefault();
                else if (formAtual == (int)TypeForm.Turma) objToDel = context.Turmas.Where(x => x.TurmaId == idEditing).FirstOrDefault();

                string registro = dataGridView1.CurrentRow.Cells[1].Value.ToString();

                if (MessageBox.Show("Deseja " + acao + " o registro " + registro + "?", acao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (objToDel is RAJ)
                        if (isAdmin)
                        {
                            if (isExcluido) context.RAJs.Remove((RAJ)objToDel);
                            else context.RAJs.Add((RAJ)objToDel);
                        }
                        else (objToDel as RAJ).IsExcluido = isExcluido;
                    else if (objToDel is CJ)
                        if (isAdmin)
                        {
                            if (isExcluido) context.CJs.Remove((CJ)objToDel);
                            else context.CJs.Add((CJ)objToDel);
                        }
                        else (objToDel as CJ).IsExcluido = isExcluido;
                    else if (objToDel is Cidade)
                        if (isAdmin)
                        {
                            if (isExcluido) context.Cidades.Remove((Cidade)objToDel);
                            else context.Cidades.Add((Cidade)objToDel);
                        }
                        else (objToDel as Cidade).IsExcluido = isExcluido;
                    else if (objToDel is Setor)
                        if (isAdmin)
                        {
                            if (isExcluido) context.Setors.Remove((Setor)objToDel);
                            else context.Setors.Add((Setor)objToDel);
                        }
                        else (objToDel as Setor).IsExcluido = isExcluido;
                    else if (objToDel is Cargo)
                        if (isAdmin)
                        {
                            if (isExcluido) context.Cargoes.Remove((Cargo)objToDel);
                            else context.Cargoes.Add((Cargo)objToDel);
                        }
                        else (objToDel as Cargo).IsExcluido = isExcluido;
                    else if (objToDel is Turma)
                        if (isAdmin)
                        {
                            if (isExcluido) context.Turmas.Remove((Turma)objToDel);
                            else context.Turmas.Add((Turma)objToDel);
                        }
                        else (objToDel as Turma).IsExcluido = isExcluido;

                    DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                    config.HasChanged = true;

                    context.SaveChanges();
                    condicaoInicial(formAtual);
                    OutEditing();
                }
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            foreach(DataGridViewRow i in dataGridView1.Rows)
            {
                bool isExluido = (bool)i.Cells[2].Value;

                if (isExluido) i.DefaultCellStyle.BackColor = Color.Tomato;
            }
        }

        private void MessageBox_AlreadyCadastrado(TextBox text, string tabela)
        {
            MessageBox.Show("Não foi possível realizar o cadastro pois o mesmo já se encontra cadastrado", "Falha Cadastro " + tabela,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            text.Clear();
        }
    }
}