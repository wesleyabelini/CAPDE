using CAPDEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static Common.CAPDEEnums;

namespace Common
{
    public class Common
    {
        public object MudarStatusVisible(object obj, bool valor, string text)
        {
            if (obj is Label)
            {
                (obj as Label).Visible = valor;
                (obj as Label).Text = text;
            }
            else if (obj is TextBox) (obj as TextBox).Visible = valor;
            else if (obj is ComboBox) (obj as ComboBox).Visible = valor;
            else if (obj is Button) (obj as Button).Visible = valor;
            else if (obj is RadioButton) (obj as RadioButton).Visible = valor;
            else if (obj is CheckBox) (obj as CheckBox).Visible = valor;
            else if (obj is DateTimePicker) (obj as DateTimePicker).Visible = valor;

            return obj;
        }

        public void PreencheCombos_Pessoa(ComboBox cmbRAJ, ComboBox cmbCJ, ComboBox cmbCidade, ComboBox cmbCargo, ComboBox cmbSetor,
            ComboBox cmbTurma)
        {
            using (capdeEntities context = new capdeEntities())
            {
                try
                {
                    IEnumerable<dynamic> cargo = context.Cargoes.Where(x=>x.IsExcluido == false).OrderBy(x => x.NomeCargo)
                        .Select(x => new { x.CargoId, x.NomeCargo }).ToList();

                    IEnumerable<dynamic> RAJ = context.RAJs.Where(x => x.NomeRaj != StringBase.TODOS.ToString() && x.IsExcluido == false)
                        .OrderByDescending(x => x.NomeRaj == StringBase.TODOS.ToString()).ThenBy(x => x.NomeRaj)
                        .Select(x => new { x.RajId, x.NomeRaj }).ToList();
                    cmbRAJ = PreencheCombo(cmbRAJ, RAJ, "RajId", "NomeRaj");

                    IEnumerable<dynamic> CJ = context.CJs.Where(x => x.CjNome != StringBase.TODOS.ToString() && x.RajId == (int)cmbRAJ.SelectedValue && 
                    x.IsExcluido == false).OrderBy(x => x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();
                    cmbCJ = PreencheCombo(cmbCJ, CJ, "CjId", "CjNome");

                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.NomeCidade != StringBase.TODOS.ToString() && 
                    x.CjId == (int)cmbCJ.SelectedValue && x.IsExcluido == false).OrderBy(x => x.NomeCidade)
                    .Select(x => new { x.CidadeId, x.NomeCidade }).ToList();
                    cmbCidade = PreencheCombo(cmbCidade, cidade, "CidadeId", "NomeCidade");

                    IEnumerable<dynamic> setor = context.Setors.Where(x => x.CidadeId == (int)cmbCidade.SelectedValue && x.IsExcluido == false)
                        .OrderBy(x => x.NomeSetor).Select(x => new { x.SetorId, x.NomeSetor }).ToList();

                    cmbCargo = PreencheCombo(cmbCargo, cargo, "CargoId", "NomeCargo");
                    cmbSetor = PreencheCombo(cmbSetor, setor, "SetorId", "NomeSetor");

                    IEnumerable<dynamic> turma = context.Turmas.Where(x=>x.IsExcluido == false).OrderBy(x => x.NomeTurma)
                        .Select(x => new { x.TurmaId, x.NomeTurma }).ToList();
                    cmbTurma = PreencheCombo(cmbTurma, turma, "TurmaId", "NomeTurma");
                }
                catch
                {
                    DialogResult message = MessageBox.Show("Não existem dados cadastrais. Deseja importar por um backup?.", "Acesso dados",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (message == DialogResult.Yes)
                    {
                        string dialogFileName = OpenFileDialog("Backup File | *.bak");
                        if (!String.IsNullOrEmpty(dialogFileName)) Backup.RestoreLocalBackup(dialogFileName);
                    }
                }
            }
        }

        public string OpenFileDialog(string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == DialogResult.OK) return dialog.FileName;
            else  return String.Empty;
        }

        public string SaveFileDrialog(string filter)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == DialogResult.OK) return dialog.FileName;
            else return String.Empty;
        }

        public ComboBox PreencheCombo(ComboBox combo, IEnumerable<dynamic> entity, string valueMember, string displayMember)
        {
            combo.DataSource = entity;
            combo.ValueMember = valueMember;
            combo.DisplayMember = displayMember;

            return combo;
        }

        public ToolStripComboBox PreencheCombo(ToolStripComboBox combo, IEnumerable<dynamic> entity, string valueMember, string displayMember)
        {
            combo.ComboBox.DataSource = entity;
            combo.ComboBox.ValueMember = valueMember;
            combo.ComboBox.DisplayMember = displayMember;

            return combo;
        }

        public ComboBox LoadTurmaCombo(ComboBox cmbTurma)
        {
            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> capacitado = context.Turmas.Where(x=>x.IsExcluido == false).OrderBy(x => x.NomeTurma)
                    .Select(x => new { x.TurmaId, x.NomeTurma }).ToList();
                cmbTurma = PreencheCombo(cmbTurma, capacitado, "TurmaId", "NomeTurma");
            }

            return cmbTurma;
        }

        public void SaveChanges_Database(capdeEntities context, bool changed)
        {
            DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
            config.HasChanged = changed;

            context.SaveChanges();
        }

        #region MessageBox

        public void MessageBox_TryConnection(string e)
        {
            MessageBox.Show("Não foi possível completar o processo. Contato o administrador. Error: " + e, "Falha Processo",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
