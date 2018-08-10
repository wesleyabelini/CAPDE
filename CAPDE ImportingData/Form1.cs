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

namespace CAPDE_ImportingData
{
    public partial class Form1 : Form
    {
        string[] matricula;
        string[] nome;
        string[] cargo;
        string[] obs;
        string[] email;

        string[] raj;
        string[] cj;
        string[] cidade;
        string[] setor;

        List<string> logMatricula = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private string getFileLocation()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else return String.Empty;
        }

        #region botao getLocationFile

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = getFileLocation();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = getFileLocation();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Text = getFileLocation();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox5.Text = getFileLocation();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox7.Text = getFileLocation();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox11.Text = getFileLocation();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox10.Text = getFileLocation();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox9.Text = getFileLocation();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox8.Text = getFileLocation();
        }

        #endregion

        private void button8_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) CadastrarEntidade();
            else if (radioButton2.Checked) CertificarPessoa();
        }

        private void CertificarPessoa()
        {
            matricula = File.ReadAllLines(textBox2.Text, Encoding.Default);
            progressBar1.Maximum = matricula.Length;
            logMatricula.Clear();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;

            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            File.AppendAllLines("C:\\Users\\Wesley\\Desktop\\docsLiene\\registro\\log.txt", logMatricula);
            progressBar1.Value = 0;
            MessageBox.Show("Dados salvos");
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (capdeEntities context = new capdeEntities())
            {
                for (int i = 0; i < matricula.Length; i++)
                {
                    string fileMatricula = matricula[i];
                    Pessoa pessoa = new Pessoa();

                    if(radioButton3.Checked) pessoa = context.Pessoas.Where(x => x.EMail == fileMatricula.ToUpper()).FirstOrDefault();
                    else if(radioButton4.Checked) pessoa = context.Pessoas.Where(x => x.Registro == fileMatricula.ToUpper()).FirstOrDefault();
                    else if(radioButton5.Checked) pessoa = context.Pessoas.Where(x => x.Nome == fileMatricula.ToUpper()).FirstOrDefault();

                    if (pessoa != null)
                    {
                        pessoa.Capacitacao.IsCapacitado = true;
                        pessoa.Capacitacao.RajId = pessoa.Setor.Cidade.CJ.RajId;

                        context.SaveChanges();
                    }
                    else logMatricula.Add(matricula[i]);

                    (sender as BackgroundWorker).ReportProgress(i);
                }
            }
        }

        private void CadastrarEntidade()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.WorkerReportsProgress = true;

            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            MessageBox.Show("Dados salvos");
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CadastroRaj(StringBase.TODOS.ToString());

            matricula = File.ReadAllLines(textBox2.Text, Encoding.Default);
            nome = File.ReadAllLines(textBox3.Text, Encoding.Default);
            email = File.ReadAllLines(textBox7.Text, Encoding.Default);
            cargo = File.ReadAllLines(textBox4.Text, Encoding.Default);
            obs = File.ReadAllLines(textBox5.Text, Encoding.Default);

            raj = File.ReadAllLines(textBox11.Text, Encoding.Default);
            cj = File.ReadAllLines(textBox10.Text, Encoding.Default);
            cidade = File.ReadAllLines(textBox9.Text, Encoding.Default);
            setor = File.ReadAllLines(textBox8.Text, Encoding.Default);

            if (progressBar1.InvokeRequired)
            {
                this.Invoke((MethodInvoker) delegate
                {
                    progressBar1.Maximum = matricula.Length;
                });
            }

            using (capdeEntities context = new capdeEntities())
            {
                for (int i = 0; i < raj.Length; i++)
                {
                    string a_raj = raj[i].ToString();
                    string a_cj = cj[i].ToString();
                    string a_cidade = cidade[i].ToString();
                    string a_setor = setor[i].ToString();
                    string a_cargo = cargo[i].ToString();

                    int rajId = context.RAJs.Where(x => x.NomeRaj == a_raj).Select(x => x.RajId).FirstOrDefault();
                    int cjId = context.CJs.Where(x => x.CjNome == a_cj).Select(x => x.CjId).FirstOrDefault();
                    int cidadeId = context.Cidades.Where(x => x.NomeCidade == a_cidade).Select(x => x.CidadeId).FirstOrDefault();
                    int setorId = context.Setors.Where(x => x.NomeSetor == a_setor).Select(x => x.SetorId).FirstOrDefault();

                    int cargoId = context.Cargoes.Where(x => x.NomeCargo == a_cargo).Select(x => x.CargoId).FirstOrDefault();

                    if (rajId == 0) rajId = CadastroRaj(a_raj);
                    if (cjId == 0) cjId = CadastroCJ(a_cj, rajId);
                    if (cidadeId == 0) cidadeId = CadastroCidade(a_cidade, cjId);
                    if (setorId == 0) setorId = CadastroSetor(a_setor, cidadeId);
                    if (cargoId == 0) cargoId = CadastroCargo(a_cargo);

                    Capacitacao capacitacao = new Capacitacao
                    {
                        IsCapacitado = false,
                    };

                    context.Capacitacaos.Add(capacitacao);

                    Pessoa pessoa = new Pessoa
                    {
                        Nome = nome[i].ToUpper(),
                        Registro = matricula[i],
                        EMail = email[i],
                        SetorId = setorId,
                        Obs = obs[i],
                        CargoId = cargoId,
                        CapacitacaoId = capacitacao.CapacitacaoId,
                        IsAposentado = false,
                        IsExcluido = false,
                    };

                    context.Pessoas.Add(pessoa);
                    context.SaveChanges();

                    (sender as BackgroundWorker).ReportProgress(i);
                }
            }
        }

        #region Novas Entidades

        private int CadastroCargo(string cargo)
        {
            using(capdeEntities context = new capdeEntities())
            {
                Cargo newCargo = new Cargo
                {
                    NomeCargo = cargo.ToUpper(),
                    IsExcluido = false,
                };

                context.Cargoes.Add(newCargo);
                context.SaveChanges();

                return newCargo.CargoId;
            }
        }

        private int CadastroRaj(string raj)
        {
            using(capdeEntities context = new capdeEntities())
            {
                RAJ newRaj = new RAJ
                {
                    NomeRaj = raj.ToUpper(),
                    IsExcluido = false,
                };

                CJ cj = new CJ
                {
                    RajId = newRaj.RajId,
                    CjIdent = context.CJs.Count() + 1,
                    CjNome = StringBase.TODOS.ToString()
                };

                Cidade cidade = new Cidade
                {
                    CjId = cj.CjId,
                    NomeCidade = StringBase.TODOS.ToString(),
                };

                context.RAJs.Add(newRaj);
                context.CJs.Add(cj);
                context.Cidades.Add(cidade);
                context.SaveChanges();

                return newRaj.RajId;
            }
        }

        private int CadastroCJ(string cj, int raj)
        {
            using(capdeEntities context = new capdeEntities())
            {
                CJ newCJ = new CJ
                {
                    RajId = raj,
                    CjNome = cj.ToUpper(),
                    IsExcluido = false,
                };

                Cidade cidade = new Cidade
                {
                    CJ = newCJ,
                    NomeCidade = StringBase.TODOS.ToString(),
                    IsExcluido = false,
                };

                context.CJs.Add(newCJ);
                context.Cidades.Add(cidade);
                context.SaveChanges();

                return newCJ.CjId;
            }
        }

        private int CadastroCidade(string cidade, int cj)
        {
            using(capdeEntities context = new capdeEntities())
            {
                Cidade newCidade = new Cidade
                {
                    CjId = cj,
                    NomeCidade = cidade.ToUpper(),
                    IsExcluido = false,
                };

                context.Cidades.Add(newCidade);
                context.SaveChanges();

                return newCidade.CidadeId;
            }
        }

        private int CadastroSetor(string setor, int cidade)
        {
            using(capdeEntities context = new capdeEntities())
            {
                Setor newSetor = new Setor
                {
                    CidadeId = cidade,
                    NomeSetor = setor.ToUpper(),
                    IsExcluido = false,
                };

                context.Setors.Add(newSetor);
                context.SaveChanges();

                return newSetor.SetorId;
            }
        }

        #endregion

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
        }
    }
}
