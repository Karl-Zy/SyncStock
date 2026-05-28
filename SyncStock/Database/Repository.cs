using Dapper;
using SyncStock.Models;
using SyncStock.Models.Accounts;
using SyncStock.Models.Item;
using SyncStock.Models.Models_Receiving_;
using SyncStock.Models.Reports;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

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
                return conn.ExecuteScalar<int>(@"
            INSERT INTO Items (ItemName)
            VALUES (@ItemName);

            SELECT CAST(SCOPE_IDENTITY() as int);",
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
            INSERT INTO PurchaseOrders
            (
                InvoiceNumber,
                PONumber,
                DepartmentID,
                OrderDate,
                Status,
                Priority,
                Remarks,
                AttachmentPath,
                POType,
                OrderMode
            )
            VALUES
            (
                @InvoiceNumber,
                @PONumber,
                @DepartmentID,
                @OrderDate,
                @Status,
                @Priority,
                @Remarks,
                @AttachmentPath,
                @POType,
                @OrderMode
            );

            SELECT CAST(SCOPE_IDENTITY() as int);",
                    order);
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
            SELECT 
                poi.PurchaseOrderItemID,
                poi.PurchaseOrderID,
                poi.ItemID,
                poi.Quantity,
                poi.UnitPrice,
                i.ItemName
            FROM PurchaseOrderItems poi
            INNER JOIN Items i 
                ON poi.ItemID = i.ItemID
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
            SELECT
                po.PurchaseOrderID,
                po.InvoiceNumber,
                po.PONumber,
                po.DepartmentID,
                d.DepartmentName,
                po.OrderDate,
                po.Status,
                po.Priority,
                po.POType,
                po.OrderMode,

                COUNT(poi.PurchaseOrderItemID) AS TotalItems,

                SUM(poi.Quantity * poi.UnitPrice) AS TotalAmount

            FROM PurchaseOrders po

            INNER JOIN Departments d
                ON po.DepartmentID = d.DepartmentID

            LEFT JOIN PurchaseOrderItems poi
                ON po.PurchaseOrderID = poi.PurchaseOrderID

            WHERE po.Priority = 'ASAP Department'

            GROUP BY
                po.PurchaseOrderID,
                po.InvoiceNumber,
                po.PONumber,
                po.DepartmentID,
                d.DepartmentName,
                po.OrderDate,
                po.Status,
                po.Priority,
                po.POType,
                po.OrderMode

            ORDER BY po.OrderDate DESC
        ").ToList();
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
        SELECT 
            po.PurchaseOrderID,
            
            ISNULL(po.InvoiceNumber, '') AS InvoiceNumber,

            ISNULL(po.PONumber, '') AS PONumber,

            po.DepartmentID,

            ISNULL(d.DepartmentName, '') AS DepartmentName,

            po.OrderDate,

            ISNULL(po.Status, '') AS Status,

            ISNULL(po.Priority, '') AS Priority,

            ISNULL(po.POType, '') AS POType,

            ISNULL(po.OrderMode, '') AS OrderMode,

            COUNT(poi.PurchaseOrderItemID) AS TotalItems,

            ISNULL(SUM(poi.Quantity * poi.UnitPrice), 0) AS TotalAmount

        FROM PurchaseOrders po

        INNER JOIN Departments d
            ON po.DepartmentID = d.DepartmentID

        LEFT JOIN PurchaseOrderItems poi
            ON po.PurchaseOrderID = poi.PurchaseOrderID

        WHERE po.Status = 'Pending'

        GROUP BY
            po.PurchaseOrderID,
            po.InvoiceNumber,
            po.PONumber,
            po.DepartmentID,
            d.DepartmentName,
            po.OrderDate,
            po.Status,
            po.Priority,
            po.POType,
            po.OrderMode

        ORDER BY po.OrderDate DESC");
            }
        }

        public int GetApprovedOrdersCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM PurchaseOrders WHERE Status = @Status",
                    new { Status = WorkflowStatus.Approved });
            }
        }

        public decimal GetTotalApprovedValue()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<decimal>(@"
            SELECT ISNULL(SUM(poi.Quantity * poi.UnitPrice), 0)
            FROM PurchaseOrderItems poi
            INNER JOIN PurchaseOrders po
                ON poi.PurchaseOrderID = po.PurchaseOrderID
            WHERE po.Status = @Status",
                    new { Status = WorkflowStatus.Approved });
            }

        }

        public int GetItemsCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Items");
            }
        }

        public int GetDepartmentsCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Departments");
            }
        }

        public decimal GetAverageOrderValue()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<decimal>(@"
            SELECT ISNULL(AVG(OrderTotal), 0)
            FROM
            (
                SELECT
                    SUM(poi.Quantity * poi.UnitPrice) AS OrderTotal
                FROM PurchaseOrders po
                INNER JOIN PurchaseOrderItems poi
                    ON po.PurchaseOrderID = poi.PurchaseOrderID
                GROUP BY po.PurchaseOrderID
            ) x");
            }
        }

        public IEnumerable<dynamic> GetMonthlyPurchaseSummary()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query(@"
            SELECT
                DATENAME(MONTH, po.OrderDate) AS [Month],
                MONTH(po.OrderDate) AS MonthNumber,
                SUM(poi.Quantity * poi.UnitPrice) AS TotalAmount
            FROM PurchaseOrders po
            INNER JOIN PurchaseOrderItems poi
                ON po.PurchaseOrderID = poi.PurchaseOrderID
            GROUP BY
                DATENAME(MONTH, po.OrderDate),
                MONTH(po.OrderDate)
            ORDER BY MonthNumber");
            }
        }

        public IEnumerable<dynamic> GetAssetCategorySummary()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query(@"
            SELECT
                CASE
                    WHEN IsCapitalizable = 1
                        THEN 'Capitalizable'
                    ELSE 'Non-Capitalizable'
                END AS Category,
                COUNT(*) AS Total
            FROM ConfirmedItems
            GROUP BY IsCapitalizable");
            }
        }

        public decimal GetPendingOrdersAmount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<decimal>(@"
            SELECT ISNULL(SUM(poi.Quantity * poi.UnitPrice), 0)
            FROM PurchaseOrders po
            INNER JOIN PurchaseOrderItems poi
                ON po.PurchaseOrderID = poi.PurchaseOrderID
            WHERE po.Status = 'Pending'");
            }
        }

        public decimal GetASAPOrdersAmount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<decimal>
                    (@" SELECT ISNULL(SUM(poi.Quantity * poi.UnitPrice), 0)
                    FROM PurchaseOrders po 
                    INNER JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID 
                    WHERE po.Priority = 'ASAP Department'");
            }
        }

        public decimal GetOverallPurchaseValue()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<decimal>(@"
            SELECT ISNULL(SUM(poi.Quantity * poi.UnitPrice), 0)
            FROM PurchaseOrderItems poi");
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
                d.DepartmentName  AS Department,
                i.ItemName,
                poi.Quantity      AS Quantity,
                (poi.Quantity * poi.UnitPrice) AS Amount,
                po.OrderDate      AS DateOrdered,
                po.Status,
                po.POType,
                po.OrderMode
            FROM PurchaseOrders po
            INNER JOIN Departments d          ON po.DepartmentID   = d.DepartmentID
            INNER JOIN PurchaseOrderItems poi ON po.PurchaseOrderID = poi.PurchaseOrderID
            INNER JOIN Items i                ON poi.ItemID         = i.ItemID
            WHERE po.Status = @Status",
                    new { Status = WorkflowStatus.Pending });
            }
        }

        public void AddConfirmedItem(ConfirmedItems item)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
        INSERT INTO ConfirmedItems
        (
            PONumber,
            ItemName,
            DateReceived,
            IsCapitalizable,
            ExpectedQuantity,
            ReceivedQuantity,
            ExpectedAmount,
            ReceivedAmount,
            Remarks,
            Status,
            AttachmentData,
            AttachmentFileName
        )
        VALUES
        (
            @PONumber,
            @ItemName,
            @DateReceived,
            @IsCapitalizable,
            @ExpectedQuantity,
            @ReceivedQuantity,
            @ExpectedAmount,
            @ReceivedAmount,
            @Remarks,
            @Status,
            @AttachmentData,
            @AttachmentFileName
        )", item);
            }
        }

        public void UpdatePurchaseOrderItemStatus(string poNumber, string itemName, string newStatus)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
            UPDATE PurchaseOrders
            SET Status = @newStatus
            WHERE PONumber = @poNumber",
                    new { poNumber, itemName, newStatus });
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

        public IEnumerable<ReceivedItemReports> GetAllReceivedOrders()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ReceivedItemReports>(@"
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

        public IEnumerable<Reconciliation> GetReconciliationItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<Reconciliation>(@"
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

        #endregion

        #region Auditor Review

        public List<AuditorReviewItemDto> GetAuditorReviewItems()
        {
            using (var conn = CreateConnection())
            {
                string query = @"
        SELECT
            po.PONumber,
            i.ItemName,
            po.InvoiceNumber,

            poi.UnitPrice,

            ci.ReceivedQuantity AS Quantity,
            ci.ReceivedAmount AS TotalAmount,

            ci.DateReceived,

            CASE
                WHEN ci.IsCapitalizable = 1 THEN 'Yes'
                ELSE 'No'
            END AS Capitalizable,

            d.DepartmentName AS Department,

            ci.Status,

            po.POType,
            po.OrderMode

        FROM ConfirmedItems ci

        INNER JOIN PurchaseOrders po
            ON ci.PONumber = po.PONumber

        INNER JOIN PurchaseOrderItems poi
            ON po.PurchaseOrderID = poi.PurchaseOrderID

        INNER JOIN Items i
            ON poi.ItemID = i.ItemID
            AND i.ItemName = ci.ItemName

        INNER JOIN Departments d
            ON po.DepartmentID = d.DepartmentID

        ORDER BY ci.DateReceived DESC";

                return conn.Query<AuditorReviewItemDto>(query).ToList();
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

        public IEnumerable<CartItems> GetCartItemsByType(string type)
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<CartItems>(@"
            SELECT *
            FROM CartItems
            WHERE CartType = @CartType
            ORDER BY CreatedAt DESC",
                    new { CartType = type });
            }
        }

        public void ClearCartByType(string type)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
            DELETE FROM CartItems
            WHERE CartType = @CartType",
                    new { CartType = type });
            }
        }

        public void DeleteCartItem(int cartItemId)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "DELETE FROM CartItems WHERE CartItemID = @id", conn);
                cmd.Parameters.AddWithValue("@id", cartItemId);
                cmd.ExecuteNonQuery();
            }
        }
        public IEnumerable<PurchaseOrderItem> GetAllOPOPurchaseOrderItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrderItem>(@"
        SELECT 
            poi.PurchaseOrderItemID,
            poi.PurchaseOrderID,
            poi.ItemID,

            po.PONumber,
            po.InvoiceNumber,
            po.OrderDate,
            po.Remarks,
            po.Priority,
            po.POType,
            po.OrderMode,

            d.DepartmentName,

            i.ItemName,

            poi.Quantity,
            poi.UnitPrice,
            (poi.Quantity * poi.UnitPrice) AS TotalPrice

        FROM PurchaseOrderItems poi

        INNER JOIN PurchaseOrders po
            ON poi.PurchaseOrderID = po.PurchaseOrderID

        INNER JOIN Departments d
            ON po.DepartmentID = d.DepartmentID

        INNER JOIN Items i
            ON poi.ItemID = i.ItemID

        WHERE po.POType = 'OPO'

        ORDER BY poi.PurchaseOrderItemID DESC");
            }
        }

        public IEnumerable<PurchaseOrderItem> GetAllGPOPurchaseOrderItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrderItem>(@"
        SELECT 
            poi.PurchaseOrderItemID,
            poi.PurchaseOrderID,
            poi.ItemID,

            po.PONumber,
            po.InvoiceNumber,
            po.OrderDate,
            po.Remarks,
            po.Priority,
            po.POType,
            po.OrderMode,

            d.DepartmentName,

            i.ItemName,

            poi.Quantity,
            poi.UnitPrice,
            (poi.Quantity * poi.UnitPrice) AS TotalPrice

        FROM PurchaseOrderItems poi

        INNER JOIN PurchaseOrders po
            ON poi.PurchaseOrderID = po.PurchaseOrderID

        INNER JOIN Departments d
            ON po.DepartmentID = d.DepartmentID

        INNER JOIN Items i
            ON poi.ItemID = i.ItemID

        WHERE po.POType = 'GPO'

        ORDER BY poi.PurchaseOrderItemID DESC");
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


        public IEnumerable<int> GetDistinctYear()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<int>(@"
                SELECT DISTINCT YEAR(OrderDate) FROM PurchaseOrders
                UNION
                SELECT DISTINCT YEAR(DateReceived) FROM ConfirmedItems
                ORDER BY 1 DESC").ToList();
            }
        }



        public int GetReceivedOrdersCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(@"
            SELECT COUNT(*)
            FROM PurchaseOrders
            WHERE Status = @Status",
                    new { Status = WorkflowStatus.Received });
            }
        }
        public decimal GetReceivedOrdersAmount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<decimal>(@"
            SELECT ISNULL(SUM(poi.Quantity * poi.UnitPrice), 0)
            FROM PurchaseOrders po
            INNER JOIN PurchaseOrderItems poi
                ON po.PurchaseOrderID = poi.PurchaseOrderID
            WHERE po.Status = @Status",
                    new { Status = WorkflowStatus.Received });
            }
        }




    }
}

