using fitness.Pages.DataPages;
using fitness.Windows;
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
    /// Логика взаимодействия для UserToolbar.xaml
    /// </summary>
    public partial class ClientToolbar : Page
    {
        public ClientToolbar()
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

        private void SubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new AbonementsManagementPage());
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new SchedulePage());
        }
    }
}
