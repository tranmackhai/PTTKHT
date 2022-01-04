using BTL.Model;
using BTL.utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BTL.DAO
{
    public class DAO_NhaCungCap
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;

        public DAO_NhaCungCap()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<NhaCungCap> getAll()
        {
            List<NhaCungCap> result = new List<NhaCungCap>();
            cnn.Open();
            scm = new SqlCommand("select * from nhacungcap", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                string dc = reader.GetString(2);
                string sdt = reader.GetString(3);
                NhaCungCap ncc = new NhaCungCap(ma, ten, dc, sdt);
                result.Add(ncc);
            }
            cnn.Close();
            return result;
        }

        public void insert(NhaCungCap ncc)
        {
            cnn.Open();
            scm = new SqlCommand(
                $@"insert into nhacungcap(mancc, tenncc, diachi, sdt)
                    values ('{ncc.ma}',N'{ncc.ten}',N'{ncc.diachi}','{ncc.sdt}')
                    ", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void updateOne(NhaCungCap ncc)
        {
            cnn.Open();
            scm = new SqlCommand(
                $@"update nhacungcap set tenncc = N'{ncc.ten}',
                    diachi = N'{ncc.diachi}', sdt = '{ncc.sdt}' where
                    mancc = '{ncc.ma}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void deleteOne(string id)
        {
            cnn.Open();
            scm = new SqlCommand($@"delete from nhacungcap where mancc = '{id}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public List<NhaCungCap> searchByName(string keyword)
        {
            List<NhaCungCap> result = new List<NhaCungCap>();
            cnn.Open();
            scm = new SqlCommand($"select * from nhacungcap where tenncc like N'%{keyword}%'", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                string dc = reader.GetString(2);
                string sdt = reader.GetString(3);
                NhaCungCap ncc = new NhaCungCap(ma, ten, dc, sdt);
                result.Add(ncc);
            }
            cnn.Close();
            return result;
        }

        public NhaCungCap getById(string id)
        {
            cnn.Open();
            scm = new SqlCommand($"select * from nhacungcap where mancc = '{id}'", cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                string dc = reader.GetString(2);
                string sdt = reader.GetString(3);
                NhaCungCap result = new NhaCungCap(ma, ten, dc, sdt);
                cnn.Close();
                return result;
            }
            cnn.Close();
            return null;
        }

        public string genetareID()
        {
            Method methods = new Method();
            string result = "";

            int i = 1;

            string str = "SUP" + methods.addZero(2, i);
            while (true)
            {
                if (getById(str) == null)
                {
                    result += str;
                    break;
                }
                else
                {
                    str = "SUP" + methods.addZero(2, ++i);
                }
            }

            return result;
        }
    }
}
