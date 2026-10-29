namespace HotelMS.DTO;
public class PhieuDatPhongDTO
{
    public int MaPhieu { get; set; }
    public string SoPhong { get; set; }
    public int MaKH { get; set; }
    public DateTime NgayNhanPhong { get; set; }
    public DateTime? NgayTraPhong { get; set; }
    public decimal TongTienThanhToan { get; set; }
}
public class SuDungDichVuDTO
{
    public int MaPhieu { get; set; }
    public int MaDV { get; set; }
    public int SoLuong { get; set; }
    public DateTime ThoiGian { get; set; }
}
// Dữ liệu hóa đơn check-out
public class BillDTO
{
    public int MaPhieu { get; set; }
    public string SoPhong { get; set; }
    public string HoTen { get; set; }
    public decimal GiaTheoGio { get; set; }
    public DateTime NgayNhan { get; set; }
    public DateTime NgayTra { get; set; }
    public int SoGio { get; set; }
    public decimal TienPhong { get; set; }
    public decimal TienDichVu { get; set; }
    public decimal TongTien => TienPhong + TienDichVu;
}
