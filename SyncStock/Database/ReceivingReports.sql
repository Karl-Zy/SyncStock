-- Run on the SyncStock database (same as DatabaseHelper connection string).
USE SyncStock;
GO

IF OBJECT_ID(N'dbo.ReceivingReports', N'U') IS NOT NULL
    DROP TABLE dbo.ReceivingReports;
GO

CREATE TABLE ReceivingReports (
    ReceivingReportID INT PRIMARY KEY IDENTITY(1,1),
    POItemID INT NOT NULL UNIQUE,
    DateReceived DATE NOT NULL,
    ReceivedQuantity INT NOT NULL,
    ReceivedAmount DECIMAL(18, 2) NOT NULL,
    IsCapitalizable BIT NOT NULL DEFAULT 0,
    Remarks NVARCHAR(500) NULL,
    ProofOfDeliveryPath NVARCHAR(500) NULL,
    AcceptedAt DATETIME NOT NULL DEFAULT GETDATE(),
    AuditorStatus NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_ReceivingReports_PurchaseOrderItems
        FOREIGN KEY (POItemID) REFERENCES PurchaseOrderItems(POItemID)
);
GO
