using InventoryMS.DAL;
using InventoryMS.DTO;
namespace InventoryMS.BUS;
public static class PhieuKhoBUS
{
    public static int LuuPhieu(PhieuKhoDTO p)
    {
        if (p.LoaiPhieu != "NHAP" && p.LoaiPhieu != "XUAT") throw new ArgumentException("Loại phiếu phải là NHAP hoặc XUAT.");
        if (p.LoaiPhieu == "NHAP" && !p.MaNCC.HasValue) throw new ArgumentException("Phiếu nhập phải chọn nhà cung cấp.");
        if (p.LoaiPhieu == "XUAT") p.MaNCC = null;
        if (p.ChiTiet == null || p.ChiTiet.Count == 0) throw new ArgumentException("Phiếu chưa có vật tư nào.");
        foreach (var ct in p.ChiTiet)
        {
            if (ct.SoLuong <= 0) throw new ArgumentException("Số lượng của " + ct.MaVatTu + " phải lớn hơn 0.");
            if (ct.DonGia < 0) throw new ArgumentException("Đơn giá của " + ct.MaVatTu + " không được âm.");
        }
        return PhieuKhoDAL.LuuPhieu(p);
    }
}
