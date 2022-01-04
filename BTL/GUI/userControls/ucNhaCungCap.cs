using BTL.Model;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BTL
{
    public partial class ucNhaCungCap : UserControl
    {
        private const string ADD = "Thêm";
        private const string EDIT = "Sửa";
        private string action;
        private List<NhaCungCap> ds_ncc = new List<NhaCungCap>();
        private NhanVien nv;
        private DAO.DAO_NhaCungCap dao_ncc = new DAO.DAO_NhaCungCap();
        public ucNhaCungCap(NhanVien x)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            nv = x;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            reset();
            if (action == ADD)
            {
                action = "";
            }
            else
            {
                action = ADD;
                cbId.Text = dao_ncc.genetareID();
            }
            setEnabled(action == ADD);
            btnCancel.Enabled = (action == ADD);
            cbId.Enabled = false;
        }

        public void reset()
        {
            if(action != ADD)
            {
                cbId.SelectedIndex = -1;
            }
            txtName.Text = txtAddress.Text = txtPhone.Text = "";
        }

        public void setEnabled(bool status)
        {
            cbId.Enabled = status;
            txtName.Enabled = status;
            txtAddress.Enabled = status;
            txtPhone.Enabled = status;
            btnSave.Enabled = status;
        }

        private void ucNhaCungCap_Load(object sender, EventArgs e)
        {
            ds_ncc = dao_ncc.getAll();
            ds_ncc.ForEach(ncc =>
            {
                cbId.Items.Add(ncc.ma);
                addToDGV(ncc);
            });
            dgvSupplier.ClearSelection();
        }

        public void addToDGV(NhaCungCap ncc)
        {
            dgvSupplier.Rows.Add(new object[]
            {
                ncc.ma, ncc.ten, ncc.diachi, ncc.sdt
            });
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            reset();
            if (action == EDIT)
            {
                action = "";
            }
            else
            {
                action = EDIT;
            }
            setEnabled(action == EDIT);
            btnCancel.Enabled = (action == EDIT);
            cbId.Text = "";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            for (int i = dgvSupplier.SelectedRows.Count - 1; i >= 0; i--)
            {
                int index = dgvSupplier.SelectedRows[i].Index;
                DialogResult answer = MessageBox.Show(this,
                        $@"Các dữ liệu liên quan đến <{dgvSupplier.Rows[index].Cells[1].Value}> sẽ bị xoá. Bạn có thật sự muốn xoá ?",
                        "Lưu ý",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                if (answer == DialogResult.Yes)
                {
                    int index2 = ds_ncc.FindIndex(item => item.ma == dgvSupplier.Rows[index].Cells[0].Value.ToString());
                    dao_ncc.deleteOne(ds_ncc[index2].ma);

                    dgvSupplier.Rows.RemoveAt(index);
                    ds_ncc.RemoveAt(index2);
                    cbId.Items.RemoveAt(index2);
                }
                dgvSupplier.ClearSelection();
                btnDelete.Enabled = false;
            }
        }
        public NhaCungCap getData()
        {
            string error = "";
            string mancc = cbId.Text;
            string ten = txtName.Text;
            string diachi = txtAddress.Text;
            string sdt = txtPhone.Text;
            if(mancc == "")
            {
                error += "Chưa chọn mã nhà cung cấp.\n";
            }
            if(ten == "")
            {
                error += "Tên không được bỏ trống.\n";
            }
            if (diachi == "")
            {
                error += "Địa chỉ không được bỏ trống.\n";
            }
            if (sdt.Length != 10)
            {
                error += "Số điện thoại chỉ có 10 chữ số.\n";
            }
            if (error == "")
            {
                NhaCungCap ncc = new NhaCungCap(mancc, ten, diachi, sdt);
                return ncc;
            }
            MessageDialog.Show(error, "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            return null;
        }
        public void setData(NhaCungCap ncc)
        {
            txtName.Text = ncc.ten;
            txtAddress.Text = ncc.diachi;
            txtPhone.Text = ncc.sdt;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            NhaCungCap ncc = getData();
            if (ncc != null)
            {
                if (action == ADD)
                {
                    dao_ncc.insert(ncc);
                    ds_ncc.Add(ncc);
                    addToDGV(ncc);
                    cbId.Items.Add(ncc.ma + 1);
                    cbId.Text = dao_ncc.genetareID();
                }
                else if (action == EDIT)
                {
                    dao_ncc.updateOne(ncc);
                    int index = ds_ncc.FindIndex(item => item.ma == ncc.ma);
                    dgvSupplier.Rows[index].Cells[1].Value = ncc.ten;
                    dgvSupplier.Rows[index].Cells[2].Value = ncc.diachi;
                    dgvSupplier.Rows[index].Cells[3].Value = ncc.sdt;
                    ds_ncc[index] = ncc;
                }
                reset();
                dgvSupplier.ClearSelection();
                btnDelete.Enabled = false;
            }
        }


        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtPhone.Text.Length > 9) e.Handled = true;
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)8) e.Handled = false;
        }

        private void cbId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(action == EDIT && cbId.SelectedItem!= null)
            {
                NhaCungCap ncc = ds_ncc.Find(item => "" + item.ma == cbId.SelectedItem.ToString());
                if (ncc != null)
                {
                    setData(ncc);
                }
            }
        }

        private void dgvSupplier_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Enabled = true;
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            dgvSupplier.Rows.Clear();
            dao_ncc.searchByName(txtKeyword.Text).ForEach(ncc =>
            {
                addToDGV(ncc);
            });
            dgvSupplier.ClearSelection();
            btnDelete.Enabled = false;
        }
    }
}
