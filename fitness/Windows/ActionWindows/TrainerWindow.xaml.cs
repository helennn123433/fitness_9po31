using fitness.DBAccess;
using fitness.Helpers;
using fitness.Models;
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

namespace fitness.Windows.ActionWindows
{
    /// <summary>
    /// Логика взаимодействия для TrainerWindow.xaml
    /// </summary>
    public partial class TrainerWindow : Window
    {
        private TrainerDetail CurrentTrainer { get; set; }

        public bool IsEditMode { get; }
        public TrainerWindow(TrainerDetail? trainer = null, bool isEditMode = false)
        {
            InitializeComponent();
            CurrentTrainer = trainer ?? new TrainerDetail();
            IsEditMode = isEditMode;

            Title = IsEditMode ? "Редактирование тренера" : "Добавление тренера";
            WindowTitle.Text = Title;

            if (CurrentTrainer.IdTrainer != null)
            {
                loginTextBox.Visibility = Visibility.Collapsed;
                loginLabel.Visibility = Visibility.Collapsed;
                passwordTextBox.Visibility = Visibility.Collapsed;
                passwordLabel.Visibility = Visibility.Collapsed;

                lastnameTextBox.Text = CurrentTrainer.LastName;
                nameTextBox.Text = CurrentTrainer.FirstName;
                middleNameTextBox.Text = CurrentTrainer.MiddleName;
                emailTextBox.Text = CurrentTrainer.Email;
                phoneTextBox.Text = CurrentTrainer.PhoneNumber;
                ageTextBox.Text = CurrentTrainer.AgeUser.ToString();
                educationTextBox.Text = CurrentTrainer.Education;
                experienceYearsTextBox.Text = CurrentTrainer.ExperienceYears.ToString();
            }

        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lastnameTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите фамилию тренера");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(nameTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите имя тренера");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(emailTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите email тренера");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(educationTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите образование тренера");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(experienceYearsTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите опыт работы тренера");
                messageBoxWindow.ShowDialog();
                return;
            }

            CurrentTrainer.FirstName = nameTextBox.Text;
            CurrentTrainer.LastName = lastnameTextBox.Text;
            CurrentTrainer.MiddleName = middleNameTextBox.Text;
            CurrentTrainer.Email = emailTextBox.Text;
            CurrentTrainer.PhoneNumber = phoneTextBox.Text;
            CurrentTrainer.AgeUser = Convert.ToInt32(ageTextBox.Text);
            CurrentTrainer.Education = educationTextBox.Text;
            CurrentTrainer.ExperienceYears = Convert.ToInt32(experienceYearsTextBox.Text);

            if (!IsEditMode)
            {
                CurrentTrainer.Login = loginTextBox.Text;
                CurrentTrainer.Password = PasswordHasher.HashPassword(passwordTextBox.Text);
            }

            bool success = false;
            if (!IsEditMode)
            {
                success = TrainerRepository.AddTrainer(CurrentTrainer);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Тренер успешно добавлен");
                    messageBoxWindow.ShowDialog();
                }
            }
            else
            {
                success = TrainerRepository.UpdateTrainer(CurrentTrainer);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Информация о тренере успешно обновлена");
                    messageBoxWindow.ShowDialog();
                }
            }

            if (success)
            {
                DialogResult = true;
                Close();
            }

        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
