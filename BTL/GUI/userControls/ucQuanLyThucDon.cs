using BTL.Model;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BTL
{
    public partial class ucQuanLyThucDon : UserControl
    {
        private const string ADD = "Thêm";
        private const string EDIT = "Sửa";
        private string action = "";
        private List<NhomMon> ds_nhom = new List<NhomMon>();
        private List<MonAn> ds_mon = new List<MonAn>();
        private DAO.DAO_NhomMon dao_nh = new DAO.DAO_NhomMon();
        private DAO.DAO_MonAn dao_m = new DAO.DAO_MonAn();
        public ucQuanLyThucDon()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        private void ucQuanLyThucDon_Load(object sender, EventArgs e)
        {
            dao_nh.getAll().ForEach(nhom =>
            {
                cbGroup.Items.Add(nhom.ten);
                ds_nhom.Add(nhom);
            });
            cbId.Items.Clear();
            dao_m.getAll().ForEach(mon =>
            {
                dgvFood.Rows.Add(new object[]{
                    mon.nhom.ten,
                    mon.mamon,
                    mon.ten,
                    mon.dvt,
                    mon.gia.ToString("#,##")
                });
                ds_mon.Add(mon);
                cbId.Items.Add(mon.mamon);
                if (!cbUnit.Items.Contains(mon.dvt))
                {
                    cbUnit.Items.Add(mon.dvt);
                }
            });
            dgvFood.ClearSelection();
        }

        public void setEnabled(bool status)
        {
            cbGroup.Enabled = status;
            txtName.Enabled = status;
            cbUnit.Enabled = status;
            numPrice.Enabled = status;
            btnSave.Enabled = status;
        }

        public void setData(MonAn mon)
        {
            cbGroup.Text = mon.nhom.ten;
            cbId.Text = ""+mon.mamon;
            txtName.Text = mon.ten;
            cbUnit.Text = mon.dvt;
            numPrice.Value = mon.gia;
        }

        public MonAn getData()
        {
            string error = "";
            string name = txtName.Text;
            string dvt = cbUnit.Text;
            NhomMon nhom = ds_nhom.Find(nh => nh.ten == cbGroup.Text);
            decimal price = numPrice.Value;
            if (nhom == null)
            {
                error += "Nhóm thực đơn không hợp lệ.\n";
            }
            if (ds_mon.Find(mon=>mon.mamon == cbId.Text) == null && action == EDIT)
            {
                error += "Thực đơn không hợp lệ.\n";
            }
            if (name == "")
            {
                error += "Tên thực đơn không được bỏ trống.\n";
            }
            if (dvt == "")
            {
                error += "Đơn vị tính không được bỏ trống.\n";
            }
            if (price == 0)
            {
                error += "Giá bán phải lớn hơn 0.\n";
            }

            if(error == "")
            {
                MonAn mon = new MonAn(
                    nhom,
                    cbId.Text,
                    name,
                    cbUnit.Text,
                    price
                );
                return mon;
            }
            MessageDialog.Show(error, "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            return null;
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
                cbId.Text = dao_m.genetareID();
                action = ADD;
            }
            cbId.Enabled = false;
            btnCancel.Enabled = (action == ADD);
            setEnabled(action == ADD);
        }

        public void reset()
        {
            txtName.Text = "";
            numPrice.Value = 0;
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
            cbId.Text = "";
            setEnabled(action == EDIT);
            cbId.Enabled = (action == EDIT);
            btnCancel.Enabled = action == EDIT;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MonAn mon = getData();
            if (mon != null)
            {
                if (action == ADD)
                {
                    dao_m.insertOne(mon);
                    dgvFood.Rows.Add(new object[]{
                        mon.nhom.ten,
                        mon.mamon,
                        mon.ten,
                        mon.dvt,
                        mon.gia.ToString("#,##")
                    });
                    ds_mon.Add(mon);
                    cbId.Items.Add(mon.mamon);
                    cbId.Text = dao_m.genetareID();
                    if (!cbUnit.Items.Contains(mon.dvt))
                    {
                        cbUnit.Items.Add(mon.dvt);
                    }
                }
                else if (action == EDIT)
                {
                    dao_m.updateOne(mon);
                    int index = ds_mon.FindIndex(item => item.mamon == mon.mamon);
                    ds_mon[index] = mon;
                    if(index < dgvFood.RowCount)
                    {
                        dgvFood.Rows[index].Cells[0].Value = mon.nhom.ten;
                        dgvFood.Rows[index].Cells[2].Value = mon.ten;
                        dgvFood.Rows[index].Cells[3].Value = mon.dvt;
                        dgvFood.Rows[index].Cells[4].Value = mon.gia.ToString("#,##");
                    }
                }
                dgvFood.ClearSelection();
                btnDelete.Enabled = false;
                reset();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = dgvFood.SelectedRows.Count - 1; i >= 0; i--)
                {
                    int index = dgvFood.SelectedRows[i].Index;
                    DialogResult answer = MessageBox.Show(this,
                        $@"Dữ liệu liên quan đến món <{dgvFood.Rows[index].Cells[2].Value}> cũng sẽ bị xoá. Bạn có chắc chắn xoá không ?",
                        "Xác nhận", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (answer == DialogResult.Yes)
                    {
                        int index2 = ds_mon.FindIndex(item => item.mamon == dgvFood.Rows[index].Cells[1].Value.ToString());
                        dao_m.deleteOne(ds_mon[index2].mamon);
                        cbId.Items.Remove(dgvFood.Rows[index].Cells[1].Value.ToString());
                        dgvFood.Rows.RemoveAt(index);
                        ds_mon.RemoveAt(index2);
                    }
                }
                dgvFood.ClearSelection();
                btnDelete.Enabled = false;
            }
            catch (SqlException er)
            {
                MessageBox.Show(this, "Xoá không thành công", "Chú ý", MessageBoxButtons.OK);
                Console.WriteLine(er);
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            dgvFood.Rows.Clear();
            dao_m.searchByName(txtKeyword.Text).ForEach(mon =>
            {
                dgvFood.Rows.Add(new object[]{
                    mon.nhom.ten,
                    mon.mamon,
                    mon.ten,
                    mon.dvt,
                    mon.gia.ToString("#,##")
                });
            });
            dgvFood.ClearSelection();
        }

        private void dgvFood_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Enabled = true;
        }

        private void cbId_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cbId.Text != "")
            {
                MonAn mon = ds_mon.Find(m => m.mamon == cbId.Text);
                if (mon != null)
                {
                    setData(mon);
                }
                else
                {
                    reset();
                }
            }
            else
            {
                reset();
            }
        }

        private void cbGroup_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if(cbGroup.Text != "")
            {
                NhomMon nh = ds_nhom.Find(nhom => nhom.ten == cbGroup.Text);
                if(nh != null)
                {
                    cbId.Items.Clear();
                    dao_m.getByGroup(nh.ma).ForEach(mon=>
                    {
                        cbId.Items.Add(mon.mamon);
                    });
                }
            }
            else
            {
                cbId.Items.Clear();
                ds_mon.ForEach(mon =>
                {
                    cbId.Items.Add(mon.mamon);
                });
            }
        }

        private void cbGroup_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void cbGroup_TextChanged(object sender, EventArgs e)
        {
            if (cbGroup.Text != "")
            {
                NhomMon nh = ds_nhom.Find(nhom => nhom.ten == cbGroup.Text);
                if (nh != null)
                {
                    cbId.Items.Clear();
                    dao_m.getByGroup(nh.ma).ForEach(mon =>
                    {
                        cbId.Items.Add(mon.mamon);
                    });
                }
            }
            else
            {
                cbId.Items.Clear();
                ds_mon.ForEach(mon =>
                {
                    cbId.Items.Add(mon.mamon);
                });
            }
        }

        private void cbId_TextChanged(object sender, EventArgs e)
        {
            if (cbId.Text != "")
            {
                MonAn mon = ds_mon.Find(m => m.mamon == cbId.Text);
                if (mon != null)
                {
                    setData(mon);
                }
                else
                {
                    reset();
                }
            }
            else
            {
                reset();
            }
        }
    }
}
