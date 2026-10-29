using System.Data;
using InventoryMS.DAL;
using InventoryMS.DTO;
namespace InventoryMS.BUS;
public static class NhaCungCapBUS
{
    public static DataTable GetAll() => NhaCungCapDAL.GetAll();
    public static DataTable Search(string kw) => string.IsNullOrWhiteSpace(kw) ? NhaCungCapDAL.GetAll() : NhaCungCapDAL.Search(kw.Trim());

    private static void Validate(NhaCungCapDTO n)
    {
        if (string.IsNullOrWhiteSpace(n.TenNCC)) throw new ArgumentException("Tên nhà cung cấp không được để trống.");
        if (string.IsNullOrWhiteSpace(n.DienThoai)) throw new ArgumentException("Số điện thoại không được để trống.");
        n.TenNCC = n.TenNCC.Trim(); n.DienThoai = n.DienThoai.Trim();
    }
    public static void Them(NhaCungCapDTO n) { Validate(n); NhaCungCapDAL.Insert(n); }
    public static void Sua(NhaCungCapDTO n)
    {
        if (n.MaNCC <= 0) throw new ArgumentException("Hãy chọn nhà cung cấp cần sửa.");
        Validate(n); NhaCungCapDAL.Update(n);
    }
    public static void Xoa(int ma)
    {
        if (ma <= 0) throw new ArgumentException("Hãy chọn nhà cung cấp cần xóa.");
        NhaCungCapDAL.Delete(ma);
    }
}
