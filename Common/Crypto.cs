using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Common
{
    public class Crypto
    {
        public string newHash(string Tsenha)
        {
            byte[] hashSenha;
            string senha = Tsenha;

            UnicodeEncoding ue = new UnicodeEncoding();

            byte[] senhaByte = ue.GetBytes(senha);

            SHA1Managed sha1 = new SHA1Managed(); 
            hashSenha = sha1.ComputeHash(senhaByte);

            string resultSenha = String.Empty;

            foreach(byte bytesenha in hashSenha)
            {
                resultSenha += bytesenha.ToString("X2");
            }

            return resultSenha;
        }
    }
}
