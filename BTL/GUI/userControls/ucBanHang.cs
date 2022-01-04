using BTL.Model;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BTL.DAO;

namespace BTL
{
    public partial class ucBanHang : UserControl
    {
        private NhanVien nv = new NhanVien();
        private List<Ban> ds_ban = new List<Ban>();
        private List<HoaDon> ds_hd = new List<HoaDon>();
        private List<NhomMon> ds_nhom = new List<NhomMon>();
        private List<MonAn> ds_mon = new List<MonAn>();
        private decimal totalPrice = 0;
        private DAO_NhomMon dao_nh = new DAO_NhomMon();
        private DAO_MonAn dao_mon = new DAO_MonAn();
        private DAO_Ban dao_b = new DAO_Ban();
        private DAO_HoaDon dao_hd = new DAO_HoaDon();
        private DAO_ChiTietHoaDon dao_cthd = new DAO_ChiTietHoaDon();
        private Ban ban_dang_chon;
        private HoaDon hoa_don_cua_ban_dang_chon;
        private ucBan btn_table;
        private List<ucBan> list_table = new List<ucBan>();
        public ucBanHang(NhanVien x)
        {
            InitializeComponent();
            nv = x;
            Dock = DockStyle.Fill;
            ds_ban = dao_b.getAll();
            ds_ban.ForEach(ban =>
            {
                list_table.Add(new ucBan(this, ban));
                fpnlTable.Controls.Add(list_table[list_table.Count - 1]);
            });
            //Lấy ra danh sách hoá đơn
            ds_hd = dao_hd.getAll();

            dao_nh.getAll().ForEach(nhom =>
            {
                ds_nhom.Add(nhom);
                cbGroup.Items.Add(nhom.ten);
            });
            if(cbGroup.Items.Count > 0)
            {
                cbGroup.SelectedIndex = 0;
            }
        }

        private void ucBanHang_Load(object sender, EventArgs e)
        {
            
        }

        public void hienThiDanhSachMon(HoaDon hd)
        {
            totalPrice = 0;
            dgvFood.Rows.Clear();
            hd.ds_mon.ForEach(ct =>
            {
                dgvFood.Rows.Add(new object[]
                {
                    ct.mon.ten, ct.mon.dvt, ct.soluong, ct.mon.gia.ToString("#,##")
                });
                totalPrice += ct.soluong * ct.mon.gia;
            });
            dgvFood.ClearSelection();
        }

        public void allowChangeTable(bool status)
        {
            cbNewTable.Enabled = btnChangeTable.Enabled = status;
        }

