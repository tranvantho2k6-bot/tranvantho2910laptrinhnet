using MySqlConnector;
using InventoryMS.DTO;
namespace InventoryMS.DAL;
public static class PhieuKhoDAL
{
    // Lưu phiếu nhập/xuất trong 1 MySqlTransaction (ACID):
    // 1) Insert PhieuKho  2) Insert ChiTietPhieuKho  3) Cập nhật tồn kho  4) Commit
    public static int LuuPhieu(PhieuKhoDTO p)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var trans = conn.BeginTransaction();
        bool rolledBack = false;
        MySqlCommand C(string sql, params MySqlParameter[] ps) => DatabaseHelper.CreateCommand(conn, trans, sql, ps);
        try
        {
            decimal tong = 0;
            foreach (var ct in p.ChiTiet) tong += ct.SoLuong * ct.DonGia;

            // 1. Chèn PhieuKho
            int maPhieu = (int)(long)C(@"INSERT INTO PhieuKho(LoaiPhieu,NgayLap,MaNCC,TongTien)
                                   VALUES(@l,NOW(),@n,@t); SELECT CAST(LAST_INSERT_ID() AS SIGNED)",
                new MySqlParameter("@l", p.LoaiPhieu),
                new MySqlParameter("@n", p.MaNCC.HasValue ? (object)p.MaNCC.Value : DBNull.Value),
                new MySqlParameter("@t", tong)).ExecuteScalar();

            foreach (var ct in p.ChiTiet)
            {
                // 2. Chèn ChiTietPhieuKho
                C("INSERT INTO ChiTietPhieuKho(MaPhieu,MaVatTu,SoLuong,DonGia) VALUES(@p,@v,@s,@g)",
                  new MySqlParameter("@p", maPhieu), new MySqlParameter("@v", ct.MaVatTu),
                  new MySqlParameter("@s", ct.SoLuong), new MySqlParameter("@g", ct.DonGia)).ExecuteNonQuery();

                // 3. Cập nhật tồn kho
                int rows;
                if (p.LoaiPhieu == "NHAP")
                {
                    rows = C("UPDATE VatTu SET SoLuongTon = SoLuongTon + @s WHERE MaVatTu=@v",
                             new MySqlParameter("@s", ct.SoLuong), new MySqlParameter("@v", ct.MaVatTu)).ExecuteNonQuery();
                    if (rows == 0) throw new InvalidOperationException("Vật tư " + ct.MaVatTu + " không tồn tại.");
                }
                else
                {
                    // Điều kiện kiểm tra tồn kho nằm ngay trong câu UPDATE (SoLuongTon >= @s)
                    rows = C("UPDATE VatTu SET SoLuongTon = SoLuongTon - @s WHERE MaVatTu=@v AND SoLuongTon >= @s",
                             new MySqlParameter("@s", ct.SoLuong), new MySqlParameter("@v", ct.MaVatTu)).ExecuteNonQuery();
                    if (rows == 0)
                    {
                        trans.Rollback(); rolledBack = true;
                        throw new InvalidOperationException("Không đủ hàng trong kho (vật tư " + ct.MaVatTu + ").");
                    }
                }
            }

            // 4. Commit khi mọi bước thành công
            trans.Commit();
            return maPhieu;
        }
        catch
        {
            if (!rolledBack) { try { trans.Rollback(); } catch { } }   // chưa rollback thì rollback
            throw;
        }
    }
}
