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

namespace fitness.Windows
{
    /// <summary>
    /// Логика взаимодействия для ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        private string confirmCode;
        private User currentUser;
        public ResetPasswordWindow()
        {
            InitializeComponent();
            currentUser = LoginWindow.currentUser;
        }

        private void SendVerificationEmail_Click(object sender, RoutedEventArgs e)
        {
            confirmCode = GeneratorHelper.GenerateConfirmCode();
            EmailSender.SendConfirmChangePasswordEmail(currentUser.Email, "Подтверждение смены пароля", confirmCode);
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (confirmCode != null && confirmCode != "" && confirmCode == confirmTextBox.Text)
            {
                if (NewPasswordBox.Password == ConfirmPasswordBox.Password)
                {
                    if (UserRepository.IsCorrectPassword(currentUser.UserId, CurrentPasswordBox.Password))
                    {
                        UserRepository.UpdatePassword(currentUser.UserId, CurrentPasswordBox.Password, NewPasswordBox.Password);
                        MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Пароль успешно изменён");
                        messageBoxWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Текущей пароль введён некорректно");
                        messageBoxWindow.ShowDialog();
                    }
                }
                else
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Пароли должны совпадать");
                    messageBoxWindow.ShowDialog();
                }
            }
            else
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Необходимо пройти верификацию по email");
                messageBoxWindow.ShowDialog();
            }
        }
    }
}
