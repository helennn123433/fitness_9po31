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
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private ClientInfoView CurrentClient {  get; set; }
        private bool IsEditMode { get; }
        public ClientWindow(ClientInfoView client = null, bool isEditMode = false)
        {
            InitializeComponent();
            LoadAbonements();
            CurrentClient = client ?? new ClientInfoView();
            IsEditMode = isEditMode;

            Title = IsEditMode ? "Редактирование клиента" : "Добавление клиента";
            WindowTitle.Text = Title;

            if (CurrentClient.ClientId != null)
            {
                loginTextBox.Visibility = Visibility.Collapsed;
                loginLabel.Visibility = Visibility.Collapsed;
                passwordTextBox.Visibility = Visibility.Collapsed;
                passwordLabel.Visibility = Visibility.Collapsed;

                lastnameTextBox.Text = CurrentClient.LastName;
                nameTextBox.Text = CurrentClient.FirstName;
                middleNameTextBox.Text = CurrentClient.MiddleName;
                emailTextBox.Text = CurrentClient.Email;
                phoneTextBox.Text = CurrentClient.PhoneNumber;
                ageTextBox.Text = CurrentClient.Age.ToString();
                datePicker.SelectedDate = CurrentClient.AbonementStartDate?.ToDateTime(TimeOnly.MinValue);

                abonementComboBox.SelectedItem = abonementComboBox.Items
                    .Cast<Abonement>()
                    .FirstOrDefault(a => a.AbonementName == CurrentClient.AbonementName);
            }
        }

        private void LoadAbonements()
        {
            abonementComboBox.ItemsSource = AbonementRepository.GetAllAbonement();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lastnameTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите фамилию клиента");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(nameTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите имя клиента");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(emailTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите email клиента");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (abonementComboBox.SelectedItem == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите абонемент для клиента");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (datePicker.SelectedDate == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите дату начала абонемент");
                messageBoxWindow.ShowDialog();
                return;
            }

            CurrentClient.FirstName = nameTextBox.Text;
            CurrentClient.LastName = lastnameTextBox.Text;
            CurrentClient.MiddleName = middleNameTextBox.Text;
            CurrentClient.Email = emailTextBox.Text;
            CurrentClient.PhoneNumber = phoneTextBox.Text;
            CurrentClient.Age = Convert.ToInt32(ageTextBox.Text);
            CurrentClient.AbonementName = (abonementComboBox.SelectedItem as Abonement).AbonementName;
            CurrentClient.AbonementStartDate = DateOnly.FromDateTime((DateTime)datePicker.SelectedDate);

            if (!IsEditMode)
            {
                CurrentClient.Login = loginTextBox.Text;
                CurrentClient.Password = PasswordHasher.HashPassword(passwordTextBox.Text);
            }

            bool success = false;
            if (!IsEditMode)
            {
                success = ClientRepository.AddClient(CurrentClient);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Клиент успешно добавлен");
                    messageBoxWindow.ShowDialog();
                }
            }
            else
            {
                success = ClientRepository.UpdateClient(CurrentClient);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Информация о клиенте успешно обновлена");
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

        }
    }
}
