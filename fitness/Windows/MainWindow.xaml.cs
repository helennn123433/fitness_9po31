using fitness.Models;
using fitness.Pages.Toolbars;
using fitness.Pages;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User user;
        public static Frame mainFrame;
        public MainWindow()
        {
            InitializeComponent();
            user = LoginWindow.currentUser;
            mainFrame = MainFrame;

            string userRole = user.Role.Name.ToLower();
            if (userRole == "клиент")
                ToolbarFrame.Navigate(new ClientToolbar());
            else if (userRole == "тренер")
                ToolbarFrame.Navigate(new CoachToolbar());
            else if (userRole == "менеджер")
                ToolbarFrame.Navigate(new ManagerToolbar());
            else if (userRole == "админ")
                ToolbarFrame.Navigate(new AdministratorToolbar());

            mainFrame.Navigate(new StartPage());
            fioTextBlock.Text = $"{LoginWindow.currentUser.LastnameUser} {LoginWindow.currentUser.NameUser}";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void fioTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.ShowDialog();
            fioTextBlock.Text = $"{LoginWindow.currentUser.LastnameUser} {LoginWindow.currentUser.NameUser}";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Closing -= Window_Closing;
            LoginWindow.currentUser = null;
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}
