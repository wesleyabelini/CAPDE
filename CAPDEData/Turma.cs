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
    
    public partial class Turma
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Turma()
        {
            this.Capacitacaos = new HashSet<Capacitacao>();
        }
    
        public int TurmaId { get; set; }
        public string NomeTurma { get; set; }
        public Nullable<bool> IsExcluido { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Capacitacao> Capacitacaos { get; set; }
    }
}
