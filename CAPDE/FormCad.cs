using CAPDEData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Common.CAPDEEnums;

namespace CAPDE
{
    public partial class FormCad : Form
    {
        Common common = new Common();
        int formAtual = 0;

        bool registerInserted = false;

        public FormCad(int typeForm, int heightForm)
        {
            InitializeComponent();

            this.Height = heightForm;

            formAtual = typeForm;
            condicaoInicial(typeForm);
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

            label2 = (Label)MudarStatusVisible(label2, false, String.Empty);
            label3 = (Label)MudarStatusVisible(label3, false, String.Empty);
            label4 = (Label)MudarStatusVisible(label4, false, String.Empty);
            comboBox1 = (ComboBox)MudarStatusVisible(comboBox1, false, String.Empty);
            comboBox2 = (ComboBox)MudarStatusVisible(comboBox2, false, String.Empty);
            comboBox3 = (ComboBox)MudarStatusVisible(comboBox3, false, String.Empty);
        }

        private void CadastroCJ_Form(int formtypeParameters)
        {
            this.Text = "Cadastro " + Enum.GetName(typeof(TypeForm), formtypeParameters);
            label1.Text = Enum.GetName(typeof(TypeForm), formtypeParameters);

            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> raj = context.RAJs.Where(x => x.NomeRaj != "TODOS").OrderBy(x => x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                common.PreencheCombo(comboBox1, raj, "RajId", "NomeRaj");

                label2 = (Label)MudarStatusVisible(label2, true, "RAJ");
            }

            label3 = (Label)MudarStatusVisible(label3, false, String.Empty);
            label4 = (Label)MudarStatusVisible(label4, false, String.Empty);

            comboBox1 = (ComboBox)MudarStatusVisible(comboBox1, true, String.Empty);

            comboBox2 = (ComboBox)MudarStatusVisible(comboBox2, false, String.Empty);
            comboBox3 = (ComboBox)MudarStatusVisible(comboBox3, false, String.Empty);
        }

