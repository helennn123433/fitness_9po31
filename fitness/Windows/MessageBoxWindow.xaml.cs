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
    /// Логика взаимодействия для MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        public bool? DialogResult { get; private set; }
        public MessageBoxWindow(string message, bool isConfirmation = false)
        {
            InitializeComponent();
            MessageText.Text = message;

            if (isConfirmation)
            {
                YesNoPanel.Visibility = Visibility.Visible;
                OkPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                YesNoPanel.Visibility = Visibility.Collapsed;
                OkPanel.Visibility = Visibility.Visible;
            }
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnYesClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
