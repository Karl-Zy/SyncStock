USE SyncStock;

DROP TABLE IF EXISTS ConfirmedItems;
DROP TABLE IF EXISTS ReceivingReports;
DROP TABLE IF EXISTS PurchaseOrderItems;
DROP TABLE IF EXISTS PurchaseOrdersItems;
DROP TABLE IF EXISTS PurchaseOrders;
DROP TABLE IF EXISTS UserAccounts;
DROP TABLE IF EXISTS Departments;
DROP TABLE IF EXISTS Items;

CREATE TABLE Items (
	ItemID INT PRIMARY KEY IDENTITY (1,1),
	ItemName NVARCHAR(255) NOT NULL
);

CREATE TABLE Departments (

	DepartmentID INT PRIMARY KEY IDENTITY (1,1),
	DepartmentName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE PurchaseOrders (

	PurchaseOrderID INT PRIMARY KEY IDENTITY (1,1),
	InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
	PONumber NVARCHAR(50) NOT NULL UNIQUE,
	OrderDate DATE NOT NULL,
	DepartmentID INT NOT NULL,
	Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
	Priority NVARCHAR(50) NOT NULL DEFAULT 'Normal',
	Remarks NVARCHAR(500) NULL,
	AttachmentPath NVARCHAR(500) NULL,

	FOREIGN KEY (DepartmentID)
	REFERENCES Departments(DepartmentID)
	
);

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

CREATE TABLE PurchaseOrderItems (

	POItemID INT PRIMARY KEY IDENTITY (1,1),
	PurchaseOrderID INT NOT NULL,
	ItemID INT NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(18, 2) NOT NULL,
	TotalPrice AS (Quantity * UnitPrice),

	FOREIGN KEY (PurchaseOrderID)
	REFERENCES PurchaseOrders(PurchaseOrderID),

	FOREIGN KEY (ItemID)
	REFERENCES Items(ItemID)
	
);

CREATE TABLE UserAccounts (

	UserID INT PRIMARY KEY IDENTITY (1,1),
	Username NVARCHAR(50) NOT NULL UNIQUE,
	Password NVARCHAR(255) NOT NULL,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL
);



INSERT INTO Departments (DepartmentName)

VALUES ('College of Computer Studies'),
		('College of Nursing'),
		('College of Accountancy');
		