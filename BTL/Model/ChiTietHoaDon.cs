using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTL.Model
{
    public class ChiTietHoaDon
    {
        public MonAn mon;
        public int soluong;

        public ChiTietHoaDon()
        {
        }

        public ChiTietHoaDon(MonAn mon, int soluong)
        {
            this.mon = mon;
            this.soluong = soluong;
        }
    }
}
