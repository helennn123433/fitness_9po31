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
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private User CurrentManager {  get; set; }
        public bool IsEditMode { get; }
        public ManagerWindow(UserWithRole? manager = null, bool isEditMode = false)
        {
            InitializeComponent();

            if (manager != null)
            {
                CurrentManager = new User()
                {
                    UserId = (int)manager.UserId,
                    LastnameUser = manager.LastName,
                    NameUser = manager.FirstName,
                    MiddleName = manager.MiddleName,
                    Email = manager.Email,
                    PhoneNumber = manager.PhoneNumber,
                    AgeUser = manager.AgeUser
                };
            }
            else 
                CurrentManager = new User();

            IsEditMode = isEditMode;

            Title = IsEditMode ? "Редактирование менеджера" : "Добавление менеджера";
            WindowTitle.Text = Title;

            if (CurrentManager.UserId != null && CurrentManager.UserId != 0)
            {
                MessageBox.Show((CurrentManager.UserId != 0).ToString());
                loginTextBox.Visibility = Visibility.Collapsed;
                loginLabel.Visibility = Visibility.Collapsed;
                passwordTextBox.Visibility = Visibility.Collapsed;
                passwordLabel.Visibility = Visibility.Collapsed;

                lastnameTextBox.Text = CurrentManager.LastnameUser;
                nameTextBox.Text = CurrentManager.NameUser;
                middleNameTextBox.Text = CurrentManager.MiddleName;
                emailTextBox.Text = CurrentManager.Email;
                phoneTextBox.Text = CurrentManager.PhoneNumber;
                ageTextBox.Text = CurrentManager.AgeUser.ToString();
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lastnameTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите фамилию менеджера");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(nameTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите имя менеджера");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(emailTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите email менеджера");
                messageBoxWindow.ShowDialog();
                return;
            }

            CurrentManager.NameUser = nameTextBox.Text;
            CurrentManager.LastnameUser = lastnameTextBox.Text;
            CurrentManager.MiddleName = middleNameTextBox.Text;
            CurrentManager.Email = emailTextBox.Text;
            CurrentManager.PhoneNumber = phoneTextBox.Text;
            CurrentManager.AgeUser = Convert.ToInt32(ageTextBox.Text);

            if (!IsEditMode)
            {
                CurrentManager.Login = loginTextBox.Text;
                CurrentManager.Password = PasswordHasher.HashPassword(passwordTextBox.Text);
            }

            bool success = false;
            if (!IsEditMode)
            {
                success = UserRepository.AddManager(CurrentManager);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Менеджер успешно добавлен");
                    messageBoxWindow.ShowDialog();
                }
            }
            else
            {
                success = UserRepository.UpdateUser(CurrentManager);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Информация о менеджере успешно обновлена");
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
