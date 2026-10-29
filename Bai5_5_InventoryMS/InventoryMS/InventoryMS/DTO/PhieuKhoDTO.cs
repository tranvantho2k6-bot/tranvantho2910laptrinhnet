namespace InventoryMS.DTO;
public class ChiTietPhieuKhoDTO
{
    public int MaPhieu { get; set; }
    public string MaVatTu { get; set; }
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
}
public class PhieuKhoDTO
{
    public int MaPhieu { get; set; }
    public string LoaiPhieu { get; set; }      // "NHAP" hoặc "XUAT"
    public DateTime NgayLap { get; set; }
    public int? MaNCC { get; set; }            // null nếu là phiếu xuất
    public decimal TongTien { get; set; }
    public List<ChiTietPhieuKhoDTO> ChiTiet { get; set; } = new List<ChiTietPhieuKhoDTO>();
}
