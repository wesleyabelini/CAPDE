﻿using CAPDEData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Common.CAPDEEnums;

namespace CAPDE
{
    public partial class FormCadPessoa : Form
    {
        Common common = new Common();
        bool insertedData = false;

        public FormCadPessoa()
        {
            InitializeComponent();

            common.PreencheCombos_Pessoa(cmbRAJ, cmbCJ, cmbCidade, cmbCargo, cmbSetor, cmbCapacitacao);
        }

        private void btCadastrar_Click(object sender, EventArgs e)
        {
            using(capdeEntities context = new capdeEntities())
            {
                Capacitacao capacitacao = new Capacitacao();

                if (ckCapacitado.Checked)
                {
                    capacitacao.DataInicio = Convert.ToDateTime(dateTInicio.Text);
                    capacitacao.DataFim = Convert.ToDateTime(dateTFim.Text);
                    capacitacao.IsCapacitado = true;

                    if (radioEAD.Checked)
                    {
                        capacitacao.IsEAD = true;
                        capacitacao.TurmaId = (int)cmbCapacitacao.SelectedValue;
                    }
                    else if(radioPresencial.Checked)
                    {
                        capacitacao.RajId = (int)cmbCapacitacao.SelectedValue;
                        capacitacao.IsEAD = false;
                    }
                }
                else capacitacao.IsCapacitado = false;

                Pessoa pessoa = new Pessoa
                {
                    Nome = txtNome.Text,
                    Registro = txtRegistro.Text,
                    EMail = txtEmail.Text,
                    CargoId = (int)cmbCargo.SelectedValue,
                    Setor = context.Setors.Where(x=>x.SetorId == (int)cmbSetor.SelectedValue).FirstOrDefault(),
                    Capacitacao = capacitacao,
                    Obs = txtOBS.Text,
                    IsExcluido = false,
                };

                DatabaseConfig config = context.DatabaseConfigs.Where(x => x.DatabaseConfigId == 1).First();
                config.HasChanged = true;

                context.Capacitacaos.Add(capacitacao);
                context.Pessoas.Add(pessoa);

                context.SaveChanges();
            }

            LimpaCampos();
            insertedData = true;
            common.PreencheCombos_Pessoa(cmbRAJ, cmbCJ, cmbCidade, cmbCargo, cmbSetor, cmbCapacitacao);
        }

        private void LimpaCampos()
        {
            txtNome.Clear();
            txtRegistro.Clear();
            txtOBS.Clear();
            txtEmail.Clear();

            dateTInicio.Text = String.Empty;
            dateTFim.Text = String.Empty;

            ckCapacitado.Checked = false;
            panelCapacitado.Enabled = false;
        }

        private void ckCapacitado_CheckedChanged(object sender, EventArgs e)
        {
            if (ckCapacitado.Checked)
            {
                panelCapacitado.Enabled = true;
                cmbCapacitacao = common.LoadTurmaCombo(cmbCapacitacao);
            }
            else panelCapacitado.Enabled = false;
        }

        private void radioEAD_Click(object sender, EventArgs e)
        {
            cmbCapacitacao.Enabled = true;
            lblCapacitacao.Text = "Turma";

            cmbCapacitacao =  common.LoadTurmaCombo(cmbCapacitacao);
        }

        private void radioPresencial_Click(object sender, EventArgs e)
        {
            cmbCapacitacao.Enabled = true;
            lblCapacitacao.Text = "RAJ";

            using (capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> capacitado = context.RAJs.Select(x => new { x.RajId, x.NomeRaj }).ToList();
                cmbCapacitacao = common.PreencheCombo(cmbCapacitacao, capacitado, "RajId", "NomeRaj");
            }
        }

        private void radioOutros_Click(object sender, EventArgs e)
        {
            cmbCapacitacao.Enabled = false;
            lblCapacitacao.Text = String.Empty;
        }

        private void cmbRAJ_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbRAJ.ValueMember != String.Empty)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> CJ = context.CJs.Where(x => x.RajId == (int)cmbRAJ.SelectedValue && x.CjNome != "TODOS")
                        .OrderBy(x=>x.CjNome).Select(x => new { x.CjId, x.CjNome }).ToList();

                    cmbCJ = common.PreencheCombo(cmbCJ, CJ, "CjId", "CjNome");
                }
            }
        }

        private void cmbCJ_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbCidade.ValueMember != String.Empty)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> cidade = context.Cidades.Where(x => x.CjId == (int)cmbCJ.SelectedValue && x.NomeCidade != "TODOS")
                        .OrderBy(x=>x.NomeCidade).Select(x => new { x.CidadeId, x.NomeCidade }).ToList();

                    cmbCidade = common.PreencheCombo(cmbCidade, cidade, "CidadeId", "NomeCidade");
                }
            }
        }

        private void cmbCidade_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbSetor.ValueMember != String.Empty)
            {
                using (capdeEntities context = new capdeEntities())
                {
                    IEnumerable<dynamic> setor = context.Setors.Where(x => x.CidadeId == (int)cmbCidade.SelectedValue).OrderBy(x=>x.NomeSetor)
                        .Select(x => new { x.SetorId, x.NomeSetor }).ToList();

                    cmbSetor = common.PreencheCombo(cmbSetor, setor, "SetorId", "NomeSetor");
                }
            }
        }

        private void FormCadPessoa_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (insertedData) this.DialogResult = DialogResult.OK;
            else this.DialogResult = DialogResult.No;
        }

        public void newFormCad(int form, int heightForm)
        {
            FormCad cad = new FormCad(form, heightForm);
            if (cad.ShowDialog() == DialogResult.OK)
            {
                common.PreencheCombos_Pessoa(cmbRAJ, cmbCJ, cmbCidade, cmbCargo, cmbSetor, cmbCapacitacao);
            }
        }

        private void tsbRAJ_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.RAJ, (int)SizeForm_Cad.RAJ);
        }

        private void tsbCJ_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.CJ, (int)SizeForm_Cad.CJ);
        }

        private void tsbCidade_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Cidade, (int)SizeForm_Cad.Cidade);
        }

        private void tsbSetor_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Setor, (int)SizeForm_Cad.Setor);
        }

        private void tsbCargo_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Cargo, (int)SizeForm_Cad.Cargo);
        }

        private void tsbTurma_Click(object sender, EventArgs e)
        {
            newFormCad((int)TypeForm.Turma, (int)SizeForm_Cad.Turma);
        }
    }
}
