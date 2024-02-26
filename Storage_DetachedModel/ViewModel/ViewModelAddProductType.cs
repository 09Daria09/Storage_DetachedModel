using Storage_DetachedModel.Commands;
using Storage_DetachedModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Storage_DetachedModel.ViewModel
{
    class ViewModelAddProductType : INotifyPropertyChanged
    {
        private string connectionString;
        private string _typeName;
        public string TypeName
        {
            get => _typeName;
            set
            {
                _typeName = value;
                OnPropertyChanged(nameof(TypeName));
            }
        }

        public ICommand AddTypeCommand { get; private set; }

        public ViewModelAddProductType(string connect)
        {
            connectionString = connect;
            AddTypeCommand = new DelegateCommand(AddTypeExecute, CanAddTypeExecute);
        }

        private bool CanAddTypeExecute(object obj)
        {
            return !string.IsNullOrWhiteSpace(TypeName);
        }

        private void AddTypeExecute(object obj)
        {
            if (string.IsNullOrEmpty(TypeName))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                M_ProductTypes.AddNewProductType(TypeName);

                MessageBox.Show("Тип товара успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении типа товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            TypeName = ""; 
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

