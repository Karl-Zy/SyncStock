-- Receiving custodian confirmations (Rin's flow). Run on SyncStock database.
USE SyncStock;
GO

IF OBJECT_ID(N'dbo.ConfirmedItems', N'U') IS NULL
BEGIN
    CREATE TABLE ConfirmedItems (
        ConfirmedItemID INT PRIMARY KEY IDENTITY(1,1),
        PONumber NVARCHAR(50) NOT NULL,
        ItemName NVARCHAR(255) NOT NULL,
        DateReceived DATE NOT NULL,
        IsCapitalizable BIT NOT NULL DEFAULT 0,
        ExpectedQuantity INT NOT NULL,
        ReceivedQuantity INT NOT NULL,
        ExpectedAmount DECIMAL(18, 2) NOT NULL,
        ReceivedAmount DECIMAL(18, 2) NOT NULL,
        AttachmentPath NVARCHAR(500) NULL,
        Remarks NVARCHAR(500) NULL
    );
END
GO
