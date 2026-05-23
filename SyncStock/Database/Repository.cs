using Dapper;
using SyncStock.Models;
using SyncStock.Models.Accounts;
using SyncStock.Models.Item;
using SyncStock.Models.Models_Receiving_;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
                    (@"SELECT poi.POItemID AS PurchaseOrderItemID, poi.PurchaseOrderID, poi.ItemID,
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
            SELECT poi.POItemID AS PurchaseOrderItemID, poi.PurchaseOrderID, poi.ItemID,
                   poi.Quantity, poi.UnitPrice, i.ItemName
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
                            AND po.Priority = 'ASAP'
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

        public IEnumerable<PendingIncomingItem> GetPendingIncomingItemsDetails()
        {
            string query = @"
        SELECT 
            po.PONumber,
            'N/A' AS Purchaser, 
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
        WHERE po.Status = 'Pending'";

            using (var conn = CreateConnection())
            {
                // Dapper safely maps the SQL query rows straight into your new class structure
                return conn.Query<PendingIncomingItem>(query);
            }
        }

        public void AddConfirmedItem(ConfirmedItems item)
        {
            string query = @"
        INSERT INTO ConfirmedItems (
            PONumber, ItemName, DateReceived, IsCapitalizable, 
            ExpectedQuantity, ReceivedQuantity, ExpectedAmount, 
            ReceivedAmount, AttachmentPath, Remarks
        ) VALUES (
            @PONumber, @ItemName, @DateReceived, @IsCapitalizable, 
            @ExpectedQuantity, @ReceivedQuantity, @ExpectedAmount, 
            @ReceivedAmount, @AttachmentPath, @Remarks
        );";

            using (var conn = CreateConnection())
            {
                conn.Execute(query, item);
            }
        }

        public void UpdatePurchaseOrderItemStatus(string poNumber, string itemName, string newStatus)
        {
            // FIX: Update the parent PurchaseOrders table directly using the PONumber
            string query = @"
        UPDATE PurchaseOrders
        SET Status = @newStatus
        WHERE PONumber = @poNumber";

            using (var conn = CreateConnection())
            {
                // Dapper safely maps the parameters and executes the update
                conn.Execute(query, new { poNumber, newStatus });
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

        

        /// <summary>
        /// PO line items not yet confirmed by the receiving custodian.
        /// </summary>
        public IEnumerable<PendingReceivingItemDto> GetPendingReceivingItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PendingReceivingItemDto>(@"
                    SELECT
                        poi.POItemID AS PurchaseOrderItemID,
                        poi.PurchaseOrderID,
                        po.PONumber,
                        i.ItemName,
                        d.DepartmentName AS Department,
                        poi.Quantity AS ExpectedQuantity,
                        poi.UnitPrice
                    FROM PurchaseOrderItems poi
                    INNER JOIN PurchaseOrders po ON poi.PurchaseOrderID = po.PurchaseOrderID
                    INNER JOIN Items i ON poi.ItemID = i.ItemID
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM ReceivingReports rr
                        WHERE rr.POItemID = poi.POItemID
                    )
                    ORDER BY po.OrderDate DESC, po.PONumber, i.ItemName");
            }
        }

        /// <summary>
        /// Records receiving custodian acceptance for a purchase order line item.
        /// </summary>
        public int AcceptReceivingReport(ReceivingReport report)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    int reportId = conn.ExecuteScalar<int>(@"
                        INSERT INTO ReceivingReports
                        (
                            POItemID,
                            DateReceived,
                            ReceivedQuantity,
                            ReceivedAmount,
                            IsCapitalizable,
                            Remarks,
                            ProofOfDeliveryPath,
                            AcceptedAt,
                            AuditorStatus
                        )
                        VALUES
                        (
                            @PurchaseOrderItemID,
                            @DateReceived,
                            @ReceivedQuantity,
                            @ReceivedAmount,
                            @IsCapitalizable,
                            @Remarks,
                            @ProofOfDeliveryPath,
                            @AcceptedAt,
                            @AuditorStatus
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        report,
                        tx);

                    conn.Execute(@"
                        UPDATE PurchaseOrders
                        SET Status = @Status
                        WHERE PurchaseOrderID = (
                            SELECT PurchaseOrderID
                            FROM PurchaseOrderItems
                            WHERE POItemID = @PurchaseOrderItemID
                        )",
                        new
                        {
                            Status = WorkflowStatus.AcceptedByCustodian,
                            report.PurchaseOrderItemID
                        },
                        tx);

                    tx.Commit();
                    return reportId;
                }
            }
        }

        /// <summary>
        /// Line items accepted by the receiving custodian, ready for internal auditor review.
        /// </summary>
        public IEnumerable<AuditorReviewItemDto> GetAuditorReviewItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<AuditorReviewItemDto>(@"
                    SELECT
                        poi.POItemID AS PurchaseOrderItemID,
                        po.PONumber,
                        i.ItemName,
                        po.InvoiceNumber,
                        poi.UnitPrice,
                        rr.ReceivedQuantity AS Quantity,
                        rr.ReceivedAmount AS TotalAmount,
                        rr.DateReceived,
                        CASE
                            WHEN rr.IsCapitalizable = 1 THEN 'Yes'
                            ELSE 'No'
                        END AS Capitalizable,
                        d.DepartmentName AS Department,
                        rr.AuditorStatus AS Status
                    FROM ReceivingReports rr
                    INNER JOIN PurchaseOrderItems poi ON rr.POItemID = poi.POItemID
                    INNER JOIN PurchaseOrders po ON poi.PurchaseOrderID = po.PurchaseOrderID
                    INNER JOIN Items i ON poi.ItemID = i.ItemID
                    INNER JOIN Departments d ON po.DepartmentID = d.DepartmentID
                    ORDER BY rr.DateReceived DESC, po.PONumber, i.ItemName");
            }
        }
    }
}