using BTL.Model;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using BTL.DAO;

namespace BTL
{
    public partial class ucQuanLyNhomMon : UserControl
    {
        private const string ADD = "Thêm";
        private const string EDIT = "Sửa";
        private List<NhomMon> ds_nhom = new List<NhomMon>();
        private string action;
        private List<int> rowIndexs = new List<int>();
        private DAO_NhomMon dao_nh = new DAO_NhomMon();
        public ucQuanLyNhomMon()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        private void ucQuanLyNhomMon_Load(object sender, EventArgs e)
        {
            dao_nh.getAll().ForEach(nhom =>
            {
                cbId.Items.Add(nhom.ma);
                ds_nhom.Add(nhom);
                dgvGroup.Rows.Add(new object[]
                {
                    nhom.ma, nhom.ten
                });
            });
            dgvGroup.ClearSelection();
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
                cbId.Text = dao_nh.genetareID();
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
            txtName.Text = "";
        }
        public void setEnabled(bool status)
        {
            cbId.Enabled = status;
            txtName.Enabled = status;
            btnSave.Enabled = status;
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
            btnCancel.Enabled = (action == EDIT);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            try
            {
                for (int i = dgvGroup.SelectedRows.Count - 1; i >= 0; i--)
                {
                    int index = dgvGroup.SelectedRows[i].Index;
                    DialogResult answer = MessageBox.Show(this,
                        $@"Dữ liệu liên quan đến món <{dgvGroup.Rows[index].Cells[1].Value}> cũng sẽ bị xoá. Bạn có chắc chắn xoá không ?",
                        "Xác nhận", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (answer == DialogResult.Yes)
                    {
                        int index2 = ds_nhom.FindIndex(item => item.ma == dgvGroup.Rows[index].Cells[0].Value.ToString());
                        dao_nh.deleteOne(ds_nhom[index2].ma);
                        dgvGroup.Rows.RemoveAt(index);
                        ds_nhom.RemoveAt(index2);
                        cbId.Items.RemoveAt(index2);
                    }
                }
                dgvGroup.ClearSelection();
            }
            catch(SqlException er)
            {
                MessageBox.Show(this, "Xoá không thành công", "Chú ý", MessageBoxButtons.OK);
                Console.WriteLine(er);
            }
        }
        public NhomMon getData()
        {
            string error = "";
            string manhom = cbId.Text;
            string ten = txtName.Text;
            if (manhom == "")
            {
                error += "Mã nhóm không được để trống\n";
            }
            if (ten == "")
            {
                error += "Tên không được để trống\n";
            }
            if(error == "")
            {
                NhomMon nhom = new NhomMon(cbId.Text, txtName.Text);
                return nhom;
            }
            MessageDialog.Show(error, "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            return null;
        }

        public void setData(NhomMon nhom)
        {
            txtName.Text = nhom.ten;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            NhomMon nhom = getData();
            if (nhom != null)
            {
                if (action == ADD)
                {
                    dao_nh.insert(nhom);
                    ds_nhom.Add(nhom);
                    dgvGroup.Rows.Add(new object[]
                    {
                        nhom.ma, nhom.ten
                    });
                    cbId.Items.Add(nhom.ma);
                    cbId.Text = dao_nh.genetareID();
                }
                else if (action == EDIT)
                {
                    dao_nh.updateOne(nhom);
                    int index = ds_nhom.FindIndex(item => item.ma == nhom.ma);
                    if(index != -1)
                    {
                        dgvGroup.Rows[index].Cells[1].Value = nhom.ten;
                        ds_nhom[index] = nhom;
                    }
                }
                reset();
                dgvGroup.ClearSelection();
                btnDelete.Enabled = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void dgvGroup_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Enabled = true;
        }

        private void cbId_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (action == EDIT && cbId.SelectedItem != null)
            {
                NhomMon nhom = ds_nhom.Find(item => "" + item.ma == cbId.SelectedItem.ToString());
                if (nhom != null)
                {
                    setData(nhom);
                }
            }
        }
    }
}
