using CAPDEData;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAPDE
{
    public class Common
    {
        public void PreencheCombos_Pessoa(ComboBox cmbRAJ, ComboBox cmbCJ, ComboBox cmbCidade, ComboBox cmbCargo, ComboBox cmbSetor,
            ComboBox cmbTurma)
        {
            using (capdeEntities context = new capdeEntities())
            {
                try
                {
                    IEnumerable<dynamic> cargo = context.Cargoes.OrderBy(x => x.NomeCargo).Select(x => new { x.CargoId, x.NomeCargo }).ToList();

                    IEnumerable<dynamic> RAJ = context.RAJs.Where(x => x.NomeRaj != "TODOS").OrderByDescending(x => x.NomeRaj == "TODOS")
                        .ThenBy(x => x.NomeRaj).Select(x => new { x.RajId, x.NomeRaj }).ToList();
                    cmbRAJ = PreencheCombo(cmbRAJ, RAJ, "RajId", "NomeRaj");

                    IEnumerable<dynamic> CJ = context.CJs.Where(x => x.CjNome != "TODOS" && x.RajId == (int)cmbRAJ.SelectedValue).OrderBy(x => x.CjNome)
                        .Select(x => new { x.CjId, x.CjNome }).ToList();
                    cmbCJ = PreencheCombo(cmbCJ, CJ, "CjId", "CjNome");

                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.NomeCidade != "TODOS" && x.CjId == (int)cmbCJ.SelectedValue)
                        .OrderBy(x => x.NomeCidade).Select(x => new { x.CidadeId, x.NomeCidade }).ToList();
                    cmbCidade = PreencheCombo(cmbCidade, cidade, "CidadeId", "NomeCidade");

                    IEnumerable<dynamic> setor = context.Setors.Where(x => x.CidadeId == (int)cmbCidade.SelectedValue).OrderBy(x => x.NomeSetor)
                        .Select(x => new { x.SetorId, x.NomeSetor }).ToList();

                    cmbCargo = PreencheCombo(cmbCargo, cargo, "CargoId", "NomeCargo");
                    cmbSetor = PreencheCombo(cmbSetor, setor, "SetorId", "NomeSetor");

                    IEnumerable<dynamic> turma = context.Turmas.OrderBy(x => x.NomeTurma).Select(x => new { x.TurmaId, x.NomeTurma }).ToList();
                    cmbTurma = PreencheCombo(cmbTurma, turma, "TurmaId", "NomeTurma");
                }
                catch
                {
                    DialogResult message = MessageBox.Show("Não existem dados cadastrais. Deseja importar por um backup?.", "Acesso dados",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if(message == DialogResult.Yes)
                    {
                        OpenFileDialog_RestoreLocalBackup("Backup File | *.bak");
                    } 
                }
            }
        }

        public void OpenFileDialog_RestoreLocalBackup(string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == DialogResult.OK) Backup.RestoreLocalBackup(dialog.FileName);
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
                IEnumerable<dynamic> capacitado = context.Turmas.OrderBy(x=>x.NomeTurma).Select(x => new { x.TurmaId, x.NomeTurma }).ToList();
                cmbTurma = PreencheCombo(cmbTurma, capacitado, "TurmaId", "NomeTurma");
            }

            return cmbTurma;
        }
    }
}
