-- Sử dụng đúng Database
USE WebBanHangDB;
GO

-- Thêm cột PaymentMethod và PaymentStatus vào bảng Orders
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'PaymentMethod')
BEGIN
    ALTER TABLE Orders ADD PaymentMethod NVARCHAR(50) DEFAULT N'COD';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'PaymentStatus')
BEGIN
    ALTER TABLE Orders ADD PaymentStatus NVARCHAR(50) DEFAULT N'Chưa thanh toán';
END
