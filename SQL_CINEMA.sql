-- =============================================
-- SCRIPT TẠO CƠ SỞ DỮ LIỆU QUẢN LÝ RẠP CHIẾU PHIM
-- =============================================

-- 1. Xóa các bảng cũ nếu tồn tại (theo thứ tự ngược để tránh lỗi khóa ngoại)
IF OBJECT_ID('lichchieu', 'U') IS NOT NULL DROP TABLE lichchieu;
IF OBJECT_ID('ghe', 'U') IS NOT NULL DROP TABLE ghe;
IF OBJECT_ID('phim', 'U') IS NOT NULL DROP TABLE phim;
IF OBJECT_ID('sanpham', 'U') IS NOT NULL DROP TABLE sanpham;
IF OBJECT_ID('phongchieu', 'U') IS NOT NULL DROP TABLE phongchieu;
IF OBJECT_ID('theloai', 'U') IS NOT NULL DROP TABLE theloai;
IF OBJECT_ID('nguoidung', 'U') IS NOT NULL DROP TABLE nguoidung;

-- 2. Tạo bảng nguoidung
CREATE TABLE nguoidung (
    ma_nguoi_dung INT IDENTITY(1,1) PRIMARY KEY,
    tai_khoan NVARCHAR(50) NOT NULL UNIQUE,
    mat_khau NVARCHAR(255) NOT NULL,
    ho_ten NVARCHAR(100) NULL,
    chuc_vu NVARCHAR(10) NOT NULL CHECK (chuc_vu IN ('Admin', 'Staff')) DEFAULT 'Staff'
);

-- 3. Tạo bảng theloai
CREATE TABLE theloai (
    ma_the_loai INT IDENTITY(1,1) PRIMARY KEY,
    ten_the_loai NVARCHAR(50) NOT NULL
);

-- 4. Tạo bảng phongchieu
CREATE TABLE phongchieu (
    ma_phong INT IDENTITY(1,1) PRIMARY KEY,
    ten_phong NVARCHAR(100) NOT NULL,
    tinh_trang NVARCHAR(20) CHECK (tinh_trang IN ('HoatDong', 'BaoTri')) DEFAULT 'HoatDong',
    nguoi_quan_ly INT NULL,
    CONSTRAINT FK_Phong_User FOREIGN KEY (nguoi_quan_ly) REFERENCES nguoidung (ma_nguoi_dung) ON DELETE SET NULL
);

-- 5. Tạo bảng phim
CREATE TABLE phim (
    ma_phim INT IDENTITY(1,1) PRIMARY KEY,
    ten_phim NVARCHAR(255) NOT NULL,
    ma_the_loai INT NOT NULL,
    thoi_luong INT NOT NULL,
    ngay_khoi_chieu DATE NOT NULL,
    ngay_ket_thuc DATE NULL,
    mo_ta NVARCHAR(MAX) NULL,
    trang_thai NVARCHAR(20) CHECK (trang_thai IN ('DangChieu', 'SapChieu', 'NgungChieu')) DEFAULT 'DangChieu',
    nguoi_nhap INT NULL,
    CONSTRAINT FK_Phim_TheLoai FOREIGN KEY (ma_the_loai) REFERENCES theloai (ma_the_loai),
    CONSTRAINT FK_Phim_User FOREIGN KEY (nguoi_nhap) REFERENCES nguoidung (ma_nguoi_dung) ON DELETE SET NULL
);

-- 6. Tạo bảng ghe
CREATE TABLE ghe (
    ma_ghe INT IDENTITY(1,1) PRIMARY KEY,
    ma_phong INT NOT NULL,
    ten_ghe NVARCHAR(10) NOT NULL,
    loai_ghe NVARCHAR(10) CHECK (loai_ghe IN ('Thuong', 'VIP', 'Doi')) DEFAULT 'Thuong',
    trang_thai NVARCHAR(10) CHECK (trang_thai IN ('KhaDung', 'Hong')) DEFAULT 'KhaDung',
    CONSTRAINT FK_Ghe_Phong FOREIGN KEY (ma_phong) REFERENCES phongchieu (ma_phong) ON DELETE CASCADE,
    CONSTRAINT UK_Ghe_Trong_Phong UNIQUE (ma_phong, ten_ghe)
);

