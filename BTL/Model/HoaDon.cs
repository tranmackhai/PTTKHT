using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTL.Model
{
    public class HoaDon
    {
        public string sohd;
        public DateTime giovao, giora;
        public NhanVien nv;
        public Ban ban;
        public List<ChiTietHoaDon> ds_mon = new List<ChiTietHoaDon>();
        public HoaDon()
        {
        }

        public HoaDon(string sohd, DateTime giovao, DateTime giora, NhanVien nv, Ban ban, List<ChiTietHoaDon> ds_mon)
        {
            this.sohd = sohd;
            this.giovao = giovao;
            this.giora = giora;
            this.nv = nv;
            this.ban = ban;
            this.ds_mon = ds_mon;
        }
        public int getTotal()
        {
            int result = 0;
            ds_mon.ForEach(ct =>
            {
                result += ct.soluong;
            });
            return result;
        }
    }
}
