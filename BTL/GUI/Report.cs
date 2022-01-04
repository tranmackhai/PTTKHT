using BTL.DAO;
using BTL.Model;
using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BTL
{
    public partial class Report : Form
    {
        private SqlConnection cnn;
        private HoaDon hd;
        private DAO_HoaDon dao_hd = new DAO_HoaDon();
        public Report(HoaDon x)
        {
            InitializeComponent();
            hd = x;
        }
        public Report()
        {
            InitializeComponent();
            DoanhThuCacNgayTrongThang(12, 2021);
        }
        public static string NumberToText(decimal inputNumber, bool suffix = true)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result + (suffix ? " đồng" : "");
        }

        public void phieuNhap(Phieu phieu)
        {
            string connetionString = @"Data Source=CHIBISUKE\SQLEXPRESS;Initial Catalog=btl_pttkht;User ID=sa;Password=123";
            cnn = new SqlConnection(connetionString);
            DataSet ds = new DataSet();
            cnn.Open();
            SqlDataAdapter adp = new SqlDataAdapter($@"
                select ct.sopn, nl.manl, nl.tennl, nl.dvt, ct.soluong, nl.giatien
	            from chitietphieunhap ct, nguyenlieu nl
	            where ct.sopn = '{phieu.sophieu}' and nl.manl = ct.manl", cnn);
            adp.Fill(ds);
            cnn.Close();
            reportViewer2.LocalReport.ReportPath = "../../GUI/reports/PhieuNhapKho.rdlc";
            ReportParameter rptId = new ReportParameter("id", "" + phieu.sophieu);
            ReportParameter rptDate = new ReportParameter("date", "" + phieu.ngay.Day);
            ReportParameter rptMonth = new ReportParameter("month", "" + phieu.ngay.Month);
            ReportParameter rptYear = new ReportParameter("year", "" + phieu.ngay.Year);
            ReportParameter rptTime = new ReportParameter("name_supplier", phieu.list[0].nl.ncc.ten);
            ReportParameter rptTable = new ReportParameter("address", "Quận 9, Thành phố Hồ Chí Minh");
            ReportParameter rptEmployee = new ReportParameter("price_toString", NumberToText(phieu.getTotalPrice()));
            reportViewer2.LocalReport.SetParameters(new ReportParameter[]
                { rptId, rptDate , rptMonth, rptYear, rptTime, rptTable, rptEmployee }
            );
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "NhapKho_DataSet";
            rds.Value = ds.Tables[0];
            reportViewer2.LocalReport.DataSources.Clear();
            reportViewer2.LocalReport.DataSources.Add(rds);
            reportViewer2.RefreshReport();
            Visible = true;
        }

        public void phieuXuat(Phieu phieu)
        {
            string connetionString = @"Data Source=CHIBISUKE\SQLEXPRESS;Initial Catalog=btl_pttkht;User ID=sa;Password=123";
            cnn = new SqlConnection(connetionString);
            DataSet ds = new DataSet();
            cnn.Open();
            SqlDataAdapter adp = new SqlDataAdapter($@"
                select ct.sopx, nl.manl, nl.tennl, nl.dvt, ct.soluong, nl.giatien
	            from chitietphieuxuat ct, nguyenlieu nl
	            where ct.sopx = '{phieu.sophieu}' and nl.manl = ct.manl", cnn);
            adp.Fill(ds);
            cnn.Close();
            reportViewer2.LocalReport.ReportPath = "../../GUI/reports/PhieuXuatKho.rdlc";
            ReportParameter rptId = new ReportParameter("id", "" + phieu.sophieu);
            ReportParameter rptDate = new ReportParameter("date", "" + phieu.ngay.Day);
            ReportParameter rptMonth = new ReportParameter("month", "" + phieu.ngay.Month);
            ReportParameter rptYear = new ReportParameter("year", "" + phieu.ngay.Year);
            ReportParameter rptTable = new ReportParameter("address", "Quận 9, Thành phố Hồ Chí Minh");
            ReportParameter rptEmployee = new ReportParameter("price_toString", NumberToText(phieu.getTotalPrice()));
            reportViewer2.LocalReport.SetParameters(new ReportParameter[]
                { rptId, rptDate , rptMonth, rptYear, rptTable, rptEmployee }
            );
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "XuatKho_DataSet";
            rds.Value = ds.Tables[0];
            reportViewer2.LocalReport.DataSources.Clear();
            reportViewer2.LocalReport.DataSources.Add(rds);
            reportViewer2.RefreshReport();
            Visible = true;
        }

        public void DoanhThuCacNgayTrongThang(int month, int year)
        {
            string connetionString = @"Data Source=CHIBISUKE\SQLEXPRESS;Initial Catalog=btl_pttkht;User ID=sa;Password=123";
            cnn = new SqlConnection(connetionString);
            DataSet ds = new DataSet();
            cnn.Open();
            SqlDataAdapter adp = new SqlDataAdapter($"execute sp_DoanhThuCacNgayTrongThang '{month}','{year}'", cnn);
            adp.Fill(ds);
            cnn.Close();
            reportViewer2.LocalReport.ReportPath = "../../GUI/reports/DT_CacNgayTrongThang.rdlc";
            //ReportParameter rptYear = new ReportParameter("year", year.ToString());
            //this.reportViewer2.LocalReport.SetParameters(new ReportParameter[] { rptYear });
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "DataSet";
            rds.Value = ds.Tables[0];
            reportViewer2.LocalReport.DataSources.Clear();
            reportViewer2.LocalReport.DataSources.Add(rds);
            reportViewer2.RefreshReport();
        }

        private void Report_Load(object sender, EventArgs e)
        {
            if (hd != null)
            {
                string connetionString = @"Data Source=CHIBISUKE\SQLEXPRESS;Initial Catalog=btl_pttkht;User ID=sa;Password=123";
                cnn = new SqlConnection(connetionString);
                DataSet ds = new DataSet();
                cnn.Open();
                SqlDataAdapter adp = new SqlDataAdapter($"execute sp_DanhSachMonAnCuaBan {hd.ban.soban}", cnn);
                adp.Fill(ds);
                cnn.Close();
                reportViewer2.LocalReport.ReportPath = "../../GUI/reports/HoaDon.rdlc";
                ReportParameter rptId = new ReportParameter("id", "" + hd.sohd);
                ReportParameter rptDate = new ReportParameter("date", "" + hd.giora.ToString("dd-MM-yyyy"));
                ReportParameter rptTime = new ReportParameter("time", "" + hd.giora.ToString("HH:mm:ss"));
                ReportParameter rptTable = new ReportParameter("table", "" + hd.ban.soban);
                ReportParameter rptEmployee = new ReportParameter("name_employee", "" + hd.nv.ten);
                reportViewer2.LocalReport.SetParameters(new ReportParameter[]
                    { rptId, rptDate , rptTime, rptTable, rptEmployee }
                );
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet1";
                rds.Value = ds.Tables[0];
                reportViewer2.LocalReport.DataSources.Clear();
                reportViewer2.LocalReport.DataSources.Add(rds);
                reportViewer2.RefreshReport();
                dao_hd.discharge(hd.sohd, hd.giora);
            }
        }
    }
}
