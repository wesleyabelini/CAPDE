using CAPDEData;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows.Forms;

namespace Common
{
    public static class Backup
    {
        static SendLog comLog = new SendLog();
        static FileVersionInfo thisAssemblyVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        static string day = DateTime.Now.Day.ToString();
        static string month = DateTime.Now.Month.ToString();
        static string year = DateTime.Now.Year.ToString();

        static string local = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static string localDb = Path.Combine(local, ConfigurationManager.AppSettings["DatabaseName"]);
        static string localDbLog = Path.Combine(local, ConfigurationManager.AppSettings["DatabaseLogName"]);
        static string restoreLocalDb = Path.Combine(local, ConfigurationManager.AppSettings["DatabaseRestoreName"]);
        static string restoreLocalDbLog = Path.Combine(local, ConfigurationManager.AppSettings["DatabaseRestoreLogName"]);

        static string zipPath = Path.Combine(local, ConfigurationManager.AppSettings["DatabaseOriginPathName"]);

        static string backupDb = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "backup");
        static string backupDateMonth = Path.Combine(backupDb, month + year);
        static string backupDay = Path.Combine(backupDateMonth, day);

        static string fileBak = backupDay + "\\backup_" + DateTime.Now.ToFileTime() + ".bak";

        public static void CreateLocalBackup()
        {
            if (!Directory.Exists(backupDb)) Directory.CreateDirectory(backupDb);
            if (!Directory.Exists(backupDateMonth)) Directory.CreateDirectory(backupDateMonth);
            if (!Directory.Exists(backupDay)) Directory.CreateDirectory(backupDay);

            using(capdeEntities context = new capdeEntities())
            {
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, 
                    @"EXEC [dbo].[Backup] @myBackupLocation = '" + fileBak + "', @myBaseName = '" + localDb + "'");
            }

            if(File.Exists(Path.Combine(local, ConfigurationManager.AppSettings["ClientFileSecretName"]))) UploadGoogleDrive();
        }

        public static void CreateLocalBackup(string fileBackup)
        {
            using (capdeEntities context = new capdeEntities())
            {
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                    @"EXEC [dbo].[Backup] @myBackupLocation = '" + fileBackup + "', @myBaseName = '" + localDb + "'");
            }
        }

        public static void UploadGoogleDrive()
        {
            string googleDrivePathName = ConfigurationManager.AppSettings["BackupPathNameGDrive"];
            UserCredential credential = GoogleDrive.Autenticate();

            using(DriveService service = GoogleDrive.AbrirServico(credential))
            {
                string[] capdeBackup = GoogleDrive.ProcurarArquivoId(service, googleDrivePathName);
                if (capdeBackup.Length == 0)
                {
                    GoogleDrive.CreateFolder(service, googleDrivePathName);
                    capdeBackup = GoogleDrive.ProcurarArquivoId(service, googleDrivePathName);
                }

                string[] monthPath = GoogleDrive.ProcurarArquivoId(service, month + year);
                if (monthPath.Length == 0)
                {
                    GoogleDrive.CreateFolder(service, month + year, capdeBackup[0]);
                    monthPath = GoogleDrive.ProcurarArquivoId(service, month + year);
                }

                string[] dayPath = GoogleDrive.ProcurarArquivoId(service, day + month + year);
                if (dayPath.Length == 0)
                {
                    GoogleDrive.CreateFolder(service, day + month + year, monthPath[0]);
                    dayPath = GoogleDrive.ProcurarArquivoId(service, day + month + year);
                }

                GoogleDrive.Upload(service, fileBak, dayPath[0]);
            }
        }

        public static void RestoreLocalBackup(string restoreLocation, string logedUser)
        {
            if (!File.Exists(restoreLocalDb)) ZipFile.ExtractToDirectory(zipPath, local);

            try
            {
                using (capdeEntities context = new capdeEntities())
                {
                    context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                        @"EXEC [dbo].[Restore] @myBackup = '" + restoreLocation + "', @myBaseName = '" + restoreLocalDb + "'");
                }
            }
            catch (Exception ex)
            {
                comLog.SendLogError(thisAssemblyVersion.FileVersion, "Restore Local Backup", ex.Message + "\r\n" + ex.StackTrace, logedUser);
                FixErrorRestoreDatabase();
                MessageBox.Show("Não foi possível restaurar a base.\r\nPara mais informações, consulte Log.", "Falha Restauração", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void FixRestauredDatabase()
        {
            File.Delete(localDb);
            File.Delete(localDbLog);

            File.Move(restoreLocalDb, localDb);
            File.Move(restoreLocalDbLog, localDbLog);
        }

        public static void FixErrorRestoreDatabase()
        {
            File.Delete(restoreLocalDb);
            File.Delete(restoreLocalDbLog);
        }
    }
}
