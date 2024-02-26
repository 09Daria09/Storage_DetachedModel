using Storage_DetachedModel.Commands;
using Storage_DetachedModel.View;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Storage_DetachedModel.Model;

namespace Storage_DetachedModel.ViewModel
{
    internal class ViewModelStorage : INotifyPropertyChanged
    {
        private DataTable _productsData;
        string connectionString;
        public M_ProductTypes ProductTypes { get; set; }

        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public ObservableCollection<MenuItem> MenuItems2 { get; set; }
        public ObservableCollection<MenuItem> MenuItems3 { get; set; }

        public ICommand ShowAllProductsCommand { get; private set; }
        public ICommand ShowAllProductTypesCommand { get; private set; }
        public ICommand ShowAllSuppliersCommand { get; private set; }
        public ICommand ShowProductWithMaxQuantityCommand { get; private set; }
        public ICommand ShowProductWithMinQuantityCommand { get; private set; }
        public ICommand ShowProductWithMinCostCommand { get; private set; }
        public ICommand ShowProductWithMaxCostCommand { get; private set; }
        public ICommand ShowOldestProductCommand { get; private set; }
        public ICommand ShowProductsBySupplierCommand { get; private set; }

        public ICommand ShowProductsByCategoryCommand { get; private set; }
        public ICommand ShowAverageQuantityByTypeCommand { get; private set; }


        public ICommand ShowSupplierWithMaxProductsCommand { get; private set; }
        public ICommand ShowSupplierWithMinProductsCommand { get; private set; }
        public ICommand ShowProductTypeWithMaxQuantityCommand { get; private set; }
        public ICommand ShowProductTypeWithMinQuantityCommand { get; private set; }
        public ICommand ShowProductsOlderThanNDaysCommand { get; private set; }
        public ICommand AddNewProductCommand { get; private set; }
        public ICommand AddNewProductTypeCommand { get; private set; }
        public ICommand AddNewSupplierCommand { get; private set; }

        public ICommand DeleteProductCommand { get; private set; }
        public ICommand DeleteProductTypeCommand { get; private set; }
        public ICommand DeleteSupplierCommand { get; private set; }

        public ICommand UpdateProductCommand { get; private set; }
        public ICommand UpdateProductTypeCommand { get; private set; }

        public ICommand UpdateSupplierCommand { get; private set; }
        public DataTable ProductsData
        {
            get { return _productsData; }
            set
            {
                if (_productsData != value)
                {
                    _productsData = value;
                    OnPropertyChanged(nameof(ProductsData));
                }
            }
        }

