using BTL.Model;
using Guna.UI2.WinForms;
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
    public partial class F_TrangChu : Form
    {
        private DangNhap preComponent;
        private NhanVien nv;
        private List<Guna2Button> sideBarItems = new List<Guna2Button>();
        private List<string> list_img_non_active = new List<string>();
        private List<string> list_img_active = new List<string>();
        private Guna2Button preButton;
        public F_TrangChu(DangNhap f, NhanVien x)
        {
            InitializeComponent();
            preComponent = f;
            nv = x;
            MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;
            list_img_non_active.Add("house.png");
            list_img_non_active.Add("dollar.png");
            list_img_non_active.Add("group.png");
            list_img_non_active.Add("box.png");
            list_img_non_active.Add("menu.png");
            list_img_non_active.Add("table.png");
            list_img_non_active.Add("distribution.png");
            list_img_non_active.Add("seafood.png");
            list_img_non_active.Add("import.png");
            list_img_non_active.Add("export.png");
            list_img_non_active.Add("statistics.png");
            list_img_active.Add("house-active.png");
            list_img_active.Add("dollar-active.png");
            list_img_active.Add("group-active.png");
            list_img_active.Add("box-active.png");
            list_img_active.Add("menu-active.png");
            list_img_active.Add("table-active.png");
            list_img_active.Add("distribution-active.png");
            list_img_active.Add("seafood-active.png");
            list_img_active.Add("import-active.png");
            list_img_active.Add("export-active.png");
            list_img_active.Add("statistics-active.png");
        }

        public void quyenNV()
        {
            fpnlSideBar.Controls.Remove(btnSideBarItem3);
            fpnlSideBar.Controls.Remove(btnSideBarItem4);
            fpnlSideBar.Controls.Remove(btnSideBarItem5);
            fpnlSideBar.Controls.Remove(btnSideBarItem6);
            fpnlSideBar.Controls.Remove(btnSideBarItem7);
            fpnlSideBar.Controls.Remove(btnSideBarItem8);
            fpnlSideBar.Controls.Remove(btnSideBarItem9);
            fpnlSideBar.Controls.Remove(btnSideBarItem10);
        }

        public void quyenKho()
        {
            fpnlSideBar.Controls.Remove(btnSideBarItem2);
            fpnlSideBar.Controls.Remove(btnSideBarItem3);
            fpnlSideBar.Controls.Remove(btnSideBarItem4);
            fpnlSideBar.Controls.Remove(btnSideBarItem5);
            fpnlSideBar.Controls.Remove(btnSideBarItem6);
            fpnlSideBar.Controls.Remove(btnSideBarItem11);
        }

        public void phanQuyen()
        {
            if(nv.chucvu != "Giám Đốc" && nv.chucvu != "Phó Giảm Đốc" && nv.chucvu != "Quản Lý")
            {
                if(nv.chucvu == "Nhân Viên Kho" || nv.chucvu == "Thủ Kho")
                {
                    quyenKho();
                }
                else
                {
                    quyenNV();
                }
            }
        }
        private void btnSideBarItem1_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucTrangChu());
        }

        private void F_TrangChu_Load(object sender, EventArgs e)
        {
            //get sideBarItems from form
            sideBarItems = Enumerable.Range(1, fpnlSideBar.Controls.Count)
                .Select(i => (Guna2Button)fpnlSideBar.Controls["btnSideBarItem" + i.ToString()])
                .ToList();
            //Add eventHandler for btnSideBarItem

            sideBarItems.ForEach(item =>
            {
                item.Click -= new EventHandler(sideBarItemClicked);
                item.Click += new EventHandler(sideBarItemClicked);
            });
            preButton = sideBarItems[0];
            //Mặc định là chọn Trang chủ
            pnlView.Controls.Add(new ucTrangChu());
            //Hiển thị thông tin tài khoản
            string gender_img = nv.gioitinh.ToLower() == "Nam".ToLower() ? "profile.png" : "profile-female.png";
            pictureAvatar.Image = Image.FromFile($"../../public/img/{gender_img}");
            lblInfoAccount.Text = $"Họ Tên: {nv.ten}\nChức Vụ: {nv.chucvu}";
            //Set ToolTip
            ToolTip tt = new ToolTip();
            tt.SetToolTip(btnExit, "Thoát");
            tt.SetToolTip(btnRestoreDown, "Phóng to");
            tt.SetToolTip(btnMinimize, "Thu nhỏ");

            phanQuyen();
        }
        private void sideBarItemClicked(object sender, EventArgs e)
        {
            Guna2Button btn = sender as Guna2Button;
            int i = 0;
            setActiveButton(preButton, false);
            setActiveButton(btn, true);
            sideBarItems.ForEach(item =>
            {
                if (item.Name == btn.Name)
                {
                    item.Image = Image.FromFile($@"../../public/img/{list_img_active[i]}");
                }
                else
                {
                    item.Image = Image.FromFile($@"../../public/img/{list_img_non_active[i]}");
                }
                item.ImageAlign = HorizontalAlignment.Left;
                item.ImageSize = new Size(34, 34);
                item.ImageOffset = new Point(4, 0);
                i++;
            });
            preButton = btn;
        }

        public void setActiveButton(Guna2Button btn, bool status)
        {
            if (status == true)
            {
                btn.FillColor = Color.White;
                btn.ForeColor = Color.FromArgb(0, 118, 212);
            }
            else
            {
                btn.FillColor = Color.FromArgb(0, 118, 212);
                btn.ForeColor = Color.White;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
            preComponent.Close();
        }

        private void btnRestoreDown_Click(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
                tt.SetToolTip(btnRestoreDown, "Phóng to");
            }
            else
            {
                WindowState = FormWindowState.Maximized;
                tt.SetToolTip(btnRestoreDown, "Bình thường");
            }
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Close();
            preComponent.Visible = true;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnSideBarItem2_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucBanHang(nv));
        }

        private void btnSideBarItem3_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucQuanLyTaiKhoan());
        }

        private void btnSideBarItem4_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucQuanLyNhomMon());
        }

        private void btnSideBarItem5_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucQuanLyThucDon());
        }

        private void btnSideBarItem6_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucQuanLyBan(nv));
        }

        private void btnSideBarItem7_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucNhaCungCap(nv));
        }

        private void btnSideBarItem8_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucNguyenLieu(nv));
        }

        private void btnSideBarItem9_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucNhapKho(nv));
        }

        private void btnSideBarItem10_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucXuatKho(nv));
        }

        private void btnSideBarItem11_Click(object sender, EventArgs e)
        {
            pnlView.Controls.Clear();
            pnlView.Controls.Add(new ucThongKe());
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            new ChangePassword(nv).Visible = true;
        }
    }
}
