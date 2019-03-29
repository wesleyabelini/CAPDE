using CAPDEData;
using System;

namespace Common
{
    public class SendLog
    {
        Common common = new Common();
        public void SendLogError(string version, string title, string message, string usuario)
        {
            using (capdeEntities context = new capdeEntities())
            {
                Log log = new Log
                {
                    Data = DateTime.Now,
                    Usuario = usuario,
                    Maquina = Environment.MachineName,
                    Version = version,
                    MethodTitle = title,
                    Message = message,
                };

                context.Logs.Add(log);
                common.SaveChanges_Database(context, true);
            }
        }
    }
}
