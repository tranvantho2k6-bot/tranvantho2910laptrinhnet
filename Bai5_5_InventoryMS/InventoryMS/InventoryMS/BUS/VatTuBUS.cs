using System.Data;
using InventoryMS.DAL;
using InventoryMS.DTO;
namespace InventoryMS.BUS;
public static class VatTuBUS
{
    public static DataTable GetAll() => VatTuDAL.GetAll();
    public static DataTable GetForCombo() => VatTuDAL.GetForCombo();
    public static DataTable Search(string kw) => string.IsNullOrWhiteSpace(kw) ? VatTuDAL.GetAll() : VatTuDAL.Search(kw.Trim());

    private static void Validate(VatTuDTO v)
    {
        if (string.IsNullOrWhiteSpace(v.MaVatTu)) throw new ArgumentException("Mã vật tư không được để trống.");
        if (string.IsNullOrWhiteSpace(v.TenVatTu)) throw new ArgumentException("Tên vật tư không được để trống.");
        if (v.SoLuongTon < 0) throw new ArgumentException("Số lượng tồn không được âm.");
        v.MaVatTu = v.MaVatTu.Trim().ToUpper(); v.TenVatTu = v.TenVatTu.Trim();
    }
    public static void Them(VatTuDTO v) { Validate(v); VatTuDAL.Insert(v); }
    public static void Sua(VatTuDTO v)
    {
        Validate(v);
        if (VatTuDAL.Update(v) == 0) throw new InvalidOperationException("Không tìm thấy vật tư để sửa.");
    }
    public static void Xoa(string ma)
    {
        if (string.IsNullOrWhiteSpace(ma)) throw new ArgumentException("Hãy chọn vật tư cần xóa.");
        VatTuDAL.Delete(ma);
    }
    public static DataTable GetTonThap(decimal nguong)
    {
        if (nguong < 0) throw new ArgumentException("Ngưỡng không được âm.");
        return VatTuDAL.GetTonThap(nguong);
    }
    public static decimal TongGiaTriTon() => VatTuDAL.TongGiaTriTon();
}
