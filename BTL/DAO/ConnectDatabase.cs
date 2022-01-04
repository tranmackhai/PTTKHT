using System.Data.SqlClient;

namespace BTL.DAO
{
    public class ConnectDatabase
    {
        public SqlConnection Connect()
        {
            return new SqlConnection(
                @"Data Source=CHIBISUKE\SQLEXPRESS;Initial Catalog=btl_pttkht;User ID=sa;Password=123"
            );
        }
    }
}
