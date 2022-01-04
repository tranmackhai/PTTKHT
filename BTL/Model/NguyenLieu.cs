namespace BTL.Model
{
    public class NguyenLieu
    {
        public string ma;
        public decimal gia;
        public string ten, dvt;
        public NhaCungCap ncc;

        public NguyenLieu()
        {
        }

        public NguyenLieu(string ma, decimal gia, string ten, string dvt, NhaCungCap ncc)
        {
            this.ma = ma;
            this.gia = gia;
            this.ten = ten;
            this.dvt = dvt;
            this.ncc = ncc;
        }
    }
}