        public ViewModelStorage(string _connectionInfo)
        {
            connectionString = _connectionInfo;
            M_ProductTypes productType = new M_ProductTypes();

            MenuItems = new ObservableCollection<MenuItem>();
            MenuItems2 = new ObservableCollection<MenuItem>();
            MenuItems3 = new ObservableCollection<MenuItem>();

            ShowAllProductsCommand = new DelegateCommand(ShowAllProducts, (object parameter) => true);
            ShowAllProductTypesCommand = new DelegateCommand(ShowAllProductTypes, (object parameter) => true);
            ShowAllSuppliersCommand = new DelegateCommand(ShowAllSuppliers, (object parameter) => true);
            ShowProductWithMaxQuantityCommand = new DelegateCommand(ShowProductWithMaxQuantity, (object parameter) => true);
            ShowProductWithMinQuantityCommand = new DelegateCommand(ShowProductWithMinQuantity, (object parameter) => true);
            ShowProductWithMinCostCommand = new DelegateCommand(ShowProductWithMinCost, (object parameter) => true);
            ShowProductWithMaxCostCommand = new DelegateCommand(ShowProductWithMaxCost, (object parameter) => true);
            ShowOldestProductCommand = new DelegateCommand(ShowOldestProduct, (object parameter) => true);


            ShowProductsByCategoryCommand = new DelegateCommand(ExecuteShowProductsByCategory, (object parameter) => true);
            ShowProductsBySupplierCommand = new DelegateCommand(ExecuteShowProductsBySupplier, (object parameter) => true);
            ShowAverageQuantityByTypeCommand = new DelegateCommand(ShowAverageQuantityByType, (object parameter) => true);
            InitializeMenuItems();
            InitializeSupplierMenuItems();
            InitializeMenuItems2();

            ShowSupplierWithMaxProductsCommand = new DelegateCommand(ShowSupplierWithMaxProducts, (object parameter) => true);
            ShowSupplierWithMinProductsCommand = new DelegateCommand(ShowSupplierWithMinProducts, (object parameter) => true);
            ShowProductTypeWithMaxQuantityCommand = new DelegateCommand(ShowProductTypeWithMaxQuantity, (object parameter) => true);
            ShowProductTypeWithMinQuantityCommand = new DelegateCommand(ShowProductTypeWithMinQuantity, (object parameter) => true);
            ShowProductsOlderThanNDaysCommand = new DelegateCommand(ShowProductsOlderThanNDays, (object parameter) => true);
            AddNewProductCommand = new DelegateCommand(AddNewProduct, (object parameter) => true);
            AddNewProductTypeCommand = new DelegateCommand(AddNewProductType, (object parameter) => true);
            AddNewSupplierCommand = new DelegateCommand(AddNewSupplier, (object parameter) => true);
            DeleteProductCommand = new DelegateCommand(OpenDeleteProductDialog, (object parameter) => true);
            DeleteProductTypeCommand = new DelegateCommand(OpenDeleteProductTypeDialog, (object parameter) => true);
            DeleteSupplierCommand = new DelegateCommand(OpenDeleteSupplierDialog, (object parameter) => true);


            UpdateProductCommand = new DelegateCommand(UpdateProduct, (object parameter) => true);
            UpdateProductTypeCommand = new DelegateCommand(UpdateProductType, (object parameter) => true);
            UpdateSupplierCommand = new DelegateCommand(UpdateSupplier, (object parameter) => true);
        }

        private void UpdateSupplier(object obj)
        {
            var UpdateSupplier = new UpdateSupplier(connectionString);
            UpdateSupplier.ShowDialog();
            InitializeSupplierMenuItems();
            ShowAllProducts(obj);
        }

        private void UpdateProductType(object obj)
        {
            var UpdateTypeProduct = new TypeProductUpdate(connectionString);
            UpdateTypeProduct.ShowDialog();
            InitializeMenuItems();
            InitializeMenuItems2(); 
            ShowAllProducts(obj);
        }

        private void UpdateProduct(object obj)
        {
            var UpDateWindow = new ProductUpdate(connectionString);
            UpDateWindow.ShowDialog();
            InitializeMenuItems();
            InitializeMenuItems2();
            ShowAllProducts(obj);
        }

        private void OpenDeleteProductDialog(object obj)
        {
            var DeletionWindow = new Deletion(connectionString, 1);
            DeletionWindow.ShowDialog();
            InitializeMenuItems();
            InitializeMenuItems2();
        }

        private void OpenDeleteProductTypeDialog(object obj)
        {
            var DeletionWindow = new Deletion(connectionString, 2);
            DeletionWindow.ShowDialog();
            InitializeMenuItems();
            InitializeMenuItems2();
        }

        private void OpenDeleteSupplierDialog(object obj)
        {
            var DeletionWindow = new Deletion(connectionString, 3);
            DeletionWindow.ShowDialog();
            InitializeSupplierMenuItems();
        }

        private void AddNewSupplier(object obj)
        {
            var additionWindow = new AddingSupplier(connectionString);
            additionWindow.ShowDialog();
            InitializeSupplierMenuItems();
            ShowAllProducts(obj);

        }

        private void AddNewProductType(object obj)
        {
            var additionWindow = new AddingProductType(connectionString);
            additionWindow.ShowDialog();
            InitializeMenuItems();
            InitializeMenuItems2();
            ShowAllProducts(obj);

        }

        private void AddNewProduct(object obj)
        {
            var additionWindow = new Addition();
            additionWindow.ShowDialog();
            InitializeMenuItems();
            InitializeMenuItems2();
            ShowAllProducts(obj);

        }

