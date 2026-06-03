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
    // Repository class   nag-handle sa tanan nga database operations sa sistema (OOP: Single Responsibility Principle)
    public class Repository
    {
        public Repository() { }

        // Nag-create og SqlConnection pinaagi sa DatabaseHelper para dili mag-hardcode sa connection string sa matag method
        private SqlConnection CreateConnection() => DatabaseHelper.GetConnection();

        // Gi-kuha ang tanan nga Items gikan sa database para magamit sa UI o logic layer
        public IEnumerable<Item> GetAllItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<Item>("SELECT * FROM Items");
            }
        }

        // Gi-validate ang user pinaagi sa username ug password para sa login authentication
        public User GetUserByCredentials(string userName, string password)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<User>(
                    "SELECT * FROM Users WHERE UserName = @UserName AND Password = @Password",
                    new { UserName = userName, Password = password });
            }
        }

        // Gi-kuha ang user base sa RFID UID para sa contactless login feature
        public User GetUserByRfid(string rfidUID)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<User>(
                    "SELECT * FROM Users WHERE RfidUID = @RfidUID",
                    new { RfidUID = rfidUID });
            }
        }

        // Gi-fetch ang usa ka Item base sa ItemID para magamit sa edit o view operations
        public Item GetItemById(int itemId)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<Item>(
                    "SELECT * FROM Items WHERE ItemID = @ItemID",
                    new { ItemID = itemId });
            }
        }

        // Gi-insert ang bag-ong Item sa database ug gi-balik ang bag-ong generated ItemID
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

        // Gi-kuha ang tanan nga Departments para magamit sa dropdowns o department-related displays
        public IEnumerable<Departments> GetAllDepartments()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<Departments>("SELECT * FROM Departments");
            }
        }

        // Gi-fetch ang usa ka Department base sa DepartmentID para sa detail view o validation
        public Departments GetDepartmentById(int departmentId)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<Departments>(
                    "SELECT * FROM Departments WHERE DepartmentID = @DepartmentID",
                    new { DepartmentID = departmentId });
            }
        }

        // Gi-kuha ang tanan nga Purchase Orders para sa grid display o reporting
        public IEnumerable<PurchaseOrders> GetAllPurchaseOrder()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrders>("SELECT * FROM PurchaseOrders");
            }
        }

        // Gi-fetch ang usa ka Purchase Order base sa ID para sa detail view o approval workflow
        public PurchaseOrders GetPurchaseORderById(int purchaseOrderId)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<PurchaseOrders>(
                    "SELECT * FROM PurchaseOrders WHERE PurchaseOrderID = @PurchaseOrderID",
                    new { PurchaseOrderID = purchaseOrderId });
            }
        }

        // Gi-insert ang bag-ong Purchase Order ug gi-balik ang bag-ong PurchaseOrderID para magamit sa pag-add sa items
        public int AddPurchaseOrder(PurchaseOrders order)
        {
            using (var conn = CreateConnection())
            {
                int newId = conn.ExecuteScalar<int>(@"
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

                // =====================================
                // NOTIFICATION
                // =====================================

                if (order.Priority == "ASAP Department")
                {
                    AddNotification(
                        "New ASAP Order",
                        $"ASAP Purchase Order Created: {order.PONumber}",
                        "ASAP");
                }
                else
                {
                    AddNotification(
                        "New Purchase Order",
                        $"Purchase Order Created: {order.PONumber}",
                        "Purchase");
                }

                return newId;
            }
        }

        // Gi-count ang total nga PurchaseOrders para sa summary o pagination display
        public int GetPurchaseOrderCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM PurchaseOrders");
            }
        }

        // Gi-fetch ang mga items sa usa ka PO (with ItemName) para ipakita ang detail sa order
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

        // Gi-insert ang daghang PO items sa usa ka batch para mas episyente kaysa mag-insert usa-usa
        public void AddPurchaseOrderItems(IEnumerable<PurchaseOrderItem> items)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
                    INSERT INTO PurchaseOrderItems (PurchaseOrderID, ItemID, Quantity, UnitPrice) 
                    VALUES (@PurchaseOrderID, @ItemID, @Quantity, @UnitPrice)", items);
            }
        }

        // Gi-insert ang usa ka PO item para sa single-item add operation
        public void AddPurchaseOrderItem(PurchaseOrderItem item)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
                    INSERT INTO PurchaseOrderItems (PurchaseOrderID, ItemID, Quantity, UnitPrice)
                    VALUES (@PurchaseOrderID, @ItemID, @Quantity, @UnitPrice)", item);
            }
        }

        // Gi-kuha ang tanan nga PO items (with ItemName) para sa global item list view
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

        // Gi-count ang normal Pending orders lang (gi-exclude ang ASAP) para dili malangkit ang ASAP sa dashboard pending count
        public int GetPendingOrdersCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(@"
                    SELECT COUNT(*)
                    FROM PurchaseOrders
                    WHERE Status = @Status
                    AND Priority <> 'ASAP Department'",
                    new { Status = WorkflowStatus.Pending });
            }
        }

        // Gi-kuha ang tanan nga ASAP orders (bisan unsa pa ang status) para ipakita sa kaugalingon nga ASAP grid
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

            -- gi-kuha tanan nga ASAP orders bisan unsa pa ang status
            -- (Pending, Approved, Received, etc.) para makita tanan sa ASAP grid
            WHERE po.Status = 'Pending'
            AND po.Priority = 'ASAP Department'

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

        // Gi-update ang status sa PO ngadto sa Approved para ipadayon ang workflow
        public void ApprovePurchaseOrder(int purchaseOrderId)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(
                    "UPDATE PurchaseOrders SET Status = @Status WHERE PurchaseOrderID = @PurchaseOrderID",
                    new { Status = WorkflowStatus.Approved, PurchaseOrderID = purchaseOrderId });
            }
        }

        // Gi-exclude ang ASAP orders sa pending list kay aduna silay kaugalingon nga grid sa dashboard
        public IEnumerable<PurchaseOrders> GetPendingPurchaseOrders()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrders>(@"
                    SELECT po.*, d.DepartmentName 
                    FROM PurchaseOrders po 
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    WHERE po.Status = @Status
                    AND po.Priority <> 'ASAP Department'",
                    new { Status = WorkflowStatus.Pending });
            }
        }

        // Gi-kuha ang pending order summary (with TotalItems ug TotalAmount) para sa dashboard overview, gi-exclude ang ASAP
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
        AND po.Priority <> 'ASAP Department'

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

        // Gi-count ang tanan nga Approved orders para ipakita sa dashboard metric card
        public int GetApprovedOrdersCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM PurchaseOrders WHERE Status = @Status",
                    new { Status = WorkflowStatus.Approved });
            }
        }

        // Gi-sum ang total value sa tanan nga Approved PO items para sa financial overview
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

        // Gi-count ang tanan nga Items para ipakita sa dashboard inventory count card
        public int GetItemsCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Items");
            }
        }

        // Gi-count ang tanan nga Departments para ipakita sa dashboard summary
        public int GetDepartmentsCount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Departments");
            }
        }

        // Gi-compute ang average order value sa tanan nga PO para sa financial analytics
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

        // Gi-kuha ang monthly purchase totals para ipakita sa chart o trend analysis sa dashboard
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

        // Gi-kuha ang count sa Capitalizable vs Non-Capitalizable assets para sa pie chart sa dashboard
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

        // Gi-sum ang total pending amount (gi-exclude ang ASAP) para dili malangkit ang ASAP total sa normal pending display
        public decimal GetPendingOrdersAmount()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<decimal>(@"
            SELECT ISNULL(SUM(poi.Quantity * poi.UnitPrice), 0)
            FROM PurchaseOrders po
            INNER JOIN PurchaseOrderItems poi
                ON po.PurchaseOrderID = poi.PurchaseOrderID
            WHERE po.Status = 'Pending'
            AND po.Priority <> 'ASAP Department'");
            }
        }

        // Gi-sum ang total value sa tanan nga ASAP orders para sa ASAP-specific financial display
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

        // Gi-sum ang tanan nga PO item values (bisan unsa pa ang status) para sa overall spending overview
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

        // Gi-kuha ang pending incoming items with details para ipakita sa Receiving Custodian module
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

        // Gi-insert ang confirmed/received item sa ConfirmedItems table para ma-track ang actual delivery
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

        // Gi-update ang status sa PurchaseOrder base sa PONumber para ipadayon ang receiving workflow
        public void UpdatePurchaseOrderItemStatus(string poNumber, string itemName, string newStatus)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
            UPDATE PurchaseOrders
            SET Status = @newStatus
            WHERE PONumber = @poNumber",
                    new { poNumber, itemName, newStatus });

                if (newStatus == "Received")
                {
                    AddNotification(
                        "Order Received",
                        $"Purchase Order {poNumber} has been received.",
                        "Receiving");
                }
            }
        }

        #endregion

        #region Reports

        // Gi-kuha ang tanan nga Approved PO items with full details para sa report generation
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

        // Gi-kuha ang all-time approved PO summary (with TotalItems ug TotalAmount) para sa monthly cost report
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
            -- Date filters removed to show all-time totals
            GROUP BY po.PONumber, d.DepartmentName, po.OrderDate, po.Priority, po.Status",
            new { Status = WorkflowStatus.Approved });
            }
        }

        // Gi-sum ang total approved item quantities para sa current month para sa monthly KPI tracking
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

        // Gi-kuha ang tanan nga Capitalizable confirmed items para sa capitalization report
        public IEnumerable<CapitalizedOrder> GetAllCapitalizedOrder()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<CapitalizedOrder>(@"
                  SELECT
                        ci.ConfirmedItemID,
                        ci.PONumber,
                        po.InvoiceNumber,
                        ci.ItemName,
                        ci.DateReceived,
                        ci.IsCapitalizable,
                        ci.ExpectedQuantity,
                        ci.ReceivedQuantity,
                        ci.ExpectedAmount,
                        ci.ReceivedAmount,
                        ci.Remarks,
                        po.POType,
                        po.OrderMode
                  FROM ConfirmedItems ci
                  INNER JOIN PurchaseOrders po ON ci.PONumber = po.PONumber
                  WHERE IsCapitalizable = 1
                  ORDER BY  DateReceived DESC");
            }
        }

        // Gi-kuha ang brief summary sa matag PO (with Quantity ug TotalPrice) para sa compact report view
        public IEnumerable<PurchaseOrderBrief> GetPurchaseOrderBrief()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrderBrief>(@"
        SELECT 
            po.PONumber,
            po.InvoiceNumber,
            
            -- because ReportUC filter uses [OrderDate]
            po.OrderDate AS OrderDate,

            po.POType,

            po.OrderMode,

            SUM(poi.Quantity) AS Quantity,

            SUM(poi.Quantity * poi.UnitPrice) AS TotalPrice

        FROM PurchaseOrders po

        INNER JOIN PurchaseOrderItems poi
            ON po.PurchaseOrderID = poi.PurchaseOrderID

        GROUP BY
            po.PONumber,
            po.InvoiceNumber,
            po.OrderDate,
            po.POType,
            po.OrderMode

        ORDER BY po.OrderDate DESC");
            }
        }

        // Gi-update ang IsCapitalizable flag sa ConfirmedItem para ma-reclassify ang asset kung kinahanglan
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

        // Gi-kuha ang tanan nga Non-Capitalizable received items para sa regular received items report
        public IEnumerable<ReceivedItemReports> GetAllReceivedOrders()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ReceivedItemReports>(@"
            SELECT 
                ci.ConfirmedItemID,
                ci.PONumber,
                po.InvoiceNumber,
                ci.ItemName,
                ci.DateReceived,
                ci.ExpectedQuantity,
                ci.ReceivedQuantity,
                ci.ExpectedAmount,
                ci.ReceivedAmount,
                ci.Remarks,
                po.POType,        
                po.OrderMode  
            FROM ConfirmedItems ci
            INNER JOIN PurchaseOrders po  
              ON ci.PONumber = po.PONumber
            WHERE ci.IsCapitalizable = 0
            ORDER BY ci.DateReceived DESC");
            }
        }

        // Gi-kuha ang reconciliation data (ordered vs received) para makita ang discrepancy sa PO ug actual delivery
        public IEnumerable<Reconciliation> GetReconciliationItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<Reconciliation>(@"
            SELECT
                po.PONumber,
                po.InvoiceNumber,   
                d.DepartmentName,
                po.OrderDate,
                po.Priority,
                i.ItemName,
                poi.Quantity        AS OrderedQuantity,
                poi.UnitPrice,
                poi.TotalPrice      AS OrderedAmount,
                ci.DateReceived,
                ci.ExpectedQuantity,
                ci.ReceivedQuantity,
                ci.ExpectedAmount,
                ci.ReceivedAmount,
                ci.Remarks,
                po.POType,
                po.OrderMode,
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

        // Gi-compute ang total approved cost para sa current month para sa monthly budget monitoring
        public decimal GetCurrentMonthApprovedCost()
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<decimal>(@"
            SELECT ISNULL(SUM(poi.Quantity * poi.UnitPrice), 0)
            FROM PurchaseOrderItems poi
            INNER JOIN PurchaseOrders po ON poi.PurchaseOrderID = po.PurchaseOrderID
            WHERE po.Status = @Status
              AND MONTH(po.OrderDate) = MONTH(GETDATE())
              AND YEAR(po.OrderDate)  = YEAR(GETDATE())",
                    new { Status = WorkflowStatus.Approved });
            }
        }

        #endregion

        #region Auditor Review

        // Gi-kuha ang combined list sa pending ug confirmed items para sa auditor review (UNION ALL para makita tanan)
        public List<AuditorReviewItemDto> GetAuditorReviewItems()
        {
            using (var conn = CreateConnection())
            {
                string query = @"

-- =========================================
-- PENDING PURCHASES
-- =========================================

SELECT
    0 AS ConfirmedItemID,

    po.PONumber,
    i.ItemName,
    po.InvoiceNumber,

    poi.UnitPrice,

    poi.Quantity AS Quantity,
    poi.TotalPrice AS TotalAmount,

    po.OrderDate AS DateReceived,

    'No' AS Capitalizable,

    d.DepartmentName AS Department,

    'Pending Review' AS Status,

    po.Priority,

    po.POType,
    po.OrderMode,

    '' AS Remarks,

-- ADD THESE PLACEHOLDERS
poi.Quantity AS ExpectedQuantity,
poi.TotalPrice AS ExpectedAmount,
0 AS ReceivedQuantity,
0 AS ReceivedAmount,
CAST(0 AS BIT) AS IsCapitalizable

FROM PurchaseOrders po

INNER JOIN PurchaseOrderItems poi
    ON po.PurchaseOrderID = poi.PurchaseOrderID

INNER JOIN Items i
    ON poi.ItemID = i.ItemID

INNER JOIN Departments d
    ON po.DepartmentID = d.DepartmentID

WHERE NOT EXISTS
(
    SELECT 1
    FROM ConfirmedItems ci
    WHERE ci.PONumber = po.PONumber
    AND ci.ItemName = i.ItemName
)


UNION ALL

-- =========================================
-- CONFIRMED / ACTIVE ITEMS
-- =========================================

SELECT
    ci.ConfirmedItemID,

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

    ISNULL(ci.Status, CASE WHEN ci.IsCapitalizable = 1 THEN 'Active' ELSE 'Received' END) AS Status,

    po.Priority,

    po.POType,
    po.OrderMode,

    ci.Remarks,

    -- ADD THESE
    ci.ExpectedQuantity,
    ci.ExpectedAmount,
    ci.ReceivedQuantity,
    ci.ReceivedAmount,
    ci.IsCapitalizable

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

ORDER BY DateReceived DESC";

                return conn.Query<AuditorReviewItemDto>(query).ToList();
            }
        }

        #endregion

        // Gi-kuha ang tanan nga cart items (sorted by newest) para ipakita sa shopping cart UI
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

        // Gi-insert ang usa ka item sa CartItems table para ma-stage ang order bago i-submit bilang PO
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

        // Gi-delete ang tanan nga cart items para ma-reset ang cart pagkahuman mag-submit og PO
        public void ClearCart()
        {
            using (var conn = CreateConnection())
            {
                conn.Execute("DELETE FROM CartItems");
            }
        }

        // Gi-filter ang cart items base sa CartType (OPO o GPO) para mas episyente ang cart management
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

        // Gi-clear ang cart base sa CartType para dili maapektuhan ang lain nga type sa cart
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

        // Gi-delete ang usa ka cart item base sa CartItemID gamit ang raw SqlCommand para sa direct low-level delete
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

        // Gi-kuha ang tanan nga OPO (One-time Purchase Order) items with full details para sa OPO-specific report o grid
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
    AND po.Status <> 'Received'

    ORDER BY poi.PurchaseOrderItemID DESC");
            }
        }

        // Gi-kuha ang tanan nga GPO (General Purchase Order) items with full details para sa GPO-specific report o grid
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
    AND po.Status <> 'Received'

    ORDER BY poi.PurchaseOrderItemID DESC");
            }
        }

        // Gi-check kung ang usa ka buwan naka-lock na para mapigilan ang mga edit o submission sa locked period
        public MonthLock GetMonthLock(DateTime monthYear)
        {
            using (var conn = CreateConnection())
            {
                return conn.QueryFirstOrDefault<MonthLock>(
                    "SELECT * FROM MonthLocks WHERE MonthYear = @MonthYear AND IsLocked = 1",
                    new { MonthYear = new DateTime(monthYear.Year, monthYear.Month, 1) });
            }
        }

        // Gi-save (INSERT o UPDATE) ang MonthLock record para ma-manage ang month locking feature
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

        // Gi-insert ang unlock request sa database para ma-track kung kinsa ang nag-request og unlock ug kanus-a
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

        // Gi-kuha ang distinct years gikan sa PurchaseOrders ug ConfirmedItems para sa year filter sa reports
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

        // Gi-update ang ConfirmedItem record para ma-edit ang receiving details kung adunay kausaban
        public void UpdateConfirmedItem(
    int confirmedItemId,
    ConfirmedItems item)
        {
            using (var conn = CreateConnection())
            {
                string query = @"
UPDATE ConfirmedItems
SET
    DateReceived = @DateReceived,
    IsCapitalizable = @IsCapitalizable,
    ReceivedQuantity = @ReceivedQuantity,
    ReceivedAmount = @ReceivedAmount,
    Remarks = @Remarks,
    Status = @Status,
    AttachmentData = ISNULL(@AttachmentData, AttachmentData),
    AttachmentFileName = ISNULL(@AttachmentFileName, AttachmentFileName)
WHERE ConfirmedItemID = @ConfirmedItemID";

                conn.Execute(query, new
                {
                    ConfirmedItemID = confirmedItemId,
                    item.DateReceived,
                    item.IsCapitalizable,
                    item.Status,
                    item.ReceivedQuantity,
                    item.ReceivedAmount,
                    item.Remarks,
                    item.AttachmentData,
                    item.AttachmentFileName
                });
            }
        }

        // Gi-count ang tanan nga Received orders para ipakita sa dashboard metric card
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

        // Gi-sum ang total value sa tanan nga Received orders para sa received spending overview
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

        #region Notifications

        public void AddNotification(
            string title,
            string message,
            string notificationType)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
        INSERT INTO Notifications
        (
            Title,
            Message,
            NotificationType
        )
        VALUES
        (
            @Title,
            @Message,
            @NotificationType
        )",
                new
                {
                    Title = title,
                    Message = message,
                    NotificationType = notificationType
                });
            }
        }

        public List<Notification> GetUnreadNotifications()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<Notification>(@"
        SELECT *
        FROM Notifications
        WHERE IsRead = 0
        ORDER BY CreatedAt DESC")
                .ToList();
            }
        }

        public void MarkNotificationAsRead(int notificationId)
        {
            using (var conn = CreateConnection())
            {
                conn.Execute(@"
        UPDATE Notifications
        SET IsRead = 1
        WHERE NotificationID = @NotificationID",
                new { NotificationID = notificationId });
            }
        }

        #endregion
    }
}