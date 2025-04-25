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
    /// Логика взаимодействия для TrainersManagementPage.xaml
    /// </summary>
    public partial class TrainersManagementPage : Page
    {
        public TrainersManagementPage()
        {
            InitializeComponent();
            LoadTrainers();
            if (!UserRepository.IsAdmin(LoginWindow.currentUser.UserId))
                ActionBar.Visibility = Visibility.Collapsed;
        }

        private void LoadTrainers()
        {
            TrainersDataGrid.ItemsSource = TrainerRepository.GetAllTrainer();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TrainerWindow trainerWindow = new TrainerWindow();
            trainerWindow.ShowDialog();
            if (trainerWindow.DialogResult == true)
            {
                LoadTrainers();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTrainer = TrainersDataGrid.SelectedItem as TrainerDetail;

            if (selectedTrainer == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите тренера для редактирования");
                messageBoxWindow.ShowDialog();
                return;
            }

            TrainerWindow trainerWindow = new TrainerWindow(selectedTrainer, true);
            trainerWindow.ShowDialog();
            if (trainerWindow.DialogResult == true)
            {
                LoadTrainers();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTrainer = TrainersDataGrid.SelectedItem as TrainerDetail;

            if (selectedTrainer == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите тренера для удаления");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите удалить тренера {selectedTrainer.LastName} {selectedTrainer.FirstName}", true);
            confirmMessageBox.ShowDialog();
            if (confirmMessageBox.DialogResult == true)
            {
                if (TrainerRepository.DeleteTrainer(selectedTrainer))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Тренер успешно удален");
                    messageBoxWindow.ShowDialog();
                    LoadTrainers();
                }

            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTrainer = TrainersDataGrid.SelectedItem as TrainerDetail;

            if (selectedTrainer == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите тренера для сбрасывания пароля");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите сбросить пароля у тренера {selectedTrainer.LastName} {selectedTrainer.FirstName}", true);
            confirmMessageBox.ShowDialog();

            if (confirmMessageBox.DialogResult == true)
            {
                if (TrainerRepository.ResetPassword(selectedTrainer))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Пароль успешно сброшен");
                    messageBoxWindow.ShowDialog();
                }
            }
        }
    }
}
