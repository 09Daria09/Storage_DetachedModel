using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Storage_DetachedModel.Model
{
    internal class M_Deliveries
    {
        public int DeliveryID { get; set; }
        public int ProductID { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

        public static List<M_Deliveries> AllDeliveries { get; private set; }

        static M_Deliveries()
        {
            string connectionString = DatabaseConnectionInfo.GetConnectionString();
            AllDeliveries = GetAllDeliveries(connectionString);
        }

        public static List<M_Deliveries> GetAllDeliveries(string connectionString)
        {
            var deliveriesList = new List<M_Deliveries>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Deliveries";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, connection);

                DataSet ds = new DataSet();
                dataAdapter.Fill(ds, "Deliveries");

                foreach (DataRow row in ds.Tables["Deliveries"].Rows)
                {
                    var delivery = new M_Deliveries()
                    {
                        DeliveryID = Convert.ToInt32(row["DeliveryID"]),
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        Date = Convert.ToDateTime(row["Date"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                    };

                    deliveriesList.Add(delivery);
                }
            }

            return deliveriesList;
        }
    }
}
