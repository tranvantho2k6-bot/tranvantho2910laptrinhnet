namespace HotelMS.DTO;
// Dịch vụ (Nước uống, giặt là, đồ ăn...)
public class ServiceDTO
{
    public int MaDV { get; set; }
    public string TenDV { get; set; }
    public decimal DonGia { get; set; }
    public override string ToString() => $"{TenDV} - {DonGia:N0}đ";
}
