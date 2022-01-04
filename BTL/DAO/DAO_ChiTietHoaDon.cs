using BTL.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace BTL.DAO
{
    public class DAO_ChiTietHoaDon
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;

        public DAO_ChiTietHoaDon()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<ChiTietHoaDon> getAll(string id)
        {
            List<ChiTietHoaDon> result = new List<ChiTietHoaDon>();
            cnn.Open();
            scm = new SqlCommand($"select mamon, soluong from chitiethoadon where sohd = '{id}'", cnn);
            reader = scm.ExecuteReader();
            DAO_MonAn dao_m = new DAO_MonAn();
            while (reader.Read())
            {
                MonAn mon = dao_m.getById(reader.GetString(0));
                int soluong = reader.GetInt32(1);
                ChiTietHoaDon cthd = new ChiTietHoaDon(mon, soluong);
                result.Add(cthd);
            }
            cnn.Close();
            return result;
        }

        public List<ChiTietHoaDon> getInStartEnd(DateTime start, DateTime end)
        {
            List<ChiTietHoaDon> result = new List<ChiTietHoaDon>();
            cnn.Open();
            scm = new SqlCommand($"execute sp_ThongKeSoLuongMonAnBanDuocTuNgayDenNgay  '{start}' , '{end}'", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string manhom = reader.GetString(0);
                string tennhom = reader.GetString(1);
                string mamon = reader.GetString(2);
                string tenmon = reader.GetString(3);
                string dvt = reader.GetString(4);
                decimal giatien = reader.GetDecimal(5);
                int soluong = reader.GetInt32(6);
                NhomMon nhom = new NhomMon(manhom, tennhom);
                MonAn mon = new MonAn(
                   nhom, mamon, tenmon, dvt, giatien
                );
                ChiTietHoaDon cthd = new ChiTietHoaDon(mon, soluong);
                result.Add(cthd);
            }
            cnn.Close();
            return result;
        }
        public void insertOne(string sohd, string mamon, int soluong)
        {
            cnn.Open();
            scm = new SqlCommand($@"insert into chitiethoadon (sohd, mamon, soluong)  
                            values('{sohd}','{mamon}',{soluong})", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();       
        }
        public void deleteAll(string sohd)
        {
            cnn.Open();
            scm = new SqlCommand($"delete from chitiethoadon where sohd = '{sohd}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }
        public void deleteOne(string sohd, string mamon)
        {
            cnn.Open();
            scm = new SqlCommand($"delete from chitiethoadon where sohd = '{sohd}' and mamon = '{mamon}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }
        public List<ChiTietHoaDon> getByTable(int soban)
        {
            List<ChiTietHoaDon> result = new List<ChiTietHoaDon>();
            cnn.Open();
            scm = new SqlCommand($@"select hd.sohd, m.mamon, m.tenmon, cthd.soluong , m.dvt, m.giatien, (cthd.soluong * m.giatien) as 'thanhtien', m.manhom, nh.tennhom
			from hoadon hd, monan m, chitiethoadon cthd, nhommon nh
			where hd.sohd = cthd.sohd and m.mamon = cthd.mamon and hd.giora = hd.giovao and m.manhom = nh.manhom and hd.soban = {soban}", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string mamon = reader.GetString(1);
                string ten = reader.GetString(2);
                int soluong = reader.GetInt32(3);
                string dvt = reader.GetString(4);
                decimal giatien = reader.GetDecimal(5);
                string manhom = reader.GetString(7);
                string tennhom = reader.GetString(8);
                result.Add(new ChiTietHoaDon(new MonAn(new NhomMon(manhom, tennhom), mamon, ten, dvt, giatien), soluong));
            }
            cnn.Close();
            return result;
        }
    }
}
