using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CAPDEEnums
    {
        public enum SizeForm_Cad
        {
            RAJ = 210,
            CJ = 250,
            Cidade = 290,
            Setor = 330,
            Cargo = 210,
            Turma = 210,
            Lote = 330,
            Login = 187,
            Login_Cad = 308
        }

        public enum TypeForm
        {
            RAJ = 1,
            CJ = 2,
            Cidade = 3,
            Setor = 4,
            Cargo = 5,
            Turma = 6,
            Lote_Capacitar = 7,
            Login = 8,
            CadastroLogin = 9
        }

        public enum FiltroCapacitado
        {
            Incapacitado = 0,
            Capacitado = 1,
            All = 2,
            Aposentado = 3,
            Excluido = 4,
        }

        public enum StatusServidor
        {
            Aposentar = 1,
            Desaposentar = 2,
            Excluir = 3,
            Incluir = 4
        }

        public enum StringBase
        {
            TODOS = 1,
        }
    }
}
