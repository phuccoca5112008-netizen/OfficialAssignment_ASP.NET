-- 1. Tạo CSDL (Chạy phần này trước nếu chưa có DB)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WebBanHangDB')
BEGIN
    CREATE DATABASE WebBanHangDB;
END
GO

-- 2. Sử dụng CSDL vừa tạo
USE WebBanHangDB;
GO

-- 3. Tạo bảng Users
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(200),
    Role INT DEFAULT 2, 
    CreatedDate DATETIME DEFAULT GETDATE()
);
END
GO

-- 4. Tạo bảng Categories
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);
END
GO

-- 5. Tạo bảng Products
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
CREATE TABLE Products (
BEGIN
CREATE TABLE Carts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
END
GO

-- 7. Tạo bảng CartDetails
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CartDetails]') AND type in (N'U'))
BEGIN
CREATE TABLE CartDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CartId INT,
    ProductId INT,
    Quantity INT DEFAULT 1,
    FOREIGN KEY (CartId) REFERENCES Carts(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
END
GO

-- 8. Nhập dữ liệu mẫu (SEED DATA)

-- Xóa dữ liệu cũ để tránh trùng lặp (nếu cần reset thì bỏ comment dòng dưới)
-- DELETE FROM CartDetails; DELETE FROM Carts; DELETE FROM Products; DELETE FROM Categories; DELETE FROM Users;

-- Users
IF NOT EXISTS (SELECT * FROM Users)
BEGIN
    INSERT INTO Users (Username, Password, FullName, Email, Role) VALUES 
    ('admin', '123456', N'Quản Trị Viên', 'admin@example.com', 0),
    ('staff', '123456', N'Nhân Viên Bán Hàng', 'staff@example.com', 1),
    ('customer', '123456', N'Khách Hàng Thân Thiết', 'customer@example.com', 2);
END
GO

-- Categories
IF NOT EXISTS (SELECT * FROM Categories)
BEGIN
    INSERT INTO Categories (Name) VALUES 
    (N'iPhone'), (N'iPad'), (N'Mac'), (N'Watch'), (N'Âm thanh'), (N'Phụ kiện');
END
GO

-- Products (Chỉ thêm nếu chưa có sản phẩm nào)
IF NOT EXISTS (SELECT * FROM Products)
BEGIN
    -- iPhone (CatId = 1)
    INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES 
    (N'iPhone 15 Pro Max', 34990000, 'iphone15promax.jpg', 'Titan Tự Nhiên', '256GB', N'Chip A17 Pro, Khung Titan', 1),
    (N'iPhone 15 Pro', 28990000, 'iphone15pro.jpg', 'Titan Xanh', '128GB', N'Chip A17 Pro, Camera chuyên nghiệp', 1),
    (N'iPhone 15 Plus', 25990000, 'iphone15plus.jpg', 'Hồng', '128GB', N'Màn hình lớn, Pin trâu', 1),
    (N'iPhone 15', 22990000, 'iphone15.jpg', 'Vàng', '128GB', N'Dynamic Island, Camera 48MP', 1),
    (N'iPhone 14 Pro Max', 27990000, 'iphone14promax.jpg', 'Tím', '128GB', N'Màn hình Always-On', 1),
    (N'iPhone 14 Plus', 21990000, 'iphone14plus.jpg', 'Xanh dương', '128GB', N'Kích thước lớn, giá tốt', 1),
    (N'iPhone 14', 19990000, 'iphone14.jpg', 'Đen', '128GB', N'Thiết kế bền bỉ', 1),
    (N'iPhone 13', 15990000, 'iphone13.jpg', 'Hồng', '128GB', N'Camera chéo độc đáo', 1),
    (N'iPhone 12', 13990000, 'iphone12.jpg', 'Trắng', '64GB', N'Màn hình OLED', 1),
    (N'iPhone 11', 10990000, 'iphone11.jpg', 'Đỏ', '64GB', N'Camera góc siêu rộng', 1);

    -- iPad (CatId = 2)
    INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES 
    (N'iPad Pro 13 inch M4', 37990000, 'ipadprom4.jpg', 'Đen', '256GB', N'Chip M4 siêu mạnh, màn hình OLED', 2),
    (N'iPad Pro 11 inch M4', 28990000, 'ipadpro11m4.jpg', 'Bạc', '256GB', N'Mỏng nhẹ, hiệu năng cao', 2),
    (N'iPad Air 13 inch M2', 22990000, 'ipadair13m2.jpg', 'Xanh', '128GB', N'Màn hình lớn, Chip M2', 2),
    (N'iPad Air 11 inch M2', 16990000, 'ipadair11m2.jpg', 'Tím', '128GB', N'Thiết kế mới, nhiều màu sắc', 2),
    (N'iPad Gen 10', 9990000, 'ipadgen10.jpg', 'Vàng', '64GB', N'Thiết kế toàn màn hình', 2),
    (N'iPad Gen 9', 7490000, 'ipadgen9.jpg', 'Xám', '64GB', N'Giá rẻ, hiệu năng ổn', 2),
    (N'iPad Mini 6', 12990000, 'ipadmini6.jpg', 'Hồng', '64GB', N'Nhỏ gọn, mạnh mẽ', 2),
    (N'iPad Pro 12.9 M2', 30000000, 'ipadpro129m2.jpg', 'Xám', '128GB', N'Màn hình Mini-LED', 2),
    (N'iPad Air 5', 14990000, 'ipadair5.jpg', 'Xanh dương', '64GB', N'Chip M1', 2),
    (N'iPad Mini 5', 9000000, 'ipadmini5.jpg', 'Vàng', '64GB', N'Nhỏ gọn, tiện lợi', 2);

    -- Mac (CatId = 3)
    INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES 
    (N'MacBook Air 13 M3', 27990000, 'macbookairm3.jpg', 'Xanh đêm', '256GB', N'Chip M3 mới nhất', 3),
    (N'MacBook Air 15 M3', 32990000, 'macbookair15m3.jpg', 'Vàng ánh sao', '256GB', N'Màn hình lớn 15 inch', 3),
    (N'MacBook Pro 14 M3', 39990000, 'macbookpro14m3.jpg', 'Xám', '512GB', N'Hiệu năng chuyên nghiệp', 3),
    (N'MacBook Pro 16 M3 Pro', 64990000, 'macbookpro16m3.jpg', 'Đen', '512GB', N'Pin trâu, màn hình đẹp', 3),
    (N'MacBook Air M2', 24990000, 'macbookairm2.jpg', 'Bạc', '256GB', N'Thiết kế mới', 3),
    (N'MacBook Air M1', 18990000, 'macbookairm1.jpg', 'Vàng', '256GB', N'Huyền thoại giá rẻ', 3),
    (N'iMac M3', 36990000, 'imacm3.jpg', 'Xanh lá', '256GB', N'Máy tính All-in-One', 3),
    (N'Mac Mini M2', 14990000, 'macminim2.jpg', 'Bạc', '256GB', N'Nhỏ gọn, để bàn', 3),
    (N'Mac Studio M2 Max', 54990000, 'macstudio.jpg', 'Bạc', '512GB', N'Sức mạnh khủng khiếp', 3),
    (N'Mac Pro M2 Ultra', 199990000, 'macpro.jpg', 'Bạc', '1TB', N'Máy trạm chuyên dụng', 3);

    -- Watch (CatId = 4)
    INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES 
    (N'Apple Watch Series 9', 10990000, 'series9.jpg', 'Đỏ', '41mm', N'Chip S9, Double Tap', 4),
    (N'Apple Watch Ultra 2', 21990000, 'ultra2.jpg', 'Titan', '49mm', N'Siêu bền, pin trâu', 4),
    (N'Apple Watch SE 2023', 6990000, 'se2023.jpg', 'Xanh đen', '40mm', N'Giá hợp lý', 4),
    (N'Apple Watch Series 8', 9990000, 'series8.jpg', 'Trắng', '45mm', N'Cảm biến nhiệt độ', 4),
    (N'Apple Watch Ultra 1', 19990000, 'ultra1.jpg', 'Titan', '49mm', N'Chuyên thể thao', 4),
    (N'Apple Watch Series 7', 8990000, 'series7.jpg', 'Xanh lá', '41mm', N'Màn hình tràn viền', 4),
    (N'Apple Watch SE 1', 5990000, 'se1.jpg', 'Vàng', '40mm', N'Cơ bản đủ dùng', 4),
    (N'Apple Watch Series 6', 7990000, 'series6.jpg', 'Xanh dương', '44mm', N'Đo oxy trong máu', 4),
    (N'Apple Watch Nike', 7500000, 'nike.jpg', 'Đen', '44mm', N'Dây đeo thể thao', 4),
    (N'Apple Watch Hermes', 30990000, 'hermes.jpg', 'Cam', '41mm', N'Sang trọng, đẳng cấp', 4);

    -- Âm thanh (CatId = 5)
    INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES 
    (N'AirPods Pro 2 USB-C', 5990000, 'airpodspro2.jpg', 'Trắng', 'Standard', N'Chống ồn chủ động', 5),
    (N'AirPods 3', 4490000, 'airpods3.jpg', 'Trắng', 'Standard', N'Âm thanh vòm', 5),
    (N'AirPods 2', 2990000, 'airpods2.jpg', 'Trắng', 'Standard', N'Giá rẻ, kết nối nhanh', 5),
    (N'AirPods Max', 13990000, 'airpodsmax.jpg', 'Xanh dương', 'Standard', N'Tai nghe chụp tai cao cấp', 5),
    (N'HomePod Gen 2', 7990000, 'homepod2.jpg', 'Đen', 'Standard', N'Loa thông minh', 5),
    (N'HomePod Mini', 2990000, 'homepodmini.jpg', 'Cam', 'Standard', N'Nhỏ gọn, âm hay', 5),
    (N'Beats Studio Pro', 7990000, 'beatsstudiopro.jpg', 'Nâu', 'Standard', N'Bass mạnh mẽ', 5),
    (N'Beats Solo 3', 3990000, 'beatssolo3.jpg', 'Đỏ', 'Standard', N'Pin 40 giờ', 5),
    (N'Sony WH-1000XM5', 8490000, 'sonywh1000xm5.jpg', 'Bạc', 'Standard', N'Chống ồn đỉnh cao', 5),
    (N'Marshall Stanmore III', 9990000, 'marshall.jpg', 'Đen', 'Standard', N'Thiết kế cổ điển', 5);

    -- Phụ kiện (CatId = 6)
    INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES 
    (N'Magic Mouse 2', 2490000, 'magicmouse.jpg', 'Đen', 'Standard', N'Chuột cảm ứng đa điểm', 6),
    (N'Magic Keyboard Touch ID', 4490000, 'magickeyboard.jpg', 'Trắng', 'Standard', N'Bàn phím kèm Touch ID', 6),
    (N'Apple Pencil 2', 3390000, 'pencil2.jpg', 'Trắng', 'Standard', N'Bút cảm ứng cho iPad', 6),
    (N'Apple Pencil USB-C', 2190000, 'pencilusbc.jpg', 'Trắng', 'Standard', N'Giá rẻ cho iPad Gen 10', 6),
    (N'Củ sạc 20W Apple', 550000, 'adapter20w.jpg', 'Trắng', 'Standard', N'Sạc nhanh iPhone', 6),
    (N'Sạc MagSafe', 1190000, 'magsafe.jpg', 'Trắng', 'Standard', N'Sạc không dây nam châm', 6),
    (N'AirTag 1 Pack', 790000, 'airtag.jpg', 'Trắng', 'Standard', N'Thiết bị định vị', 6),
    (N'Ốp lưng Clear Case MagSafe', 1290000, 'clearcase.jpg', 'Trong suốt', 'iPhone 15', N'Bảo vệ, hỗ trợ MagSafe', 6),
    (N'Cáp C to C 60W Woven', 550000, 'cablec2c.jpg', 'Trắng', '1m', N'Cáp dù bền bỉ', 6),
    (N'Cáp Lightning to USB', 490000, 'cablelightning.jpg', 'Trắng', '1m', N'Cáp sạc truyền thống', 6);
END
GO


-- 8. Tạo bảng Orders
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2),
    Status NVARCHAR(50) DEFAULT N'Chờ xử lý', -- Chờ xử lý, Đang giao, Đã giao, Đã hủy
    ReceiverName NVARCHAR(100),
    ReceiverPhone NVARCHAR(20),
    ReceiverAddress NVARCHAR(200),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
END
GO

-- 9. Tạo bảng OrderDetails
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND type in (N'U'))
BEGIN
CREATE TABLE OrderDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT,
    ProductId INT,
    Quantity INT,
    Price DECIMAL(18, 2),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
END
GO