-- 7. Tạo bảng lichchieu
CREATE TABLE lichchieu (
    ma_lich_chieu INT IDENTITY(1,1) PRIMARY KEY,
    ma_phim INT NOT NULL,
    ma_phong INT NOT NULL,
    nguoi_lap_lich INT NOT NULL,
    ngay_chieu DATE NOT NULL,
    gio_bat_dau TIME NOT NULL,
    gia_ve_co_ban DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_Lich_Phim FOREIGN KEY (ma_phim) REFERENCES phim (ma_phim) ON DELETE CASCADE,
    CONSTRAINT FK_Lich_Phong FOREIGN KEY (ma_phong) REFERENCES phongchieu (ma_phong) ON DELETE CASCADE,
    CONSTRAINT FK_Lich_User FOREIGN KEY (nguoi_lap_lich) REFERENCES nguoidung (ma_nguoi_dung),
    CONSTRAINT UK_Lich_Chieu_Unique UNIQUE (ma_phong, ngay_chieu, gio_bat_dau)
);

-- 8. Tạo bảng sanpham
CREATE TABLE sanpham (
    ma_san_pham INT IDENTITY(1,1) PRIMARY KEY,
    ten_san_pham NVARCHAR(100) NOT NULL,
    loai NVARCHAR(10) CHECK (loai IN ('DoAn', 'NuocUong', 'Combo')) NOT NULL,
    gia_ban DECIMAL(10, 2) NOT NULL,
    so_luong_ton INT NOT NULL DEFAULT 0,
    trang_thai NVARCHAR(20) CHECK (trang_thai IN ('DangBan', 'NgungBan')) DEFAULT 'DangBan',
    nguoi_cap_nhat INT NULL,
    CONSTRAINT FK_SanPham_User FOREIGN KEY (nguoi_cap_nhat) REFERENCES nguoidung (ma_nguoi_dung) ON DELETE SET NULL
);

-- =============================================
-- 9. CHÈN DỮ LIỆU MẪU ĐA DẠNG
-- =============================================

-- Chèn Nguoidung
INSERT INTO nguoidung (tai_khoan, mat_khau, ho_ten, chuc_vu) VALUES 
('admin', '123', N'Quản Trị Viên', 'Admin'),
('manh', '123', N'Nguyễn Văn Duy Mạnh', 'Staff'),
('loc', '123', N'Trần Viết Lộc', 'Staff'),
('nam', '123', N'Trần Hoài Nam', 'Staff'),
('tin', '123', N'Huỳnh Trần Tin', 'Staff'),
('viet', '123', N'Đặng Đại Việt', 'Staff');

-- Chèn Theloai
INSERT INTO theloai (ten_the_loai) VALUES 
(N'Hành Động'), (N'Tình Cảm'), (N'Hoạt Hình'), (N'Kinh Dị'), 
(N'Hài Hước'), (N'Viễn Tưởng'), (N'Phiêu Lưu'), (N'Tâm Lý');

-- Chèn Phongchieu (Rạp 04 bảo trì)
INSERT INTO phongchieu (ten_phong, tinh_trang, nguoi_quan_ly) VALUES 
(N'Rạp 01', 'HoatDong', 1), (N'Rạp 02', 'HoatDong', 1), (N'Rạp 03', 'HoatDong', 3),
(N'Rạp 04', 'BaoTri', 3), (N'Rạp 05', 'HoatDong', 1), (N'Rạp 06', 'HoatDong', 3),
(N'Rạp 07', 'HoatDong', 1), (N'Rạp 08', 'HoatDong', 3), (N'Rạp 09', 'HoatDong', 1),
(N'Rạp 10', 'HoatDong', 3);

