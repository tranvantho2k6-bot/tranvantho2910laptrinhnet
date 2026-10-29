using System.Data;
using MySqlConnector;
using CafePOS.DTO;
namespace CafePOS.DAL;
public static class MenuDAL
{
    public static DataTable GetDanhMuc() =>
        DatabaseHelper.GetTable("SELECT MaDanhMuc, TenDanhMuc FROM DanhMuc ORDER BY TenDanhMuc");

    public static DataTable GetMonAn() =>
        DatabaseHelper.GetTable(@"SELECT m.MaMon, m.TenMon, m.MaDanhMuc, d.TenDanhMuc, m.DonGia
                                  FROM MonAn m JOIN DanhMuc d ON d.MaDanhMuc = m.MaDanhMuc
                                  ORDER BY d.TenDanhMuc, m.TenMon");

    public static void InsertDanhMuc(string ten) =>
        DatabaseHelper.ExecuteNonQuery("INSERT INTO DanhMuc(TenDanhMuc) VALUES(@t)", new MySqlParameter("@t", ten));

    public static void UpdateDanhMuc(int ma, string ten) =>
        DatabaseHelper.ExecuteNonQuery("UPDATE DanhMuc SET TenDanhMuc=@t WHERE MaDanhMuc=@m",
            new MySqlParameter("@t", ten), new MySqlParameter("@m", ma));

    public static void DeleteDanhMuc(int ma) =>
        DatabaseHelper.ExecuteDelete("DELETE FROM DanhMuc WHERE MaDanhMuc=@m", new MySqlParameter("@m", ma));

    public static void InsertMon(MonAnDTO m) =>
        DatabaseHelper.ExecuteNonQuery("INSERT INTO MonAn(TenMon,MaDanhMuc,DonGia) VALUES(@t,@d,@g)",
            new MySqlParameter("@t", m.TenMon), new MySqlParameter("@d", m.MaDanhMuc), new MySqlParameter("@g", m.DonGia));

    public static void UpdateMon(MonAnDTO m) =>
        DatabaseHelper.ExecuteNonQuery("UPDATE MonAn SET TenMon=@t, MaDanhMuc=@d, DonGia=@g WHERE MaMon=@m",
            new MySqlParameter("@t", m.TenMon), new MySqlParameter("@d", m.MaDanhMuc),
            new MySqlParameter("@g", m.DonGia), new MySqlParameter("@m", m.MaMon));

    public static void DeleteMon(int ma) =>
        DatabaseHelper.ExecuteDelete("DELETE FROM MonAn WHERE MaMon=@m", new MySqlParameter("@m", ma));
}
