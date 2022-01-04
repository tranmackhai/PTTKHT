using BTL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL
{
    public partial class ucBan : UserControl
    {
        private Ban ban;
        private ucBanHang preComponent;
        public ucBan(ucBanHang f, Ban b)
        {
            InitializeComponent();
            ban = b;
            btnInfoTable.Text = $"Bàn {ban.soban}";
            preComponent = f;
            if (ban.trangthai == true)
            {
                btnInfoTable.FillColor = Color.FromArgb(94, 148, 255);
                btnInfoTable.ForeColor = Color.White;
            }
            else
            {
                btnInfoTable.FillColor = Color.LightCoral;
                btnInfoTable.ForeColor = Color.White;
            }
        }

        public int getID()
        {
            return ban.soban;
        }
        public ucBan()
        {
            InitializeComponent();
        }
        private void ucBan_Load(object sender, EventArgs e)
        {
            
        }

        public void order()
        {
            btnInfoTable.FillColor = Color.LightCoral;
            ban.trangthai = false;
        }

        public void discharge()
        {
            btnInfoTable.FillColor = Color.FromArgb(94, 148, 255);
            ban.trangthai = true;
        }

        private void btnInfoTable_Click(object sender, EventArgs e)
        {
            //Xem thông tin
            preComponent.xemThongTinBan(this, ban);
        }
    }
}
