using BTL.Model;
using BTL.utils;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace BTL.DAO
{
    public class DAO_MonAn
    {
        private SqlConnection cnn;
        private SqlCommand scm;
        private SqlDataReader reader;

        public DAO_MonAn()
        {
            ConnectDatabase ConnectDB = new ConnectDatabase();
            cnn = ConnectDB.Connect();
        }

        public List<MonAn> getAll()
        {
            List<MonAn> result = new List<MonAn>();
            cnn.Open();
            scm = new SqlCommand($@"select b.manhom, b.tennhom, a.mamon, a.tenmon, a.dvt, a.giatien 
                    from monan a, nhommon b where a.manhom = b.manhom", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string manhom = reader.GetString(0);
                string tennhom = reader.GetString(1);
                string mamon = reader.GetString(2);
                string tenmon = reader.GetString(3);
                string dvt = reader.GetString(4);
                decimal giaban = reader.GetDecimal(5);
                NhomMon _nhom = new NhomMon(manhom, tennhom);
                MonAn mon = new MonAn(
                    _nhom,
                    mamon,
                    tenmon,
                    dvt,
                    giaban
                );
                result.Add(mon);
            }
            cnn.Close();
            return result;
        }

        public List<MonAn> getByGroup(string _manhom)
        {
            List<MonAn> result = new List<MonAn>();
            cnn.Open();
            scm = new SqlCommand($@"select b.manhom,b.tennhom,a.mamon,a.tenmon,a.dvt,a.giatien from monan a, nhommon b 
                where a.manhom = b.manhom and a.manhom = '{_manhom}'", cnn);
            reader = scm.ExecuteReader();
            while (reader.Read())
            {
                string manhom = reader.GetString(0);
                string tennhom = reader.GetString(1);
                string mamon = reader.GetString(2);
                string tenmon = reader.GetString(3);
                string dvt = reader.GetString(4);
                decimal giaban = reader.GetDecimal(5);
                NhomMon _nhom = new NhomMon(manhom, tennhom);
                MonAn mon = new MonAn(
                    _nhom,
                    mamon,
                    tenmon,
                    dvt,
                    giaban
                );
                result.Add(mon);
            }
            cnn.Close();
            return result;
        }

        public MonAn getById(string id)
        {
            cnn.Open();
            scm = new SqlCommand($@"select b.manhom, b.tennhom, a.mamon, a.tenmon, a.dvt, a.giatien 
                    from monan a, nhommon b where a.manhom = b.manhom and a.mamon = '{id}'", cnn);
            reader = scm.ExecuteReader();
            if (reader.Read())
            {
                string manhom = reader.GetString(0);
                string tennhom = reader.GetString(1);
                string mamon = reader.GetString(2);
                string tenmon = reader.GetString(3);
                string dvt = reader.GetString(4);
                decimal giaban = reader.GetDecimal(5);
                NhomMon _nhom = new NhomMon(manhom, tennhom);
                MonAn result = new MonAn(
                    _nhom,
                    mamon,
                    tenmon,
                    dvt,
                    giaban
                );
                cnn.Close();
                return result;
            }
            cnn.Close();
            return null;
        }

        public void insertOne(MonAn mon)
        {
            cnn.Open();
            scm = new SqlCommand(
                $@"insert into monan (mamon, tenmon, dvt, giatien, manhom) 
                values('{mon.mamon}', N'{mon.ten}', N'{mon.dvt}', {mon.gia},'{mon.nhom.ma}')", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void updateOne(MonAn mon)
        {
            cnn.Open();
            scm = new SqlCommand(
                $@"update monan set  tenmon = N'{mon.ten}', dvt=N'{mon.dvt}', giatien={mon.gia},
                manhom='{mon.nhom.ma}' where mamon = '{mon.mamon}'" , cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public void deleteOne(string mamon)
        {
            cnn.Open();
            scm = new SqlCommand($"delete from monan where mamon = '{mamon}'", cnn);
            scm.ExecuteNonQuery();
            cnn.Close();
        }

        public List<MonAn> searchByName(string keyword)
        {
            List<MonAn> result = new List<MonAn>();
            cnn.Open();
            scm = new SqlCommand(
                $@"select  b.manhom,  b.tennhom, a.mamon, a.tenmon, a.dvt, a.giatien from monan a, nhommon b
                where a.manhom = b.manhom and a.tenmon like N'%{keyword}%'", cnn);
            reader = scm.ExecuteReader();
            
            while (reader.Read())
            {
                string manhom = reader.GetString(0);
                string tennhom = reader.GetString(1);
                string mamon = reader.GetString(2);
                string tenmon = reader.GetString(3);
                string dvt = reader.GetString(4);
                decimal giaban = reader.GetDecimal(5);

                NhomMon _nhom = new NhomMon(manhom, tennhom);
                MonAn mon = new MonAn(
                    _nhom,
                    mamon,
                    tenmon,
                    dvt,
                    giaban
                );
                result.Add(mon);
            }
            cnn.Close();
            return result;
        }
        public string genetareID()
        {
            Method methods = new Method();
            string result = "";

            int i = 1;

            string str = "P" + methods.addZero(3, i);
            while (true)
            {
                if (getById(str) == null)
                {
                    result += str;
                    break;
                }
                else
                {
                    str = "P" + methods.addZero(3, ++i);
                }
            }

            return result;
        }
    }
}