        private void CadastroCidade_Form()
        {
            this.Text = "Cadastro Cidade";
            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> raj = context.RAJs.Where(x=>x.NomeRaj != "TODOS").OrderBy(x=>x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                common.PreencheCombo(comboBox2, raj, "RajId", "NomeRaj");
                IEnumerable<dynamic> cj = context.CJs.Where(x => x.CjNome != "TODOS" && x.RajId == (int)comboBox2.SelectedValue)
                    .OrderBy(x=>x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                common.PreencheCombo(comboBox1, cj, "CjId", "CjNome");
            }

            label1 = (Label)MudarStatusVisible(label1, true, "Cidade");
            label2 = (Label)MudarStatusVisible(label2, true, "CJ");
            label3 = (Label)MudarStatusVisible(label3, true, "RAJ");
            label4 = (Label)MudarStatusVisible(label4, false, string.Empty);

            comboBox1 = (ComboBox)MudarStatusVisible(comboBox1, true, String.Empty);
            comboBox2 = (ComboBox)MudarStatusVisible(comboBox2, true, String.Empty);
            comboBox3 = (ComboBox)MudarStatusVisible(comboBox3, false, String.Empty);
        }

        private void CadastroSetor_Form(int formtypeParameters)
        {
            if (TypeForm.Lote_Capacitar.ToString() == Enum.GetName(typeof(TypeForm), formtypeParameters)) this.Text = "Lote - Capacitação";
            else this.Text = "Cadastro Setor";

            using (capdeEntities context = new capdeEntities())
            {
                if(formAtual == (int)TypeForm.Setor)
                {
                    IEnumerable<dynamic> raj = context.RAJs.Where(x => x.NomeRaj != "TODOS").OrderBy(x => x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                    common.PreencheCombo(comboBox3, raj, "RajId", "NomeRaj");

                    IEnumerable<dynamic> cj = context.CJs.Where(x => x.CjNome != "TODOS" && x.RajId == (int)comboBox3.SelectedValue)
                        .OrderBy(x => x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                    common.PreencheCombo(comboBox2, cj, "CjId", "CjNome");

                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.NomeCidade != "TODOS" && x.CjId == (int)comboBox2.SelectedValue)
                        .OrderBy(x => x.NomeCidade).Select(x => new { x.CidadeId, x.NomeCidade }).ToList();
                    common.PreencheCombo(comboBox1, cidade, "CidadeId", "NomeCidade");

                    label1 = (Label)MudarStatusVisible(label1, true, "Setor");
                    label2 = (Label)MudarStatusVisible(label2, true, "Cidade");
                    label3 = (Label)MudarStatusVisible(label3, true, "CJ");
                    label4 = (Label)MudarStatusVisible(label4, true, "RAJ");

                    comboBox1 = (ComboBox)MudarStatusVisible(comboBox1, true, String.Empty);
                    comboBox2 = (ComboBox)MudarStatusVisible(comboBox2, true, String.Empty);
                    comboBox3 = (ComboBox)MudarStatusVisible(comboBox3, true, String.Empty);
                }
                else if (formAtual == (int)TypeForm.Lote_Capacitar)
                {
                    IEnumerable<dynamic> lote;

                    if (radioEAD.Checked)
                    {
                        lote = context.Turmas.OrderBy(x => x.NomeTurma).Select(x => new { x.TurmaId, x.NomeTurma }).ToList();
                        comboBox1.Enabled = true;
                        common.PreencheCombo(comboBox1, lote, "TurmaId", "NomeTurma");
                        label2 = (Label)MudarStatusVisible(label2, true, "Turma");
                        label3 = (Label)MudarStatusVisible(label3, true, "Data 1");
                        label4 = (Label)MudarStatusVisible(label4, true, "Data 2");
                    }
                    else if (radioPresencial.Checked)
                    {
                        lote = context.RAJs.Where(x => x.NomeRaj != "TODOS").OrderBy(x => x.NomeRaj)
                            .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                        comboBox1.Enabled = true;
                        common.PreencheCombo(comboBox1, lote, "RajId", "NomeRaj");
                        label2 = (Label)MudarStatusVisible(label2, true, "RAJ");
                    }
                    else if (radioOutro.Checked)
                    {
                        comboBox1.Enabled = false;
                        label2 = (Label)MudarStatusVisible(label2, false, String.Empty);
                    }

                    label1 = (Label)MudarStatusVisible(label1, true, "Lote");
                    dtPicker1 = (DateTimePicker)MudarStatusVisible(dtPicker1, true, String.Empty);
                    dtPicker2 = (DateTimePicker)MudarStatusVisible(dtPicker2, true, String.Empty);
                    btnArquivo = (Button)MudarStatusVisible(btnArquivo, true, String.Empty);
                    radioEAD = (RadioButton)MudarStatusVisible(radioEAD, true, String.Empty);
                    radioPresencial = (RadioButton)MudarStatusVisible(radioPresencial, true, String.Empty);
                    radioOutro = (RadioButton)MudarStatusVisible(radioOutro, true, String.Empty);
                }
            }   
        }
        #region MudarStatus Component

        private object MudarStatusVisible(object obj, bool valor, string text)
        {
            if (obj is Label)
            {
                (obj as Label).Visible = valor;
                (obj as Label).Text = text;
            }
            else if (obj is TextBox) (obj as TextBox).Visible = valor;
            else if (obj is ComboBox) (obj as ComboBox).Visible = valor;
            else if (obj is Button) (obj as Button).Visible = valor;
            else if (obj is RadioButton) (obj as RadioButton).Visible = valor;
            else if (obj is DateTimePicker) (obj as DateTimePicker).Visible = valor;

            return obj;
        }

        #endregion

        private void btCadastrar_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != String.Empty)
            {
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
                RAJ raj = new RAJ
                {
                    NomeRaj = textBox1.Text,
                    RajIdent = context.RAJs.Count() + 1,
                };

                CJ cj = new CJ
                {
                    RajId = raj.RajId,
                    CjIdent = context.CJs.Count() + 1,
                    CjNome = "TODOS"
                };

                Cidade cidade = new Cidade
                {
                    CjId = cj.CjId,
                    NomeCidade = "TODOS",
                };

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

                context.RAJs.Add(raj);
                context.CJs.Add(cj);
                context.Cidades.Add(cidade);
                context.SaveChanges();
            };
            
        }

