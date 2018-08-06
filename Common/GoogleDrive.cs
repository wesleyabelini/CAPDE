using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Common
{
    public static class GoogleDrive
    {
        public static void Upload(DriveService service, string caminhoArquivo, string pastaId = "")
        {
            var arquivo = new Google.Apis.Drive.v3.Data.File();
            arquivo.Name = Path.GetFileName(caminhoArquivo);
            if(pastaId != String.Empty) arquivo.Parents = new List<string> { pastaId };
            arquivo.MimeType = MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(caminhoArquivo));

            using (var stream = new FileStream(caminhoArquivo, FileMode.Open, FileAccess.Read))
            {
                string[] ids = ProcurarArquivoId(service, arquivo.Name);
                Google.Apis.Upload.ResumableUpload<Google.Apis.Drive.v3.Data.File, Google.Apis.Drive.v3.Data.File> request;

                if (ids == null || !ids.Any()) request = service.Files.Create(arquivo, stream, arquivo.MimeType);
                else request = service.Files.Update(arquivo, ids.First(), stream, arquivo.MimeType);
                request.Upload();
            }
        }

        public static void CreateFolder(DriveService service, string pathName, string folderId = "")
        {
            Google.Apis.Drive.v3.Data.File pasta = new Google.Apis.Drive.v3.Data.File();
            pasta.Name = pathName;
            if(folderId != String.Empty) pasta.Parents = new List<string> { folderId };
            pasta.MimeType = "application/vnd.google-apps.folder";
            var request = service.Files.Create(pasta);
            request.Execute();
        }

        public static string[] ProcurarArquivoId(DriveService servico, string nome, bool procurarNaLixeira = false)
        {
            var retorno = new List<string>();

            var request = servico.Files.List();
            request.Q = string.Format("name = '{0}'", nome);

            if (!procurarNaLixeira) request.Q += " and trashed = false";

            request.Fields = "files(id)";
            var resultado = request.Execute();
            var arquivos = resultado.Files;

            if (arquivos != null && arquivos.Any())
            {
                foreach (var arquivo in arquivos)
                {
                    retorno.Add(arquivo.Id);
                }
            }

            return retorno.ToArray();
        }

        public static UserCredential Autenticate()
        {
            UserCredential credential;

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var diretorioAtual = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var diretorioCredencial = Path.Combine(diretorioAtual, "credencial\\drive");

                if (!Directory.Exists(diretorioAtual + "\\credencial")) Directory.CreateDirectory(diretorioAtual + "\\credencial");
                if (!Directory.Exists(diretorioCredencial)) Directory.CreateDirectory(diretorioCredencial);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, 
                    new[] { DriveService.Scope.Drive}, "user", CancellationToken.None,
                    new FileDataStore(diretorioCredencial, true)).Result;
            }

            return credential;
        }

        public static DriveService AbrirServico(UserCredential credential)
        {
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }
    }
}
