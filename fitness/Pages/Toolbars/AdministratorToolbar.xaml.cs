using fitness.Windows;
using fitness.Pages.DataPages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fitness.Pages.Toolbars
{
    /// <summary>
    /// Логика взаимодействия для AdministratorToolbar.xaml
    /// </summary>
    public partial class AdministratorToolbar : Page
    {
        public AdministratorToolbar()
        {
            InitializeComponent();
        }

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new ServicesManagementPage());
        }

        private void HallsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new HallsManagementPage());
        }

        private void TrainersButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new TrainersManagementPage());

        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new ManagersManagementPage());
        }

        private void SubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new AbonementsManagementPage());
        }
    }
}
