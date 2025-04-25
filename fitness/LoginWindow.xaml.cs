using fitness.DBAccess;
using fitness.Models;
using fitness.Windows;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fitness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static User currentUser = null;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = ShowPasswordButton.IsChecked == true
                            ? VisiblePasswordBox.Text
                            : PasswordBox.Password;

            if (string.IsNullOrEmpty(login))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите логин и пароль");
                messageBoxWindow.ShowDialog();
                return;
            }

           try
           {
                var user = UserRepository.Authenticate(login, password);

                if (user != null)
                {
                    currentUser = user;
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    this.Close();
                }
                else
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Неверный логин или пароль");
                    messageBoxWindow.ShowDialog();
                }
           }
           catch (System.Exception ex)
           {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow($"Ошибка авторизации: {ex.Message}");
                messageBoxWindow.ShowDialog();
           }
        }
        private void ShowPasswordButton_Checked(object sender, RoutedEventArgs e)
        {
            VisiblePasswordBox.Text = PasswordBox.Password;
            VisiblePasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;

        }

        private void ShowPasswordButton_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = VisiblePasswordBox.Text;
            PasswordBox.Visibility = Visibility.Visible;
            VisiblePasswordBox.Visibility = Visibility.Collapsed;
        }
    }
}