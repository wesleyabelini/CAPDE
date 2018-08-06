using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Common
{
    public class Gmail
    {
        public GmailService AbrirServico(UserCredential credential)
        {
            return new GmailService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        public UserCredential Autenticate()
        {
            UserCredential credential;

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var diretorioAtual = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var diretorioCredencial = Path.Combine(diretorioAtual, "credencial\\gmail");

                if (!Directory.Exists(diretorioAtual + "\\credencial")) Directory.CreateDirectory(diretorioAtual + "\\credencial");
                if (!Directory.Exists(diretorioCredencial)) Directory.CreateDirectory(diretorioCredencial);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                    new[] { GmailService.Scope.GmailSend }, "user", CancellationToken.None,
                    new FileDataStore(diretorioCredencial, true)).Result;
            }

            return credential;
        }

        public Message SendMessage(GmailService service, string userId, Message email)
        {
            try
            {
                return service.Users.Messages.Send(email, userId).Execute();
            }
            catch(Exception)
            {
                System.Windows.Forms.MessageBox.Show("Não foi possível enviar o e-mail de redefinição de senha. Por favor, " +
                    "tente mais tarde ou contate o administrador.", "Falha e-mail", System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Information);
            }

            return null;
        }
    }
}
