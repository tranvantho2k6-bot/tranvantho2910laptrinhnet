# Bài 5.5: Hệ thống quản lý xuất - nhập kho nguyên vật liệu

## MỤC 1: THÔNG TIN SINH VIÊN
- **Họ và tên sinh viên:** [Điền họ tên]
- **Mã sinh viên:** [Điền MSV]
- **Tên Đề tài / Bài tập:** Bài 5.5 - Hệ thống quản lý xuất - nhập kho nguyên vật liệu

## Chức năng chính
- CRUD Vật tư & Nhà cung cấp, tìm kiếm bằng `MySqlParameter`
- Lập phiếu Nhập / Xuất bằng `MySqlTransaction` (Insert PhieuKho -> ChiTiet -> cập nhật tồn -> Commit)
- Xuất kho thiếu hàng: `Rollback` + thông báo "Không đủ hàng trong kho"
- Cảnh báo tồn dưới ngưỡng, tổng giá trị tồn kho bằng `ExecuteScalar`

## Cấu trúc thư mục (3-Tier Architecture)
```
├── Database/
│   └── ScriptDB.sql          # Script tạo CSDL InventoryDB và dữ liệu mẫu
├── InventoryMS/
│   ├── DTO/                  # Data Transfer Object
│   ├── DAL/                  # Data Access Layer (ADO.NET)
│   ├── BUS/                  # Business Logic Layer
│   ├── GUI/                  # WinForms (Presentation Layer)
│   ├── App.config            # Chuỗi kết nối
│   └── Program.cs
├── images/                   # Ảnh chụp màn hình dùng trong README
├── README.md
└── InventoryMS.sln
```

## MỤC 2: HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN
1. Mở **XAMPP Control Panel**, bấm Start **MySQL**. Vào http://localhost/phpmyadmin, chọn tab **Import**, chọn file `Database/ScriptDB.sql`, bấm **Import/Go** để tạo CSDL và dữ liệu mẫu.
2. Mở file `InventoryMS.sln` bằng Visual Studio 2022 (cần workload **.NET desktop development** và **.NET 8 SDK**). Visual Studio sẽ tự khôi phục NuGet (`MySqlConnector`, `System.Configuration.ConfigurationManager`).
3. Mở file `InventoryMS/App.config`, sửa lại chuỗi kết nối `DefaultConnection` (`Server`, `Port`, `User ID`, `Password`; XAMPP mặc định: root, mật khẩu rỗng) cho phù hợp với máy cá nhân.
4. Build & Run dự án (F5).

## MỤC 3: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG
> Chạy chương trình thật, chụp màn hình, lưu vào thư mục `images/` với đúng tên file bên dưới.

1. Màn hình chính (Vật tư): ![Màn hình chính (Vật tư)](images/01_main_screen.png)

2. Quản lý Vật tư / Nhà cung cấp (CRUD): ![Quản lý Vật tư / Nhà cung cấp (CRUD)](images/02_crud_screen.png)

3. Lập phiếu nhập / xuất: ![Lập phiếu nhập / xuất](images/03_transaction_screen.png)

4. Cảnh báo tồn kho & tổng giá trị: ![Cảnh báo tồn kho & tổng giá trị](images/04_report_screen.png)

5. Rollback khi xuất quá tồn / chống SQL Injection: ![Rollback khi xuất quá tồn / chống SQL Injection](images/05_error_handling.png)

