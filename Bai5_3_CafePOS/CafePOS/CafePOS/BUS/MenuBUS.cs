using System.Data;
using CafePOS.DAL;
using CafePOS.DTO;
namespace CafePOS.BUS;
public static class MenuBUS
{
    public static DataTable GetDanhMuc() => MenuDAL.GetDanhMuc();
    public static DataTable GetMonAn() => MenuDAL.GetMonAn();

    private static void CheckTenDM(string ten)
    {
        if (string.IsNullOrWhiteSpace(ten)) throw new ArgumentException("Tên danh mục không được để trống.");
    }
    private static void ValidateMon(MonAnDTO m)
    {
        if (string.IsNullOrWhiteSpace(m.TenMon)) throw new ArgumentException("Tên món không được để trống.");
        if (m.DonGia < 0) throw new ArgumentException("Đơn giá không được âm.");
        if (m.MaDanhMuc <= 0) throw new ArgumentException("Vui lòng chọn danh mục.");
    }

    public static void ThemDanhMuc(string ten) { CheckTenDM(ten); MenuDAL.InsertDanhMuc(ten.Trim()); }
    public static void SuaDanhMuc(int ma, string ten)
    {
        if (ma <= 0) throw new ArgumentException("Hãy chọn danh mục cần sửa.");
        CheckTenDM(ten); MenuDAL.UpdateDanhMuc(ma, ten.Trim());
    }
    public static void XoaDanhMuc(int ma)
    {
        if (ma <= 0) throw new ArgumentException("Hãy chọn danh mục cần xóa.");
        MenuDAL.DeleteDanhMuc(ma);
    }

    public static void ThemMon(MonAnDTO m) { ValidateMon(m); m.TenMon = m.TenMon.Trim(); MenuDAL.InsertMon(m); }
    public static void SuaMon(MonAnDTO m)
    {
        if (m.MaMon <= 0) throw new ArgumentException("Hãy chọn món cần sửa.");
        ValidateMon(m); m.TenMon = m.TenMon.Trim(); MenuDAL.UpdateMon(m);
    }
    public static void XoaMon(int ma)
    {
        if (ma <= 0) throw new ArgumentException("Hãy chọn món cần xóa.");
        MenuDAL.DeleteMon(ma);
    }
}
