using System.Data;
namespace CafePOS.DAL;
public static class BanAnDAL
{
    // Mô hình ngắt kết nối: MySqlDataAdapter.Fill -> DataTable
    public static DataTable GetAll() =>
        DatabaseHelper.GetTable("SELECT MaBan, TenBan, TrangThai FROM BanAn ORDER BY MaBan");
}
