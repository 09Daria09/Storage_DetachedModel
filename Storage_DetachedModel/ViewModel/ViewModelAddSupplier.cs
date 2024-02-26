using Storage_DetachedModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Storage_DetachedModel.Model;

namespace Storage_DetachedModel.ViewModel
{
    class ViewModelAddSupplier : INotifyPropertyChanged
    {
        private string connectionString;
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _Address;
        public string Address
        {
            get => _Address;
            set
            {
                _Address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        private string _Phone;
        public string Phone
        {
            get => _Phone;
            set
            {
                _Phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }
        public ICommand AddSupplierCommand { get; private set; }

        public ViewModelAddSupplier(string connect) 
        {
            connectionString = connect;
            AddSupplierCommand = new DelegateCommand(AddTypeExecute, CanAddTypeExecute);
        }

        private bool CanAddTypeExecute(object obj)
        {
            return !string.IsNullOrWhiteSpace(Name)&& !string.IsNullOrWhiteSpace(Phone)&& !string.IsNullOrWhiteSpace(Address);
        }

        private void AddTypeExecute(object obj)
        {
                if (string.IsNullOrEmpty(Name) || Address == null || Phone == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    M_Suppliers.AddNewSupplier(Name, Address, Phone);

                    MessageBox.Show("Поставщик успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            Name = null;
            Address = null;
            Phone = null;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

