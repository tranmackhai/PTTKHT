using System;
namespace BTL.Model
{
    public class NhanVien
    {
        public string ma, ten, gioitinh, sdt, chucvu, diachi, matkhau;
        public DateTime ngaysinh;

        public NhanVien()
        {
        }

        public NhanVien(string ma,  string ten, DateTime ngaysinh, string gioitinh, string diachi, string sdt, string chucvu,  string matkhau)
        {
            this.ma = ma;
            this.ten = ten;
            this.ngaysinh = ngaysinh;
            this.gioitinh = gioitinh;
            this.diachi = diachi;
            this.sdt = sdt;
            this.chucvu = chucvu;
            this.matkhau = matkhau;
        }
    }
}
