using Dapper;
using SyncStock.Models;
using SyncStock.Models.Accounts;
using SyncStock.Models.Item;
using SyncStock.Models.Models_Receiving_;
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
                return conn.ExecuteScalar<int>(@"
            INSERT INTO PurchaseOrders 
                (InvoiceNumber, PONumber, DepartmentID, OrderDate, Status, Priority, Remarks, AttachmentPath, POType) 
            VALUES 
                (@InvoiceNumber, @PONumber, @DepartmentID, @OrderDate, @Status, @Priority, @Remarks, @AttachmentPath, @POType);
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
                        ReceivedAmount, Remarks, Status,
                        AttachmentData, AttachmentFileName
                    ) VALUES (
                        @PONumber, @ItemName, @DateReceived, @IsCapitalizable,
                        @ExpectedQuantity, @ReceivedQuantity, @ExpectedAmount,
                        @ReceivedAmount, @Remarks, @Status,
                        @AttachmentData, @AttachmentFileName
                    );";
            using (var conn = CreateConnection())
            {
                conn.Execute(query, item);
            }
        }

        public void UpdatePurchaseOrderItemStatus(string poNumber, string itemName, string newStatus)
        {
            string query = @"
                    UPDATE PurchaseOrders
                    SET Status = @newStatus
                    WHERE PONumber = @poNumber
                    AND ItemName = @itemName";
            using (var conn = CreateConnection())
            {
                conn.Execute(query, new { poNumber, itemName, newStatus });
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
                ORDER BY po.OrderDate DESC");
            }
        }

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
        public IEnumerable<PurchaseOrderItem> GetAllGPOPurchaseOrderItems()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<PurchaseOrderItem>(@"
            SELECT 
                poi.PurchaseOrderItemID,
                poi.PurchaseOrderID,
                poi.ItemID,
                i.ItemName,
                poi.Quantity,
                poi.UnitPrice,
                (poi.Quantity * poi.UnitPrice) AS TotalPrice
            FROM PurchaseOrderItems poi
            INNER JOIN Items i ON poi.ItemID = i.ItemID
            INNER JOIN PurchaseOrders po ON poi.PurchaseOrderID = po.PurchaseOrderID
            WHERE po.POType = 'GPO'
            ORDER BY poi.PurchaseOrderItemID DESC");
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
                        i.ItemName,
                        poi.Quantity,
                        poi.UnitPrice,
                        (poi.Quantity * poi.UnitPrice) AS TotalPrice
                    FROM PurchaseOrderItems poi
                    INNER JOIN Items i ON poi.ItemID = i.ItemID
                    INNER JOIN PurchaseOrders po ON poi.PurchaseOrderID = po.PurchaseOrderID
                    WHERE po.POType = 'OPO'
                    ORDER BY poi.PurchaseOrderItemID DESC");
            }
        }
    }
}