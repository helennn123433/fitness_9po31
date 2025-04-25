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
    /// Логика взаимодействия для CoachToolbar.xaml
    /// </summary>
    public partial class CoachToolbar : Page
    {
        public CoachToolbar()
        {
            InitializeComponent();
        }

        private void HallsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new HallsManagementPage());
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new ClientsManagementPage());
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainFrame.Navigate(new SchedulePage());
        }
    }
}