-- Chèn Phim (Bổ sung ĐẦY ĐỦ Ngày Kết Thúc và Mô Tả)
INSERT INTO phim (ten_phim, ma_the_loai, thoi_luong, ngay_khoi_chieu, ngay_ket_thuc, mo_ta, trang_thai, nguoi_nhap) VALUES 
(N'Mai', 2, 131, '2026-02-10', '2026-03-31', N'Câu chuyện về cuộc đời của một người phụ nữ tên Mai, đầy rẫy những thăng trầm và khát khao hạnh phúc.', 'DangChieu', 1),
(N'Lật Mặt 7: Một Điều Ước', 5, 120, '2026-04-30', '2026-06-30', N'Phần 7 của series Lật Mặt mang đến câu chuyện gia đình cảm động và hài hước, quy tụ dàn diễn viên quen thuộc.', 'SapChieu', 1),
(N'Dune 2', 6, 166, '2026-03-01', '2026-04-15', N'Hành trình trả thù của Paul Atreides trên hành tinh cát Arrakis tiếp diễn với quy mô hoành tráng và ác liệt hơn.', 'DangChieu', 1),
(N'Kung Fu Panda 4', 3, 94, '2026-03-08', '2026-04-30', N'Gấu Po đối mặt với một kẻ thù mới có khả năng biến hình đáng sợ. Cậu phải tìm người kế vị chức Thần Long Đại Hiệp.', 'DangChieu', 3),
(N'Godzilla x Kong', 1, 115, '2026-03-29', '2026-05-15', N'Hai siêu quái thú phải hợp sức để chống lại một mối đe dọa ẩn sâu trong Trái Đất có thể hủy diệt cả nhân loại.', 'DangChieu', 1),
(N'Exhuma: Quật Mộ Trùng Ma', 4, 134, '2026-03-15', '2026-04-20', N'Bộ phim kinh dị siêu nhiên Hàn Quốc về một ngôi mộ bí ẩn mang đến những điềm gở và những thế lực tâm linh hắc ám.', 'DangChieu', 3),
(N'Deadpool 3', 1, 125, '2026-07-26', '2026-09-01', N'Màn kết hợp bùng nổ và hài hước giữa hai dị nhân được yêu thích nhất vũ trụ Marvel.', 'SapChieu', 1),
(N'Joker 2', 8, 110, '2026-10-04', '2026-11-30', N'Sự trở lại của Arthur Fleck cùng với người bạn đồng hành Harley Quinn trong một câu chuyện đen tối mới.', 'SapChieu', 3),
(N'Inside Out 2', 3, 100, '2026-06-14', '2026-08-10', N'Những cảm xúc mới xuất hiện trong tâm trí Riley khi cô bé chính thức bước vào tuổi dậy thì phức tạp.', 'SapChieu', 1),
(N'Spider-Man: No Way Home', 1, 140, '2023-12-01', '2024-02-15', N'Đa vũ trụ mở ra mang theo những ác nhân từ các vũ trụ khác đến thế giới của Peter Parker.', 'NgungChieu', 1),
(N'Iron Man', 2, 140, '2026-02-19', '2026-03-19', N'Thiên tài tỷ phú Tony Stark tạo ra bộ giáp sắt siêu đẳng để chống lại cái ác, mở đầu cho kỷ nguyên siêu anh hùng.', 'DangChieu', 1),
(N'Đào, Phở và Piano', 8, 100, '2026-02-15', '2026-03-20', N'Khúc tráng ca bi tráng về tình yêu và sự hy sinh của người Hà Nội trong những ngày khói lửa.', 'DangChieu', 1);

-- Chèn Ghe (Mô phỏng mỗi rạp đều có ghế cơ bản để hiển thị)
INSERT INTO ghe (ma_phong, ten_ghe, loai_ghe) VALUES 
-- Rạp 01
(1, 'A1', 'Thuong'), (1, 'A2', 'Thuong'), (1, 'B1', 'VIP'), (1, 'B2', 'VIP'), (1, 'C1', 'Doi'),
-- Rạp 02
(2, 'A1', 'Thuong'), (2, 'A2', 'Thuong'), (2, 'B1', 'VIP'), (2, 'B2', 'VIP'), (2, 'C1', 'Doi'),
-- Rạp 03
(3, 'A1', 'Thuong'), (3, 'A2', 'Thuong'), (3, 'B1', 'VIP'), (3, 'B2', 'VIP'), (3, 'C1', 'Doi'),
-- Rạp 05 (Bỏ qua rạp 4 vì đang bảo trì)
(5, 'A1', 'Thuong'), (5, 'A2', 'Thuong'), (5, 'B1', 'VIP'), (5, 'B2', 'VIP'), (5, 'C1', 'Doi'),
-- Rạp 06
(6, 'A1', 'Thuong'), (6, 'A2', 'Thuong'), (6, 'B1', 'VIP'), (6, 'B2', 'VIP'), (6, 'C1', 'Doi'),
-- Rạp 07
(7, 'A1', 'Thuong'), (7, 'A2', 'Thuong'), (7, 'B1', 'VIP'), (7, 'B2', 'VIP'), (7, 'C1', 'Doi'),
-- Rạp 08
(8, 'A1', 'Thuong'), (8, 'A2', 'Thuong'), (8, 'B1', 'VIP'), (8, 'B2', 'VIP'), (8, 'C1', 'Doi'),
-- Rạp 09
(9, 'A1', 'Thuong'), (9, 'A2', 'Thuong'), (9, 'B1', 'VIP'), (9, 'B2', 'VIP'), (9, 'C1', 'Doi'),
-- Rạp 10
(10, 'A1', 'Thuong'), (10, 'A2', 'Thuong'), (10, 'B1', 'VIP'), (10, 'B2', 'VIP'), (10, 'C1', 'Doi');

