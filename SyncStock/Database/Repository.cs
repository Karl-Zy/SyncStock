using SyncStock.Models.Item;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SyncStock.Models;
using SyncStock.Models.Accounts;
using SyncStock.Models.Reports;



namespace SyncStock.Database
{
    public class Repository
    {
        private SqlConnection CreateConnection() => DatabaseHelper.GetConnection();

        public IEnumerable<Item> GetAllItems()
        {

            using (var conn = CreateConnection())
            {
                return conn.Query<Item>("SELECT * FROM Items");
            }

        }
        public User GetUserByCredentials(string userName, string password)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<User>(
                    "SELECT * FROM Users WHERE UserName = @UserName AND Password = @Password",
                    new { UserName = userName, Password = password });
            }
        }

        public User GetUserByRfid(string rfidUID)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<User>(
                    "SELECT * FROM Users WHERE RfidUID = @RfidUID",
                    new { RfidUID = rfidUID });
            }
        }

        public Item GetItemById(int itemId)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<Item>("SELECT * FROM Items WHERE ItemID = @ItemID", new { ItemID = itemId });
            }
        }

        public int AddItem(string itemName)
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>("INSERT INTO Items (ItemName) VALUES (@ItemName); SELECT SCOPE_IDENTITY();",
                    new { ItemName = itemName });

            }
        }

        public IEnumerable<Departments> GetAllDepartments()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<Departments>("SELECT * FROM Departments");
            }
        }

        public Departments GetDepartmentById(int departmentId)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<Departments>("SELECT * FROM Departments WHERE DepartmentID = @DepartmentID",
                    new { DepartmentID = departmentId });
            }
        }

        public IEnumerable<PurchaseOrders> GetAllPurchaseOrder()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrders>("SELECT * FROM PurchaseOrders");

            }
        }

        public PurchaseOrders GetPurchaseORderById(int purchaseOrderId)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<PurchaseOrders>("SELECT * FROM PurchaseOrders WHERE PurchaseOrderID = @PurchaseOrderID",
                    new { PurchaseOrderID = purchaseOrderId });
            }
        }

        public int AddPurchaseOrder(PurchaseOrders order)
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(@"INSERT INTO PurchaseOrders (InvoiceNumber, PONumber, DepartmentID, OrderDate, Status, Priority, Remarks, AttachmentPath) 
                        VALUES (@InvoiceNumber, @PONumber, @DepartmentID, @OrderDate, @Status, @Priority, @Remarks, @AttachmentPath);
                    SELECT SCOPE_IDENTITY();", order);
            }
        }

        public int GetPurchaseOrderCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM PurchaseOrders");
            }
        }
        public IEnumerable<PurchaseOrderItem> GetItemsByPurchaseOrder(int purchaseOrderId)
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrderItem>
                    (@"SELECT poi.*, i.ItemName
                    FROM PurchaseOrderItems poi
                    INNER JOIN Items i ON poi.ItemID = i.ItemID
                    WHERE poi.PurchaseOrderID = @PurchaseOrderID",
                    new { PurchaseOrderID = purchaseOrderId });
            }
        }

        public void AddPurchaseOrderItems(IEnumerable<PurchaseOrderItem> items)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"INSERT INTO PurchaseOrderItems (PurchaseOrderID, ItemID, Quantity, UnitPrice) 
                            VALUES (@PurchaseOrderID, @ItemID, @Quantity, @UnitPrice)", items);
            }
        }

        public void AddPurchaseOrderItem(PurchaseOrderItem item)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"INSERT INTO PurchaseOrderItems(PurchaseOrderID, ItemID, Quantity, UnitPrice)
                        VALUES (@PurchaseOrderID, @ItemID, @Quantity, @UnitPrice)", item);
            }
        }
        public IEnumerable<PurchaseOrderItem> GetAllPurchaseOrderItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrderItem>(@"
            SELECT poi.*, i.ItemName
            FROM PurchaseOrderItems poi
            INNER JOIN Items i ON poi.ItemID = i.ItemID");
            }
        }

        public int GetPendingOrdersCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM PurchaseOrders WHERE Status = 'Pending'");
            }
        }

        public List<PurchaseOrders> GetASAPOrders()
        {
            var orders = new List<PurchaseOrders>();

            string query = @"SELECT po.*, d.DepartmentName
                            FROM PurchaseOrders po
                            JOIN Departments d ON po.DepartmentID = d.DepartmentID
                            WHERE po.Priority = 'ASAP Department'
                            OR po.Priority = 'ASAP'
                            ORDER BY po.OrderDate DESC";

            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrders>(query).ToList();
            }
        }

        public void ApprovePurchaseOrder(int purchaseOrderId)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute("UPDATE PurchaseOrders SET Status = 'Approved' WHERE PurchaseOrderID = @PurchaseOrderID",
                    new { PurchaseOrderID = purchaseOrderId });
            }
        }

        public IEnumerable<PurchaseOrders> GetPendingPurchaseOrders()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrders>(@"SELECT po.*, d.DepartmentName 
                    FROM PurchaseOrders po 
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    WHERE po.Status = 'Pending'");
            }
        }

        public IEnumerable<PendingOrderSummary> GetPendingOrderSummary()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PendingOrderSummary>(@"
                    SELECT po.PONumber,
                    d.DepartmentName,
                    po.OrderDate,
                    po.Priority,
                    po.Status,
                    COUNT (poi.PurchaseOrderItemID) AS TotalItems,
                    SUM (poi.TotalPrice) AS TotalAmount
                    FROM PurchaseOrders po
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    LEFT JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
                    WHERE po.Status = 'Pending'
                    GROUP BY po.PONumber, d.DepartmentName, po.OrderDate, po.Priority, po.Status");

            }
        }

        public IEnumerable<ApprovedPurchaseOrder> GetAllApprovedMonthlyCost()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ApprovedPurchaseOrder>
                    (@"SELECT 
                    po.PONumber,
                    d.DepartmentName,
                    po.OrderDate,
                    po.Priority,
                    po.Status,
                    COUNT(poi.PurchaseOrderItemID) AS TotalItems,
                    SUM(poi.TotalPrice) AS TotalAmount
                    FROM PurchaseOrders po
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    LEFT JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
                    WHERE po.Status = 'Approved'
                    AND MONTH(po.OrderDate) = MONTH(GETDATE())
                    AND YEAR(po.OrderDate) = YEAR(GETDATE())
                    GROUP BY po.PONumber, d.DepartmentName, po.OrderDate, po.Priority, po.Status");
            }

        }

        public int GetAllApprovedTotalItems() 
        {
            using (var conn = CreateConnection()) 
            {
                return conn.ExecuteScalar<int>(@"
                SELECT ISNULL(COUNT(poi.PurchaseOrderItemID), 0)
                FROM PurchaseOrders po
                INNER JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
                WHERE po.Status = 'Approved'
                AND MONTH(po.OrderDate) = MONTH(GETDATE())
                AND YEAR(po.OrderDate) = YEAR(GETDATE())");
            }
        }

        public IEnumerable <ReportItem> GetAllReportItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ReportItem>(@"
                SELECT 
                    po.PONumber,
                    d.DepartmentName,
                    po.OrderDate,
                    po.Priority,
                    po.Status,
                    i.ItemName,
                    poi.Quantity,
                    poi.UnitPrice,
                    poi.TotalPrice AS Amount
                FROM PurchaseOrders po
                INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                INNER JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
                INNER JOIN Items i ON poi.ItemID = i.ItemID
                WHERE po.Status = 'Approved'
                AND MONTH(po.OrderDate) = MONTH(GETDATE())
                AND YEAR(po.OrderDate) = YEAR(GETDATE())
                ORDER BY po.OrderDate DESC").ToList();
            }
        }

        public void AddItemImage(int purchaseOrderId, string imagePath) 
        {
            using (var conn = CreateConnection()) 
            {
                conn.Execute(@"INSERT INTO ItemImages (PurchaseOrderID, ImagePath)
                              VALUES (@PurchaseOrderID, @ImagePath)",
                    new { PurchaseOrderID = purchaseOrderId, ImagePath = imagePath });
            }
        }

        public IEnumerable<ItemImages> GetImagesByPurchaseOrder(int purchaseOrderId) 
        {
            using (var conn = CreateConnection()) 
            {
                return conn.Query<ItemImages>(
                    "SELECT * FROM ItemImages WHERE PurchaseOrderID = @PurchaseOrderId",
                    new { PurchaseOrderID = purchaseOrderId });
            }
        }

        public IEnumerable<PurchaseOrderBrief> GetPurchaseOrderBrief() 
        {
            using (var conn = CreateConnection()) 
            {
                return conn.Query<PurchaseOrderBrief>(@"
                 Select
                        po.PONumber,
                        po.OrderDate,
                        poi.Quantity,
                        poi.TotalPrice,
                        po.Remarks
                 FROM PurchaseOrders po
                 INNER JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
                 ORDER BY po.OrderDate DESC");
            }
        }

        public IEnumerable<CapitalizedOrder> GetAllCapitalizedOrder() 
        {
            using (var conn = CreateConnection()) 
            {
                return conn.Query<CapitalizedOrder>(@"
                  SELECT
                        ConfirmedItemID,
                        PONumber,
                        ItemName,
                        DateReceived,
                        IsCapitalizable,
                        ExpectedQuantity,
                        ReceivedQuantity,
                        ExpectedAmount,
                        ReceivedAmount,
                        Remarks,
                        Status
                  FROM ConfirmedItems
                  WHERE IsCapitalizable = 1
                  ORDER BY  DateReceived DESC");
            }
        }

        public void MarkAsCapitalizable(int confirmedItemId, bool isCapitalizable) 
        {
            using (var conn = CreateConnection()) 
            {
                conn.Execute(@"UPDATE ConfirmedItems
                              SET IsCapitalizable = @IsCapitalizable
                              WHERE ConfirmedItemID = @ConfirmedItemID",
                              new { ConfirmedItemID = confirmedItemId, IsCapitalizable = isCapitalizable });
            }
        }

        public IEnumerable<ReceivingReports> GetAllReceivedOrders()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ReceivingReports>(@"
            SELECT 
                ConfirmedItemID,
                PONumber,
                ItemName,
                DateReceived,
                ExpectedQuantity,
                ReceivedQuantity,
                ExpectedAmount,
                ReceivedAmount,
                Remarks,
                Status
            FROM ConfirmedItems
            WHERE IsCapitalizable = 0
            ORDER BY DateReceived DESC");
            }
        }

        public IEnumerable<ReconcilationItem> GetReconciliationItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ReconcilationItem>(@"
            SELECT
                po.PONumber,
                d.DepartmentName,
                po.OrderDate,
                po.Priority,
                po.Status           AS POStatus,
                i.ItemName,
                poi.Quantity        AS OrderedQuantity,
                poi.UnitPrice,
                poi.TotalPrice      AS OrderedAmount,
                ci.DateReceived,
                ci.ExpectedQuantity,
                ci.ReceivedQuantity,
                ci.ExpectedAmount,
                ci.ReceivedAmount,
                ci.Status           AS ReceivingStatus,
                ci.Remarks,
                ci.AttachmentData,
                ci.AttachmentFileName
            FROM PurchaseOrders po
            INNER JOIN Departments d          ON po.DepartmentID = d.DepartmentID
            INNER JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
            INNER JOIN Items i                ON poi.ItemID = i.ItemID
            LEFT  JOIN ConfirmedItems ci      ON ci.PONumber = po.PONumber
                                             AND ci.ItemName = i.ItemName
            ORDER BY po.OrderDate DESC, i.ItemName"
                ).ToList();
            }
        }
    }
}