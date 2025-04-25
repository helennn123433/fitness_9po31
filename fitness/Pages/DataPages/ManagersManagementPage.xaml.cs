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
    /// Логика взаимодействия для ManagersManagementPage.xaml
    /// </summary>
    public partial class ManagersManagementPage : Page
    {
        public ManagersManagementPage()
        {
            InitializeComponent();
            LoadManagers();
            if (!UserRepository.IsAdmin(LoginWindow.currentUser.UserId))
                ActionBar.Visibility = Visibility.Collapsed;
        }

        private void LoadManagers()
        {
            ManagersDataGrid.ItemsSource = UserRepository.GetAllManagers();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow managerWindow = new ManagerWindow();
            managerWindow.ShowDialog();
            if (managerWindow.DialogResult == true)
            {
                LoadManagers();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedManager = ManagersDataGrid.SelectedItem as UserWithRole;

            if (selectedManager == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите менеджера для редактирования");
                messageBoxWindow.ShowDialog();
                return;
            }

            ManagerWindow trainerWindow = new ManagerWindow(selectedManager, true);
            trainerWindow.ShowDialog();
            if (trainerWindow.DialogResult == true)
            {
                LoadManagers();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedManager = ManagersDataGrid.SelectedItem as UserWithRole;

            if (selectedManager == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите менеджера для удаления");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите удалить менеджера {selectedManager.LastName} {selectedManager.FirstName}", true);
            confirmMessageBox.ShowDialog();
            if (confirmMessageBox.DialogResult == true)
            {
                if (UserRepository.DeleteUser((int)selectedManager.UserId))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Менеджер успешно удален");
                    messageBoxWindow.ShowDialog();
                    LoadManagers();
                }

            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedManager = ManagersDataGrid.SelectedItem as UserWithRole;

            if (selectedManager == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите менеджера для сбрасывания пароля");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите сбросить пароля у менеджера {selectedManager.LastName} {selectedManager.FirstName}", true);
            confirmMessageBox.ShowDialog();

            if (confirmMessageBox.DialogResult == true)
            {
                if (UserRepository.ResetPassword((int)selectedManager.UserId))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Пароль успешно сброшен");
                    messageBoxWindow.ShowDialog();
                }
            }
        }
    }
}
