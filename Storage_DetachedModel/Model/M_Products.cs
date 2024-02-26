using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Storage_DetachedModel.Model
{
    internal class M_Products
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int ProductTypeID { get; set; }
        public int SupplierID { get; set; }
        public decimal Cost { get; set; }

        public static List<M_Products> AllProducts { get; private set; }

        static M_Products()
        {
            string connectionString = DatabaseConnectionInfo.GetConnectionString();
            AllProducts = GetAllProducts(connectionString);
        }

        public static List<M_Products> GetAllProducts(string connectionString)
        {
            var productsList = new List<M_Products>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Products";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, connection);

                DataSet ds = new DataSet();
                dataAdapter.Fill(ds, "Products");

                foreach (DataRow row in ds.Tables["Products"].Rows)
                {
                    var product = new M_Products()
                    {
                        ProductID = row["ProductID"] != DBNull.Value ? Convert.ToInt32(row["ProductID"]) : 0,
                        Name = row["Name"].ToString(),
                        ProductTypeID = row["ProductTypeID"] != DBNull.Value ? Convert.ToInt32(row["ProductTypeID"]) : 0,
                        SupplierID = row["SupplierID"] != DBNull.Value ? Convert.ToInt32(row["SupplierID"]) : 0,
                        Cost = row["Cost"] != DBNull.Value ? Convert.ToDecimal(row["Cost"]) : 0m
                    };

                    productsList.Add(product);
                }
            }

            return productsList;
        }
        public static void AddNewProduct(string name, int productTypeID, int supplierID, decimal cost)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                var dataSet = new DataSet();
                var query = "SELECT * FROM Products WHERE 1 = 0"; 
                var adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataSet, "Products");

                var newRow = dataSet.Tables["Products"].NewRow();
                newRow["Name"] = name;
                newRow["ProductTypeID"] = productTypeID;
                newRow["SupplierID"] = supplierID;
                newRow["Cost"] = cost;
                dataSet.Tables["Products"].Rows.Add(newRow);

                var commandBuilder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = commandBuilder.GetInsertCommand();

                adapter.Update(dataSet, "Products");

                AllProducts.Add(new M_Products
                {
                    Name = name,
                    ProductTypeID = productTypeID,
                    SupplierID = supplierID,
                    Cost = cost
                });
            }
            AllProducts = GetAllProducts(connectionString);
        }
        public static void DeleteProduct(int productId)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var adapterDeliveries = new SqlDataAdapter("SELECT * FROM Deliveries WHERE ProductID = @ProductID", connection);
                    adapterDeliveries.SelectCommand.Parameters.AddWithValue("@ProductID", productId);
                    var commandBuilderDeliveries = new SqlCommandBuilder(adapterDeliveries);

                    var adapterProducts = new SqlDataAdapter("SELECT * FROM Products WHERE ProductID = @ProductID", connection);
                    adapterProducts.SelectCommand.Parameters.AddWithValue("@ProductID", productId);
                    var commandBuilderProducts = new SqlCommandBuilder(adapterProducts);

                    var dataSet = new DataSet();

                    adapterDeliveries.Fill(dataSet, "Deliveries");
                    adapterProducts.Fill(dataSet, "Products");

                    foreach (DataRow row in dataSet.Tables["Deliveries"].Rows)
                    {
                        row.Delete();
                    }
                    adapterDeliveries.Update(dataSet, "Deliveries");

                    foreach (DataRow row in dataSet.Tables["Products"].Rows)
                    {
                        row.Delete();
                    }
                    adapterProducts.Update(dataSet, "Products");

                    AllProducts.RemoveAll(p => p.ProductID == productId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении продукта: {ex.Message}");
            }
            AllProducts = GetAllProducts(connectionString);
        }
        public static void UpdateProduct(int productId, string newName, int newProductTypeID, int newSupplierID, decimal newCost)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                var adapter = new SqlDataAdapter($"SELECT * FROM Products WHERE ProductID = @ProductID", connection);
                adapter.SelectCommand.Parameters.AddWithValue("@ProductID", productId);
                var commandBuilder = new SqlCommandBuilder(adapter);

                var dataSet = new DataSet();
                adapter.Fill(dataSet, "Products");

                if (dataSet.Tables["Products"].Rows.Count == 0)
                {
                    return;
                }

                var row = dataSet.Tables["Products"].Rows[0];
                row["Name"] = newName;
                row["ProductTypeID"] = newProductTypeID;
                row["SupplierID"] = newSupplierID;
                row["Cost"] = newCost;

                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.Update(dataSet, "Products");

                var productToUpdate = AllProducts.FirstOrDefault(p => p.ProductID == productId);
                if (productToUpdate != null)
                {
                    productToUpdate.Name = newName;
                    productToUpdate.ProductTypeID = newProductTypeID;
                    productToUpdate.SupplierID = newSupplierID;
                    productToUpdate.Cost = newCost;
                }
            }
        }

    }
}
