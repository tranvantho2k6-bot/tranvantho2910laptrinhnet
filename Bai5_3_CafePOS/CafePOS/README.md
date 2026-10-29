# Bài 5.3: Hệ thống quản lý quán Cafe POS

## MỤC 1: THÔNG TIN SINH VIÊN
- **Họ và tên sinh viên:** [Điền họ tên]
- **Mã sinh viên:** [Điền MSV]
- **Tên Đề tài / Bài tập:** Bài 5.3 - Hệ thống quản lý quán Cafe POS

## Chức năng chính
- Sơ đồ bàn dạng Button động (xanh = Trống, đỏ = Có khách)
- Gọi món, cộng dồn số lượng, tự chuyển bàn sang "Có khách"
- Đổi bàn
- Thanh toán bằng `MySqlTransaction`
- Quản lý danh mục & thực đơn (CRUD 3-Tier, validation giá âm / tên rỗng)
- Mô hình ngắt kết nối: `MySqlDataAdapter` + `DataTable`

## Cấu trúc thư mục (3-Tier Architecture)
```
├── Database/
│   └── ScriptDB.sql          # Script tạo CSDL CafePOSDB và dữ liệu mẫu
├── CafePOS/
│   ├── DTO/                  # Data Transfer Object
│   ├── DAL/                  # Data Access Layer (ADO.NET)
│   ├── BUS/                  # Business Logic Layer
│   ├── GUI/                  # WinForms (Presentation Layer)
│   ├── App.config            # Chuỗi kết nối
│   └── Program.cs
├── images/                   # Ảnh chụp màn hình dùng trong README
├── README.md
└── CafePOS.sln
```

## MỤC 2: HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN
1. Mở **XAMPP Control Panel**, bấm Start **MySQL**. Vào http://localhost/phpmyadmin, chọn tab **Import**, chọn file `Database/ScriptDB.sql`, bấm **Import/Go** để tạo CSDL và dữ liệu mẫu.
2. Mở file `CafePOS.sln` bằng Visual Studio 2022 (cần workload **.NET desktop development** và **.NET 8 SDK**). Visual Studio sẽ tự khôi phục NuGet (`MySqlConnector`, `System.Configuration.ConfigurationManager`).
3. Mở file `CafePOS/App.config`, sửa lại chuỗi kết nối `DefaultConnection` (`Server`, `Port`, `User ID`, `Password`; XAMPP mặc định: root, mật khẩu rỗng) cho phù hợp với máy cá nhân.
4. Build & Run dự án (F5).

## MỤC 3: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG
> Chạy chương trình thật, chụp màn hình, lưu vào thư mục `images/` với đúng tên file bên dưới.

1. Màn hình chính (sơ đồ bàn): ![Màn hình chính (sơ đồ bàn)](images/01_main_screen.png)

2. Quản lý thực đơn (CRUD): ![Quản lý thực đơn (CRUD)](images/02_crud_screen.png)

3. Gọi món / Thanh toán: ![Gọi món / Thanh toán](images/03_transaction_screen.png)

4. Hóa đơn - tổng tiền sau thanh toán: ![Hóa đơn - tổng tiền sau thanh toán](images/04_report_screen.png)

5. Báo lỗi validation (giá âm / tên món trống): ![Báo lỗi validation (giá âm / tên món trống)](images/05_error_handling.png)

