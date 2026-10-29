using MySqlConnector;
using HotelMS.DTO;
namespace HotelMS.DAL;
public static class KhachHangDAL
{
    // Tra cứu theo CCCD bằng MySqlParameter (chống SQL Injection)
    public static KhachHangDTO GetByCCCD(string cccd)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand("SELECT MaKH, HoTen, CCCD, SoDienThoai FROM KhachHang WHERE CCCD=@c", conn);
        cmd.Parameters.AddWithValue("@c", cccd);
        using var rd = cmd.ExecuteReader();
        if (!rd.Read()) return null;
        return new KhachHangDTO
        {
            MaKH = rd.GetInt32(0),
            HoTen = rd.GetString(1),
            CCCD = rd.GetString(2),
            SoDienThoai = rd.IsDBNull(3) ? "" : rd.GetString(3)
        };
    }

    public static int Insert(KhachHangDTO kh)
    {
        var o = DatabaseHelper.ExecuteScalar(
            "INSERT INTO KhachHang(HoTen,CCCD,SoDienThoai) VALUES(@h,@c,@s); SELECT CAST(LAST_INSERT_ID() AS SIGNED)",
            new MySqlParameter("@h", kh.HoTen), new MySqlParameter("@c", kh.CCCD),
            new MySqlParameter("@s", (object)kh.SoDienThoai ?? DBNull.Value));
        return Convert.ToInt32(o);
    }
}