        public void xemThongTinBan(ucBan btn, Ban b)
        {
            ban_dang_chon = b;
            hoa_don_cua_ban_dang_chon = dao_b.getInvoice(b.soban);
            btn_table = btn;

            //Hiển thị số bàn
            lblInfoTable.Text = $"Bàn: {ban_dang_chon.soban}";

            if (hoa_don_cua_ban_dang_chon == null)//Bàn này chưa có khách
            {
                lblTimeIn.Text = "Giờ khách vào:";
                lbPriceSum.Text = "Tổng tiền: 0đ";
                dgvFood.Rows.Clear();
            }
            else//Bàn này đã có khách
            {
                //Hiển thị giờ khách vào
                lblTimeIn.Text = $"Giờ khách vào: {hoa_don_cua_ban_dang_chon.giovao.ToString("dd-MM-yyyy HH:mm:ss")}";

                //Hiển thị danh sách món lên datagridview
                hienThiDanhSachMon(hoa_don_cua_ban_dang_chon);

                //Hiển thị tổng tiền
                lbPriceSum.Text = $"Tổng tiền: {totalPrice.ToString("#,##")}đ";
            }

            
            if(ban_dang_chon.trangthai == false)//Bàn này có khách, nên dc cho phép chuyển bàn
            {
                cbNewTable.Items.Clear();
                allowChangeTable(true);
                dao_b.searchByStatus(true).ForEach(bn =>
                {
                    if (bn.soban != ban_dang_chon.soban)
                    {
                        cbNewTable.Items.Add(bn.soban);
                    }
                });
            }
            else
            {
                cbNewTable.Items.Clear();
                allowChangeTable(false);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (hoa_don_cua_ban_dang_chon != null && ban_dang_chon != null)
            {
                int index = ds_hd.FindIndex(hd => hd.sohd == hoa_don_cua_ban_dang_chon.sohd);
                if (index != -1)
                {
                    hoa_don_cua_ban_dang_chon.ds_mon = dao_cthd.getAll(hoa_don_cua_ban_dang_chon.sohd);
                    ds_hd[index] = hoa_don_cua_ban_dang_chon;
                }
                ds_hd[index].giora = DateTime.Now;
                dgvFood.Rows.Clear();
                totalPrice = 0;
                lbPriceSum.Text = "Tổng tiền: 0đ";
                lblTimeIn.Text = "Giờ khách vào:";
                new Report(ds_hd[index]).Visible = true;
                lblInfoTable.Text = "Bàn:";
                btn_table.discharge();
                ds_hd.RemoveAt(index);
                hoa_don_cua_ban_dang_chon = null;
                ban_dang_chon = null;
                btn_table = null;
            }
            else
            {
                MessageDialog.Show("Chưa chọn bàn để thanh toán", "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            }
        }

        private void cbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbGroup.SelectedIndex!=-1)
            {
                ds_mon.Clear();
                cbFood.Items.Clear();
                dao_mon.getByGroup(ds_nhom[cbGroup.SelectedIndex].ma).ForEach(mon =>
                {
                    ds_mon.Add(mon);
                    cbFood.Items.Add(mon.ten);
                });
                if (cbFood.Items.Count > 0)
                {
                    cbFood.SelectedIndex = 0;
                }
            }
        }

        public void getTableClicked(Ban ban)
        {
            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ban_dang_chon == null)
            {
                MessageDialog.Show("Chưa chọn bàn", "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            }
            else
            {
                //Tiến hành thêm món

                if (cbFood.SelectedIndex != -1)
                {
                    //Lấy ra món ăn
                    MonAn mon = ds_mon[cbFood.SelectedIndex];
                    //Lấy ra số lượng
                    int soluong = Convert.ToInt32(numQuantity.Value);

                    if(soluong > 0)
                    {
                        //Tạo chi tiết hoá đơn
                        ChiTietHoaDon cthd = new ChiTietHoaDon();
                        cthd.mon = mon;
                        cthd.soluong = soluong;

                        if (hoa_don_cua_ban_dang_chon == null)
                        {
                            hoa_don_cua_ban_dang_chon = new HoaDon();
                            hoa_don_cua_ban_dang_chon.sohd = dao_hd.genetareID();
                            hoa_don_cua_ban_dang_chon.giovao = hoa_don_cua_ban_dang_chon.giora = DateTime.Now;
                            hoa_don_cua_ban_dang_chon.ban = ban_dang_chon;
                            hoa_don_cua_ban_dang_chon.nv = nv;
                            dao_hd.bookTable(hoa_don_cua_ban_dang_chon);
                            ds_hd.Add(hoa_don_cua_ban_dang_chon);
                            btn_table.order();

                            cbNewTable.Items.Clear();
                            allowChangeTable(true);
                            dao_b.searchByStatus(true).ForEach(bn =>
                            {
                                if (bn.soban != ban_dang_chon.soban)
                                {
                                    cbNewTable.Items.Add(bn.soban);
                                }
                            });
                        }
                        //Lưu chi tiết hoá đơn vào CSDL
                        dao_cthd.insertOne(hoa_don_cua_ban_dang_chon.sohd, mon.mamon, soluong);
                        //Lưu danh sách món
                        int index = ds_hd.FindIndex(hd => hd.sohd == hoa_don_cua_ban_dang_chon.sohd);
                        if (index != -1)
                        {
                            hoa_don_cua_ban_dang_chon.ds_mon = dao_cthd.getAll(hoa_don_cua_ban_dang_chon.sohd);
                            ds_hd[index] = hoa_don_cua_ban_dang_chon;
                        }

                        hienThiDanhSachMon(hoa_don_cua_ban_dang_chon);
                        lblInfoTable.Text = $"Bàn: {hoa_don_cua_ban_dang_chon.ban.soban}";
                        lblTimeIn.Text = $"Giờ khách vào: {hoa_don_cua_ban_dang_chon.giovao.ToString("dd-MM-yyyy HH:mm:ss")}";
                        lbPriceSum.Text = $"Tổng tiền: {totalPrice.ToString("#,##")}đ";

                        numQuantity.Value = 1;
                    }
                    else
                    {
                        MessageDialog.Show("Số lượng phải > 0", "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
                    }
                }
            }
        }

        private void dgvFood_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(dgvFood.SelectedRows.Count == 0)
            {
                MessageDialog.Show("Chưa chọn thực đơn cần xoá", "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            }
            else
            {
                for (int i = dgvFood.SelectedRows.Count - 1; i >= 0; i--)
                {
                    int index = dgvFood.SelectedRows[i].Index;
                    string foodName = dgvFood.Rows[index].Cells[0].Value.ToString();
                    string foodUnit = dgvFood.Rows[index].Cells[1].Value.ToString();
                    int quantity = Convert.ToInt32(dgvFood.Rows[index].Cells[2].Value);

                    DialogResult answer = MessageDialog.Show(
                        $"Chắc chắn xoá\n{foodName} - {quantity}?",
                        "Xác Nhận", MessageDialogButtons.YesNo,
                        MessageDialogIcon.Question, MessageDialogStyle.Light);
                    if (answer == DialogResult.Yes)
                    {
                        int vi_tri = hoa_don_cua_ban_dang_chon.ds_mon.FindIndex(ct => ct.mon.ten == foodName && ct.mon.dvt == foodUnit);
                        if (vi_tri != -1)
                        {
                            dao_cthd.deleteOne(hoa_don_cua_ban_dang_chon.sohd, hoa_don_cua_ban_dang_chon.ds_mon[vi_tri].mon.mamon);
                            hoa_don_cua_ban_dang_chon.ds_mon.RemoveAt(index);
                            dgvFood.Rows.RemoveAt(index);
                        }
                    }
                }
                dgvFood.ClearSelection();
            }
        }

        private void cbFood_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbFood.SelectedIndex != -1)
            {
                lblUnit.Text = $"ĐVT: {ds_mon[cbFood.SelectedIndex].dvt}";
            }
        }

        private void btnChangeTable_Click(object sender, EventArgs e)
        {
            if (cbNewTable.SelectedItem != null)
            {
                DialogResult answer = MessageDialog.Show(
                        $"Chuyển từ bàn {ban_dang_chon.soban} sang bàn {cbNewTable.SelectedItem}",
                        "Xác Nhận", MessageDialogButtons.YesNo,
                        MessageDialogIcon.Question, MessageDialogStyle.Light);
                if (answer == DialogResult.Yes)
                {
                    dao_b.updateOne(new Ban(ban_dang_chon.soban, !ban_dang_chon.trangthai));

                    cbNewTable.Items.Add(ban_dang_chon.soban);

                    list_table.Find(table => table.getID() == ban_dang_chon.soban).discharge();

                    ban_dang_chon = new Ban(Convert.ToInt32(cbNewTable.SelectedItem), false);

                    dao_b.updateOne(new Ban(ban_dang_chon.soban, ban_dang_chon.trangthai));

                    string sohd = hoa_don_cua_ban_dang_chon.sohd;
                    DateTime giovao = hoa_don_cua_ban_dang_chon.giovao;
                    NhanVien nv = hoa_don_cua_ban_dang_chon.nv;
                    Ban ban = hoa_don_cua_ban_dang_chon.ban;
                    List<ChiTietHoaDon> dsm = hoa_don_cua_ban_dang_chon.ds_mon;

                    hoa_don_cua_ban_dang_chon = new HoaDon(sohd, giovao, giovao, nv, ban_dang_chon, dsm);

                    dao_hd.updateOne(hoa_don_cua_ban_dang_chon);
                    
                    lblInfoTable.Text = $"Bàn: {ban_dang_chon.soban}";

                    btn_table = list_table.Find(table => table.getID() == ban_dang_chon.soban);

                    btn_table.order();

                    cbNewTable.Items.Remove(ban_dang_chon.soban);
                }
            }
        }
    }
}
