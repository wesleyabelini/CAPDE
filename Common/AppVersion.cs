using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AppVersion
    {
        public string nome { get; set; }
        public string versao { get; set; }
        public string data { get; set; }
        public string obs { get; set; }
        public string dev { get; set; }
        public string updatefields { get; set; }
        public List<string> collectionCmd { get; set; }
    }
}
