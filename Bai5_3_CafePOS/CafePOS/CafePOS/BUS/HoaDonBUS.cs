using System.Data;
using CafePOS.DAL;
namespace CafePOS.BUS;
public static class HoaDonBUS
{
    public static int? GetHoaDonMo(int maBan) => HoaDonDAL.GetHoaDonMo(maBan);
    public static DataTable GetChiTiet(int maHoaDon) => HoaDonDAL.GetChiTiet(maHoaDon);

    public static void ThemMon(int maBan, int maMon, int soLuong)
    {
        if (maBan <= 0) throw new ArgumentException("Vui lòng chọn bàn trước.");
        if (maMon <= 0) throw new ArgumentException("Vui lòng chọn món.");
        if (soLuong <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0.");
        HoaDonDAL.ThemMon(maBan, maMon, soLuong);
    }

    public static decimal ThanhToan(int maBan)
    {
        if (maBan <= 0) throw new ArgumentException("Vui lòng chọn bàn cần thanh toán.");
        return HoaDonDAL.ThanhToan(maBan);
    }

    public static void DoiBan(int banCu, int banMoi)
    {
        if (banCu <= 0) throw new ArgumentException("Vui lòng chọn bàn cần đổi.");
        if (banMoi <= 0) throw new ArgumentException("Vui lòng chọn bàn mới.");
        if (banCu == banMoi) throw new ArgumentException("Bàn mới phải khác bàn hiện tại.");
        HoaDonDAL.DoiBan(banCu, banMoi);
    }
}
