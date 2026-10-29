using HotelMS.DAL;
using HotelMS.DTO;
namespace HotelMS.BUS;
public static class KhachHangBUS
{
    public static KhachHangDTO TimTheoCCCD(string cccd)
    {
        if (string.IsNullOrWhiteSpace(cccd)) throw new ArgumentException("Vui lòng nhập số CCCD.");
        return KhachHangDAL.GetByCCCD(cccd.Trim());
    }
}
