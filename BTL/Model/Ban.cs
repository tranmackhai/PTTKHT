using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTL.Model
{
    public class Ban
    {
        public int soban;
        public bool trangthai;

        public Ban()
        {
        }

        public Ban(int soban, bool trangthai)
        {
            this.soban = soban;
            this.trangthai = trangthai;
        }
    }
}
