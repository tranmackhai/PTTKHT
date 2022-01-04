using BTL.Model;
using BTL.utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BTL.DAO
{
    public class DAO_Phieu
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;

        public DAO_Phieu()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<Phieu> getAll(bool type)//true: phiếu nhập, false: phiếu xuất
        {
            List<Phieu> result = new List<Phieu>();
            cnn.Open();
            string query = (type == true) ? "select * from phieunhap" : "select * from phieuxuat";
            scm = new SqlCommand(query, cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                DateTime ngaynhap = reader.GetDateTime(1);
                Phieu ph = new Phieu(ma, ngaynhap, new DAO_NhanVien().getById(reader.GetString(2)), new List<ChiTietPhieu>());
                result.Add(ph);
            }
            cnn.Close();
            return result;
        }

        public Phieu getById(bool type, string sophieu)
        {
            cnn.Open();
            string query = (type == true) ? $"select * from phieunhap where sopn ='{sophieu}'" : 
                                            $"select * from phieuxuat where sopx ='{sophieu}'";
            scm = new SqlCommand(query, cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                DateTime ngaynhap = reader.GetDateTime(1);
                Phieu ph = new Phieu(ma, ngaynhap, new DAO_NhanVien().getById(reader.GetString(2)), new List<ChiTietPhieu>());
                cnn.Close();
                return ph;
            }
            cnn.Close();
            return null;
        }

        public Phieu insert(bool type, Phieu phieu, NhanVien nv)
        {
            phieu.nv = nv;
            DateTime now = DateTime.Now;
            cnn.Open();
            string query = (type == true) ? 
                $@"insert into phieunhap(sopn, ngaynhap, manv) values ('{phieu.sophieu}','{now}',{nv.ma})" :
                $@"insert into phieuxuat(sopx, ngayxuat, manv) values ('{phieu.sophieu}','{now}',{nv.ma})";
            scm = new SqlCommand(query, cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
            return phieu;
        }

        public void deleteOne(bool type, string sophieu)
        {
            cnn.Open();
            string query = (type == true) ?
                $@"delete from phieunhap where  sopn = '{sophieu}'" :
                $@"delete from phieuxuat where  sopx = '{sophieu}'";
            scm = new SqlCommand(query, cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public string genetareID(bool type)
        {
            Method methods = new Method();
            string result = "";

            int i = 1;

            string str = (type == true ? "PN" : "PX") + methods.addZero(7, i);
            while (true)
            {
                if (getById(type, str) == null)
                {
                    result += str;
                    break;
                }
                else
                {
                    str = (type == true ? "PN" : "PX") + methods.addZero(7, ++i);
                }
            }

            return result;
        }
    }
}
