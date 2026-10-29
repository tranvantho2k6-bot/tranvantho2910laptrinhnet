using System.Data;
using HotelMS.DAL;
using HotelMS.DTO;
namespace HotelMS.BUS;
public static class DichVuBUS
{
    public static List<ServiceDTO> GetAll() => DichVuDAL.GetAll();
    public static DataTable GetByPhieu(int maPhieu) => DichVuDAL.GetByPhieu(maPhieu);

    public static void GoiDichVu(SuDungDichVuDTO d)
    {
        if (d.MaPhieu <= 0) throw new ArgumentException("Vui lòng chọn phòng đang ở.");
        if (d.MaDV <= 0) throw new ArgumentException("Vui lòng chọn dịch vụ.");
        if (d.SoLuong <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0.");
        DichVuDAL.ThemSuDung(d);
    }
}
