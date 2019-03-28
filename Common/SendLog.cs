﻿using CAPDEData;
using System;

namespace Common
{
    public class SendLog
    {
        Common common = new Common();
        public void SendLogError(string version, string message, string usuario)
        {
            using (capdeEntities context = new capdeEntities())
            {
                Log log = new Log
                {
                    Data = DateTime.Now,
                    Usuario = usuario,
                    Maquina = Environment.MachineName,
                    Version = version,
                    MethodTitle = "Falha Backup",
                    Message = message,
                };

                context.Logs.Add(log);
                common.SaveChanges_Database(context, true);
            }
        }
    }
}
