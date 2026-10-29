using System.Data;
using MySqlConnector;
using InventoryMS.DTO;
namespace InventoryMS.DAL;
public static class VatTuDAL
{
    public static DataTable GetAll() =>
        DatabaseHelper.GetTable("SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu ORDER BY MaVatTu");

    // Dùng cho ComboBox chọn vật tư khi lập phiếu
    public static DataTable GetForCombo() =>
        DatabaseHelper.GetTable("SELECT MaVatTu, TenVatTu, CONCAT(MaVatTu, ' - ', TenVatTu, '  (tồn: ', CAST(SoLuongTon AS CHAR), ' ', IFNULL(DVT, ''), ')') AS MoTa FROM VatTu ORDER BY MaVatTu");

    // Tìm theo mã hoặc tên - MySqlParameter chống SQL Injection
    public static DataTable Search(string kw) =>
        DatabaseHelper.GetTable(@"SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu
                                  WHERE MaVatTu LIKE @k OR TenVatTu LIKE @k ORDER BY MaVatTu",
            new MySqlParameter("@k", "%" + kw + "%"));

    public static void Insert(VatTuDTO v) =>
        DatabaseHelper.ExecuteNonQuery("INSERT INTO VatTu(MaVatTu,TenVatTu,DVT,SoLuongTon) VALUES(@m,@t,@d,@s)",
            new MySqlParameter("@m", v.MaVatTu), new MySqlParameter("@t", v.TenVatTu),
            new MySqlParameter("@d", (object)v.DVT ?? DBNull.Value), new MySqlParameter("@s", v.SoLuongTon));

    // Sửa thông tin: không sửa tồn kho trực tiếp (tồn kho chỉ biến động qua phiếu kho)
    public static int Update(VatTuDTO v) =>
        DatabaseHelper.ExecuteNonQuery("UPDATE VatTu SET TenVatTu=@t, DVT=@d WHERE MaVatTu=@m",
            new MySqlParameter("@t", v.TenVatTu), new MySqlParameter("@d", (object)v.DVT ?? DBNull.Value),
            new MySqlParameter("@m", v.MaVatTu));

    public static int Delete(string ma) =>
        DatabaseHelper.ExecuteDelete("DELETE FROM VatTu WHERE MaVatTu=@m", new MySqlParameter("@m", ma));

    // Vật tư dưới ngưỡng an toàn
    public static DataTable GetTonThap(decimal nguong) =>
        DatabaseHelper.GetTable(@"SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu
                                  WHERE SoLuongTon < @n ORDER BY SoLuongTon",
            new MySqlParameter("@n", nguong));

    // ExecuteScalar: tổng giá trị tồn = Σ (tồn × đơn giá nhập bình quân)
    public static decimal TongGiaTriTon()
    {
        var o = DatabaseHelper.ExecuteScalar(@"
            SELECT IFNULL(SUM(v.SoLuongTon * IFNULL(g.GiaBQ, 0)), 0)
            FROM VatTu v
            LEFT JOIN (SELECT ct.MaVatTu, AVG(ct.DonGia) AS GiaBQ
                       FROM ChiTietPhieuKho ct JOIN PhieuKho pk ON pk.MaPhieu = ct.MaPhieu
                       WHERE pk.LoaiPhieu = 'NHAP' GROUP BY ct.MaVatTu) g ON g.MaVatTu = v.MaVatTu");
        return Convert.ToDecimal(o);
    }
}
