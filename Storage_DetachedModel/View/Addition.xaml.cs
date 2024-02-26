using Storage_DetachedModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Storage_DetachedModel.View
{
    /// <summary>
    /// Interaction logic for Addition.xaml
    /// </summary>
    public partial class Addition : Window
    {
        public Addition()
        {
            InitializeComponent();
            this.DataContext = new AdditionViewModel();
        }
    }
}
