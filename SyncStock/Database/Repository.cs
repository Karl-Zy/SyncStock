using SyncStock.Models.Item;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SyncStock.Models;



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
    }
}