using BTL.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BTL
{
    public partial class ucNguyenLieu : UserControl
    {
        private const string EDIT = "Sửa";
        private const string ADD = "Thêm";
        private string action;
        private List<NhaCungCap> ds_ncc = new List<NhaCungCap>();
        private List<NguyenLieu> ds_nl = new List<NguyenLieu>();
        private NhanVien nv;
        private DAO.DAO_NguyenLieu dao_nl = new DAO.DAO_NguyenLieu();
        private DAO.DAO_NhaCungCap dao_ncc = new DAO.DAO_NhaCungCap();
        public ucNguyenLieu(NhanVien x)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            nv = x;
        }

        private void ucNguyenLieu_Load(object sender, EventArgs e)
        {
            dao_ncc.getAll().ForEach(ncc =>
            {
                ds_ncc.Add(ncc);
                cbSupplier.Items.Add(ncc.ten);
            });
            cbId.Items.Clear();

            dao_nl.getAll().ForEach(nl =>
            {
                addToDGV(nl);
                if (!cbUnit.Items.Contains(nl.dvt))
                {
                    cbUnit.Items.Add(nl.dvt);
                }
                ds_nl.Add(nl);
                cbId.Items.Add(nl.ma);
            });
            dgvProduct.ClearSelection();
        }

        public void addToDGV(NguyenLieu nl)
        {
            dgvProduct.Rows.Add(new object[]
            {
                nl.ncc.ten, nl.ma, nl.ten, nl.dvt, dao_nl.getInventory(nl.ma), nl.gia.ToString("#,##")
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
            btnCancel.Enabled = cbSupplier.Enabled = cbId.Enabled = (action == EDIT);
            cbId.Text = "";
        }

        public void setEnabled(bool status)
        {
            txtName.Enabled = status;
            cbUnit.Enabled = status;
            numPrice.Enabled = status;
            btnSave.Enabled = status;
        }
        public void setData(NguyenLieu ngl)
        {
            cbSupplier.Text = ngl.ncc.ten;
            txtName.Text = ngl.ten;
            cbUnit.Text = ngl.dvt;
            txtInventory.Text = $"{dao_nl.getInventory(ngl.ma)}";
            numPrice.Value = ngl.gia;
            cbId.Text = ngl.ma;
        }
        public void reset()
        {
            txtName.Text = "";
            numPrice.Value =  0;
            txtInventory.Text = "0";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            reset();
        }

        public NguyenLieu getData()
        {
            string error = "";
            decimal price = numPrice.Value;
            NhaCungCap nccp = ds_ncc.Find(ncc => ncc.ten == cbSupplier.Text);
            if (nccp == null)
            {
                error += "Nhà cung cấp không hợp lệ\n";
            }
            if (cbId.Text == "")
            {
                error += "Mã nguyên liệu không được để trống\n";
            }
            if (txtName.Text == "")
            {
                error += "Tên không được để trống\n";
            }
            if (cbUnit.Text == "")
            {
                error += "Đơn vị tính không được để trống\n";
            }
            if (price == 0)
            {
                error += "Giá tiền không được để trống\n";
            }
            
            if (error == "")
            {
                return new NguyenLieu(
                    cbId.Text,
                    price,
                    txtName.Text,
                    cbUnit.Text,
                    nccp
                );
            }
            MessageBox.Show(this, error, "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            NguyenLieu nl = getData();
            if(nl != null)
            {
                if(action == EDIT)
                {
                    dao_nl.updateOne(nl);
                    int index = ds_nl.FindIndex(item => item.ma == cbId.Text);
                    ds_nl[index] = nl;
                    dgvProduct.Rows[index].Cells[0].Value = nl.ncc.ten;
                    dgvProduct.Rows[index].Cells[1].Value = nl.ma;
                    dgvProduct.Rows[index].Cells[2].Value = nl.ten;
                    dgvProduct.Rows[index].Cells[3].Value = nl.dvt;
                    dgvProduct.Rows[index].Cells[5].Value = nl.gia.ToString("#,##");
                }
                else if(action == ADD)
                {
                    dao_nl.insertOne(nl);
                    ds_nl.Add(nl);
                    addToDGV(nl);
                    cbId.Items.Add(nl.ma);
                    cbId.Text = dao_nl.genetareID();
                    if (!cbUnit.Items.Contains(nl.dvt))
                    {
                        cbUnit.Items.Add(nl.dvt);
                    }
                }
                reset();
                dgvProduct.ClearSelection();
                btnDelete.Enabled = false;
            }
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
                cbId.Text = dao_nl.genetareID();
            }
            setEnabled(action == ADD);
            btnCancel.Enabled = (action == ADD);
            cbSupplier.Enabled = (action == ADD);
            cbId.Enabled = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            for (int i = dgvProduct.SelectedRows.Count - 1; i >= 0; i--)
            {
                int index = dgvProduct.SelectedRows[i].Index;
                DialogResult answer = MessageBox.Show(this,
                        $@"Các dữ liệu liên quan đến <{dgvProduct.Rows[index].Cells[2].Value}> sẽ bị xoá. Bạn có thật sự muốn xoá ?",
                        "Lưu ý",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                if (answer == DialogResult.Yes)
                {
                    int index2 = ds_nl.FindIndex(item => item.ma == dgvProduct.Rows[index].Cells[1].Value.ToString());
                    if(index2 != -1)
                    {
                        dao_nl.deleteOne(ds_nl[index2].ma);
                        cbId.Items.Remove(dgvProduct.Rows[index].Cells[1].Value.ToString());
                        dgvProduct.Rows.RemoveAt(index);
                        ds_nl.RemoveAt(index2);
                    }
                }
            }
            dgvProduct.ClearSelection();
            btnDelete.Enabled = false;
        }

        private void txtKeyword_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Enabled = true;
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            dgvProduct.Rows.Clear();
            dao_nl.searchByName(txtKeyword.Text).ForEach(nl =>
            {
                dgvProduct.Rows.Add(new object[]
                {
                    nl.ncc.ten, nl.ma, nl.ten, nl.dvt, dao_nl.getInventory(nl.ma), nl.gia.ToString("#,##")
                });
            });
            dgvProduct.ClearSelection();
            btnDelete.Enabled = false;
        }
        private void cbSupplier_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if(action == EDIT && cbSupplier.SelectedIndex != -1 && cbId.Text == "")
            {
                NhaCungCap nccp = ds_ncc.Find(ncc => ncc.ten == cbSupplier.Text);
                if(nccp != null)
                {
                    cbId.Items.Clear();
                    dao_nl.getBySupplier(nccp.ma).ForEach(nl =>
                    {
                        cbId.Items.Add(nl.ma);
                    });
                }
            }
            else
            {
                cbId.Items.Clear();
                ds_nl.ForEach(nl =>
                {
                    cbId.Items.Add(nl.ma);
                });
            }
        }

        private void cbId_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (action == EDIT && cbId.SelectedIndex != -1)
            {
                setEnabled(true);
                int index = ds_nl.FindIndex(item => item.ma == cbId.Text);
                if(index != -1)
                {
                    setData(ds_nl[index]);
                }
            }
        }

        private void cbSupplier_TextChanged(object sender, EventArgs e)
        {
            if(action == EDIT && cbSupplier.Text == "" )
            {
                cbId.Items.Clear();
                ds_nl.ForEach(nl =>
                {
                    cbId.Items.Add(nl.ma);
                });
            }
        }
    }
}
