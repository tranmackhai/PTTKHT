using BTL.Model;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BTL
{
    public partial class ucXuatKho : UserControl
    {
        private const string ADD = "Thêm";
        private const string EDIT = "Chọn";
        private string action = "";
        private List<NhaCungCap> ds_ncc = new List<NhaCungCap>();
        private List<NguyenLieu> ds_nl = new List<NguyenLieu>();
        private List<Phieu> ds_ph = new List<Phieu>();
        private Phieu phieu = new Phieu();
        private NhanVien nv;
        private DAO.DAO_Phieu dao_p = new DAO.DAO_Phieu();
        private DAO.DAO_ChiTietPhieu dao_ctp = new DAO.DAO_ChiTietPhieu();
        private DAO.DAO_NhaCungCap dao_ncc = new DAO.DAO_NhaCungCap();
        private DAO.DAO_NguyenLieu dao_nl = new DAO.DAO_NguyenLieu();
        private bool type = false;
        public ucXuatKho(NhanVien x)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            nv = x;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if(action != ADD)
            {
                action = ADD;
                phieu.sophieu = dao_p.genetareID(false);
                btnExport.Enabled = true;
                phieu.ngay = DateTime.Now;
                txtId.Text = $"{phieu.sophieu}";
            }
            else
            {
                txtId.Text = "";
                action = "";
            }
            groupDetail.Enabled = action == ADD;
        }
        public void reset()
        {
            cbId.Text = txtName.Text = txtUnit.Text = txtPrice.Text = "";
            txtInventory.Text = "0";
            numQuantity.Value = 0;
        }

        public void setEnabled(bool status)
        {
            cbId.Enabled = status;
            numQuantity.Enabled = status;
        }
        public void setData(string manl)
        {
            NguyenLieu nl = dao_nl.getById(manl);
            ChiTietPhieu ctp = phieu.list.Find(ct => ct.nl.ma == manl);
            cbSupplier.Text = nl.ncc.ten;
            txtName.Text = nl.ten;
            txtPrice.Text = nl.gia.ToString();
            numQuantity.Value = (ctp==null)?0:ctp.soluong;
            txtInventory.Text = $"{dao_nl.getInventory(manl)}";
            txtUnit.Text = nl.dvt;
        }

        private void ucXuatKho_Load(object sender, EventArgs e)
        {
            ds_ncc = dao_ncc.getAll();
            ds_ncc.ForEach(ncc =>
            {
                cbSupplier.Items.Add(ncc.ten);
            });

            cbId.Items.Clear();

            dao_nl.getAll().ForEach(nl =>
            {
                cbId.Items.Add(nl.ma);
                ds_nl.Add(nl);
            });
            ds_ph = dao_p.getAll(type);

            dgvProduct.ClearSelection();
        }

        internal void moPhieu(Phieu p)
        {
            dgvProduct.Rows.Clear();
            phieu = p;
            setEnabled(false);
            cbSupplier.Enabled = false;
            groupDetail.Enabled = true;
            txtId.Text = phieu.sophieu.ToString();
            btnExport.Enabled = true;
            phieu.list = dao_ctp.getById(type, p.sophieu);
            action = EDIT;
            phieu.list.ForEach(ct =>
            {
                dgvProduct.Rows.Add(new object[]
                {
                    ct.nl.ma, ct.nl.ten, ct.nl.dvt, ct.soluong, ct.nl.gia
                });
            });

            dgvProduct.ClearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            reset();
            if (action == ADD)
            {
                cbSupplier.Text = action = "";
            }
            else
            {
                action = ADD;
            }
            cbSupplier.Enabled = !cbSupplier.Enabled;
            setEnabled(false);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void cbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSupplier.SelectedIndex != -1)
            {
                setEnabled(true);
                cbId.Items.Clear();

                List<NguyenLieu> nls = dao_nl.getBySupplier(ds_ncc[cbSupplier.SelectedIndex].ma);
                nls.ForEach(nl =>
                {
                    cbId.Items.Add(nl.ma);
                });
                if(cbId.Text != "")
                {
                    reset();
                    cbId.Text = "";
                }
            }
        }
        public NguyenLieu getData()
        {
            NguyenLieu ncc = new NguyenLieu(
            cbId.Text,
            Convert.ToInt32(txtPrice.Text),
            txtName.Text,
            txtUnit.Text,
            ds_ncc[cbSupplier.SelectedIndex]);
            return ncc;

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            NguyenLieu nl = getData();
            int soluong = Convert.ToInt32(numQuantity.Value);
            if (action == ADD && soluong > 0)
            {
                int tonkho = dao_nl.getInventory(nl.ma);
                if(tonkho != 0)
                {
                    if (soluong > tonkho)
                    {
                        MessageDialog.Show("Số lượng xuất > tồn kho", "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
                    }
                    else
                    {
                        ChiTietPhieu ctp = new ChiTietPhieu(nl, soluong);

                        //Thêm phiếu xuất
                        if (phieu.list.Count == 0)
                        {
                            phieu = dao_p.insert(type, phieu, nv);
                            ds_ph.Add(phieu);
                        }
                        int index = phieu.list.FindIndex(ct => ct.nl.ma == ctp.nl.ma);

                        if (index == -1)
                        {
                            dao_ctp.insert(type, phieu.sophieu, ctp);
                            phieu.list.Add(ctp);
                        }
                        else
                        {
                            dao_ctp.updateOne(type, phieu.sophieu, ctp);
                            phieu.list[index].soluong = ctp.soluong;
                        }
                        ds_ph[ds_ph.Count - 1] = phieu;
                        if (action == ADD)
                        {
                            dgvProduct.Rows.Clear();
                            phieu.list.ForEach(ct =>
                            {
                                dgvProduct.Rows.Add(new object[]
                                {
                                ct.nl.ma, ct.nl.ten, ct.nl.dvt, ct.soluong, ct.nl.gia.ToString("#,##")
                                });
                            });
                        }
                        reset();
                        btnDelete.Enabled = false;
                        cbId.SelectedIndex = -1;
                        btnSave.Enabled = false;
                        dgvProduct.ClearSelection();
                    }
                }
                else
                {
                    MessageDialog.Show("Nguyên liệu này đã hết", "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
                }
            }
            else if(soluong <= 0)
            {
                MessageDialog.Show("Số lượng phải > 0", "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            }
        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            cbSupplier.Enabled = groupDetail.Enabled = !groupDetail.Enabled;
            cbSupplier.Text = "";
            setEnabled(!cbSupplier.Enabled);
            
            dgvProduct.Rows.Clear();
            
            new Report(null).phieuXuat(phieu);
            ds_ph.Add(phieu);
            action = "";
            phieu = new Phieu();
        }

        private void cbId_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = ds_nl.FindIndex(item => item.ma == cbId.Text);
            if (action == ADD && index != -1)
            {
                setData(ds_nl[index].ma);
                btnSave.Enabled = true;
                cbId.Text = ds_nl[index].ma;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            for (int i = dgvProduct.SelectedRows.Count - 1; i >= 0; i--)
            {
                int index = dgvProduct.SelectedRows[i].Index;
                dao_ctp.deleteOne(type, phieu.sophieu, Convert.ToInt32(dgvProduct.Rows[index].Cells[0].Value));
                
                dgvProduct.Rows.RemoveAt(index);
                phieu.list.RemoveAt(index);
                if (phieu.list.Count == 0)//ko có chi tiết hoá đơn thì xoá luôn phiếu
                {
                    dao_p.deleteOne(type, phieu.sophieu);
                    ds_ph.RemoveAt(ds_ph.Count - 1);
                }
            }
            dgvProduct.ClearSelection();
            btnDelete.Enabled = false;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            new DSPhieu(null,this).phieu(false);
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)8) e.Handled = false;
        }
    }
}
