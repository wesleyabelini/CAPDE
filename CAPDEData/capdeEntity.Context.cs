﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class capdeEntities : DbContext
    {
        public capdeEntities()
            : base("name=capdeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Capacitacao> Capacitacaos { get; set; }
        public virtual DbSet<Cargo> Cargoes { get; set; }
        public virtual DbSet<Cidade> Cidades { get; set; }
        public virtual DbSet<CJ> CJs { get; set; }
        public virtual DbSet<DatabaseConfig> DatabaseConfigs { get; set; }
        public virtual DbSet<Pessoa> Pessoas { get; set; }
        public virtual DbSet<RAJ> RAJs { get; set; }
        public virtual DbSet<Setor> Setors { get; set; }
        public virtual DbSet<Turma> Turmas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
    
        public virtual int Backup(string myBackupLocation, string myBaseName)
        {
            var myBackupLocationParameter = myBackupLocation != null ?
                new ObjectParameter("myBackupLocation", myBackupLocation) :
                new ObjectParameter("myBackupLocation", typeof(string));
    
            var myBaseNameParameter = myBaseName != null ?
                new ObjectParameter("myBaseName", myBaseName) :
                new ObjectParameter("myBaseName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Backup", myBackupLocationParameter, myBaseNameParameter);
        }
    
        public virtual int Restore(string myBaseName, string myBackup)
        {
            var myBaseNameParameter = myBaseName != null ?
                new ObjectParameter("myBaseName", myBaseName) :
                new ObjectParameter("myBaseName", typeof(string));
    
            var myBackupParameter = myBackup != null ?
                new ObjectParameter("myBackup", myBackup) :
                new ObjectParameter("myBackup", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Restore", myBaseNameParameter, myBackupParameter);
        }
    
        public virtual int Backup1(string myBackupLocation, string myBaseName)
        {
            var myBackupLocationParameter = myBackupLocation != null ?
                new ObjectParameter("myBackupLocation", myBackupLocation) :
                new ObjectParameter("myBackupLocation", typeof(string));
    
            var myBaseNameParameter = myBaseName != null ?
                new ObjectParameter("myBaseName", myBaseName) :
                new ObjectParameter("myBaseName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Backup1", myBackupLocationParameter, myBaseNameParameter);
        }
    
        public virtual int Restore1(string myBaseName, string myBackup)
        {
            var myBaseNameParameter = myBaseName != null ?
                new ObjectParameter("myBaseName", myBaseName) :
                new ObjectParameter("myBaseName", typeof(string));
    
            var myBackupParameter = myBackup != null ?
                new ObjectParameter("myBackup", myBackup) :
                new ObjectParameter("myBackup", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Restore1", myBaseNameParameter, myBackupParameter);
        }
    }
}
