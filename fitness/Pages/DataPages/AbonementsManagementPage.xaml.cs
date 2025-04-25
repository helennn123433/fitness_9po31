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
    /// Логика взаимодействия для AbonementsManagementPage.xaml
    /// </summary>
    public partial class AbonementsManagementPage : Page
    {
        public AbonementsManagementPage()
        {
            InitializeComponent();
            LoadAbonements();
            if (!UserRepository.IsAdmin(LoginWindow.currentUser.UserId))
                ActionBar.Visibility = Visibility.Collapsed;
        }

        private void LoadAbonements()
        {
            AbonementsDataGrid.ItemsSource = AbonementRepository.GetAllAbonement();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AbonementWindow abonementWindow = new AbonementWindow();
            abonementWindow.ShowDialog();
            if (abonementWindow.DialogResult == true)
            {
                LoadAbonements();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAbonement = AbonementsDataGrid.SelectedItem as Abonement;

            if (selectedAbonement == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите абонемент для редактирования");
                messageBoxWindow.ShowDialog();
                return;
            }

            AbonementWindow abonementWindow= new AbonementWindow(selectedAbonement, true);
            abonementWindow.ShowDialog();
            if (abonementWindow.DialogResult == true)
            {
                LoadAbonements();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAbonement = AbonementsDataGrid.SelectedItem as Abonement;

            if (selectedAbonement == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите абонемент для удаления");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите удалить абонемент '{selectedAbonement.AbonementName}'", true);
            confirmMessageBox.ShowDialog();
            if (confirmMessageBox.DialogResult == true)
            {
                if (AbonementRepository.DeleteAbonement(selectedAbonement))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Абонемент успешно удален");
                    messageBoxWindow.ShowDialog();
                    LoadAbonements();
                }

            }
        }
    }
}
