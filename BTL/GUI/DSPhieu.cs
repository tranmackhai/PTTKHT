using BTL.Model;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL
{
    public partial class DSPhieu : MetroForm
    {
        private List<Phieu> ds_ph = new List<Phieu>();
        private ucNhapKho nk;
        private ucXuatKho xk;
        private DAO.DAO_Phieu dao_p = new DAO.DAO_Phieu();

        public DSPhieu(ucNhapKho _nk, ucXuatKho _xk)
        {
            InitializeComponent();
            nk = _nk;
            xk = _xk;
            if(nk == null)
            {
                phieu(false);
            }
            else
            {
                phieu(true);
            }
        }

        public void phieu(bool kieu)
        {
            //kieu = true: nhập kho
            //kieu = false: xuất kho
            dgvList.Rows.Clear();
            ds_ph.Clear();
            ds_ph = dao_p.getAll(kieu);
            ds_ph.ForEach(ph =>
            {
                dgvList.Rows.Add(new object[]
                {
                    ph.sophieu, ph.ngay, ph.nv.ten
                });
            });
            dgvList.ClearSelection();
            Visible = true;
        }

        private void DSPhieu_Load(object sender, EventArgs e)
        {

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Phieu ph = ds_ph.Find(item => item.sophieu.ToString() == txtResult.Text);
            if(ph != null)
            {
                if (nk == null)
                {
                    xk.moPhieu(ph);
                }
                else
                {
                    nk.moPhieu(ph);
                }
                Visible = false;
            }
            else
            {
                MessageBox.Show(this, "Số phiếu không hợp lệ", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void dgvList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtResult.Text = dgvList.Rows[e.RowIndex].Cells[0].Value.ToString();
        }
    }
}
