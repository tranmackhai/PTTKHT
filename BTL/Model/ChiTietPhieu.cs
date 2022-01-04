namespace BTL.Model
{
    public class ChiTietPhieu
    {
        public NguyenLieu nl;
        public int soluong = 0;

        public ChiTietPhieu()
        {
        }

        public ChiTietPhieu(NguyenLieu nl, int soluong)
        {
            this.nl = nl;
            this.soluong = soluong;
        }
    }
}
