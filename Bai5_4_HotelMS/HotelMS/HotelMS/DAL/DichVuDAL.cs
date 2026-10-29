using System.Data;
using MySqlConnector;
using HotelMS.DTO;
namespace HotelMS.DAL;
public static class DichVuDAL
{
    public static List<ServiceDTO> GetAll()
    {
        var list = new List<ServiceDTO>();
        foreach (DataRow r in DatabaseHelper.GetTable("SELECT MaDV, TenDV, DonGia FROM DichVu ORDER BY TenDV").Rows)
            list.Add(new ServiceDTO { MaDV = (int)r["MaDV"], TenDV = r["TenDV"].ToString(), DonGia = (decimal)r["DonGia"] });
        return list;
    }

    public static void ThemSuDung(SuDungDichVuDTO d) =>
        DatabaseHelper.ExecuteNonQuery("INSERT INTO SuDungDichVu(MaPhieu,MaDV,SoLuong,ThoiGian) VALUES(@p,@d,@s,NOW())",
            new MySqlParameter("@p", d.MaPhieu), new MySqlParameter("@d", d.MaDV), new MySqlParameter("@s", d.SoLuong));

    public static DataTable GetByPhieu(int maPhieu) =>
        DatabaseHelper.GetTable(@"SELECT d.TenDV AS `Dịch vụ`, s.SoLuong AS `Số lượng`, d.DonGia AS `Đơn giá`,
                                         s.SoLuong * d.DonGia AS `Thành tiền`, s.ThoiGian AS `Thời gian`
                                  FROM SuDungDichVu s JOIN DichVu d ON d.MaDV = s.MaDV
                                  WHERE s.MaPhieu=@p ORDER BY s.ThoiGian",
            new MySqlParameter("@p", maPhieu));

    public static decimal TongTienDichVu(int maPhieu) =>
        Convert.ToDecimal(DatabaseHelper.ExecuteScalar(
            @"SELECT IFNULL(SUM(s.SoLuong * d.DonGia), 0) FROM SuDungDichVu s
              JOIN DichVu d ON d.MaDV = s.MaDV WHERE s.MaPhieu=@p",
            new MySqlParameter("@p", maPhieu)));
}
