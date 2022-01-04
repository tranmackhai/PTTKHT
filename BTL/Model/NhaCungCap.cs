namespace BTL.Model
{
    public class NhaCungCap
    {
        public string ma;
        public string ten, diachi, sdt;

        public NhaCungCap()
        {
        }

        public NhaCungCap(string ma, string ten, string diachi, string sdt)
        {
            this.ma = ma;
            this.ten = ten;
            this.diachi = diachi;
            this.sdt = sdt;
        }
    }
}
