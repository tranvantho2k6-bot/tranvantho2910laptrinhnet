using System.Data;
using MySqlConnector;
namespace HotelMS.DAL;
public static class PhongDAL
{
    public static DataTable GetAll() =>
        DatabaseHelper.GetTable("SELECT SoPhong, LoaiPhong, GiaTheoGio, TrangThai FROM Phong ORDER BY SoPhong");

    public static DataTable GetPhongTrong() =>
        DatabaseHelper.GetTable("SELECT SoPhong, CONCAT(SoPhong, ' - ', LoaiPhong, ' (', CAST(GiaTheoGio AS SIGNED), 'đ/giờ)') AS MoTa FROM Phong WHERE TrangThai = 'Trống' ORDER BY SoPhong");

    public static int CapNhatTrangThai(string soPhong, string trangThai) =>
        DatabaseHelper.ExecuteNonQuery("UPDATE Phong SET TrangThai=@t WHERE SoPhong=@p",
            new MySqlParameter("@t", trangThai), new MySqlParameter("@p", soPhong));
}
