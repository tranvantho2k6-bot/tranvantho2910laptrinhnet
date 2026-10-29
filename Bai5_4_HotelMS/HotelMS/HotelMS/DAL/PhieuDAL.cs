using System.Data;
using MySqlConnector;
namespace HotelMS.DAL;
public static class PhieuDAL
{
    // Check-in: chuyển phòng sang "Đang ở" + lập phiếu (1 transaction)
    public static int CheckIn(int maKH, string soPhong)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tran = conn.BeginTransaction();
        MySqlCommand C(string sql, params MySqlParameter[] ps) => DatabaseHelper.CreateCommand(conn, tran, sql, ps);
        try
        {
            int n = C("UPDATE Phong SET TrangThai='Đang ở' WHERE SoPhong=@p AND TrangThai='Trống'",
                      new MySqlParameter("@p", soPhong)).ExecuteNonQuery();
            if (n == 0) throw new InvalidOperationException("Phòng không còn trống.");

            int ma = (int)(long)C(@"INSERT INTO PhieuDatPhong(SoPhong,MaKH,NgayNhanPhong,TongTienThanhToan)
                              VALUES(@p,@k,NOW(),0); SELECT CAST(LAST_INSERT_ID() AS SIGNED)",
                            new MySqlParameter("@p", soPhong), new MySqlParameter("@k", maKH)).ExecuteScalar();
            tran.Commit();
            return ma;
        }
        catch { tran.Rollback(); throw; }
    }

    // Danh sách phiếu đang ở (chưa trả phòng)
    public static DataTable GetPhieuDangO() =>
        DatabaseHelper.GetTable(@"SELECT p.MaPhieu, p.SoPhong, k.HoTen, p.NgayNhanPhong, CONCAT(p.SoPhong, ' - ', k.HoTen) AS MoTa FROM PhieuDatPhong p JOIN KhachHang k ON k.MaKH = p.MaKH WHERE p.NgayTraPhong IS NULL ORDER BY p.SoPhong");

    public static DataRow GetThongTinPhieu(int maPhieu)
    {
        var dt = DatabaseHelper.GetTable(@"SELECT p.MaPhieu, p.SoPhong, k.HoTen, ph.GiaTheoGio, p.NgayNhanPhong
                                           FROM PhieuDatPhong p
                                           JOIN KhachHang k ON k.MaKH = p.MaKH
                                           JOIN Phong ph ON ph.SoPhong = p.SoPhong
                                           WHERE p.MaPhieu=@m AND p.NgayTraPhong IS NULL",
            new MySqlParameter("@m", maPhieu));
        return dt.Rows.Count == 0 ? null : dt.Rows[0];
    }

    // Check-out: lưu tổng tiền + ngày trả + phòng -> "Đang dọn dẹp" (MySqlTransaction)
    public static void CheckOut(int maPhieu, string soPhong, DateTime ngayTra, decimal tong)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tran = conn.BeginTransaction();
        MySqlCommand C(string sql, params MySqlParameter[] ps) => DatabaseHelper.CreateCommand(conn, tran, sql, ps);
        try
        {
            int n = C("UPDATE PhieuDatPhong SET NgayTraPhong=@nt, TongTienThanhToan=@t WHERE MaPhieu=@m AND NgayTraPhong IS NULL",
                      new MySqlParameter("@nt", ngayTra), new MySqlParameter("@t", tong), new MySqlParameter("@m", maPhieu)).ExecuteNonQuery();
            if (n == 0) throw new InvalidOperationException("Phiếu đã được thanh toán trước đó.");
            C("UPDATE Phong SET TrangThai='Đang dọn dẹp' WHERE SoPhong=@p", new MySqlParameter("@p", soPhong)).ExecuteNonQuery();
            tran.Commit();
        }
        catch { tran.Rollback(); throw; }
    }
}
