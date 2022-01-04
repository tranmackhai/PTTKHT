using BTL.Model;
using BTL.utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BTL.DAO
{
    public class DAO_Ban
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;

        public DAO_Ban()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<Ban> getAll()
        {
            List<Ban> result = new List<Ban>();
            cnn.Open();
            scm = new SqlCommand("select * from ban", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                Ban ban = new Ban(reader.GetInt32(0), reader.GetBoolean(1));
                result.Add(ban);
            }
            cnn.Close();
            return result;
        }

        public Ban getById(int id)
        {
            cnn.Open();
            scm = new SqlCommand($"select * from ban where soban = {id}", cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                
                Ban result = new Ban(reader.GetInt32(0), reader.GetBoolean(1));
                cnn.Close();
                return result;
            }
            cnn.Close();
            return null;
        }

        public void insert(Ban ban)
        {
            cnn.Open();
            int status = (ban.trangthai == true) ? 1 : 0;
            scm = new SqlCommand($"insert into ban (soban, trangthai) values( {ban.soban} , {status})", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void updateOne(Ban ban)
        {
            cnn.Open();
            int status = (ban.trangthai == true) ? 1 : 0;
            scm = new SqlCommand($"update ban set trangthai = {status} where soban = {ban.soban}", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void deleteOne(int soban)
        {
            cnn.Open();
            scm = new SqlCommand($"delete from ban where soban = {soban}", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public List<Ban> searchByStatus(bool status)
        {
            List<Ban> result = new List<Ban>();
            
            cnn.Open();
            scm = new SqlCommand($"select * from ban where trangthai = {(status == true ? 1 : 0)}", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                Ban ban = new Ban(reader.GetInt32(0), reader.GetBoolean(1));
                result.Add(ban);
            }
            cnn.Close();
            return result;
        }

        public HoaDon getInvoice(int soban)
        {
            DAO_HoaDon dao_hd = new DAO_HoaDon();
            cnn.Open();
            scm = new SqlCommand($@"
                select sohd 
                from hoadon
                where soban = {soban} and giovao = giora", cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                HoaDon hd = dao_hd.getById(reader.GetString(0));
                cnn.Close();
                return hd;
            }
            cnn.Close();
            return null;
        }

        public int generateID()
        {
            int i = 1;
            while (true)
            {
                if (getById(i) == null)
                {
                    return i;
                }
                i++;
            }
        }
    }
}
