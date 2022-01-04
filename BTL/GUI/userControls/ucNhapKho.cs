using BTL.Model;
using Guna.UI2.WinForms;
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
    public partial class ucNhapKho : UserControl
    {
        private const string EDIT = "Sửa";
        private string action;
        private List<NhaCungCap> ds_ncc = new List<NhaCungCap>();
        private List<NguyenLieu> ds_nl = new List<NguyenLieu>();
        private List<Phieu> ds_ph = new List<Phieu>();
        private Phieu phieu = new Phieu();
        private NhanVien nv;
        private DAO.DAO_Phieu dao_p = new DAO.DAO_Phieu();
        private DAO.DAO_ChiTietPhieu dao_ctp = new DAO.DAO_ChiTietPhieu();
        private DAO.DAO_NhaCungCap dao_ncc = new DAO.DAO_NhaCungCap();
        private DAO.DAO_NguyenLieu dao_nl = new DAO.DAO_NguyenLieu();
        private bool type = true;
        public ucNhapKho(NhanVien x)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            nv = x;
        }
        private void ucNhapKho_Load(object sender, EventArgs e)
        {
            ds_ncc = dao_ncc.getAll();
            ds_ncc.ForEach(ncc =>
            {
                cbSupplier.Items.Add(ncc.ten);
            });

            
            ds_ph = dao_p.getAll(type);
        }

        public void moPhieu(Phieu p)
        {
            dgvProduct.Rows.Clear();
            phieu = p;
            groupDetail.Enabled = true;
            btnExport.Enabled = true;
            txtId.Text = "" + phieu.sophieu;
            phieu.list = dao_ctp.getById(type, p.sophieu);
            phieu.list.ForEach(ct =>
            {
                dgvProduct.Rows.Add(new object[]
                {
                    ct.nl.ma, ct.nl.ten, ct.nl.dvt, ct.soluong, ct.nl.gia.ToString("#,##")
                });
            });
            dgvProduct.ClearSelection();
            cbSupplier.Text = phieu.list[0].nl.ncc.ten;
            cbSupplier.Enabled = false;
            btnDelete.Enabled = false;
        }
        //Bấm tạo phiếu
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if(cbSupplier.Text != "")
            {
                if (txtId.Text == "")//Tạo phiếu
                {
                    groupDetail.Enabled = true;
                    cbSupplier.Enabled = false;
                    btnExport.Enabled = true;
                    cbId.Enabled = false;
                    phieu.sophieu = dao_p.genetareID(true);
                    txtId.Text = phieu.sophieu.ToString();
                    phieu.ngay = DateTime.Now;
                }
                else//Huỷ phiếu
                {
                    txtId.Text = "";
                    groupDetail.Enabled = false;
                    cbSupplier.Enabled = true;
                    btnExport.Enabled = false;
                    phieu = new Phieu();
                }
                dgvProduct.Rows.Clear();
            }
            else
            {
                MessageDialog.Show("Chọn nhà cung cấp trước khi tạo phiếu", "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            reset();
            cbId.SelectedIndex = -1;
            if (action == EDIT)
            {
                setEnabled(false);
                action = "";
                btnCancel.Enabled = false;
            }
            else
            {
                setEnabled(true);
                action = EDIT;
                txtName.Enabled = false;
                txtUnit.Enabled = false;
                txtPrice.Enabled = false;
                btnCancel.Enabled = true;
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
                if(phieu.list.Count == 0)
                {
                    dao_p.deleteOne(type, phieu.sophieu);
                    ds_ph.RemoveAt(ds_ph.Count - 1);
                }
            }
        }

        public NguyenLieu getData()
        {
            string error = "";
            if(cbId.Text == "")
            {
                error += "Chưa chọn nguyên liệu";
            }
            if(txtPrice.Text == "")
            {
                error += "Giá nguyên liệu phải > 0";
            }
            if(txtName.Text == "")
            {
                error += "Tên nguyên liệu không được để trống";
            }
            if (txtUnit.Text == "")
            {
                error += "Đơn vị tính không được để trống";
            }
            if(error == "")
            {
                NguyenLieu ncc = new NguyenLieu(
                cbId.Text,
                Convert.ToInt32(txtPrice.Text),
                txtName.Text,
                txtUnit.Text,
                ds_ncc[cbSupplier.SelectedIndex]);
                return ncc;
            }
            return null;
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            NguyenLieu nl = getData();
            int soluong = Convert.ToInt32(numQuantity.Value);
            if (nl != null && soluong > 0)
            {
                ChiTietPhieu ctp = new ChiTietPhieu(nl, soluong);
                //Thêm phiếu nhập
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
                if (action == EDIT)
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
                dgvProduct.ClearSelection();
                btnDelete.Enabled = false;
                reset();
                cbId.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show(this, "Dữ liệu không được để trống", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            txtId.Text = "";
            groupDetail.Enabled = false;
            cbSupplier.Enabled = true;
            btnExport.Enabled = false;
            dgvProduct.Rows.Clear();
            new Report(null).phieuNhap(phieu);
            phieu = new Phieu();
            action = "";
            setEnabled(false);
        }

        public void reset()
        {
            cbId.Text = "";
            txtName.Text = txtUnit.Text = txtPrice.Text = "";
            numQuantity.Value = 0;
        }

        public void setEnabled(bool status)
        {
            cbId.Enabled = status;
            txtName.Enabled = status;
            txtUnit.Enabled = status;
            numQuantity.Enabled = status;
            txtPrice.Enabled = status;
            btnSave.Enabled = status;
        }
        public void setData(string manl)
        {
            NguyenLieu nl = dao_nl.getById(manl);
            txtName.Text = nl.ten;
            txtPrice.Text = nl.gia.ToString();
            numQuantity.Value = dao_ctp.getQuantity(type, phieu.sophieu, manl);
            txtUnit.Text = nl.dvt;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            new DSPhieu(this, null).phieu(true);
        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Enabled = true;
        }
        private void cbId_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (action == EDIT && cbId.SelectedIndex != -1)
            {
                int index = ds_nl.FindIndex(item => item.ma == ds_nl[cbId.SelectedIndex].ma);
                if (index != -1)
                {
                    setData(ds_nl[index].ma);
                }
            }
        }

        private void cbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSupplier.SelectedIndex != -1)
            {
                cbId.Items.Clear();
                cbId.Enabled = !cbId.Enabled;
                cbId.Enabled = !cbId.Enabled;
                ds_nl = dao_nl.getBySupplier(ds_ncc[cbSupplier.SelectedIndex].ma);
                ds_nl.ForEach(nl =>
                {
                    cbId.Items.Add(nl.ma);
                });
            }
        }
    }
}
