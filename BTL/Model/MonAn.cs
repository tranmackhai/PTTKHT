namespace BTL.Model
{
    public class MonAn
    {
        public string ten, dvt;
        public decimal gia;
        public string mamon;
        public NhomMon nhom;

        public MonAn() { }
        public MonAn(NhomMon nhom, string mamon, string ten, string dvt, decimal gia)
        {
            this.nhom = nhom;
            this.mamon = mamon;
            this.ten = ten;
            this.dvt = dvt;
            this.gia = gia;
        }
    }
}
