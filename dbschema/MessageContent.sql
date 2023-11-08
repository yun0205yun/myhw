CREATE TABLE Content (
    ContentId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    Content NVARCHAR(1000) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE() -- 將 CreatedAt 添加到 Content 表中，默認值為當前日期和時間
);