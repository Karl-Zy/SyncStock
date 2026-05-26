using Dapper;
using SyncStock.Models;
using SyncStock.Models.Accounts;
using SyncStock.Models.Item;
using SyncStock.Models.Models_Receiving_;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System;

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
                return conn.QueryFirstOrDefault<Item>(
                    "SELECT * FROM Items WHERE ItemID = @ItemID",
                    new { ItemID = itemId });
            }
        }

        public int AddItem(string itemName)
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(
                    "INSERT INTO Items (ItemName) VALUES (@ItemName); SELECT SCOPE_IDENTITY();",
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
                return conn.QueryFirstOrDefault<Departments>(
                    "SELECT * FROM Departments WHERE DepartmentID = @DepartmentID",
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
                return conn.QueryFirstOrDefault<PurchaseOrders>(
                    "SELECT * FROM PurchaseOrders WHERE PurchaseOrderID = @PurchaseOrderID",
                    new { PurchaseOrderID = purchaseOrderId });
            }
        }

        public int AddPurchaseOrder(PurchaseOrders order)
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(@"
                    INSERT INTO PurchaseOrders (InvoiceNumber, PONumber, DepartmentID, OrderDate, Status, Priority, Remarks, AttachmentPath) 
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
                return conn.Query<PurchaseOrderItem>(@"
            SELECT poi.PurchaseOrderItemID AS PurchaseOrderItemID, poi.PurchaseOrderID, poi.ItemID,
                   poi.Quantity, poi.UnitPrice, i.ItemName
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
                conn.Execute(@"
                    INSERT INTO PurchaseOrderItems (PurchaseOrderID, ItemID, Quantity, UnitPrice) 
                    VALUES (@PurchaseOrderID, @ItemID, @Quantity, @UnitPrice)", items);
            }
        }

        public void AddPurchaseOrderItem(PurchaseOrderItem item)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
                    INSERT INTO PurchaseOrderItems (PurchaseOrderID, ItemID, Quantity, UnitPrice)
                    VALUES (@PurchaseOrderID, @ItemID, @Quantity, @UnitPrice)", item);
            }
        }

        public IEnumerable<PurchaseOrderItem> GetAllPurchaseOrderItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrderItem>(@"
            SELECT poi.PurchaseOrderItemID AS PurchaseOrderItemID, poi.PurchaseOrderID, poi.ItemID,
                   poi.Quantity, poi.UnitPrice, i.ItemName
            FROM PurchaseOrderItems poi
            INNER JOIN Items i ON poi.ItemID = i.ItemID");
            }
        }

        #region Dashboard

        public int GetPendingOrdersCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM PurchaseOrders WHERE Status = @Status",
                    new { Status = WorkflowStatus.Pending });
            }
        }

        public List<PurchaseOrders> GetASAPOrders()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrders>(@"
                    SELECT po.*, d.DepartmentName
                    FROM PurchaseOrders po
                    JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    WHERE po.Priority IN ('ASAP Department', 'ASAP')
                    ORDER BY po.OrderDate DESC").ToList();
            }
        }

        public void ApprovePurchaseOrder(int purchaseOrderId)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(
                    "UPDATE PurchaseOrders SET Status = @Status WHERE PurchaseOrderID = @PurchaseOrderID",
                    new { Status = WorkflowStatus.Approved, PurchaseOrderID = purchaseOrderId });
            }
        }

        public IEnumerable<PurchaseOrders> GetPendingPurchaseOrders()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrders>(@"
                    SELECT po.*, d.DepartmentName 
                    FROM PurchaseOrders po 
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    WHERE po.Status = @Status",
                    new { Status = WorkflowStatus.Pending });
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
                   COUNT(poi.PurchaseOrderItemID) AS TotalItems,
                   SUM(poi.Quantity * poi.UnitPrice) AS TotalAmount
            FROM PurchaseOrders po
            INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
            LEFT JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
            WHERE po.Status = @Status
            GROUP BY po.PONumber, d.DepartmentName, po.OrderDate, po.Priority, po.Status",
                    new { Status = WorkflowStatus.Pending });
            }
        }

        #endregion

        #region Receiving Custodian

        public IEnumerable<PendingIncomingItem> GetPendingIncomingItemsDetails()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PendingIncomingItem>(@"
                    SELECT 
                        po.PONumber,
                        d.DepartmentName AS Department,
                        i.ItemName,
                        poi.Quantity AS Quantity,
                        (poi.Quantity * poi.UnitPrice) AS Amount,
                        po.OrderDate AS DateOrdered,
                        po.Status
                    FROM PurchaseOrders po
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    INNER JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
                    INNER JOIN Items i ON poi.ItemID = i.ItemID
                    WHERE po.Status = @Status",
                    new { Status = WorkflowStatus.Pending });
            }
        }

        public void AddConfirmedItem(ConfirmedItems item)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
                    INSERT INTO ConfirmedItems (
                        PONumber, ItemName, DateReceived, IsCapitalizable, 
                        ExpectedQuantity, ReceivedQuantity, ExpectedAmount, 
                        ReceivedAmount, AttachmentPath, Remarks
                    ) VALUES (
                        @PONumber, @ItemName, @DateReceived, @IsCapitalizable, 
                        @ExpectedQuantity, @ReceivedQuantity, @ExpectedAmount, 
                        @ReceivedAmount, @AttachmentPath, @Remarks
                    );", item);
            }
        }

        public void UpdatePurchaseOrderStatus(string poNumber, string newStatus)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
                    UPDATE PurchaseOrders
                    SET Status = @newStatus
                    WHERE PONumber = @poNumber",
                    new { poNumber, newStatus });
            }
        }

        #endregion

        #region Reports

        public IEnumerable<ReportItem> GetAllReportItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ReportItem>(@"
                    SELECT
                        po.PurchaseOrderID,
                        po.InvoiceNumber,
                        po.PONumber,
                        po.DepartmentID,
                        d.DepartmentName,
                        po.OrderDate,
                        po.Status,
                        po.Priority,
                        po.Remarks,
                        po.AttachmentPath,
                        i.ItemName,
                        poi.Quantity,
                        poi.UnitPrice AS BuyingPrice,
                        (poi.Quantity * poi.UnitPrice) AS Amount
                    FROM PurchaseOrders po
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    INNER JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
                    INNER JOIN Items i ON poi.ItemID = i.ItemID
                    WHERE po.Status = @Status
                    ORDER BY po.OrderDate DESC, po.PONumber, i.ItemName",
                    new { Status = WorkflowStatus.Approved });
            }
        }

        public IEnumerable<ApprovedPurchaseOrder> GetAllApprovedMonthlyCost()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ApprovedPurchaseOrder>(@"
            SELECT
                po.PONumber,
                d.DepartmentName,
                po.OrderDate,
                po.Priority,
                po.Status,
                COUNT(poi.PurchaseOrderItemID) AS TotalItems,
                SUM(poi.Quantity * poi.UnitPrice) AS TotalAmount
            FROM PurchaseOrders po
            INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
            LEFT JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
            WHERE po.Status = @Status
              AND MONTH(po.OrderDate) = MONTH(GETDATE())
              AND YEAR(po.OrderDate) = YEAR(GETDATE())
            GROUP BY po.PONumber, d.DepartmentName, po.OrderDate, po.Priority, po.Status",
                    new { Status = WorkflowStatus.Approved });
            }
        }

        public int GetAllApprovedTotalItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(@"
                    SELECT ISNULL(SUM(poi.Quantity), 0)
                    FROM PurchaseOrderItems poi
                    INNER JOIN PurchaseOrders po ON poi.PurchaseOrderID = po.PurchaseOrderID
                    WHERE po.Status = @Status
                      AND MONTH(po.OrderDate) = MONTH(GETDATE())
                      AND YEAR(po.OrderDate) = YEAR(GETDATE())",
                    new { Status = WorkflowStatus.Approved });
            }
        }

        #endregion

        #region Auditor Review

        public IEnumerable<AuditorReviewItemDto> GetAuditorReviewItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<AuditorReviewItemDto>(@"
                    SELECT
                        ci.ConfirmedItemID AS PurchaseOrderItemID,
                        ci.PONumber,
                        ci.ItemName,
                        po.InvoiceNumber,
                        CASE WHEN ci.ReceivedQuantity > 0
                             THEN ci.ReceivedAmount / ci.ReceivedQuantity
                             ELSE 0 END AS UnitPrice,
                        ci.ReceivedQuantity AS Quantity,
                        ci.ReceivedAmount AS TotalAmount,
                        ci.DateReceived,
                        CASE WHEN ci.IsCapitalizable = 1 THEN 'Yes' ELSE 'No' END AS Capitalizable,
                        d.DepartmentName AS Department,
                        po.Status
                    FROM ConfirmedItems ci
                    INNER JOIN PurchaseOrders po ON ci.PONumber = po.PONumber
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    ORDER BY ci.DateReceived DESC, ci.PONumber, ci.ItemName");
            }
        }

        #endregion

        public IEnumerable<CartItems> GetAllCartItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<CartItems>(@"
            SELECT *
            FROM CartItems
            ORDER BY CreatedAt DESC");
            }
        }

        public void AddCartItem(CartItems cartItem)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
            INSERT INTO CartItems
            (
                ItemName,
                Quantity,
                UnitPrice,
                InvoiceNumber,
                PONumber,
                OrderDate,
                CartType    
            )
            VALUES
            (
                @ItemName,
                @Quantity,
                @UnitPrice,
                @InvoiceNumber,
                @PONumber,
                @OrderDate,
                @CartType
            )", cartItem);
            }

        }
        public void ClearCart()
        {
            using (var conn = CreateConnection())
            {
                conn.Execute("DELETE FROM CartItems");
            }
        }
        public IEnumerable<PurchaseOrders> GetPurchaseOrderBrief()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrders>(@"
            SELECT po.*, d.DepartmentName
            FROM PurchaseOrders po
            INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
            ORDER BY po.OrderDate DESC");
            }
        }
        // In Repository.cs

        public MonthLock GetMonthLock(DateTime monthYear)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<MonthLock>(
                    "SELECT * FROM MonthLocks WHERE MonthYear = @MonthYear AND IsLocked = 1",
                    new { MonthYear = new DateTime(monthYear.Year, monthYear.Month, 1) });
            }
        }

        public void SaveMonthLock(MonthLock monthLock)
        {
            using (var conn = CreateConnection())
            {
                // Normalize to 1st of the month
                monthLock.MonthYear = new DateTime(monthLock.MonthYear.Year, monthLock.MonthYear.Month, 1);

                int exists = conn.ExecuteScalar<int>(
                    "SELECT COUNT(1) FROM MonthLocks WHERE MonthYear = @MonthYear",
                    new { monthLock.MonthYear });

                if (exists > 0)
                {
                    conn.Execute(@"
                UPDATE MonthLocks
                SET IsLocked       = @IsLocked,
                    LockedByUserId = @LockedByUserId,
                    LockedAt       = @LockedAt
                WHERE MonthYear = @MonthYear", monthLock);
                }
                else
                {
                    conn.Execute(@"
                INSERT INTO MonthLocks (MonthYear, IsLocked, LockedByUserId, LockedAt)
                VALUES (@MonthYear, @IsLocked, @LockedByUserId, @LockedAt)", monthLock);
                }
            }
        }

        public void SaveUnlockRequest(UnlockRequest request)
        {
            using (var conn = CreateConnection())
            {
                request.MonthYear = new DateTime(request.MonthYear.Year, request.MonthYear.Month, 1);

                conn.Execute(@"
            INSERT INTO UnlockRequests (MonthYear, RequestedByUserId, RequestedAt, Status)
            VALUES (@MonthYear, @RequestedByUserId, @RequestedAt, @Status)", request);
            }
        }
    }
}

