using Storage_DetachedModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Storage_DetachedModel.Model;

namespace Storage_DetachedModel.ViewModel
{
    internal class ViewModelUpDateTypeProduct : INotifyPropertyChanged
    {

        private string connectionString;
        public ICommand UpdateTypeProductCommand { get; private set; }
        private string newName;
        public string NewName
        {
            get => newName;
            set
            {
                if (newName != value)
                {
                    newName = value;
                    OnPropertyChanged(nameof(NewName));
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
                }
            }
        }

        public ViewModelUpDateTypeProduct(string connectionInfo) 
        {
            connectionString = connectionInfo;
            LoadProductTypes();
            UpdateTypeProductCommand = new DelegateCommand(UpdateTypeProduct, CanUpdateTypeProduct);
        }

        private bool CanUpdateTypeProduct(object obj)
        {
            return SelectedProductType != null && !string.IsNullOrEmpty(NewName) && NewName != SelectedProductType.Type;
        }

        private void UpdateTypeProduct(object obj)
        {
            if (SelectedProductType == null) return;

            try
            {
                M_ProductTypes.UpdateProductType(SelectedProductType.ProductTypeID, NewName);
                MessageBox.Show("Тип продукта успешно обновлен.");
                LoadProductTypes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления типа продукта: {ex.Message}");
            }

            NewName = "";
        }


        private void LoadProductTypes()
        {
            ProductTypes.Clear();

            try
            {
                foreach (var productType in M_ProductTypes.AllProductTypes)
                {
                    ProductTypes.Add(new ProductType
                    {
                        ProductTypeID = productType.ProductTypeID,
                        Type = productType.Type
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
