using System.Data;
using HotelMS.DAL;
namespace HotelMS.BUS;
public static class PhongBUS
{
    public static DataTable GetAll() => PhongDAL.GetAll();
    public static DataTable GetPhongTrong() => PhongDAL.GetPhongTrong();
    public static void DonXong(string soPhong)
    {
        if (string.IsNullOrWhiteSpace(soPhong)) throw new ArgumentException("Hãy chọn phòng.");
        PhongDAL.CapNhatTrangThai(soPhong, "Trống");
    }
}
