USE WebBanHangDB;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reviews]') AND type in (N'U'))
BEGIN
CREATE TABLE Reviews (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    ProductId INT,
    Rating INT,
    Content NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
END
GO
