# Bài 5.4: Hệ thống quản lý khách sạn (Hotel Management System)

## MỤC 1: THÔNG TIN SINH VIÊN
- **Họ và tên sinh viên:** [Điền họ tên]
- **Mã sinh viên:** [Điền MSV]
- **Tên Đề tài / Bài tập:** Bài 5.4 - Hệ thống quản lý khách sạn (Hotel Management System)

## Chức năng chính
- Check-in: tra cứu khách theo CCCD (`MySqlParameter`), khách mới tự thêm
- Gọi dịch vụ qua `ServiceDTO` -> BUS -> DAL
- Check-out: (giờ ở × giá/giờ) + tiền dịch vụ, bọc `MySqlTransaction`
- Lọc phòng theo loại / trạng thái / số phòng bằng `DataView`

## Cấu trúc thư mục (3-Tier Architecture)
```
├── Database/
│   └── ScriptDB.sql          # Script tạo CSDL HotelDB và dữ liệu mẫu
├── HotelMS/
│   ├── DTO/                  # Data Transfer Object
│   ├── DAL/                  # Data Access Layer (ADO.NET)
│   ├── BUS/                  # Business Logic Layer
│   ├── GUI/                  # WinForms (Presentation Layer)
│   ├── App.config            # Chuỗi kết nối
│   └── Program.cs
├── images/                   # Ảnh chụp màn hình dùng trong README
├── README.md
└── HotelMS.sln
```

## MỤC 2: HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN
1. Mở **XAMPP Control Panel**, bấm Start **MySQL**. Vào http://localhost/phpmyadmin, chọn tab **Import**, chọn file `Database/ScriptDB.sql`, bấm **Import/Go** để tạo CSDL và dữ liệu mẫu.
2. Mở file `HotelMS.sln` bằng Visual Studio 2022 (cần workload **.NET desktop development** và **.NET 8 SDK**). Visual Studio sẽ tự khôi phục NuGet (`MySqlConnector`, `System.Configuration.ConfigurationManager`).
3. Mở file `HotelMS/App.config`, sửa lại chuỗi kết nối `DefaultConnection` (`Server`, `Port`, `User ID`, `Password`; XAMPP mặc định: root, mật khẩu rỗng) cho phù hợp với máy cá nhân.
4. Build & Run dự án (F5).

## MỤC 3: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG
> Chạy chương trình thật, chụp màn hình, lưu vào thư mục `images/` với đúng tên file bên dưới.

1. Danh sách phòng + lọc: ![Danh sách phòng + lọc](images/01_main_screen.png)

2. Check-in: ![Check-in](images/02_crud_screen.png)

3. Gọi dịch vụ: ![Gọi dịch vụ](images/03_transaction_screen.png)

4. Hóa đơn Check-out: ![Hóa đơn Check-out](images/04_report_screen.png)

5. Báo lỗi (phòng không còn trống / thiếu thông tin): ![Báo lỗi (phòng không còn trống / thiếu thông tin)](images/05_error_handling.png)

