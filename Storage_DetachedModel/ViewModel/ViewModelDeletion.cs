using Storage_DetachedModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using Storage_DetachedModel.Model;

namespace Storage_DetachedModel.ViewModel
{
    public class ComboBoxItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    internal class ViewModelDeletion : INotifyPropertyChanged
    {
        private string connectionString;
        private object selectedObject;
        int index;
        public ICommand DeleteCommand { get; private set; }

        public object SelectedObject
        {
            get => selectedObject;
            set
            {
                selectedObject = value;
                OnPropertyChanged(nameof(SelectedObject));
            }
        }
        public ViewModelDeletion(string connectionInfo, int i)
        {
            connectionString = connectionInfo;
            LoadData(i);
            DeleteCommand = new DelegateCommand(Delete, (object parameter) => true);
            index = i;
        }

        private void Delete(object parameter)
        {
            if (parameter is ComboBoxItem selectedItem)
            {
                int objectId = selectedItem.ID;

                try
                {
                    switch (index)
                    {
                        case 1:
                            M_Products.DeleteProduct(objectId);
                            break;
                        case 2:
                            M_ProductTypes.DeleteProductType(objectId);
                            break;
                        case 3:
                            M_Suppliers.DeleteSupplier(objectId);
                            break;
                    }

                    MessageBox.Show("Удаление прошло успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData(index);
                    SelectedObject = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка при удалении данных: {ex.Message}");
                    MessageBox.Show($"Ошибка при удалении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Debug.WriteLine("Предоставленный параметр имеет неверный формат или тип.");
            }
        }


        private ObservableCollection<ComboBoxItem> deletableObjects = new ObservableCollection<ComboBoxItem>();

        public ObservableCollection<ComboBoxItem> DeletableObjects
        {
            get => deletableObjects;
            set
            {
                deletableObjects = value;
                OnPropertyChanged(nameof(DeletableObjects));
            }
        }

        private void LoadData(int index)
        {
            DeletableObjects.Clear();

            try
            {
                switch (index)
                {
                    case 1:
                        foreach (var product in M_Products.AllProducts)
                        {
                            DeletableObjects.Add(new ComboBoxItem
                            {
                                ID = product.ProductID,
                                Name = product.Name
                            });
                        }
                        break;
                    case 2:
                        foreach (var productType in M_ProductTypes.AllProductTypes)
                        {
                            DeletableObjects.Add(new ComboBoxItem
                            {
                                ID = productType.ProductTypeID,
                                Name = productType.Type
                            });
                        }
                        break;
                    case 3: 
                        foreach (var supplier in M_Suppliers.AllSuppliers)
                        {
                            DeletableObjects.Add(new ComboBoxItem
                            {
                                ID = supplier.SupplierID,
                                Name = supplier.Name
                            });
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
