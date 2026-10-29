-- MySQL / MariaDB (XAMPP). Import bằng phpMyAdmin: tab Import -> chọn file này -> Go
SET NAMES utf8mb4;
DROP DATABASE IF EXISTS HotelDB;
CREATE DATABASE HotelDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE HotelDB;

CREATE TABLE Phong(
    SoPhong VARCHAR(10) PRIMARY KEY,
    LoaiPhong VARCHAR(50) NOT NULL,
    GiaTheoGio DECIMAL(18,2) NOT NULL,
    TrangThai VARCHAR(30) NOT NULL DEFAULT 'Trống',   -- Trống / Đang ở / Đang dọn dẹp
    CHECK (GiaTheoGio >= 0)
);
CREATE TABLE KhachHang(
    MaKH INT AUTO_INCREMENT PRIMARY KEY,
    HoTen VARCHAR(100) NOT NULL,
    CCCD VARCHAR(20) NOT NULL UNIQUE,
    SoDienThoai VARCHAR(15)
);
CREATE TABLE DichVu(
    MaDV INT AUTO_INCREMENT PRIMARY KEY,
    TenDV VARCHAR(100) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    CHECK (DonGia >= 0)
);
CREATE TABLE PhieuDatPhong(
    MaPhieu INT AUTO_INCREMENT PRIMARY KEY,
    SoPhong VARCHAR(10) NOT NULL,
    MaKH INT NOT NULL,
    NgayNhanPhong DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    NgayTraPhong DATETIME NULL,
    TongTienThanhToan DECIMAL(18,2) NULL DEFAULT 0,
    FOREIGN KEY (SoPhong) REFERENCES Phong(SoPhong),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);
CREATE TABLE SuDungDichVu(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MaPhieu INT NOT NULL,
    MaDV INT NOT NULL,
    SoLuong INT NOT NULL,
    ThoiGian DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (SoLuong > 0),
    FOREIGN KEY (MaPhieu) REFERENCES PhieuDatPhong(MaPhieu),
    FOREIGN KEY (MaDV) REFERENCES DichVu(MaDV)
);

INSERT INTO Phong(SoPhong,LoaiPhong,GiaTheoGio,TrangThai) VALUES
 ('P101','Đơn',80000,'Trống'),('P102','Đơn',80000,'Trống'),('P103','Đơn',80000,'Trống'),
 ('P201','Đôi',120000,'Trống'),('P202','Đôi',120000,'Trống'),('P203','Đôi',120000,'Trống'),
 ('P301','VIP',250000,'Trống'),('P302','VIP',250000,'Trống');
INSERT INTO KhachHang(HoTen,CCCD,SoDienThoai) VALUES
 ('Nguyễn Văn An','001200000001','0901234567'),
 ('Trần Thị Bình','001200000002','0912345678');
INSERT INTO DichVu(TenDV,DonGia) VALUES
 ('Nước suối',10000),('Nước ngọt',15000),('Giặt là',50000),('Mì ly',20000),('Bữa sáng',60000);
