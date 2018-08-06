using CAPDEData;
using Common;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static Common.CAPDEEnums;

namespace CAPDELogin
{
    public partial class FormLogin : Form
    {
        Common.Common common = new Common.Common();
        Crypto crypt = new Crypto();
        Gmail gmail = new Gmail();

        private string logedUsername = String.Empty;
        private string logedUser = String.Empty;

        public string LogedUserName
        {
            get { return logedUsername; }
            set { logedUsername = value; }
        }

        public string LogedUser
        {
            get { return logedUser; }
            set { logedUser = value; }
        }

        int formAtual = 0;

        public FormLogin(int form, int heightForm)
        {
            InitializeComponent();

            formAtual = form;
            this.Height = heightForm;

            condicaoInicial();
        }

        private void condicaoInicial()
        {
            if (formAtual == (int)TypeForm.CadastroLogin) CadastroLogin_Form();
            else if (formAtual == (int)TypeForm.Login) Login_Form();
        }

        private void CadastroLogin_Form()
        {
            this.Text = "Cadastro";
            btnConfirm.Text = "Cadastrar";
            lblEsqueci = (Label)common.MudarStatusVisible(lblEsqueci, false, String.Empty);
        }

        private void Login_Form()
        {
            this.Text = "Login";
            textBox2.PasswordChar = Convert.ToChar("*");

            txtSenha = (TextBox)common.MudarStatusVisible(txtSenha, false, String.Empty);
            txtSenhaConfirm = (TextBox)common.MudarStatusVisible(txtSenhaConfirm, false, String.Empty);
            txtUsuario = (TextBox)common.MudarStatusVisible(txtUsuario, false, String.Empty);

            lblCinfirmSenha = (Label)common.MudarStatusVisible(lblCinfirmSenha, false, String.Empty);
            lblSenha = (Label)common.MudarStatusVisible(lblSenha, false, String.Empty);
            lblUsuario = (Label)common.MudarStatusVisible(lblUsuario, false, String.Empty);

            chkIsAdmin = (CheckBox)common.MudarStatusVisible(chkIsAdmin, false, String.Empty);

            label1 = (Label)common.MudarStatusVisible(label1, true, "Usuário");
            label2 = (Label)common.MudarStatusVisible(label2, true, "Senha");
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (formAtual == (int)TypeForm.Login) Login();
            else if (formAtual == (int)TypeForm.CadastroLogin && txtSenha.Text == txtSenhaConfirm.Text) CadastroUser();
            else if(formAtual == (int)TypeForm.CadastroLogin && txtSenha.Text != txtSenhaConfirm.Text)
            {
                MessageBox.Show("Os campos 'Senha' e 'Confirmar Senha' não correspondem.", "Password", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                txtSenha.Clear();
                txtSenhaConfirm.Clear();
                txtSenha.Focus();
            }
        }

        private void Login()
        {
            string hash = crypt.newHash(textBox1.Text + textBox2.Text);

            using(capdeEntities context = new capdeEntities())
            {
                Usuario user = context.Usuarios.Where(x => x.Senha == hash).FirstOrDefault();
                if (user != null && user.IsAdmin)
                {
                    logedUser = user.Login;
                    logedUsername = user.Nome;
                    this.DialogResult = DialogResult.Yes;
                }
                else this.DialogResult = DialogResult.No;
            }

            this.Close();
        }

        private void CadastroUser()
        {
            using(capdeEntities context = new capdeEntities())
            {
                Usuario usuario = context.Usuarios.Where(x => x.Login == txtUsuario.Text).FirstOrDefault();
                if (usuario == null)
                {
                    Usuario user = new Usuario
                    {
                        Nome = textBox1.Text,
                        Email = textBox2.Text,
                        Login = txtUsuario.Text,
                        Senha = crypt.newHash(txtUsuario.Text + txtSenha.Text),
                        IsAdmin = chkIsAdmin.Checked
                    };

                    DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                    config.HasChanged = true;

                    context.Usuarios.Add(user);
                    context.SaveChanges();

                    this.DialogResult = DialogResult.No;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Este usuário não está disponível. Favor, insira um novo usuário", "Disponibilidade Usuário",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUsuario.Clear();
                    txtUsuario.Focus();
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void lblEsqueci_Click(object sender, EventArgs e)
        {
            using(capdeEntities context = new capdeEntities())
            {
                if (!String.IsNullOrEmpty(textBox1.Text))
                {
                    Usuario user = context.Usuarios.Where(x => x.Login == textBox1.Text).FirstOrDefault();

                    if (user != null)
                    {
                        string newPass = RandonNumer();

                        user.Senha = crypt.newHash(user.Login + newPass);
                        context.SaveChanges();

                        UserCredential credential = gmail.Autenticate();
                        using(GmailService service = gmail.AbrirServico(credential))
                        {
                            string plainText = "To: " + user.Email + "\r\n" +
                               "Subject: CAPDE - Nova senha\r\n" +
                               "Content-Type: text/html; charset=us-ascii\r\n\r\n" +
                               "<h2>CAPDE</h2>\r\n\r\n" + 
                               "<p>Olá " + user.Nome +  ", você solicitou a troca da sua senha.</p>" + 
                               "</p><b>Nova senha: </b>" + newPass + "</p>";

                            Google.Apis.Gmail.v1.Data.Message message = new Google.Apis.Gmail.v1.Data.Message
                            {
                                Raw = Base64UrlEncode(plainText),
                            };

                            gmail.SendMessage(service, "me", message);
                        }

                        MessageBox.Show("Sua senha foi redefinida e encaminhada uma nova para " + user.Email, "Redefinir Senha",
                         MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("Usuário inexistente", "Usuário inexistente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("O valor para 'Usuário' deve ser definido", "Usuário inexistente", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private string RandonNumer()
        {
            Random ran = new Random();
            return ran.Next(10000, 1000000).ToString();
        }

        public string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
