using System.Data;
using HotelMS.DAL;
using HotelMS.DTO;
namespace HotelMS.BUS;
public static class LuuTruBUS
{
    public static DataTable GetPhieuDangO() => PhieuDAL.GetPhieuDangO();

    public static int CheckIn(KhachHangDTO kh, string soPhong)
    {
        if (string.IsNullOrWhiteSpace(soPhong)) throw new ArgumentException("Vui lòng chọn phòng trống.");
        int maKH = kh.MaKH;
        if (maKH == 0) // khách mới
        {
            if (string.IsNullOrWhiteSpace(kh.CCCD)) throw new ArgumentException("CCCD không được để trống.");
            if (string.IsNullOrWhiteSpace(kh.HoTen)) throw new ArgumentException("Họ tên khách không được để trống.");
            maKH = KhachHangDAL.Insert(kh);
        }
        return PhieuDAL.CheckIn(maKH, soPhong);
    }

    // Tổng tiền = (thời gian ở × giá phòng/giờ) + tổng tiền dịch vụ. Làm tròn lên theo giờ, tối thiểu 1 giờ.
    public static BillDTO TinhHoaDon(int maPhieu, DateTime thoiDiemTra)
    {
        var r = PhieuDAL.GetThongTinPhieu(maPhieu);
        if (r == null) throw new InvalidOperationException("Không tìm thấy phiếu đang ở.");
        var nhan = (DateTime)r["NgayNhanPhong"];
        int gio = (int)Math.Ceiling((thoiDiemTra - nhan).TotalHours);
        if (gio < 1) gio = 1;
        decimal gia = (decimal)r["GiaTheoGio"];
        return new BillDTO
        {
            MaPhieu = maPhieu,
            SoPhong = r["SoPhong"].ToString(),
            HoTen = r["HoTen"].ToString(),
            GiaTheoGio = gia,
            NgayNhan = nhan,
            NgayTra = thoiDiemTra,
            SoGio = gio,
            TienPhong = gio * gia,
            TienDichVu = DichVuDAL.TongTienDichVu(maPhieu)
        };
    }

    public static BillDTO CheckOut(int maPhieu)
    {
        var bill = TinhHoaDon(maPhieu, DateTime.Now);
        PhieuDAL.CheckOut(maPhieu, bill.SoPhong, bill.NgayTra, bill.TongTien);
        return bill;
    }
}
