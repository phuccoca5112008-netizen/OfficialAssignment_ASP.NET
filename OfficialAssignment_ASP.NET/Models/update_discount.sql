USE WebBanHangDB;
GO

-- Thêm cột Discount vào bảng Products nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'Discount')
BEGIN
    ALTER TABLE Products ADD Discount INT DEFAULT 0;
END
GO
