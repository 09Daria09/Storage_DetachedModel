using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Storage_DetachedModel.Model
{
    internal class M_Suppliers
    {
        public int SupplierID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public static List<M_Suppliers> AllSuppliers { get; private set; }

        static M_Suppliers()
        {
            string connectionString = DatabaseConnectionInfo.GetConnectionString();
            AllSuppliers = GetAllSuppliers(connectionString);
        }

        public static List<M_Suppliers> GetAllSuppliers(string connectionString)
        {
            var suppliersList = new List<M_Suppliers>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Suppliers";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, connection);

                DataSet ds = new DataSet();
                dataAdapter.Fill(ds, "Suppliers");

                foreach (DataRow row in ds.Tables["Suppliers"].Rows)
                {
                    var supplier = new M_Suppliers()
                    {
                        SupplierID = Convert.ToInt32(row["SupplierID"]),
                        Name = row["Name"].ToString(),
                        Address = row.IsNull("Address") ? null : row["Address"].ToString(),
                        Phone = row.IsNull("Phone") ? null : row["Phone"].ToString(),
                    };

                    suppliersList.Add(supplier);
                }
            }

            return suppliersList;
        }
        public static void AddNewSupplier(string name, string address, string phone)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var adapter = new SqlDataAdapter("SELECT * FROM Suppliers WHERE 1=0", connection);
                var cmdBuilder = new SqlCommandBuilder(adapter);

                var dataSet = new DataSet();
                adapter.Fill(dataSet, "Suppliers");

                var newRow = dataSet.Tables["Suppliers"].NewRow();
                newRow["Name"] = name;
                newRow["Address"] = address;
                newRow["Phone"] = phone;
                dataSet.Tables["Suppliers"].Rows.Add(newRow);

                adapter.InsertCommand = cmdBuilder.GetInsertCommand();

                adapter.Update(dataSet, "Suppliers");

                AllSuppliers = GetAllSuppliers(connectionString);
            }
        }

        public static void DeleteSupplier(int supplierId)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    var adapterProducts = new SqlDataAdapter($"SELECT * FROM Products WHERE SupplierID = {supplierId}", connection);
                    adapterProducts.SelectCommand.Transaction = transaction;
                    var commandBuilderProducts = new SqlCommandBuilder(adapterProducts);
                    var adapterSuppliers = new SqlDataAdapter($"SELECT * FROM Suppliers WHERE SupplierID = {supplierId}", connection);
                    adapterSuppliers.SelectCommand.Transaction = transaction;
                    var commandBuilderSuppliers = new SqlCommandBuilder(adapterSuppliers);

                    var dataSet = new DataSet();
                    adapterProducts.Fill(dataSet, "Products");
                    adapterSuppliers.Fill(dataSet, "Suppliers");

                    foreach (DataRow row in dataSet.Tables["Products"].Rows)
                    {
                        row.Delete();
                    }
                    adapterProducts.Update(dataSet, "Products");

                    foreach (DataRow row in dataSet.Tables["Suppliers"].Rows)
                    {
                        row.Delete();
                    }
                    adapterSuppliers.Update(dataSet, "Suppliers");

                    transaction.Commit();

                    AllSuppliers.RemoveAll(s => s.SupplierID == supplierId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении поставщика: {ex.Message}");
            }
            AllSuppliers = GetAllSuppliers(connectionString);
        }

        public static void UpdateSupplier(int supplierId, string newName, string newAddress, string newPhone)
        {
            var connectionString = DatabaseConnectionInfo.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = $"SELECT * FROM Suppliers WHERE SupplierID = {supplierId}";
                    var adapter = new SqlDataAdapter(query, connection);
                    var commandBuilder = new SqlCommandBuilder(adapter);
                    var dataSet = new DataSet();

                    adapter.Fill(dataSet, "Suppliers");

                    var rows = dataSet.Tables["Suppliers"].Select($"SupplierID = {supplierId}");
                    if (rows.Length > 0)
                    {
                        rows[0]["Name"] = newName;
                        rows[0]["Address"] = newAddress;
                        rows[0]["Phone"] = newPhone;

                        adapter.Update(dataSet, "Suppliers");

                        AllSuppliers = GetAllSuppliers(connectionString);
                    }
                    else
                    {
                        throw new Exception("Поставщик с указанным ID не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении поставщика: {ex.Message}");
            }
        }
    }

}