using ArpScan.Desktop.WPF.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArpScan.Desktop.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowsViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowsViewModel();
            viewModel.window = this;
            DataContext = viewModel;
        }
    }
}