-- Chèn Lichchieu (Phân bổ cho tất cả các rạp Hoạt Động)
INSERT INTO lichchieu (ma_phim, ma_phong, nguoi_lap_lich, ngay_chieu, gio_bat_dau, gia_ve_co_ban) VALUES 
-- Lịch chiếu Rạp 01 & 02
(1, 1, 1, '2026-03-01', '09:00:00', 90000),
(3, 1, 1, '2026-03-01', '14:00:00', 110000),
(1, 2, 3, '2026-03-01', '19:30:00', 100000),
(4, 2, 3, '2026-03-01', '10:00:00', 85000),
-- Lịch chiếu Rạp 03 (Có Dune 2 như hình UI của bạn)
(3, 3, 1, '2026-03-01', '10:30:00', 110000),
(3, 3, 1, '2026-03-02', '14:00:00', 110000),
(12, 3, 3, '2026-03-02', '19:00:00', 95000),
-- Lịch chiếu Rạp 05 & 06
(5, 5, 4, '2026-03-01', '13:00:00', 95000),
(11, 5, 4, '2026-03-02', '20:00:00', 90000),
(6, 6, 4, '2026-03-15', '22:00:00', 105000),
(1, 6, 4, '2026-03-03', '15:30:00', 100000),
-- Lịch chiếu Rạp 07 (Phủ dữ liệu để không bị trống)
(4, 7, 1, '2026-03-01', '08:30:00', 80000),
(5, 7, 1, '2026-03-01', '16:00:00', 95000),
(3, 7, 3, '2026-03-02', '20:30:00', 120000),
-- Lịch chiếu Rạp 08 & 09
(11, 8, 3, '2026-03-01', '11:00:00', 90000),
(12, 8, 1, '2026-03-03', '18:00:00', 95000),
(6, 9, 3, '2026-03-16', '19:45:00', 105000),
(1, 9, 1, '2026-03-04', '14:15:00', 100000),
-- Lịch chiếu Rạp 10
(4, 10, 4, '2026-03-01', '09:30:00', 85000),
(3, 10, 4, '2026-03-01', '13:30:00', 110000),
(5, 10, 1, '2026-03-02', '21:00:00', 100000);

-- Chèn Sanpham
INSERT INTO sanpham (ten_san_pham, loai, gia_ban, so_luong_ton) VALUES 
(N'Bắp Rang Bơ (Ngọt)', 'DoAn', 45000, 150),
(N'Bắp Rang Bơ (Phô mai)', 'DoAn', 55000, 100),
(N'Coca Cola (Vừa)', 'NuocUong', 30000, 200),
(N'Pepsi (Lớn)', 'NuocUong', 35000, 180),
(N'Dasani (Nước suối)', 'NuocUong', 15000, 500),
(N'Combo 1 (1 Bắp + 1 Nước)', 'Combo', 65000, 120),
(N'Combo 2 (1 Bắp + 2 Nước)', 'Combo', 85000, 150),
(N'Xúc xích Đức nướng', 'DoAn', 25000, 80),
(N'Khoai tây chiên', 'DoAn', 35000, 60),
(N'Combo Couple (2 Bắp + 2 Nước)', 'Combo', 120000, 50);