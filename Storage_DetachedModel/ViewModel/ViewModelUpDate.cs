using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Storage_DetachedModel.Commands;
using System.Data.SqlClient;
using Storage_DetachedModel.ViewModel;
using System.Windows;
using Storage_DetachedModel.Model;


namespace Storage_DetachedModel.ViewModel
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int ProductType { get; set; }
        public int Supplier{ get; set; } 
        public decimal Cost { get; set; }
        public ProductViewModel DeepCopy()
        {
            return new ProductViewModel
            {
                ProductID = this.ProductID,
                Name = this.Name,
                ProductType = this.ProductType,
                Supplier = this.Supplier,
                Cost = this.Cost
            };
        }
    }

    internal class ViewModelUpDate : INotifyPropertyChanged
    {
        private string connectionString;

        private ProductViewModel originalSelectedProduct;

        private string productName;
        public string ProductName
        {
            get => productName;
            set
            {
                if (productName != value)
                {
                    productName = value;
                    OnPropertyChanged(nameof(ProductName));
                }
            }
        }

        private decimal productCost;
        public decimal ProductCost
        {
            get => productCost;
            set
            {
                if (productCost != value)
                {
                    productCost = value;
                    OnPropertyChanged(nameof(ProductCost));
                }
            }
        }


        private ProductViewModel selectedProductForUpdate;
        public ProductViewModel SelectedProductForUpdate
        {
            get => selectedProductForUpdate;
            set
            {
                if (selectedProductForUpdate != value)
                {
                    selectedProductForUpdate = value;
                    OnPropertyChanged(nameof(SelectedProductForUpdate));
                    ProductName = selectedProductForUpdate?.Name;
                    SelectedProductType = ProductTypes.FirstOrDefault(pt => pt.ProductTypeID == selectedProductForUpdate.ProductType);
                    SelectedSupplier = Suppliers.FirstOrDefault(s => s.SupplierID == selectedProductForUpdate.Supplier);
                    ProductCost = selectedProductForUpdate?.Cost ?? 0m;
                    originalSelectedProduct = selectedProductForUpdate?.DeepCopy();
                }
            }
        }

        private ObservableCollection<ProductType> productTypes = new ObservableCollection<ProductType>();
        public ObservableCollection<ProductType> ProductTypes
        {
            get => productTypes;
            set
            {
                productTypes = value;
                OnPropertyChanged(nameof(ProductTypes));
            }
        }

        public ObservableCollection<Supplier> suppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers 
        {
            get => suppliers;
            set
            {
                suppliers = value;
                OnPropertyChanged(nameof(Suppliers));
            }
        }

        private ProductType selectedProductType;
        public ProductType SelectedProductType
        {
            get => selectedProductType;
            set
            {
                if (selectedProductType != value)
                {
                    selectedProductType = value;
                    OnPropertyChanged(nameof(SelectedProductType));

                    if (SelectedProductForUpdate != null && selectedProductType != null)
                    {
                        SelectedProductForUpdate.ProductType = selectedProductType.ProductTypeID;
                        UpdateProductTypeInDatabase(SelectedProductForUpdate.ProductID, selectedProductType.ProductTypeID);
                    }
                }
            }
        }

        private Supplier selectedSupplier;
        public Supplier SelectedSupplier
        {
            get => selectedSupplier;
            set
            {
                if (selectedSupplier != value)
                {
                    selectedSupplier = value;
                    OnPropertyChanged(nameof(SelectedSupplier));

                    if (SelectedProductForUpdate != null && selectedSupplier != null)
                    {
                        SelectedProductForUpdate.Supplier = selectedSupplier.SupplierID;
                        UpdateProductSupplierInDatabase(SelectedProductForUpdate.ProductID, selectedSupplier.SupplierID);
                    }
                }
            }
        }
        private void UpdateProductSupplierInDatabase(int productId, int supplierId)
        {
            string query = "UPDATE Products SET SupplierID = @SupplierID WHERE ProductID = @ProductID";
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", supplierId);
                command.Parameters.AddWithValue("@ProductID", productId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private void UpdateProductTypeInDatabase(int productId, int productTypeId)
        {
            string query = "UPDATE Products SET ProductTypeID = @ProductTypeID WHERE ProductID = @ProductID";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductTypeID", productTypeId);
                command.Parameters.AddWithValue("@ProductID", productId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private ObservableCollection<ProductViewModel> products = new ObservableCollection<ProductViewModel>();
        public ObservableCollection<ProductViewModel> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        public ICommand UpdateProductCommand { get; private set; }

        public ViewModelUpDate(string connectionString)
        {
            this.connectionString = connectionString;
            LoadData();
            UpdateProductCommand = new DelegateCommand(UpdateProduct, CanUpdateProduct);
        }

        private bool CanUpdateProduct(object obj)
        {
            if (SelectedProductForUpdate == null || originalSelectedProduct == null) return false;

            return SelectedProductForUpdate.ProductID == originalSelectedProduct.ProductID &&
                   (ProductName != originalSelectedProduct.Name ||
                    SelectedProductForUpdate.ProductType != originalSelectedProduct.ProductType ||
                    SelectedProductForUpdate.Supplier != originalSelectedProduct.Supplier ||
                    ProductCost != originalSelectedProduct.Cost);
        }


        private void UpdateProduct(object obj)
        {
            try
            {
                M_Products.UpdateProduct(SelectedProductForUpdate.ProductID, ProductName, SelectedProductForUpdate.ProductType, SelectedProductForUpdate.Supplier, ProductCost);
                MessageBox.Show("Продукт успешно обновлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления продукта: {ex.Message}");
            }
            LoadData();
        }


        private void LoadData()
        {
            LoadProductTypes();
            LoadSuppliers();

            Products.Clear();
            ProductCost = 0;

            var productsList = M_Products.GetAllProducts(connectionString);

            foreach (var product in productsList)
            {
                Products.Add(new ProductViewModel
                {
                    ProductID = product.ProductID,
                    Name = product.Name,
                    ProductType = product.ProductTypeID,
                    Supplier = product.SupplierID,
                    Cost = product.Cost
                });
            }
        }

        private void LoadProductTypes()
        {
            ProductTypes.Clear();

            var productTypesList = M_ProductTypes.GetAllProductTypes(DatabaseConnectionInfo.GetConnectionString());

            foreach (var productType in productTypesList)
            {
                ProductTypes.Add(new ProductType
                {
                    ProductTypeID = productType.ProductTypeID,
                    Type = productType.Type
                });
            }
        }
        private void LoadSuppliers()
        {
            var suppliersList = M_Suppliers.GetAllSuppliers(DatabaseConnectionInfo.GetConnectionString());

            foreach (var supplier in suppliersList)
            {
                Suppliers.Add(new Supplier
                {
                    SupplierID = supplier.SupplierID,
                    Name = supplier.Name
                });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
