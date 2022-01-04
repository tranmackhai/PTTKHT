using BTL.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Data.SqlClient;

namespace BTL
{
    public partial class ucQuanLyTaiKhoan : UserControl
    {
        private const string ADD = "Thêm";
        private const string EDIT = "Sửa";
        private string action = "";
        private List<NhanVien> ds_nv = new List<NhanVien>();
        private DAO.DAO_NhanVien dao_nv = new DAO.DAO_NhanVien();
        public ucQuanLyTaiKhoan()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        private void ucQuanLyTaiKhoan_Load(object sender, EventArgs e)
        {
            //Lấy tất cả dữ liệu đổ vào datagridview
            ds_nv = dao_nv.getAll();
            ds_nv.ForEach(nv =>
            {
                addToDGV(nv);
                cbId.Items.Add(nv.ma);
                if (!cbPosition.Items.Contains(nv.chucvu))
                {
                    cbPosition.Items.Add(nv.chucvu);
                }
            });
            dgvEmployee.ClearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            reset();
            if(action == ADD)//Huỷ chế độ thêm
            {
                action = "";
                cbId.SelectedIndex = -1;
            }
            else//Mở chế độ thêm
            {
                action = ADD;
                cbId.Text = dao_nv.genetareID();
            }
            setEnabled(action == ADD);
            btnCancel.Enabled = (action == ADD);
            cbId.Enabled = false;
        }

        public void setEnabled(bool status)
        {
            txtName.Enabled = status;
            txtPhone.Enabled = status;
            txtAddress.Enabled = status;
            cbId.Enabled = status;
            rbtnFemale.Enabled = status;
            rbtnMale.Enabled = status;
            cbPosition.Enabled = status;
            dateTime.Enabled = status;
            btnSave.Enabled = status;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            reset();
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
            cbId.SelectedIndex = -1;
            cbId.Text = "";
        }

        public void reset()
        {
            cbId.SelectedIndex = -1;
            txtName.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = dgvEmployee.SelectedRows.Count - 1; i >= 0; i--)
                {
                    int index = dgvEmployee.SelectedRows[i].Index;
                    DialogResult answer = MessageBox.Show(
                        this,
                        $@"Các dữ liệu liên quan đến nhân viên <{ds_nv[index].ten}> sẽ bị xoá. Bạn có chắn chắn xoá không ?",
                        "Xác nhận",
                        MessageBoxButtons.YesNo);
                    if (answer == DialogResult.Yes)
                    {
                        dao_nv.deleteById(ds_nv[index].ma);
                        ds_nv.RemoveAt(index);
                        dgvEmployee.Rows.RemoveAt(index);
                        cbId.Items.RemoveAt(index);
                        dgvEmployee.ClearSelection();
                        btnDelete.Enabled = false;
                    }
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(this, "Xoá không thành công", "Chú ý", MessageBoxButtons.OK);
                Console.WriteLine(er);
            }
        }

        public void setData(NhanVien nv)
        {
            if (nv.gioitinh == "Nam")
            {
                rbtnMale.Checked = true; 
            }
            else
            {
                rbtnFemale.Checked = true;
            }
            cbPosition.Text = nv.chucvu;
            txtName.Text = nv.ten;
            txtPhone.Text = nv.sdt;
            dateTime.Value = nv.ngaysinh;
            txtAddress.Text = nv.diachi;
        }

        public NhanVien getData()
        {
            string error = "";
            string manv = cbId.Text;
            string ten = txtName.Text;
            DateTime ngaysinh = dateTime.Value;
            string sdt = txtPhone.Text;
            string diachi = txtAddress.Text;
            string chucvu = cbPosition.Text;
            string gioitinh = (rbtnMale.Checked == true) ? "Nam" : "Nữ";
            if (manv == "")
            {
                error += "Mã nhân viên không được để trống\n";
            }
            if (ten == "")
            {
                error += "Tên không được để trống\n";
            }
            if ((DateTime.Now - ngaysinh).Days <= 18 * 365)
            {
                error += "Tuổi phải từ 18 trở lên\n";
            }
            if (diachi == "")
            {
                error += "Địa chỉ không được để trống\n";
            }
            if (sdt.Length > 10)
            {
                error += "Số điện thoại chỉ có 10 số\n";
            }
            if (chucvu == "")
            {
                error += "Chức vụ không được bỏ trống\n";
            }
            if(error == "")
            {
                NhanVien nv = new NhanVien(
                    manv, 
                    ten,
                    ngaysinh,
                    gioitinh,
                    diachi,
                    sdt,
                    chucvu,
                    "123456"
                );
                return nv;
            }
            MessageDialog.Show(error, "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            return null;
        }

        private void cbId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(action == EDIT && cbId.SelectedItem != null)
            {
                NhanVien nv = ds_nv.Find(item => "" + item.ma == cbId.SelectedItem.ToString());
                if (nv != null)
                {
                    setData(nv);
                }
            }
        }
        public void addToDGV(NhanVien nv)
        {
            dgvEmployee.Rows.Add(new object[]{
                nv.ma,
                nv.ten,
                nv.ngaysinh.Date.ToString("dd/MM/yyyy"),
                nv.gioitinh,
                nv.diachi,
                nv.sdt,
                nv.chucvu
            });
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            NhanVien nv = getData();
            if (nv != null)
            {
                if (action == ADD)
                {
                    dao_nv.insert(nv);
                    addToDGV(nv);
                    ds_nv.Add(nv);
                    cbId.Items.Add(nv.ma);
                    cbId.Text = dao_nv.genetareID();
                }
                else if (action == EDIT)
                {
                    dao_nv.update(nv);
                    int index = ds_nv.FindIndex(item => item.ma == nv.ma);
                    ds_nv[index] = nv;
                    for(int i = 0;i< dgvEmployee.RowCount; i++)
                    {
                        if(dgvEmployee.Rows[i].Cells[0].Value.ToString() == ds_nv[index].ma.ToString())
                        {
                            dgvEmployee.Rows[i].Cells[1].Value = nv.ten;
                            dgvEmployee.Rows[i].Cells[2].Value = nv.ngaysinh.Date.ToString("dd/MM/yyyy");
                            dgvEmployee.Rows[i].Cells[3].Value = nv.gioitinh;
                            dgvEmployee.Rows[i].Cells[4].Value = nv.diachi;
                            dgvEmployee.Rows[i].Cells[5].Value = nv.sdt;
                            dgvEmployee.Rows[i].Cells[6].Value = nv.chucvu;
                            break;
                        }
                    }
                    if (!cbPosition.Items.Contains(nv.chucvu))
                    {
                        cbPosition.Items.Add(nv.chucvu);
                    }
                }
                dgvEmployee.ClearSelection();
                btnDelete.Enabled = false;
                reset();
                btnCancel.Enabled = false;
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)8) e.Handled = false;
            if (e.KeyChar == (char)127) e.Handled = false;
        }

        private void dgvEmployee_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if(dgvEmployee.SelectedRows.Count > 0)
            {
                btnDelete.Enabled = true;
            }
        }

        private void dgvEmployee_Sorted(object sender, EventArgs e)
        {
            dgvEmployee.ClearSelection();
        }

        private void cbId_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            dgvEmployee.Rows.Clear();
            dao_nv.searchByName(guna2TextBox1.Text).ForEach(nv =>
            {
                addToDGV(nv);
            });
            dgvEmployee.ClearSelection();
            btnDelete.Enabled = false;
        }
    }
}