        private void ShowProductsOlderThanNDays(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Название товара", typeof(string));
            dataTable.Columns.Add("Дата поставки", typeof(DateTime));
            dataTable.Columns.Add("Дней на складе", typeof(int));

            string input = Interaction.InputBox("Введите количество дней:", "Ввод дней", "10", -1, -1);
            if (int.TryParse(input, out int days))
            {
                var query = M_Deliveries.AllDeliveries
                    .Join(M_Products.AllProducts,
                          delivery => delivery.ProductID,
                          product => product.ProductID,
                          (delivery, product) => new
                          {
                              ProductName = product.Name,
                              DeliveryDate = delivery.Date,
                              DaysInStock = (DateTime.Now - delivery.Date).Days
                          })
                    .Where(item => item.DaysInStock >= days);

                foreach (var item in query)
                {
                    dataTable.Rows.Add(item.ProductName, item.DeliveryDate, item.DaysInStock);
                }
            }
            else
            {
                MessageBox.Show("Некорректный ввод! Пожалуйста, введите число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ProductsData = dataTable;
        }
        private void ShowProductTypeWithMinQuantity(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Тип товара", typeof(string));
            dataTable.Columns.Add("Количество товаров", typeof(int));

            var productTypeWithMinQuantity = M_Products.AllProducts
                .GroupBy(p => p.ProductTypeID)
                .Select(g => new
                {
                    ProductTypeID = g.Key,
                    ProductCount = g.Count()
                })
                .Join(M_ProductTypes.AllProductTypes,
                      g => g.ProductTypeID,
                      pt => pt.ProductTypeID,
                      (g, pt) => new
                      {
                          Type = pt.Type,
                          ProductCount = g.ProductCount
                      })
                .OrderBy(g => g.ProductCount)
                .FirstOrDefault();

            if (productTypeWithMinQuantity != null)
            {
                dataTable.Rows.Add(productTypeWithMinQuantity.Type, productTypeWithMinQuantity.ProductCount);
            }

            ProductsData = dataTable;
        }


        private void ShowProductTypeWithMaxQuantity(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Тип товара", typeof(string));
            dataTable.Columns.Add("Количество товаров", typeof(int));

            var productTypeWithMaxQuantity = M_Products.AllProducts
                .GroupBy(p => p.ProductTypeID)
                .Select(g => new
                {
                    ProductTypeID = g.Key,
                    ProductCount = g.Count()
                })
                .Join(M_ProductTypes.AllProductTypes,
                      g => g.ProductTypeID,
                      pt => pt.ProductTypeID,
                      (g, pt) => new
                      {
                          Type = pt.Type,
                          ProductCount = g.ProductCount
                      })
                .OrderByDescending(g => g.ProductCount)
                .FirstOrDefault();

            if (productTypeWithMaxQuantity != null)
            {
                dataTable.Rows.Add(productTypeWithMaxQuantity.Type, productTypeWithMaxQuantity.ProductCount);
            }

            ProductsData = dataTable;
        }


        private void ShowSupplierWithMinProducts(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор поставщика", typeof(int));
            dataTable.Columns.Add("Название поставщика", typeof(string));
            dataTable.Columns.Add("Количество товаров", typeof(int));

            var supplierWithMinProducts = M_Suppliers.AllSuppliers
                .GroupJoin(M_Products.AllProducts,
                           s => s.SupplierID,
                           p => p.SupplierID,
                           (s, products) => new
                           {
                               SupplierID = s.SupplierID,
                               SupplierName = s.Name,
                               ProductCount = products.Count()
                           })
                .OrderBy(s => s.ProductCount)
                .FirstOrDefault();

            if (supplierWithMinProducts != null)
            {
                dataTable.Rows.Add(supplierWithMinProducts.SupplierID,
                                   supplierWithMinProducts.SupplierName,
                                   supplierWithMinProducts.ProductCount);
            }

            ProductsData = dataTable;
        }


        private void ShowSupplierWithMaxProducts(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор поставщика", typeof(int));
            dataTable.Columns.Add("Название поставщика", typeof(string));
            dataTable.Columns.Add("Количество товаров", typeof(int));

            var supplierWithMaxProducts = M_Suppliers.AllSuppliers
                .GroupJoin(M_Products.AllProducts,
                           s => s.SupplierID,
                           p => p.SupplierID,
                           (s, products) => new
                           {
                               SupplierID = s.SupplierID,
                               SupplierName = s.Name,
                               ProductCount = products.Count()
                           })
                .OrderByDescending(s => s.ProductCount)
                .FirstOrDefault();

            if (supplierWithMaxProducts != null)
            {
                dataTable.Rows.Add(supplierWithMaxProducts.SupplierID,
                                   supplierWithMaxProducts.SupplierName,
                                   supplierWithMaxProducts.ProductCount);
            }

            ProductsData = dataTable;
        }


        private void ShowAverageQuantityByType(object obj)
        {
            string selectedType = obj.ToString();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Тип продукта", typeof(string));
            dataTable.Columns.Add("Среднее количество", typeof(float));

            var query = M_Deliveries.AllDeliveries
                .Join(M_Products.AllProducts, d => d.ProductID, p => p.ProductID, (d, p) => new { d, p })
                .Join(M_ProductTypes.AllProductTypes, combined => combined.p.ProductTypeID, pt => pt.ProductTypeID, (combined, pt) => new { combined.d, combined.p, pt })
                .Where(combined => combined.pt.Type == selectedType)
                .GroupBy(combined => combined.pt.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    AverageQuantity = g.Average(x => x.d.Quantity)
                })
                .ToList();

            foreach (var item in query)
            {
                dataTable.Rows.Add(item.Type, Math.Round(item.AverageQuantity, 2));
            }

            ProductsData = dataTable;
        }


        public void InitializeMenuItems2()
        {
            var productTypes = M_ProductTypes.AllProductTypes;

            MenuItems3.Clear();

            foreach (var type in productTypes)
            {
                var menuItem = new MenuItem
                {
                    Header = type.Type, 
                    Command = ShowAverageQuantityByTypeCommand,
                    CommandParameter = type.Type
                };

                MenuItems3.Add(menuItem);
            }
        }


        public void InitializeSupplierMenuItems()
        {
            var suppliers = M_Suppliers.AllSuppliers; 

            MenuItems2.Clear(); 

            foreach (var supplier in suppliers)
            {
                var menuItem = new MenuItem
                {
                    Header = supplier.Name, 
                    Command = ShowProductsBySupplierCommand,
                    CommandParameter = supplier.Name
                };

                MenuItems2.Add(menuItem); 
            }
        }
        public void InitializeMenuItems()
        {
            var productTypes = M_ProductTypes.AllProductTypes;

            MenuItems.Clear();

            foreach (var type in productTypes)
            {
                var menuItem = new MenuItem
                {
                    Header = type.Type,
                    Command = ShowProductsByCategoryCommand,
                    CommandParameter = type.Type 
                };

                MenuItems.Add(menuItem);
            }
        }



        private void ExecuteShowProductsBySupplier(object obj)
        {
            string supplierName = obj.ToString();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор продукта", typeof(int));
            dataTable.Columns.Add("Название продукта", typeof(string));
            dataTable.Columns.Add("Тип продукта", typeof(string));
            dataTable.Columns.Add("Название поставщика", typeof(string));
            dataTable.Columns.Add("Адрес", typeof(string));
            dataTable.Columns.Add("Телефон", typeof(string));
            dataTable.Columns.Add("Стоимость", typeof(decimal));

            var query = M_Products.AllProducts
                .Where(p => M_Suppliers.AllSuppliers.Any(s => s.SupplierID == p.SupplierID && s.Name == supplierName))
                .Select(p => new
                {
                    ProductID = p.ProductID,
                    ProductName = p.Name,
                    ProductType = M_ProductTypes.AllProductTypes.FirstOrDefault(pt => pt.ProductTypeID == p.ProductTypeID).Type,
                    SupplierName = supplierName,
                    Address = M_Suppliers.AllSuppliers.FirstOrDefault(s => s.SupplierID == p.SupplierID).Address,
                    Phone = M_Suppliers.AllSuppliers.FirstOrDefault(s => s.SupplierID == p.SupplierID).Phone,
                    Cost = p.Cost
                });

            foreach (var item in query)
            {
                dataTable.Rows.Add(item.ProductID, item.ProductName, item.ProductType, item.SupplierName, item.Address, item.Phone, item.Cost);
            }

            ProductsData = dataTable;
        }


        private void ExecuteShowProductsByCategory(object obj)
        {
            string productType = obj.ToString();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор продукта", typeof(int));
            dataTable.Columns.Add("Название продукта", typeof(string));
            dataTable.Columns.Add("Тип продукта", typeof(string));
            dataTable.Columns.Add("Название поставщика", typeof(string));
            dataTable.Columns.Add("Стоимость", typeof(decimal));

            var query = M_Products.AllProducts
                .Where(p => M_ProductTypes.AllProductTypes.Any(pt => pt.ProductTypeID == p.ProductTypeID && pt.Type == productType))
                .Select(p => new
                {
                    ProductID = p.ProductID,
                    ProductName = p.Name,
                    ProductType = M_ProductTypes.AllProductTypes.FirstOrDefault(pt => pt.ProductTypeID == p.ProductTypeID)?.Type,
                    SupplierName = M_Suppliers.AllSuppliers.FirstOrDefault(s => s.SupplierID == p.SupplierID)?.Name,

                    Cost = p.Cost
                });

            foreach (var item in query)
            {
                dataTable.Rows.Add(item.ProductID, item.ProductName, item.ProductType, item.SupplierName, item.Cost);
            }

            ProductsData = dataTable;
        }


        private void ExecuteQueryAndFillDataTable(string query, DataTable dataTable, string productType, string scalar)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(scalar, productType);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при попытке получения данных из базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public async Task<List<string>> GetCategoriesAsync()
        {
            var productTypes = new List<string>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT DISTINCT Type FROM ProductTypes", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        productTypes.Add(reader.GetString(0));
                    }
                }
            }
            return productTypes;
        }//dddd

        public async Task<List<string>> GetSuppliersAsync()
        {
            var suppliers = new List<string>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT DISTINCT Name FROM Suppliers", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        suppliers.Add(reader.GetString(0));
                    }
                }
            }
            return suppliers;
        }

        private void ShowOldestProduct(object obj)
        {
            DataTable dataTable = new DataTable();
            string query = @"
SELECT TOP 1
    p.ProductID AS ""Идентификатор продукта"",
    p.Name AS ""Название продукта"",
    MIN(d.Date) AS ""Дата поступления""
FROM Products p
JOIN Deliveries d ON p.ProductID = d.ProductID
GROUP BY p.ProductID, p.Name
ORDER BY ""Дата поступления"" ASC;
";
            ExecuteQueryAndFillDataTable(query, dataTable);
            ProductsData = dataTable;
        }

        private void ShowProductWithMaxCost(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор продукта", typeof(int));
            dataTable.Columns.Add("Название продукта", typeof(string));
            dataTable.Columns.Add("Себестоимость", typeof(decimal));

            var productWithMaxCost = M_Products.AllProducts
                .OrderByDescending(p => p.Cost)
                .FirstOrDefault();

            if (productWithMaxCost != null)
            {
                dataTable.Rows.Add(productWithMaxCost.ProductID, productWithMaxCost.Name, productWithMaxCost.Cost);
            }

            ProductsData = dataTable;
        }


        private void ShowProductWithMinCost(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор продукта", typeof(int));
            dataTable.Columns.Add("Название продукта", typeof(string));
            dataTable.Columns.Add("Себестоимость", typeof(decimal));
            var productWithMinCost = M_Products.AllProducts
                .OrderBy(p => p.Cost)
                .FirstOrDefault();

            if (productWithMinCost != null)
            {
                dataTable.Rows.Add(productWithMinCost.ProductID, productWithMinCost.Name, productWithMinCost.Cost);
            }

            ProductsData = dataTable;
        }

        private void ShowProductWithMinQuantity(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор продукта", typeof(int));
            dataTable.Columns.Add("Название продукта", typeof(string));
            dataTable.Columns.Add("Общее количество", typeof(int));

            var productQuantities = M_Deliveries.AllDeliveries
                .GroupBy(d => d.ProductID)
                .Select(g => new
                {
                    ProductID = g.Key,
                    TotalQuantity = g.Sum(d => d.Quantity)
                });

            var minQuantityProduct = productQuantities
                .OrderBy(pq => pq.TotalQuantity)
                .FirstOrDefault();

            if (minQuantityProduct != null)
            {
                var productName = M_Products.AllProducts
                    .Where(p => p.ProductID == minQuantityProduct.ProductID)
                    .Select(p => p.Name)
                    .FirstOrDefault();

                dataTable.Rows.Add(minQuantityProduct.ProductID, productName, minQuantityProduct.TotalQuantity);
            }

            ProductsData = dataTable;
        }


        private void ShowProductWithMaxQuantity(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор продукта", typeof(int));
            dataTable.Columns.Add("Название продукта", typeof(string));
            dataTable.Columns.Add("Общее количество", typeof(int));

            var productWithMaxQuantity = M_Deliveries.AllDeliveries
                .GroupBy(d => d.ProductID)
                .Select(group => new
                {
                    ProductID = group.Key,
                    TotalQuantity = group.Sum(g => g.Quantity)
                })
                .OrderByDescending(q => q.TotalQuantity)
                .FirstOrDefault();

            if (productWithMaxQuantity != null)
            {
                var productDetails = M_Products.AllProducts
                    .Where(p => p.ProductID == productWithMaxQuantity.ProductID)
                    .Select(p => new { p.ProductID, p.Name })
                    .FirstOrDefault();

                if (productDetails != null)
                {
                    dataTable.Rows.Add(productDetails.ProductID, productDetails.Name, productWithMaxQuantity.TotalQuantity);
                }
            }

            ProductsData = dataTable;
        }


        private void ShowAllSuppliers(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор поставщика", typeof(int));
            dataTable.Columns.Add("Название поставщика", typeof(string));
            dataTable.Columns.Add("Адрес", typeof(string));
            dataTable.Columns.Add("Телефон", typeof(string));
            var query = M_Suppliers.AllSuppliers
                .Select(supplier => new
                {
                    SupplierID = supplier.SupplierID,
                    Name = supplier.Name,
                    Address = supplier.Address,
                    Phone = supplier.Phone
                });

            foreach (var supplier in query)
            {
                dataTable.Rows.Add(supplier.SupplierID, supplier.Name, supplier.Address, supplier.Phone);
            }

            ProductsData = dataTable; 
        }
        private void ShowAllProductTypes(object obj)
        {
            DataTable dataTable = M_ProductTypes.GetAllProductTypesAsDataTable();
            ProductsData = dataTable; 
        }


        private void ShowAllProducts(object obj)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Идентификатор продукта", typeof(int));
            dataTable.Columns.Add("Название продукта", typeof(string));
            dataTable.Columns.Add("Тип продукта", typeof(string));
            dataTable.Columns.Add("Название поставщика", typeof(string));
            dataTable.Columns.Add("Стоимость", typeof(double));


            var query = from product in M_Products.AllProducts
                        join productType in M_ProductTypes.AllProductTypes on product.ProductTypeID equals productType.ProductTypeID
                        join supplier in M_Suppliers.AllSuppliers on product.SupplierID equals supplier.SupplierID
                        select new
                        {
                            ProductID = product.ProductID,
                            ProductName = product.Name,
                            ProductType = productType.Type,
                            SupplierName = supplier.Name,
                            Cost = product.Cost
                        };

            foreach (var item in query)
            {
                dataTable.Rows.Add(item.ProductID, item.ProductName, item.ProductType, item.SupplierName, item.Cost);
            }

            ProductsData = dataTable;
        }


        private void ExecuteQueryAndFillDataTable(string query, DataTable dataTable)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при попытке получения данных из базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
