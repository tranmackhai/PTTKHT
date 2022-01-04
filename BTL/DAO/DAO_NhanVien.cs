using BTL.Model;
using BTL.utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BTL.DAO
{
    public class DAO_NhanVien
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;

        public DAO_NhanVien()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<NhanVien> getAll()
        {
            List<NhanVien> result = new List<NhanVien>();
            cnn.Open();
            scm = new SqlCommand("select * from nhanvien", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string tennv = reader.GetString(1);
                DateTime ngaysinh = reader.GetDateTime(2);
                string gioitinh = reader.GetString(3);
                string diachi = reader.GetString(4);
                string sdt = reader.GetString(5);
                string chucvu = reader.GetString(6);
                string mk = reader.GetString(7);
                NhanVien nv = new NhanVien(
                    ma, tennv, ngaysinh, gioitinh, diachi, sdt, chucvu, mk
                );
                result.Add(nv);
            }
            cnn.Close();
            return result;
        }

        public List<NhanVien> searchByName(string keyword)
        {
            List<NhanVien> result = new List<NhanVien>();
            cnn.Open();
            scm = new SqlCommand($"select * from nhanvien where tennv like N'%{keyword}%'", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string tennv = reader.GetString(1);
                DateTime ngaysinh = reader.GetDateTime(2);
                string gioitinh = reader.GetString(3);
                string diachi = reader.GetString(4);
                string sdt = reader.GetString(5);
                string chucvu = reader.GetString(6);
                string mk = reader.GetString(7);
                NhanVien nv = new NhanVien(
                    ma, tennv, ngaysinh, gioitinh, diachi, sdt, chucvu, mk
                );
                result.Add(nv);
            }
            cnn.Close();
            return result;
        }

        public NhanVien getById(string id)
        {
            cnn.Open();
            scm = new SqlCommand($"select * from nhanvien where manv = '{id}'", cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                string ma = reader.GetString(0);
                string tennv = reader.GetString(1);
                DateTime ngaysinh = reader.GetDateTime(2);
                string gioitinh = reader.GetString(3);
                string diachi = reader.GetString(4);
                string sdt = reader.GetString(5);
                string chucvu = reader.GetString(6);
                string mk = reader.GetString(7);
                NhanVien nv = new NhanVien(
                    ma, tennv, ngaysinh, gioitinh, diachi, sdt, chucvu, mk
                );
                cnn.Close();
                return nv;
            }
            cnn.Close();
            return null;
        }

        public void insert(NhanVien nv)
        {
            cnn.Open();
            scm = new SqlCommand( 
                $@"insert into nhanvien(manv, tennv, ngaysinh, gioitinh, sdt, chucvu, matkhau, diachi) values
                    ('{nv.ma}', N'{nv.ten}','{nv.ngaysinh}', N'{nv.gioitinh}', '{nv.sdt}', N'{nv.chucvu}', 
                    '{nv.matkhau}', N'{nv.diachi}')", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void update(NhanVien nv)
        {
            cnn.Open();
            scm = new SqlCommand(
                $@"update nhanvien set 
                        tennv = N'{nv.ten}', 
                        ngaysinh = '{nv.ngaysinh}', 
                        gioitinh = N'{nv.gioitinh}', 
                        diachi = N'{nv.diachi}', 
                        sdt ='{nv.sdt}',
                        chucvu =N'{nv.chucvu}',
                        matkhau='{nv.matkhau}'
                        where manv ='{nv.ma}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void deleteById(string id)
        {
            cnn.Open();
            scm = new SqlCommand($"delete from nhanvien where manv = {id}" , cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public NhanVien login(string manv, string matkhau)
        {
            try
            {
                cnn.Open();
                scm = new SqlCommand($"select * from nhanvien where manv = '{manv}' and matkhau = '{matkhau}'", cnn);
                reader = scm.ExecuteReader();
                if (reader.Read())
                {
                    string ma = reader.GetString(0);
                    string tennv = reader.GetString(1);
                    DateTime ngaysinh = reader.GetDateTime(2);
                    string gioitinh = reader.GetString(3);
                    string diachi = reader.GetString(4);
                    string sdt = reader.GetString(5);
                    string chucvu = reader.GetString(6);
                    string mk = reader.GetString(7);
                    NhanVien nv = new NhanVien(
                        ma, tennv, ngaysinh, gioitinh, diachi, sdt, chucvu, mk
                    );
                    cnn.Close();
                    return nv;
                }
                else
                {
                    cnn.Close();
                    return null;
                }
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex);
                cnn.Close();
                return null;
            }
        }

        public string changePassword(NhanVien nv, string oldPassword, string newPassword, string confirmNewPassword)
        {
            if(nv.matkhau == oldPassword)
            {
                if(newPassword == confirmNewPassword)
                {
                    cnn.Open();
                    SqlCommand scm = new SqlCommand(
                        $@"update nhanvien set 
                        matkhau='{newPassword}' 
                        where manv = '{nv.ma}'", cnn);
                    scm.ExecuteNonQuery();
                    cnn.Close();
                    return "";
                }
                else
                {
                    return "Nhập lại mật khẩu không chính xác";
                }
            }
            return "Mật khẩu hiện tại không chính xác";
        }

        public string genetareID()
        {
            Method methods = new Method();
            string result = "";

            string str_now = DateTime.Now.Year.ToString().Substring(2, 2);
            int i = 1;

            string str = str_now + methods.addZero(4, i);
            while (true)
            {
                if (getById(str) == null)
                {
                    result += str;
                    break;
                }
                else
                {
                    str = str_now + methods.addZero(4, ++i);
                }
            }

            return result;
        }
    }
}
