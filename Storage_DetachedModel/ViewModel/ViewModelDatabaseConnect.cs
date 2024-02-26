using Storage_DetachedModel.Commands;
using Storage_DetachedModel.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Storage_DetachedModel.ViewModel
{
    internal class ViewModelDatabaseConnect : INotifyPropertyChanged
    {
        private Storage _windowStorage;

        string connectionString = null;

        public string UserName
        {
            get { return DatabaseConnectionInfo.UserName; }
            set
            {
                if (DatabaseConnectionInfo.UserName != value)
                {
                    DatabaseConnectionInfo.UserName = value;
                    OnPropertyChanged(nameof(UserName));
                    (ConnectCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string DatabaseName
        {
            get { return DatabaseConnectionInfo.DatabaseName; }
            set
            {
                if (DatabaseConnectionInfo.DatabaseName != value)
                {
                    DatabaseConnectionInfo.DatabaseName = value;
                    OnPropertyChanged(nameof(DatabaseName));
                    (ConnectCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand ConnectCommand { get; private set; }

        public ViewModelDatabaseConnect()
        {
            ConnectCommand = new AsyncDelegateCommand(
                 execute: async () => await ConnectToDatabase(),
                 canExecute: () => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(DatabaseName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task ConnectToDatabase() 
        {
            try
            {
                connectionString = DatabaseConnectionInfo.GetConnectionString();
                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    MessageBox.Show("Успешное подключение к базе данных!", "Подключение", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                    _windowStorage = new Storage(connectionString);
                _windowStorage.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}
