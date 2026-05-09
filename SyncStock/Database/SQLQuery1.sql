USE Syncho5DB;

DROP TABLE IF EXISTS PurchaseOrdersItems;
DROP TABLE IF EXISTS PurchaseOrders;
DROP TABLE IF EXISTS UserAccounts;
DROP TABLE IF EXISTS Departments;
DROP TABLE IF EXISTS Items;

CREATE TABLE Items (

	ItemID INT PRIMARY KEY IDENTITY (1,1),
	InvoiceNumber NVARCHAR(50) NOT NULL,
	ItemName NVARCHAR(255) NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(18, 2) NOT NULL,
	TotalPrice AS (Quantity * UnitPrice),
	ItemDate DATE NOT NULL,
	CreatedAt DATETIME DEFAULT GETDATE()

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

	FOREIGN KEY (DepartmentID)
	REFERENCES Departments(DepartmentID)
	
);

CREATE TABLE PurchaseOrdersItems (

	PurchaseOrderItemID INT PRIMARY KEY IDENTITY (1,1),
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
		