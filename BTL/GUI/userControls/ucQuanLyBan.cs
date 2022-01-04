using BTL.DAO;
using BTL.Model;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BTL
{
    public partial class ucQuanLyBan : UserControl
    {
        private const string ADD = "Thêm";
        private const string EDIT = "Sửa";
        private List<Ban> ds_ban = new List<Ban>();
        private DAO_Ban dao_b = new DAO_Ban();
        private DAO_HoaDon dao_hd = new DAO_HoaDon();
        private NhanVien nv;
        private string action;
        public ucQuanLyBan(NhanVien x)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            nv = x;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Ban ban = getData();
            if (ban != null)
            {
                if (action == ADD)
                {
                    dao_b.insert(ban);
                    ds_ban.Add(ban);
                    addToDGV(ban);
                    cbId.Items.Add(ban.soban);
                    cbId.Text = dao_b.generateID().ToString();
                }
                else if (action == EDIT)
                {
                    if(ban.trangthai == false)
                    {
                        DateTime now = DateTime.Now;
                        dao_hd.bookTable(new HoaDon(dao_hd.genetareID(), now, now, nv, ban, new List<ChiTietHoaDon>()));
                    }
                    else
                    {
                        HoaDon hd = dao_b.getInvoice(ban.soban);
                        if(hd != null)
                        {
                            if (hd.ds_mon.Count == 0)
                            {
                                dao_hd.deleteOne(hd.sohd);
                                dao_b.updateOne(ban);
                            }
                            else
                            {
                                dao_hd.discharge(hd.sohd, DateTime.Now);
                            }
                        }
                    }
                    int index = ds_ban.FindIndex(item => item.soban == ban.soban);
                    if(index != -1)
                    {
                        dgvTable.Rows[index].Cells[1].Value = (ban.trangthai == true) ? "Trống" : "Có Khách";
                        ds_ban[index] = ban;
                    }
                }
                dgvTable.ClearSelection();
                btnDelete.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (action == ADD)
            {
                action = "";
            }
            else
            {
                action = ADD;
                cbId.Text = dao_b.generateID().ToString();
            }
            setEnabled(action == ADD);
            cbId.Enabled = false;
        }
        public void setEnabled(bool status)
        {
            cbStatus.Enabled = status;
            btnSave.Enabled = status;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
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
        }

        private void cbId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbId.Text != "")
            {
                Ban ban = ds_ban.Find(item => "" + item.soban == cbId.Text);
                if (ban != null)
                {
                    setData(ban);
                }
            }
        }
        public void setData(Ban ban)
        {
            cbStatus.SelectedIndex = ((ban.trangthai == true) ? 0 : 1);
        }
        private void ucQuanLyBan_Load(object sender, EventArgs e)
        {
            cbSearch.SelectedIndex = 2;
            dgvTable.Rows.Clear();
            cbId.Items.Clear();
            dao_b.getAll().ForEach(ban =>
            {
                cbId.Items.Add(ban.soban);
                ds_ban.Add(ban);
                addToDGV(ban);
            });
            dgvTable.ClearSelection();
        }
        public Ban getData()
        {
            string error = "";
            if(cbId.Text == "")
            {
                error += "Chưa chọn số bàn\n";
            }
            if(cbStatus.Text == "")
            {
                error += "Chưa chọn trạng thái bàn\n";
            }
            try
            {
                int soban = Convert.ToInt32(cbId.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                error += "Số bàn không hợp lệ\n";
            }
            if (error == "")
            {
                bool status = cbStatus.SelectedIndex == 1 ? false : true;
                Ban ban = new Ban(Convert.ToInt32(cbId.Text), status);
                return ban;
            }
            MessageDialog.Show(error, "Lưu ý", MessageDialogButtons.OK, MessageDialogIcon.Error, MessageDialogStyle.Light);
            return null;
        }

        private void cbSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbSearch.SelectedIndex != -1)
            {
                dgvTable.Rows.Clear();
                cbId.Items.Clear();
                if (cbSearch.SelectedIndex == 2)
                {
                    dao_b.getAll().ForEach(ban =>
                    {
                        cbId.Items.Add(ban.soban);
                        ds_ban.Add(ban);
                        addToDGV(ban);
                    });
                }
                else
                {
                    bool status = false;
                    if (cbSearch.SelectedIndex == 0)
                    {
                        status = true;
                    }
                    else if (cbSearch.SelectedIndex == 1)
                    {
                        status = false;
                    }
                    dao_b.searchByStatus(status).ForEach(ban =>
                    {
                        cbId.Items.Add(ban.soban);
                        ds_ban.Add(ban);
                        addToDGV(ban);
                    });
                }
                dgvTable.ClearSelection();
            }
        }

        public void addToDGV(Ban ban)
        {
            dgvTable.Rows.Add(new object[]
            {
                ban.soban.ToString(), (ban.trangthai==true)?"Trống":"Có Khách"
            });
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = dgvTable.SelectedRows.Count - 1; i >= 0; i--)
                {
                    int index = dgvTable.SelectedRows[i].Index;
                    DialogResult answer = MessageBox.Show(this, 
                        $@"Dữ liệu liên quan đến bàn <{dgvTable.Rows[index].Cells[0].Value}> cũng sẽ bị xoá. Bạn có chắc chắn xoá không ?", 
                        "Xác nhận", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (answer == DialogResult.Yes)
                    {
                        int index2 = ds_ban.FindIndex(item => ""+item.soban == dgvTable.Rows[index].Cells[0].Value.ToString());
                        dao_b.deleteOne(Convert.ToInt32(dgvTable.Rows[index].Cells[0].Value));
                        dgvTable.Rows.RemoveAt(index);
                        ds_ban.RemoveAt(index2);
                        cbId.Items.RemoveAt(index2);
                    }
                }
                dgvTable.ClearSelection();
                btnDelete.Enabled = false;
            }
            catch (Exception er)
            {
                MessageBox.Show(this, "Xoá không thành công", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Console.WriteLine(er);
            }
        }

        private void dgvTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Enabled = true;
        }

        private void cbId_TextChanged(object sender, EventArgs e)
        {
            if (cbId.Text != "")
            {
                Ban ban = ds_ban.Find(item => "" + item.soban == cbId.Text);
                if (ban != null)
                {
                    setData(ban);
                }
            }
        }
    }
}