        private void CadastroTurma()
        {
            using (capdeEntities context = new capdeEntities())
            {
                Turma turma = new Turma
                {
                    NomeTurma = textBox1.Text,
                };

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

                context.Turmas.Add(turma);
                context.SaveChanges();
            };
        }

        private void CadastroCargo()
        {
            using (capdeEntities context = new capdeEntities())
            {
                Cargo cargo = new Cargo
                {
                    NomeCargo = textBox1.Text,
                };

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

                context.Cargoes.Add(cargo);
                context.SaveChanges();
            };
        }

        private void CadastroCJ()
        {
            using (capdeEntities context = new capdeEntities())
            {
                CJ cj = new CJ
                {
                    RajId = (int)comboBox1.SelectedValue,
                    CjIdent = context.CJs.Count() + 1,
                    CjNome = textBox1.Text,
                };

                Cidade cidade = new Cidade
                {
                    CjId = cj.CjId,
                    NomeCidade = "TODOS",
                };

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

                context.CJs.Add(cj);
                context.Cidades.Add(cidade);
                context.SaveChanges();
            };
        }

        private void CadastroCidade()
        {
            using (capdeEntities context = new capdeEntities())
            {
                Cidade cidade = new Cidade
                {
                    CjId = (int)comboBox1.SelectedValue,
                    NomeCidade = textBox1.Text,
                };

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

                context.Cidades.Add(cidade);
                context.SaveChanges();
            };
        }

        private void CadastroSetor()
        {
            using (capdeEntities context = new capdeEntities())
            {
                Setor setor = new Setor
                {
                    CidadeId = (int)comboBox1.SelectedValue,
                    NomeSetor = textBox1.Text,
                };

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

                context.Setors.Add(setor);
                context.SaveChanges();
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
            if(comboBox3.ValueMember != "")
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> cj = context.CJs.Where(x => x.RajId == (int)comboBox3.SelectedValue && x.CjNome != "TODOS").OrderBy(x=>x.CjNome)
                        .Select(x => new { x.CjId, x.CjNome }).ToList();
                    common.PreencheCombo(comboBox2, cj, "CjId", "CjNome");
                }
            } 
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (capdeEntities context = new capdeEntities())
            {
                if(comboBox2.ValueMember != String.Empty)
                {
                    if (formAtual == (int)TypeForm.Setor)
                    {
                        IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.CjId == (int)comboBox2.SelectedValue && x.NomeCidade != "TODOS")
                            .OrderBy(x=>x.NomeCidade).Select(x => new { x.CidadeId, x.NomeCidade }).ToList();
                        common.PreencheCombo(comboBox1, cidade, "CidadeId", "NomeCidade");
                    }
                    else if (formAtual == (int)TypeForm.Cidade)
                    {
                        IEnumerable<dynamic> cj = context.CJs.Where(x=>x.RajId == (int)comboBox2.SelectedValue && x.CjNome != "TODOS").OrderBy(x=>x.CjNome)
                            .Select(x => new { x.CjId, x.CjNome }).ToList();
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
            if(e.KeyCode == Keys.Enter)
            {
                btCadastrar.PerformClick();
            }
        }

        private void btnArquivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text Document | *.txt";

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
            }
        }

        private void radioEAD_CheckedChanged(object sender, EventArgs e)
        {
            using(capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> turma = context.Turmas.OrderBy(x => x.NomeTurma).Select(x => new { x.TurmaId, x.NomeTurma }).ToList();
                comboBox1.Enabled = true;
                comboBox1 = common.PreencheCombo(comboBox1, turma, "TurmaId", "NomeTurma");

                label2 = (Label)MudarStatusVisible(label2, true, "Turma");
            }
        }

        private void radioPresencial_CheckedChanged(object sender, EventArgs e)
        {
            using(capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> raj = context.RAJs.Where(x=>x.NomeRaj != "TODOS").OrderBy(x => x.NomeRaj)
                    .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                comboBox1.Enabled = true;
                comboBox1 = common.PreencheCombo(comboBox1, raj, "RajId", "NomeRaj");

                label2 = (Label)MudarStatusVisible(label2, true, "RAJ");
            }
        }

        private void radioOutro_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            label2 = (Label)MudarStatusVisible(label2, false, String.Empty);
        }
    }
}


