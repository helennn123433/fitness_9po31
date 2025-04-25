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
    /// Логика взаимодействия для PassportWindow.xaml
    /// </summary>
    public partial class PassportWindow : Window
    {
        public string Series { get; set; }
        public string Number { get; set; }
        public string IssuedBy { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string Address { get; set; }
        public PassportWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SeriesTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите серию паспорта");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(NumberTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите номер паспорта");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(IssuedByTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите кем выдан паспорта");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(AddressTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите адрес регистрации");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (IssueDatePicker.SelectedDate == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите дату выдачи паспорта");
                messageBoxWindow.ShowDialog();
                return;
            }

            Series = SeriesTextBox.Text;
            Number = NumberTextBox.Text;
            IssuedBy = IssuedByTextBox.Text;
            IssuedDate = IssueDatePicker.SelectedDate;
            Address = AddressTextBox.Text;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
