using Storage_DetachedModel.Commands;
using Storage_DetachedModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Storage_DetachedModel.ViewModel
{
    public class SupplierFull
    {
        public int SupplierID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public SupplierFull DeepCopy()
        {
            return new SupplierFull
            {
                SupplierID = this.SupplierID,
                Name = this.Name,
                Address = this.Address,
                Phone = this.Phone
            };
        }
    }

    internal class ViewModelUpdateSupplier : INotifyPropertyChanged
    {
        private string connectionString;

        private SupplierFull originalSelectedSupplier; 
        public ICommand UpdateSupplierCommand { get; private set; }

        private SupplierFull selectedSupplier;
        public SupplierFull SelectedSupplier 
        {
            get => selectedSupplier;
            set
            {
                if (selectedSupplier != value)
                {
                    selectedSupplier = value;
                    OnPropertyChanged(nameof(SelectedSupplier));
                    Name = selectedSupplier?.Name;
                    Address = selectedSupplier?.Address;
                    Phone = selectedSupplier?.Phone;
                    originalSelectedSupplier = selectedSupplier?.DeepCopy();
                }
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string address;
        public string Address
        {
            get => address;
            set
            {
                if (address != value)
                {
                    address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        private string phone;
        public string Phone
        {
            get => phone;
            set
            {
                if (phone != value)
                {
                    phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        public ObservableCollection<SupplierFull> suppliers = new ObservableCollection<SupplierFull>();
        public ObservableCollection<SupplierFull> Suppliers
        {
            get => suppliers;
            set
            {
                suppliers = value;
                OnPropertyChanged(nameof(Suppliers));
            }
        }

        public ViewModelUpdateSupplier(string connectionString) 
        {
            this.connectionString = connectionString;
            LoadSuppliers();
            UpdateSupplierCommand = new DelegateCommand(UpdateSupplier, CanUpdateSupplier);
        }

        private bool CanUpdateSupplier(object obj)
        {
            return selectedSupplier != null &&
            (Name != originalSelectedSupplier.Name ||
             Address != originalSelectedSupplier.Address ||
             Phone != originalSelectedSupplier.Phone);
        }

        private void UpdateSupplier(object obj)
        {
            try
            {
               M_Suppliers.UpdateSupplier(SelectedSupplier.SupplierID, Name, Address, Phone);
                MessageBox.Show("Поставщик успешно обновлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления поставщика: {ex.Message}");
            }
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            Suppliers.Clear();

            try
            {
                foreach (var supplier in M_Suppliers.AllSuppliers)
                {
                    Suppliers.Add(new SupplierFull
                    {
                        SupplierID = supplier.SupplierID,
                        Name = supplier.Name,
                        Address = supplier.Address ?? string.Empty,
                        Phone = supplier.Phone ?? string.Empty
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
