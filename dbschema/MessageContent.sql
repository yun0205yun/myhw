CREATE TABLE Content (
    ContentId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    Content NVARCHAR(1000) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE() -- �N CreatedAt �K�[�� Content ���A�q�{�Ȭ���e����M�ɶ�
);