using BTL.DAO;
using BTL.Model;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace BTL
{
    public partial class ucThongKe : UserControl
    {
        private List<Ban> ds_ban = new List<Ban>();
        private List<MonAn> ds_mon = new List<MonAn>();
        private List<NhomMon> ds_nhom = new List<NhomMon>();
        private DAO_NhomMon dao_nh = new DAO_NhomMon();
        private DAO_MonAn dao_m = new DAO_MonAn();
        private DAO_HoaDon dao_hd = new DAO_HoaDon();
        private DAO_ChiTietHoaDon dao_cthd = new DAO_ChiTietHoaDon();
        private DAO_Ban dao_b = new DAO_Ban();
        private DAO_DoanhThu dao_dt = new DAO_DoanhThu();
        private DataTable table = new DataTable();
        public ucThongKe()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            table.Columns.Add("key", typeof(string));
            table.Columns.Add("value", typeof(decimal));
        }

        public void thongKeHoaDon(DateTime start, DateTime end)
        {
            //Thống kê hoá đơn từ ngày đến ngày
            List<HoaDon> list_order = dao_hd.getInStartEnd(start, end);
            //Hiển thị lên datagridview
            displayOrder_DataGridView(list_order);
        }
        public void thongKeSoLuongMonAn(DateTime start, DateTime end)
        {
            List<ChiTietHoaDon> result = dao_cthd.getInStartEnd(start, end);
            displayOrderItem_DataGridView(result);
        }

        public void thongKeChung()
        {
            decimal total = dao_hd.getTotalPriceAll();
            totalOrder.Text = "Tổng số hoá đơn: " + dao_hd.getAll().Count;
            totalPrice.Text = "Tổng tiền bán được: " + (total == 0 ? "" + 0 : total.ToString("#,##")) + "đ";
        }

        public void displayOrder_DataGridView(List<HoaDon> list_order)
        {
            dgvOrder.Rows.Clear();
            list_order.ForEach(order =>
            {
                decimal totalPrice = dao_hd.getTotalPrice(order.sohd);
                dgvOrder.Rows.Add(new object[]
                {
                    order.sohd, 
                    order.giovao.ToString("dd-MM-yyyy HH:mm:ss"), 
                    order.giora.ToString("dd-MM-yyyy HH:mm:ss"), 
                    order.ban.soban,
                    totalPrice.ToString("#,##"),
                    order.nv.ten
                });
                
            });
            dgvOrder.ClearSelection();
            
        }

        public void displayOrderItem_DataGridView(List<ChiTietHoaDon> items)
        {
            dgvFood.Rows.Clear();
            items.ForEach(item =>
            {
                dgvFood.Rows.Add(new object[]
                {
                    item.mon.mamon,
                    item.mon.ten,
                    item.mon.dvt,
                    item.soluong,
                    item.mon.gia.ToString("#,##"),
                    item.mon.nhom.ten
                });
            });
            dgvFood.ClearSelection();
        }

        private void ucThongKe_Load(object sender, EventArgs e)
        {
            //Lấy danh sách bàn
            ds_ban = dao_b.getAll();
            //Lấy danh sách nhóm
            ds_nhom = dao_nh.getAll();
            //Lấy danh sách món ăn
            ds_mon = dao_m.getAll();
            //Mặc định là thống kê theo ngày hôm nay
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime end = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            thongKeHoaDon(start, end);
            thongKeSoLuongMonAn(start, end);
            //Thống kê chung
            totalTable.Text = "Tổng số bàn: " + ds_ban.Count;
            totalFood.Text = "Tổng số thực đơn: " + ds_mon.Count;
            totalGroup.Text = "Tổng số nhóm thực đơn: " + ds_nhom.Count;
            cbFilter.SelectedIndex = 0;

            thongKeChung();

            tabControl1.Visible = true;
        }

        public void thongKeCacNgayTrongTuan(DateTime day)
        {
            List<DoanhThu> result = dao_dt.doanhThuCacNgayTrongTuan(day);
            table.Rows.Clear();
            DoanhThu chunhat = result[0];
            result.RemoveAt(0);
            result.Add(chunhat);
            result.ForEach(item =>
            {
                if(item.time == 1)
                {
                    table.Rows.Add("Chủ nhật", item.value);
                }
                else
                {
                    table.Rows.Add("Thứ " + item.time, item.value);
                }
            });
            loadStatistics("Doanh Thu Các Ngày Trong Tuần");
        }

        public void loadStatistics(string title)
        {
            rpvStatistics.LocalReport.ReportPath = "../../GUI/reports/BieuDo.rdlc";
            ReportParameter rptTitle = new ReportParameter("title", title);
            rpvStatistics.LocalReport.SetParameters(new ReportParameter[] { rptTitle });
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "DataSet1";
            rds.Value = table;
            rpvStatistics.LocalReport.DataSources.Clear();
            rpvStatistics.LocalReport.DataSources.Add(rds);
            rpvStatistics.RefreshReport();
        }

        public void thongKeCacThangTrongNam(DateTime day)
        {
            List<DoanhThu> result = dao_dt.doanhThuCacThangTrongNam(day);
            table.Rows.Clear();

            result.ForEach(item =>
            {
                table.Rows.Add(item.time, item.value);
            });
            loadStatistics($"Doanh Thu Các Tháng Trong Năm {day.Year}");
        }

        public void thongKeCacQuyTrongNam(DateTime day)
        {
            List<DoanhThu> result = dao_dt.doanhThuCacQuyTrongNam(day);
            table.Rows.Clear();
            result.ForEach(item =>
            {
                table.Rows.Add("Quý " + item.time, item.value);
            });
            loadStatistics($"Doanh Thu Các Quý Trong Năm {day.Year}");
        }

        public void thongKeCacNgayTrongThang(DateTime day)
        {
            List<DoanhThu> result = dao_dt.doanhThuCacNgayTrongThang(day);
            table.Rows.Clear();
            result.ForEach(item =>
            {
                table.Rows.Add(item.time, item.value);
            });
            loadStatistics($"Doanh Thu Các Ngày Trong Tháng {day.Month}");
        }

        public void thongKeCacNamGanDay(int num)
        {
            List<DoanhThu> result = dao_dt.doanhThuCacNamGanDay(num);
            table.Rows.Clear();
            result.ForEach(item =>
            {
                table.Rows.Add("Năm " + item.time, item.value);
            });
            loadStatistics($"Doanh Thu {num} Năm Gần Đây");
        }

        public void bieuDoThongKe(int index)
        {
            if (index == 0)
            {
                thongKeCacNgayTrongTuan(dateTimeFilter.Value);
            }
            else if (index == 1)
            {
                thongKeCacNgayTrongThang(dateTimeFilter.Value);
            }
            else if (index == 2)
            {
                thongKeCacThangTrongNam(dateTimeFilter.Value);
            }
            else if (index == 3)
            {
                thongKeCacQuyTrongNam(dateTimeFilter.Value);
            }
            else if (index == 4)
            {
                thongKeCacNamGanDay(3);
            }
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            bieuDoThongKe(cbFilter.SelectedIndex);
        }

        private void dateTimeFilter_ValueChanged(object sender, EventArgs e)
        {
            bieuDoThongKe(cbFilter.SelectedIndex);
        }

        private void dateTimeStart1_ValueChanged(object sender, EventArgs e)
        {
            DateTime start = new DateTime(dateTimeStart1.Value.Year, dateTimeStart1.Value.Month, dateTimeStart1.Value.Day, 0, 0, 0);
            DateTime end = new DateTime(dateTimeEnd1.Value.Year, dateTimeEnd1.Value.Month, dateTimeEnd1.Value.Day, 23, 59, 59);
            thongKeHoaDon(start, end);
            dgvOrder.ClearSelection();
        }

        private void dateTimeEnd1_ValueChanged(object sender, EventArgs e)
        {
            DateTime start = new DateTime(dateTimeStart1.Value.Year, dateTimeStart1.Value.Month, dateTimeStart1.Value.Day, 0, 0, 0);
            DateTime end = new DateTime(dateTimeEnd1.Value.Year, dateTimeEnd1.Value.Month, dateTimeEnd1.Value.Day, 23, 59, 59);
            thongKeHoaDon(start, end);
            dgvOrder.ClearSelection();
        }

        private void dateTimeStart2_ValueChanged(object sender, EventArgs e)
        {
            DateTime start = new DateTime(dateTimeStart2.Value.Year, dateTimeStart2.Value.Month, dateTimeStart2.Value.Day, 0, 0, 0);
            DateTime end = new DateTime(dateTimeEnd2.Value.Year, dateTimeEnd2.Value.Month, dateTimeEnd2.Value.Day, 23, 59, 59);
            thongKeSoLuongMonAn(start, end);
            dgvFood.ClearSelection();
        }

        private void dateTimeEnd2_ValueChanged(object sender, EventArgs e)
        {
            DateTime start = new DateTime(dateTimeStart2.Value.Year, dateTimeStart2.Value.Month, dateTimeStart2.Value.Day, 0, 0, 0);
            DateTime end = new DateTime(dateTimeEnd2.Value.Year, dateTimeEnd2.Value.Month, dateTimeEnd2.Value.Day, 23, 59, 59);
            thongKeSoLuongMonAn(start, end);
            dgvFood.ClearSelection();
        }
    }
}
