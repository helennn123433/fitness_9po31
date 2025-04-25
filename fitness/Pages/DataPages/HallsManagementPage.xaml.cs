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
    /// Логика взаимодействия для HallsManagementPage.xaml
    /// </summary>
    public partial class HallsManagementPage : Page
    {
        public HallsManagementPage()
        {
            InitializeComponent();
            LoadHalls();
            if (!UserRepository.IsAdmin(LoginWindow.currentUser.UserId))
                ActionBar.Visibility = Visibility.Collapsed;
        }

        private void LoadHalls()
        {
            var temp = HallRepository.GetAllHalls();
            HallsDataGrid.ItemsSource = HallRepository.GetAllHalls();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            HallWindow hallWindow = new HallWindow();
            hallWindow.ShowDialog();
            if (hallWindow.DialogResult == true)
            {
                LoadHalls();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedHall = HallsDataGrid.SelectedItem as HallDetail;

            if (selectedHall == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите зал для редактирования");
                messageBoxWindow.ShowDialog();
                return;
            }

            HallWindow hallWindow = new HallWindow(selectedHall.IdHall, true);
            hallWindow.ShowDialog();
            if (hallWindow.DialogResult == true)
            {
                LoadHalls();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedHall = HallsDataGrid.SelectedItem as HallDetail;

            if (selectedHall == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите зал для удаления");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите удалить зал '{selectedHall.HallType}'", true);
            confirmMessageBox.ShowDialog();
            if (confirmMessageBox.DialogResult == true)
            {
                if (HallRepository.DeleteHall((int)selectedHall.IdHall))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Зал успешно удален");
                    messageBoxWindow.ShowDialog();
                    LoadHalls();
                }

            }
        }

        private void HallsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedHall = HallsDataGrid.SelectedItem as HallDetail;

            if (selectedHall != null)
            {
                var viewWindow = new HallWindow(selectedHall.IdHall, false);

                viewWindow.hallTypeComboBox.IsEditable = false;
                viewWindow.capacityTextBox.IsReadOnly = true;
                viewWindow.areaTextBox.IsReadOnly = true;

                viewWindow.ShowDialog();
            }
        }

    }
}
