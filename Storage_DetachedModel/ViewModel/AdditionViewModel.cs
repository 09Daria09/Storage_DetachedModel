using Storage_DetachedModel.Commands;
using Storage_DetachedModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Storage_DetachedModel.ViewModel
{
    public class ProductType
    {
        public int ProductTypeID { get; set; }
        public string Type { get; set; }
    }

    public class Supplier
    {
        public int SupplierID { get; set; }
        public string Name { get; set; }
    }

    class AdditionViewModel : INotifyPropertyChanged
    { 
         public ICommand AddProductCommand { get; private set; }

        public AdditionViewModel()
        {
            AddProductCommand = new DelegateCommand(AddProduct, (object parameter) => true);
            LoadProductTypes();
            LoadSuppliers();
        }

        private void AddProduct(object obj)
        {
            if (string.IsNullOrEmpty(ProductName) || SelectedProductType == null || SelectedSupplier == null || ProductCost <= 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                M_Products.AddNewProduct(ProductName, SelectedProductType.ProductTypeID, SelectedSupplier.SupplierID, ProductCost);

                MessageBox.Show("Товар успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            SelectedSupplier = null;
            SelectedProductType = null;
            ProductCost = 0;
            ProductName = string.Empty;

        }

        private decimal _productCost;
        public decimal ProductCost
        {
            get => _productCost;
            set
            {
                _productCost = value;
                OnPropertyChanged(nameof(ProductCost));
            }
        }
        private string _productName;
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged(nameof(ProductName));
            }
        }
        public ObservableCollection<ProductType> ProductTypes { get; set; } = new ObservableCollection<ProductType>();
        public ObservableCollection<Supplier> Suppliers { get; set; } = new ObservableCollection<Supplier>();

        private ProductType _selectedProductType;
        public ProductType SelectedProductType
        {
            get => _selectedProductType;
            set
            {
                _selectedProductType = value;
                OnPropertyChanged(nameof(SelectedProductType));
            }
        }

        private Supplier _selectedSupplier;
        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged(nameof(SelectedSupplier));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void LoadProductTypes()
        {
            ProductTypes.Clear(); 

            foreach (var type in M_ProductTypes.AllProductTypes)
            {
                ProductTypes.Add(new ProductType
                {
                    ProductTypeID = type.ProductTypeID,
                    Type = type.Type
                });
            }
        }


        private void LoadSuppliers()
        {
            Suppliers.Clear(); 

            foreach (var supplier in M_Suppliers.AllSuppliers)
            {
                Suppliers.Add(new Supplier
                {
                    SupplierID = supplier.SupplierID,
                    Name = supplier.Name
                });
            }
        }

    }
}
