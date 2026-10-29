using System.Data;
using MySqlConnector;
namespace CafePOS.DAL;
public static class HoaDonDAL
{
    public static int? GetHoaDonMo(int maBan)
    {
        var o = DatabaseHelper.ExecuteScalar(
            "SELECT MaHoaDon FROM HoaDon WHERE MaBan=@b AND TrangThai=0 ORDER BY MaHoaDon DESC LIMIT 1",
            new MySqlParameter("@b", maBan));
        return (o == null || o == DBNull.Value) ? (int?)null : Convert.ToInt32(o);
    }

    // Nạp chi tiết hóa đơn tạm vào DataTable (MySqlDataAdapter)
    public static DataTable GetChiTiet(int maHoaDon) =>
        DatabaseHelper.GetTable(@"SELECT m.MaMon, m.TenMon AS `Tên món`, ct.SoLuong AS `Số lượng`,
                                         ct.DonGia AS `Đơn giá`, ct.SoLuong * ct.DonGia AS `Thành tiền`
                                  FROM ChiTietHoaDon ct JOIN MonAn m ON m.MaMon = ct.MaMon
                                  WHERE ct.MaHoaDon=@h",
            new MySqlParameter("@h", maHoaDon));

    // Thêm món: tạo hóa đơn nếu chưa có, cộng dồn số lượng, bàn -> "Có khách"
    public static void ThemMon(int maBan, int maMon, int soLuong)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tran = conn.BeginTransaction();
        MySqlCommand C(string sql, params MySqlParameter[] ps) => DatabaseHelper.CreateCommand(conn, tran, sql, ps);
        try
        {
            int maHD;
            var o = C("SELECT MaHoaDon FROM HoaDon WHERE MaBan=@b AND TrangThai=0 ORDER BY MaHoaDon DESC LIMIT 1",
                      new MySqlParameter("@b", maBan)).ExecuteScalar();
            if (o == null)
                maHD = (int)(long)C("INSERT INTO HoaDon(MaBan,GioVao,TongTien,TrangThai) VALUES(@b,NOW(),0,0); SELECT CAST(LAST_INSERT_ID() AS SIGNED)",
                              new MySqlParameter("@b", maBan)).ExecuteScalar();
            else
                maHD = Convert.ToInt32(o);

            var gia = C("SELECT DonGia FROM MonAn WHERE MaMon=@m", new MySqlParameter("@m", maMon)).ExecuteScalar();
            if (gia == null) throw new InvalidOperationException("Món không tồn tại.");

            int n = C("UPDATE ChiTietHoaDon SET SoLuong = SoLuong + @sl WHERE MaHoaDon=@h AND MaMon=@m",
                      new MySqlParameter("@sl", soLuong), new MySqlParameter("@h", maHD), new MySqlParameter("@m", maMon)).ExecuteNonQuery();
            if (n == 0)
                C("INSERT INTO ChiTietHoaDon(MaHoaDon,MaMon,SoLuong,DonGia) VALUES(@h,@m,@sl,@g)",
                  new MySqlParameter("@h", maHD), new MySqlParameter("@m", maMon),
                  new MySqlParameter("@sl", soLuong), new MySqlParameter("@g", (decimal)gia)).ExecuteNonQuery();

            C("UPDATE BanAn SET TrangThai='Có khách' WHERE MaBan=@b", new MySqlParameter("@b", maBan)).ExecuteNonQuery();
            tran.Commit();
        }
        catch { tran.Rollback(); throw; }
    }

    // Thanh toán: tính tiền -> cập nhật HoaDon -> bàn về "Trống" (1 MySqlTransaction)
    public static decimal ThanhToan(int maBan)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tran = conn.BeginTransaction();
        MySqlCommand C(string sql, params MySqlParameter[] ps) => DatabaseHelper.CreateCommand(conn, tran, sql, ps);
        try
        {
            var o = C("SELECT MaHoaDon FROM HoaDon WHERE MaBan=@b AND TrangThai=0 ORDER BY MaHoaDon DESC LIMIT 1",
                      new MySqlParameter("@b", maBan)).ExecuteScalar();
            if (o == null) throw new InvalidOperationException("Bàn này chưa có hóa đơn.");
            int maHD = Convert.ToInt32(o);

            decimal tong = Convert.ToDecimal(C("SELECT IFNULL(SUM(SoLuong*DonGia),0) FROM ChiTietHoaDon WHERE MaHoaDon=@h",
                                               new MySqlParameter("@h", maHD)).ExecuteScalar());

            C("UPDATE HoaDon SET GioRa=NOW(), TongTien=@t, TrangThai=1 WHERE MaHoaDon=@h",
              new MySqlParameter("@t", tong), new MySqlParameter("@h", maHD)).ExecuteNonQuery();
            C("UPDATE BanAn SET TrangThai='Trống' WHERE MaBan=@b", new MySqlParameter("@b", maBan)).ExecuteNonQuery();

            tran.Commit();
            return tong;
        }
        catch { tran.Rollback(); throw; }
    }

    // Đổi bàn: chuyển hóa đơn chưa thanh toán sang bàn trống khác
    public static void DoiBan(int banCu, int banMoi)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tran = conn.BeginTransaction();
        MySqlCommand C(string sql, params MySqlParameter[] ps) => DatabaseHelper.CreateCommand(conn, tran, sql, ps);
        try
        {
            var tt = C("SELECT TrangThai FROM BanAn WHERE MaBan=@b", new MySqlParameter("@b", banMoi)).ExecuteScalar() as string;
            if (tt != "Trống") throw new InvalidOperationException("Bàn mới phải đang ở trạng thái Trống.");

            int n = C("UPDATE HoaDon SET MaBan=@moi WHERE MaBan=@cu AND TrangThai=0",
                      new MySqlParameter("@moi", banMoi), new MySqlParameter("@cu", banCu)).ExecuteNonQuery();
            if (n == 0) throw new InvalidOperationException("Bàn cũ chưa có hóa đơn để chuyển.");

            C("UPDATE BanAn SET TrangThai='Trống' WHERE MaBan=@b", new MySqlParameter("@b", banCu)).ExecuteNonQuery();
            C("UPDATE BanAn SET TrangThai='Có khách' WHERE MaBan=@b", new MySqlParameter("@b", banMoi)).ExecuteNonQuery();
            tran.Commit();
        }
        catch { tran.Rollback(); throw; }
    }
}
