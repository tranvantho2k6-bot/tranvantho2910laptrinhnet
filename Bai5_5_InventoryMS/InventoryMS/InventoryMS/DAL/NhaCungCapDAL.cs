using System.Data;
using MySqlConnector;
using InventoryMS.DTO;
namespace InventoryMS.DAL;
public static class NhaCungCapDAL
{
    public static DataTable GetAll() =>
        DatabaseHelper.GetTable("SELECT MaNCC, TenNCC, DienThoai, DiaChi FROM NhaCungCap ORDER BY TenNCC");

    public static DataTable Search(string kw) =>
        DatabaseHelper.GetTable(@"SELECT MaNCC, TenNCC, DienThoai, DiaChi FROM NhaCungCap
                                  WHERE TenNCC LIKE @k OR DienThoai LIKE @k ORDER BY TenNCC",
            new MySqlParameter("@k", "%" + kw + "%"));

    public static void Insert(NhaCungCapDTO n) =>
        DatabaseHelper.ExecuteNonQuery("INSERT INTO NhaCungCap(TenNCC,DienThoai,DiaChi) VALUES(@t,@d,@a)",
            new MySqlParameter("@t", n.TenNCC), new MySqlParameter("@d", n.DienThoai),
            new MySqlParameter("@a", (object)n.DiaChi ?? DBNull.Value));

    public static void Update(NhaCungCapDTO n) =>
        DatabaseHelper.ExecuteNonQuery("UPDATE NhaCungCap SET TenNCC=@t, DienThoai=@d, DiaChi=@a WHERE MaNCC=@m",
            new MySqlParameter("@t", n.TenNCC), new MySqlParameter("@d", n.DienThoai),
            new MySqlParameter("@a", (object)n.DiaChi ?? DBNull.Value), new MySqlParameter("@m", n.MaNCC));

    public static void Delete(int ma) =>
        DatabaseHelper.ExecuteDelete("DELETE FROM NhaCungCap WHERE MaNCC=@m", new MySqlParameter("@m", ma));
}
