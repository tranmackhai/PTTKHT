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
using BTL.Model;

namespace BTL
{
    public partial class DangNhap : Form
    {
        private DAO.DAO_NhanVien dao_nv = new DAO.DAO_NhanVien();
        public DangNhap()
        {
            InitializeComponent();
        }

        private void DangNhap_Load(object sender, EventArgs e)
        {
            
            
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            NhanVien nv = dao_nv.login(txtID.Text, txtPassword.Text);
            
            if (nv != null)
            {
                new F_TrangChu(this, nv).Visible = true;
                Visible = false;
            }
            else
            {
                MessageBox.Show(this, "Mã nhân viên hoặc mật khẩu không chính xác", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void picToggleViewPassword_Click(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar == true)
            {
                picToggleViewPassword.Image = Image.FromFile(@"../../public/img/visibility.png");
            }
            else
            {
                picToggleViewPassword.Image = Image.FromFile(@"../../public/img/view.png");
            }
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
        }
    }
}
