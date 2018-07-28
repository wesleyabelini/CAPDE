using CAPDEData;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using System;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Common
{
    public static class Backup
    {
        static string day = DateTime.Now.Day.ToString();
        static string month = DateTime.Now.Month.ToString();
        static string year = DateTime.Now.Year.ToString();

        static string local = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static string localDb = Path.Combine(local, "capde.mdf");
        static string localDbLog = Path.Combine(local, "capde_log.ldf");
        static string restoreLocalDb = Path.Combine(local, "capdeRestore.mdf");
        static string restoreLocalDbLog = Path.Combine(local, "capdeRestore_log.ldf");

        static string zipPath = Path.Combine(local, "origin.zip");

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

            UploadGoogleDrive();
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
            string googleDrivePathName = "CAPDE Backup";
            UserCredential credential = GoogleDrive.Autenticate();

            using(DriveService service = GoogleDrive.AbrirServico(credential))
            {
                string[] capdeBackup = GoogleDrive.ProcurarArquivoId(service, "CAPDE Backup");
                if (capdeBackup.Length == 0)
                {
                    GoogleDrive.CreateFolder(service, googleDrivePathName);
                    capdeBackup = GoogleDrive.ProcurarArquivoId(service, "CAPDE Backup");
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

        public static void RestoreLocalBackup(string restoreLocation)
        {
            if (!File.Exists(restoreLocalDb)) ZipFile.ExtractToDirectory(zipPath, local);

            using (capdeEntities context = new capdeEntities())
            {
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                    @"EXEC [dbo].[Restore] @myBackup = '" + restoreLocation + "', @myBaseName = '" + restoreLocalDb + "'");
            }
        }

        public static void FixRestauredDatabase()
        {
            File.Delete(localDb);
            File.Delete(localDbLog);

            File.Move(restoreLocalDb, localDb);
            File.Move(restoreLocalDbLog, localDbLog);
        }
    }
}
