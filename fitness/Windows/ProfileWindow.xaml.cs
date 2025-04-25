using fitness.DBAccess;
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

namespace fitness.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private User currentUser;
        private bool isReadOnlyMode = true;
        public ProfileWindow()
        {
            InitializeComponent();
            currentUser = LoginWindow.currentUser;
            NameTextBox.Text = currentUser.NameUser;
            LastNameTextBox.Text = currentUser.LastnameUser;
            MiddleNameTextBox.Text = currentUser.MiddleName;
            EmailTextBox.Text = currentUser.Email;
            PhoneTextBox.Text = currentUser.PhoneNumber;
            AgeTextBox.Text = currentUser.AgeUser.ToString();
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (!isReadOnlyMode)
            {
                currentUser.NameUser = NameTextBox.Text;
                currentUser.LastnameUser = LastNameTextBox.Text;
                currentUser.MiddleName = MiddleNameTextBox.Text;
                currentUser.Email = EmailTextBox.Text;
                currentUser.PhoneNumber = PhoneTextBox.Text;
                currentUser.AgeUser = Convert.ToInt32(AgeTextBox.Text);
                if (UserRepository.UpdateUser(currentUser))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Информация успешно обновлена");
                    messageBoxWindow.ShowDialog();
                }
                else
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Ошибка при обновлении информации");
                    messageBoxWindow.ShowDialog();
                }
            }
            isReadOnlyMode = !isReadOnlyMode;
            NameTextBox.IsReadOnly = isReadOnlyMode;
            LastNameTextBox.IsReadOnly= isReadOnlyMode;
            MiddleNameTextBox.IsReadOnly = isReadOnlyMode;
            EmailTextBox.IsReadOnly = isReadOnlyMode;
            PhoneTextBox.IsReadOnly = isReadOnlyMode;
            AgeTextBox.IsReadOnly = isReadOnlyMode;
            if (isReadOnlyMode)
                profSettingsButton.Content = "Редактировать профиль";
            else
                profSettingsButton.Content = "Сохранить";
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ResetPasswordWindow resetPasswordWindow = new ResetPasswordWindow();
            resetPasswordWindow.ShowDialog();
        }
    }
}
