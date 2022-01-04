using System;
using System.Collections.Generic;

namespace BTL.Model
{
    public class Phieu
    {
        public string sophieu;
        public NhanVien nv;
        public DateTime ngay;
        public List<ChiTietPhieu> list = new List<ChiTietPhieu>();

        public Phieu()
        {
        }

        public Phieu(string s, DateTime d, NhanVien x, List<ChiTietPhieu> list)
        {
            sophieu = s;
            ngay = d;
            nv = x;
            this.list = list;
        }

        public decimal getTotalPrice()
        {
            decimal result = 0;
            list.ForEach(item =>
            {
                result += (item.soluong * item.nl.gia);
            });
            return result;
        }
    }
}
