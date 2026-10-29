using System.Configuration;
using System.Data;
using MySqlConnector;
namespace HotelMS.DAL;

public static class DatabaseHelper
{
    // Chuỗi kết nối đọc từ App.config (khóa "DefaultConnection"). Sửa lại cho phù hợp máy cá nhân.
    public static string ConnectionString
    {
        get
        {
            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            return cs != null ? cs.ConnectionString
                : @"Server=localhost;Port=3306;Database=HotelDB;User ID=root;Password=;CharSet=utf8mb4;SslMode=None;";
        }
    }

    public static MySqlConnection GetConnection() => new MySqlConnection(ConnectionString);

    // Mô hình ngắt kết nối: MySqlDataAdapter.Fill -> DataTable
    public static DataTable GetTable(string sql, params MySqlParameter[] ps)
    {
        using var conn = GetConnection();
        using var da = new MySqlDataAdapter(sql, conn);
        if (ps != null && ps.Length > 0) da.SelectCommand.Parameters.AddRange(ps);
        var dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    public static int ExecuteNonQuery(string sql, params MySqlParameter[] ps)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand(sql, conn);
        if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
        return cmd.ExecuteNonQuery();
    }

    public static object ExecuteScalar(string sql, params MySqlParameter[] ps)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand(sql, conn);
        if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
        return cmd.ExecuteScalar();
    }

    public static int ExecuteDelete(string sql, params MySqlParameter[] ps)
    {
        try { return ExecuteNonQuery(sql, ps); }
        catch (MySqlException ex) when ((ex.Number == 1451 || ex.Number == 1452))
        {
            throw new InvalidOperationException("Không thể xóa vì dữ liệu đang được sử dụng ở nơi khác.");
        }
    }

    public static MySqlCommand CreateCommand(MySqlConnection conn, MySqlTransaction tran, string sql, params MySqlParameter[] ps)
    {
        var cmd = new MySqlCommand(sql, conn, tran);
        if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
        return cmd;
    }
}
