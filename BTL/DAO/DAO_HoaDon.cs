using BTL.Model;
using BTL.utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace BTL.DAO
{
    public class DAO_HoaDon
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;

        public DAO_HoaDon()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<HoaDon> getAll()
        {
            List<HoaDon> result = new List<HoaDon>();
            cnn.Open();
            scm = new SqlCommand("select * from hoadon ", cnn);
            reader = scm.ExecuteReader();
            DAO_NhanVien dao_nv = new DAO_NhanVien();
            DAO_Ban dao_b = new DAO_Ban();
            DAO_ChiTietHoaDon dao_cthd = new DAO_ChiTietHoaDon();
            while (reader.Read())
            {
                string sohd = reader.GetString(0);
                HoaDon hd = new HoaDon(
                    sohd,
                    reader.GetDateTime(1),
                    reader.GetDateTime(2),
                    dao_nv.getById(reader.GetString(3)),
                    dao_b.getById(reader.GetInt32(4)),
                    dao_cthd.getAll(sohd)
                );
                result.Add(hd);
            }
            cnn.Close();
            return result;
        }

        public HoaDon getById(string id)
        {
            cnn.Open();
            scm = new SqlCommand($"select * from hoadon where sohd = '{id}'", cnn);
            reader = scm.ExecuteReader();
            DAO_NhanVien dao_nv = new DAO_NhanVien();
            DAO_Ban dao_b = new DAO_Ban();
            DAO_ChiTietHoaDon dao_cthd = new DAO_ChiTietHoaDon();
            if (reader.Read())
            {
                string sohd = reader.GetString(0);
                HoaDon result = new HoaDon(
                    sohd,
                    reader.GetDateTime(1),
                    reader.GetDateTime(2),
                    dao_nv.getById(reader.GetString(3)),
                    dao_b.getById(reader.GetInt32(4)),
                    dao_cthd.getAll(sohd)
                );
                cnn.Close();
                return result;
            }
            cnn.Close();
            return null;
        }

        public List<HoaDon> getInStartEnd(DateTime start, DateTime end)
        {
            List<HoaDon> result = new List<HoaDon>();
            cnn.Open();
            scm = new SqlCommand($@"
                select * from hoadon where giovao != giora and giora between '{start}' and '{end}'", cnn);
            reader = scm.ExecuteReader();
            DAO_NhanVien dao_nv = new DAO_NhanVien();
            DAO_Ban dao_b = new DAO_Ban();
            DAO_ChiTietHoaDon dao_cthd = new DAO_ChiTietHoaDon();
            while (reader.Read())
            {
                string sohd = reader.GetString(0);
                HoaDon hd = new HoaDon(
                    sohd,
                    reader.GetDateTime(1),
                    reader.GetDateTime(2),
                    dao_nv.getById(reader.GetString(3)),
                    dao_b.getById(reader.GetInt32(4)),
                    dao_cthd.getAll(sohd)
                );
                result.Add(hd);
            }
            cnn.Close();
            return result;
        }

        public void updateOne(HoaDon hd)
        {
            cnn.Open();
            SqlCommand scm = new SqlCommand(
              $@"update hoadon set giovao = '{hd.giovao}', giora = '{hd.giora}', manv = '{hd.nv.ma}', 
                soban = {hd.ban.soban} where sohd = '{hd.sohd}'"  
                , cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public decimal getTotalPriceAll()
        {
            decimal result = 0;
            cnn.Open();
            scm = new SqlCommand($@"
			declare @doanhthu decimal
			select @doanhthu = sum(m.giatien * cthd.soluong) 
			from hoadon hd, monan m, chitiethoadon cthd
			where hd.sohd = cthd.sohd and 
				m.mamon = cthd.mamon
            if(@doanhthu is null)
                begin
                    set @doanhthu = 0
                end
			select @doanhthu ", cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetDecimal(0);
            }
            cnn.Close();
            return result;
        }

        public decimal getTotalPrice(string sohd)
        {
            decimal result = 0;
            cnn.Open();
            scm = new SqlCommand($@"
			declare @doanhthu decimal
			select @doanhthu = sum(m.giatien * cthd.soluong) 
			from hoadon hd, monan m, chitiethoadon cthd
			where hd.sohd = cthd.sohd and 
				m.mamon = cthd.mamon and hd.sohd = '{sohd}'
            if(@doanhthu is null)
                begin
                    set @doanhthu = 0
                end
			select @doanhthu ", cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetDecimal(0);
            }
            cnn.Close();
            return result;
        }

        public void bookTable(HoaDon _hd)
        {
            cnn.Open();
            string now = DateTime.Now.ToString();
            string query = $@"
		        update ban set trangthai = 0 where soban = {_hd.ban.soban};
		        insert into hoadon(sohd, giovao,giora, soban, manv) 
		        values ('{_hd.sohd}', '{now}','{now}',{_hd.ban.soban}, '{_hd.nv.ma}')";
            scm = new SqlCommand(query, cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void discharge(string sohd, DateTime giora)
        {
            cnn.Open();
            SqlCommand scm = new SqlCommand($"execute sp_ThanhToan '{sohd}','{giora}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void deleteOne(string sohd)
        {
            cnn.Open();
            SqlCommand scm = new SqlCommand($"delete from hoadon where sohd = '{sohd}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public List<DoanhThu> daysInMonth(int month, int year)
        {
            List<DoanhThu> result = new List<DoanhThu>();
            string query = $@"select day(hd.giora), sum(m.giatien * cthd.soluong) 
	            from hoadon hd, monan m, chitiethoadon cthd
	            where hd.sohd = cthd.sohd and m.mamon = cthd.mamon and 
                MONTH(hd.giora) = {month} and YEAR(hd.giora) = {year}
	            group by day(hd.giora)";
            cnn.Open();
            scm = new SqlCommand(query, cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new DoanhThu(reader.GetInt32(0), reader.GetInt32(1)));
            }
            cnn.Close();
            return result;
        }
        public string genetareID()
        {
            Method methods = new Method();
            string result = "";

            int i = 1;

            string str = "HD" + methods.addZero(7, i);
            while (true)
            {
                if (getById(str) == null)
                {
                    result += str;
                    break;
                }
                else
                {
                    str = "HD" + methods.addZero(7, ++i);
                }
            }

            return result;
        }
    }
}
