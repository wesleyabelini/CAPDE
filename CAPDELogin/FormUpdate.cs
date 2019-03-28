using Common;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Google.Apis.Drive.v3;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Security.Principal;
using CAPDEData;

namespace CAPDELogin
{
    public partial class FormUpdate : Form
    {
        Common.SendLog commonLog = new SendLog();

        private static FileVersionInfo thisAssemblyVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        private static UserCredential credential = GoogleDrive.Autenticate();
        private static AppVersion serverVersion;
        private static List<string> devList = ReturnListByStringConcat(ConfigurationManager.AppSettings["DevUsers"]);

        public bool hasUpdate = (VerifyExistUpdates() && (Convert.ToBoolean(serverVersion.dev) == false || 
            devList.Contains(WindowsIdentity.GetCurrent().Name)));

        List<string> listFields = new List<string>();

        public FormUpdate()
        {
            if (hasUpdate)
            {
                InitializeComponent();
                PreencheCampos();

                listFields = ReturnListByStringConcat(serverVersion.updatefields);
            }
        }

        private static List<string> ReturnListByStringConcat(string concated)
        {
            string s = String.Empty;
            List<string> devList = new List<string>();
            foreach(char c in concated)
            {
                if (c.ToString() != ",") s += c.ToString();
                else
                {
                    devList.Add(s);
                    s = String.Empty;
                }
            }

            if (s.Length > 0) devList.Add(s);

            return devList;
        }

        private static bool VerifyExistUpdates()
        {
            string gbackupFolder = ConfigurationManager.AppSettings["UpdatePathNameGDrive"];
            string jsonUpdate = ConfigurationManager.AppSettings["jsonFileUpdate"];

            using (DriveService service = GoogleDrive.AbrirServico(credential))
            {
                string[] capdeUpdate = GoogleDrive.ProcurarArquivoId(service, gbackupFolder);

                if(capdeUpdate.Length == 0) GoogleDrive.CreateFolder(service, gbackupFolder);

                string[] fileJsonUpdate = GoogleDrive.ProcurarArquivoId(service, jsonUpdate);
                if (fileJsonUpdate.Length != 0)
                {
                    string ss = Encoding.ASCII.GetString(GoogleDrive.DownloadFile(service, fileJsonUpdate[0]).ToArray());
                    serverVersion = JsonConvert.DeserializeObject<AppVersion>(ss);

                    if (Convert.ToDouble(thisAssemblyVersion.FileVersion.Replace(".", "")) < Convert.ToDouble(serverVersion.versao.Replace(".", "")))
                        return true;
                }
            }
                return false;
        }

        private void PreencheCampos()
        {
            lblVersaoAtual.Text += thisAssemblyVersion.FileVersion;
            lblVersaoNew.Text += serverVersion.versao;
            lblDataNew.Text += serverVersion.data;
            rickTxtObs.Text = serverVersion.obs;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            tsProgressBar.Maximum = listFields.Count;

            BackgroundWorker workDownload = new BackgroundWorker();
            workDownload.DoWork += WorkDownload_DoWork;
            workDownload.ProgressChanged += WorkDownload_ProgressChanged;
            workDownload.RunWorkerCompleted += WorkDownload_RunWorkerCompleted;
            workDownload.WorkerReportsProgress = true;

            workDownload.RunWorkerAsync(); 
        }

        private void WorkDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tsProgressBar.Value = 0;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(("ECHO OFF\r\nPING 127.0.0.1 -n 6 > nul\r\nMOVE '" + 
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\update\\*.*' '"+ 
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "'\r\nSTART '' '" + 
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\CAPDE.exe'").Replace("'", "\"")));
            using(FileStream fs = new FileStream(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\copy.bat",
                FileMode.Create, FileAccess.Write)) ms.WriteTo(fs);

            commonLog.SendLogError(thisAssemblyVersion.FileVersion, "CAPDE UPDATE from " + thisAssemblyVersion.FileVersion + "to " + serverVersion.versao, String.Empty);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void WorkDownload_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsProgressBar.Value = e.ProgressPercentage;
        }

        private void WorkDownload_DoWork(object sender, DoWorkEventArgs e)
        {
            using (DriveService service = GoogleDrive.AbrirServico(credential))
            {
                Dictionary<string, string[]> dicioFields = new Dictionary<string, string[]>();
                foreach (string s in listFields) dicioFields.Add(s, GoogleDrive.ProcurarArquivoId(service, s));

                int i = 0;

                if (!Directory.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\update\\"))
                    Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\update\\");

                foreach (var field in dicioFields)
                {
                    MemoryStream ms = GoogleDrive.DownloadFile(service, field.Value[0]);
                    using (FileStream fs = new FileStream(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\update\\" + field.Key,
                        FileMode.Create, FileAccess.Write)) ms.WriteTo(fs);
                    i++;
                    (sender as BackgroundWorker).ReportProgress(i);
                }
            }
        }
    }
}
