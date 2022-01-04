using BTL.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BTL.DAO
{
    public class DAO_DoanhThu
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;

        public DAO_DoanhThu()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<DoanhThu> doanhThuCacNgayTrongTuan(DateTime day)
        {
            List<DoanhThu> result = new List<DoanhThu>();
            cnn.Open();
            scm = new SqlCommand($"execute sp_DoanhThuCacNgayTrongTuan '{day}'", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                int key = reader.GetInt32(0);
                decimal value = reader.GetDecimal(1);
                result.Add(new DoanhThu(key, value));
            }
            cnn.Close();
            return result;
        }

        public List<DoanhThu> doanhThuCacNgayTrongThang(DateTime day)
        {
            List<DoanhThu> result = new List<DoanhThu>();
            cnn.Open();
            scm = new SqlCommand($"execute sp_DoanhThuCacNgayTrongThang {day.Month},{day.Year}", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                int key = reader.GetInt32(0);
                decimal value = reader.GetDecimal(1);
                result.Add(new DoanhThu(key, value));
            }
            cnn.Close();
            return result;
        }

        public List<DoanhThu> doanhThuCacThangTrongNam(DateTime day)
        {
            List<DoanhThu> result = new List<DoanhThu>();
            cnn.Open();
            scm = new SqlCommand($"execute sp_DoanhThuCacThangTrongNam {day.Year}", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                int key = reader.GetInt32(0);
                decimal value = reader.GetDecimal(1);
                result.Add(new DoanhThu(key, value));
            }
            cnn.Close();
            return result;
        }

        public List<DoanhThu> doanhThuCacQuyTrongNam(DateTime day)
        {
            List<DoanhThu> result = new List<DoanhThu>();
            cnn.Open();
            scm = new SqlCommand($"execute sp_DoanhThuCacQuyTrongNam {day.Year}", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                int key = reader.GetInt32(0);
                decimal value = reader.GetDecimal(1);
                result.Add(new DoanhThu(key, value));
            }
            cnn.Close();
            return result;
        }

        public List<DoanhThu> doanhThuCacNamGanDay(int num)
        {
            List<DoanhThu> result = new List<DoanhThu>();
            cnn.Open();
            scm = new SqlCommand($"execute sp_DoanhThuCacNamGanDay {num}", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                int key = reader.GetInt32(0);
                decimal value = reader.GetDecimal(1);
                result.Add(new DoanhThu(key, value));
            }
            cnn.Close();
            return result;
        }
    }
}
