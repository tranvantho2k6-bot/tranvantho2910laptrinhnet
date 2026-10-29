-- MySQL / MariaDB (XAMPP). Import bằng phpMyAdmin: tab Import -> chọn file này -> Go
SET NAMES utf8mb4;
DROP DATABASE IF EXISTS InventoryDB;
CREATE DATABASE InventoryDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE InventoryDB;

CREATE TABLE NhaCungCap(
    MaNCC INT AUTO_INCREMENT PRIMARY KEY,
    TenNCC VARCHAR(150) NOT NULL,
    DienThoai VARCHAR(15) UNIQUE,
    DiaChi VARCHAR(255)
);
CREATE TABLE VatTu(
    MaVatTu VARCHAR(20) PRIMARY KEY,
    TenVatTu VARCHAR(150) NOT NULL,
    DVT VARCHAR(20),
    SoLuongTon DECIMAL(18,2) NOT NULL DEFAULT 0,
    CHECK (SoLuongTon >= 0)
);
CREATE TABLE PhieuKho(
    MaPhieu INT AUTO_INCREMENT PRIMARY KEY,
    LoaiPhieu VARCHAR(10) NOT NULL,
    NgayLap DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    MaNCC INT NULL,                                  -- NULL nếu là phiếu xuất
    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0,
    CHECK (LoaiPhieu IN ('NHAP','XUAT')),
    FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC)
);
CREATE TABLE ChiTietPhieuKho(
    MaPhieu INT NOT NULL,
    MaVatTu VARCHAR(20) NOT NULL,
    SoLuong DECIMAL(18,2) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaPhieu, MaVatTu),
    CHECK (SoLuong > 0),
    FOREIGN KEY (MaPhieu) REFERENCES PhieuKho(MaPhieu),
    FOREIGN KEY (MaVatTu) REFERENCES VatTu(MaVatTu)
);

INSERT INTO NhaCungCap(TenNCC,DienThoai,DiaChi) VALUES
 ('Công ty TNHH Thực phẩm An Phát','0241234567','Hà Nội'),
 ('Công ty CP Bao bì Hoàng Gia','0287654321','TP. Hồ Chí Minh');
INSERT INTO VatTu(MaVatTu,TenVatTu,DVT,SoLuongTon) VALUES
 ('VT001','Đường cát trắng','Kg',100),
 ('VT002','Bột mì','Kg',50),
 ('VT003','Hộp giấy đựng bánh','Cái',500),
 ('VT004','Sữa tươi','Thùng',5),
 ('VT005','Cà phê hạt','Kg',8);
INSERT INTO PhieuKho(LoaiPhieu,MaNCC,TongTien) VALUES ('NHAP',1,0),('NHAP',2,0);
INSERT INTO ChiTietPhieuKho(MaPhieu,MaVatTu,SoLuong,DonGia) VALUES
 (1,'VT001',100,20000),(1,'VT002',50,15000),(1,'VT004',5,350000),(1,'VT005',8,180000),
 (2,'VT003',500,1500);
UPDATE PhieuKho SET TongTien = (SELECT SUM(c.SoLuong*c.DonGia) FROM ChiTietPhieuKho c WHERE c.MaPhieu = PhieuKho.MaPhieu);
