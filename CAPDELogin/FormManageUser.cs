using CAPDEData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CAPDELogin
{
    public partial class FormManageUser : Form
    {
        Common.Common common = new Common.Common();

        public FormManageUser()
        {
            InitializeComponent();
            AllUser();
        }

        private void AllUser()
        {
            using(capdeEntities context = new capdeEntities())
            {
                IEnumerable<dynamic> user = context.Usuarios.Select(x => new { x.UsuarioId, x.Nome, x.IsAdmin }).ToList();

                dgvUser.DataSource = user;
                dgvUser.Columns[0].Visible = false;
                dgvUser.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvUser.Columns[2].Visible = false;
            }
        }

        private void dgvUser_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            foreach(DataGridViewRow row in dgvUser.Rows)
            {
                bool isAdmin = (bool)row.Cells[2].Value;

                if (isAdmin) row.DefaultCellStyle.BackColor = Color.PaleGreen;
                else row.DefaultCellStyle.BackColor = Color.Tomato;
            }
        }

        private void dgvUser_DoubleClick(object sender, EventArgs e)
        {
            if(dgvUser.CurrentRow != null)
            {
                using(capdeEntities context = new capdeEntities())
                {
                    int idUser = (int)dgvUser.CurrentRow.Cells[0].Value;
                    Usuario user = context.Usuarios.Where(x => x.UsuarioId == idUser).FirstOrDefault();

                    if (user.IsAdmin) user.IsAdmin = false;
                    else user.IsAdmin = true;

                    common.SaveChanges_Database(context, true);
                    AllUser();
                }
            }
        }
    }
}
