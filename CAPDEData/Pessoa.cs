//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CAPDEData
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pessoa
    {
        public int PessoaId { get; set; }
        public string Registro { get; set; }
        public string Nome { get; set; }
        public string EMail { get; set; }
        public Nullable<int> CargoId { get; set; }
        public int SetorId { get; set; }
        public Nullable<int> CapacitacaoId { get; set; }
        public string Obs { get; set; }
        public Nullable<bool> IsExcluido { get; set; }
    
        public virtual Capacitacao Capacitacao { get; set; }
        public virtual Cargo Cargo { get; set; }
        public virtual Setor Setor { get; set; }
    }
}
