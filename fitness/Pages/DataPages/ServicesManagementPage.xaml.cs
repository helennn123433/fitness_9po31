using fitness.DBAccess;
using fitness.Models;
using fitness.Windows;
using fitness.Windows.ActionWindows;
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

namespace fitness.Pages.DataPages
{
    /// <summary>
    /// Логика взаимодействия для ServicesManagementPage.xaml
    /// </summary>
    public partial class ServicesManagementPage : Page
    {
        public ServicesManagementPage()
        {
            InitializeComponent();
            LoadServices();
            if (!UserRepository.IsAdmin(LoginWindow.currentUser.UserId))
                ActionBar.Visibility = Visibility.Collapsed;
        }

        private void LoadServices()
        {
            ServicesDataGrid.ItemsSource = AdditionalServiceRepository.GetAllAdditionalService();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AdditionalServiceWindow additionalServiceWindow = new AdditionalServiceWindow();
            additionalServiceWindow.ShowDialog();
            if (additionalServiceWindow.DialogResult == true)
            {
                LoadServices();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedService = ServicesDataGrid.SelectedItem as AdditionalService;

            if (selectedService == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите услугу для редактирования");
                messageBoxWindow.ShowDialog();
                return;
            }

            AdditionalServiceWindow additionalServiceWindow = new AdditionalServiceWindow(selectedService, true);
            additionalServiceWindow.ShowDialog();
            if (additionalServiceWindow.DialogResult == true)
            {
                LoadServices();
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedService = ServicesDataGrid.SelectedItem as AdditionalService;

            if (selectedService == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите услугу для удаления");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите удалить услугу '{selectedService.ServicesName}'", true);
            confirmMessageBox.ShowDialog();
            if (confirmMessageBox.DialogResult == true)
            {
                if (AdditionalServiceRepository.DeleteAdditionalService(selectedService))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Услуга успешно удалена");
                    messageBoxWindow.ShowDialog();
                    LoadServices();
                }
            }
        }

        private void ServicesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedService = ServicesDataGrid.SelectedItem as AdditionalService;

            if (selectedService != null)
            {
                var viewWindow = new AdditionalServiceWindow(selectedService, false);

                viewWindow.serviceName.IsReadOnly = true;
                viewWindow.servicePrice.IsReadOnly = true;
                viewWindow.serviceDesc.IsReadOnly = true;

                viewWindow.ShowDialog();
            }
        }
    }
}
