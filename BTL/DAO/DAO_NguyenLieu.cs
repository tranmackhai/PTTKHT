using BTL.Model;
using BTL.utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BTL.DAO
{
    public class DAO_NguyenLieu
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;
        public DAO_NguyenLieu()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }
        public List<NguyenLieu> getAll()
        {
            List<NguyenLieu> result = new List<NguyenLieu>();
            cnn.Open();
            scm = new SqlCommand("select * from nguyenlieu", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                decimal giatien = reader.GetDecimal(2);
                string dvt = reader.GetString(3);
                NhaCungCap ncc = new DAO_NhaCungCap().getById(reader.GetString(4));
                NguyenLieu nl = new NguyenLieu(ma, giatien, ten, dvt, ncc);
                result.Add(nl);
            }
            cnn.Close();
            return result;
        }

        public NguyenLieu getById(string id)
        {
            cnn.Open();
            scm = new SqlCommand($"select * from nguyenlieu where manl = '{id}'", cnn);
            reader = scm.ExecuteReader();
            if(reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                decimal giatien = reader.GetDecimal(2);
                string dvt = reader.GetString(3);
                NhaCungCap ncc = new DAO_NhaCungCap().getById(reader.GetString(4));
                NguyenLieu result = new NguyenLieu(ma, giatien, ten, dvt, ncc);
                cnn.Close();
                return result;
            }
            cnn.Close();
            return null;
        }

        public List<NguyenLieu> getBySupplier(string id)
        {
            List<NguyenLieu> result = new List<NguyenLieu>();
            cnn.Open();
            scm = new SqlCommand($"select * from nguyenlieu where mancc = '{id}'", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                decimal giatien = reader.GetDecimal(2);
                string dvt = reader.GetString(3);
                NhaCungCap ncc = new DAO_NhaCungCap().getById(reader.GetString(4));
                NguyenLieu nl = new NguyenLieu(ma, giatien, ten, dvt, ncc);
                result.Add(nl);
            }
            cnn.Close();
            return result;
        }

        public int getInventory(string id)
        {
            int result = 0;
            cnn.Open();
            scm = new SqlCommand($"execute sp_TonKhoCuaNguyenLieu '{id}'", cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            cnn.Close();
            return result;
        }

        public List<NguyenLieu> searchByName(string keyword)
        {
            List<NguyenLieu> result = new List<NguyenLieu>();
            cnn.Open();
            scm = new SqlCommand($"select * from nguyenlieu where tennl like N'%{keyword}%'", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                decimal giatien = reader.GetDecimal(2);
                string dvt = reader.GetString(3);
                NhaCungCap ncc = new DAO_NhaCungCap().getById(reader.GetString(4));
                NguyenLieu nl = new NguyenLieu(ma, giatien, ten, dvt, ncc);
                result.Add(nl);
            }
            cnn.Close();
            return result;
        }

        public void updateOne(NguyenLieu nl)
        {
            cnn.Open();
            scm = new SqlCommand($@"update nguyenlieu set tennl = N'{nl.ten}', 
                    giatien = {nl.gia}, dvt = N'{nl.dvt}', mancc = '{nl.ncc.ma}' where manl = '{nl.ma}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void insertOne(NguyenLieu nl)
        {
            cnn.Open();
            scm = new SqlCommand(
                $@"insert into nguyenlieu(manl, tennl, dvt, giatien, mancc)
                            values ('{nl.ma}',N'{nl.ten}',N'{nl.dvt}',{nl.gia},'{nl.ncc.ma}')", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void deleteOne(string id)
        {
            cnn.Open();
            scm = new SqlCommand($@" delete from nguyenlieu where manl = '{id}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public string genetareID()
        {
            Method methods = new Method();
            string result = "";

            int i = 1;

            string str = methods.addZero(4, i);
            while (true)
            {
                if (getById(str) == null)
                {
                    result += str;
                    break;
                }
                else
                {
                    str = methods.addZero(4, ++i);
                }
            }

            return result;
        }
    }
}
