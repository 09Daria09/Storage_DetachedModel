using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace Storage_DetachedModel.Model
{
    internal class M_ProductTypes
    {
        public static List<M_ProductTypes> AllProductTypes { get; private set; }

        public int ProductTypeID { get; set; }
        public string Type { get; set; }

        static M_ProductTypes()
        {
            string connectionString = DatabaseConnectionInfo.GetConnectionString();

            AllProductTypes = GetAllProductTypes(connectionString);
        }
        public static List<M_ProductTypes> GetAllProductTypes(string connectionString)
        {
            var productTypesList = new List<M_ProductTypes>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM ProductTypes";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, connection);

                DataSet ds = new DataSet();
                dataAdapter.Fill(ds, "ProductTypes");

                foreach (DataRow row in ds.Tables["ProductTypes"].Rows)
                {
                    var productType = new M_ProductTypes()
                    {
                        ProductTypeID = row["ProductTypeID"] != DBNull.Value ? Convert.ToInt32(row["ProductTypeID"]) : 0,
                        Type = row["Type"].ToString()
                    };

                    productTypesList.Add(productType);
                }
            }

            return productTypesList;
        }
        public static DataTable GetAllProductTypesAsDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Тип продукта", typeof(string));

            foreach (var productType in AllProductTypes)
            {
                var row = dataTable.NewRow();
                row["ID"] = productType.ProductTypeID;
                row["Тип продукта"] = productType.Type;
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        public static void AddNewProductType(string type)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();

            using (var connection = new SqlConnection(connectionString))
            {
                var dataSet = new DataSet();
                var query = "SELECT * FROM ProductTypes WHERE 1 = 0";
                var adapter = new SqlDataAdapter(query, connection);

                adapter.Fill(dataSet, "ProductTypes");

                var newRow = dataSet.Tables["ProductTypes"].NewRow();
                newRow["Type"] = type; 

                dataSet.Tables["ProductTypes"].Rows.Add(newRow);

                var commandBuilder = new SqlCommandBuilder(adapter);

                adapter.InsertCommand = commandBuilder.GetInsertCommand();

                adapter.Update(dataSet, "ProductTypes");

                AllProductTypes = GetAllProductTypes(connectionString);
            }
        }

        public static void DeleteProductType(int productTypeId)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();
             
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var transaction = connection.BeginTransaction();

                    var adapterProducts = new SqlDataAdapter($"SELECT * FROM Products WHERE ProductTypeID = {productTypeId}", connection);
                    var adapterProductTypes = new SqlDataAdapter($"SELECT * FROM ProductTypes WHERE ProductTypeID = {productTypeId}", connection);
                    var adapterDeliveries = new SqlDataAdapter("SELECT * FROM Deliveries WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductTypeID = @ProductTypeID)", connection);
                    adapterDeliveries.SelectCommand.Parameters.AddWithValue("@ProductTypeID", productTypeId);

                    adapterProducts.SelectCommand.Transaction = transaction;
                    adapterProductTypes.SelectCommand.Transaction = transaction;
                    adapterDeliveries.SelectCommand.Transaction = transaction;

                    var commandBuilderProducts = new SqlCommandBuilder(adapterProducts);
                    var commandBuilderProductTypes = new SqlCommandBuilder(adapterProductTypes);
                    var commandBuilderDeliveries = new SqlCommandBuilder(adapterDeliveries);

                    var dataSet = new DataSet();

                    adapterDeliveries.Fill(dataSet, "Deliveries");
                    adapterProducts.Fill(dataSet, "Products");
                    adapterProductTypes.Fill(dataSet, "ProductTypes");

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

                    foreach (DataRow row in dataSet.Tables["ProductTypes"].Rows)
                    {
                        row.Delete();
                    }
                    adapterProductTypes.Update(dataSet, "ProductTypes");

                    transaction.Commit();

                    AllProductTypes.RemoveAll(pt => pt.ProductTypeID == productTypeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении типа продукта: {ex.Message}");
            }
            AllProductTypes = GetAllProductTypes(connectionString);
        }
        public static void UpdateProductType(int productTypeId, string newType)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = $"SELECT * FROM ProductTypes WHERE ProductTypeID = {productTypeId}";
                    var adapter = new SqlDataAdapter(query, connection);
                    var commandBuilder = new SqlCommandBuilder(adapter);
                    var dataSet = new DataSet();

                    adapter.Fill(dataSet, "ProductTypes");

                    var rows = dataSet.Tables["ProductTypes"].Select($"ProductTypeID = {productTypeId}");
                    if (rows.Length > 0)
                    {
                        rows[0]["Type"] = newType;

                        adapter.Update(dataSet, "ProductTypes");

                        AllProductTypes = GetAllProductTypes(connectionString);
                    }
                    else
                    {
                        throw new Exception("Тип продукта с указанным ID не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении типа продукта: {ex.Message}");
            }
        }


    }
}
