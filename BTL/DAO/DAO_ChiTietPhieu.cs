using BTL.Model;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BTL.DAO
{
    public class DAO_ChiTietPhieu
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;
        public DAO_ChiTietPhieu()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<ChiTietPhieu> getById(bool type, string id)
        {
            List<ChiTietPhieu> result = new List<ChiTietPhieu>();
            string query = (type == true) ?$@"select manl, soluong from chitietphieunhap where sopn = '{id}'":$@"
                select manl, soluong from chitietphieuxuat where sopx = '{id}'";
            cnn.Open();
            scm = new SqlCommand(query, cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                ChiTietPhieu ctp = new ChiTietPhieu(new DAO_NguyenLieu().getById(reader.GetString(0)), reader.GetInt32(1));
                result.Add(ctp);
            }
            cnn.Close();
            return result;
        }

        public void insert(bool type, string sophieu, ChiTietPhieu ct)
        {
            cnn.Open();
            string query = (type == true) ?
                $@"insert into chitietphieunhap(sopn, manl, soluong) values ('{sophieu}','{ct.nl.ma}',{ct.soluong})" :
                $@"insert into chitietphieuxuat(sopx, manl, soluong) values ('{sophieu}','{ct.nl.ma}',{ct.soluong})";
            scm = new SqlCommand(query, cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void updateOne(bool type, string sophieu, ChiTietPhieu ct)
        {
            cnn.Open();
            string query = (type == true) ?
                $@"update chitietphieunhap set soluong = {ct.soluong} where sopn = '{sophieu}' and manl = '{ct.nl.ma}'" :
                $@"update chitietphieuxuat set soluong = {ct.soluong} where sopx = '{sophieu}' and manl = '{ct.nl.ma}'";
            scm = new SqlCommand(query, cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void deleteOne(bool type, string sophieu, int manl)
        {
            cnn.Open();
            string query = (type == true) ? $@"delete from chitietphieunhap where sopn = '{sophieu}' and manl = '{manl}'" :
                 $@"delete from chitietphieuxuat where sopx = '{sophieu}' and manl = '{manl}'";
            scm = new SqlCommand(query, cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public int getQuantity(bool type, string sophieu, string manl)
        {
            int result = 0;
            cnn.Open();
            string query = (type == true) ? $@"select soluong from chitietphieunhap where sopn = '{sophieu}' and manl = '{manl}'" :
                 $@"select soluong from chitietphieuxuat where sopx = '{sophieu}' and manl = '{manl}'";
            scm = new SqlCommand(query, cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            cnn.Close();
            return result;
        }
    }
}
