using Word = Microsoft.Office.Interop.Word;
using fitness.DBAccess;
using fitness.Models;
using fitness.Windows;
using fitness.Windows.ActionWindows;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ClientsManagementPage.xaml
    /// </summary>
    public partial class ClientsManagementPage : System.Windows.Controls.Page
    {
        public ClientsManagementPage()
        {
            InitializeComponent();
            LoadClients();
            if (!UserRepository.IsManager(LoginWindow.currentUser.UserId))
                ActionBar.Visibility = Visibility.Collapsed;


        }

        private void LoadClients()
        {
            ClientsDataGrid.ItemsSource = ClientRepository.GetAllClients();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow();
            clientWindow.ShowDialog();
            if (clientWindow.DialogResult == true)
            {
                LoadClients();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientsDataGrid.SelectedItem as ClientInfoView;

            if (selectedClient == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите клиента для редактирования");
                messageBoxWindow.ShowDialog();
                return;
            }

            ClientWindow clientWindow = new ClientWindow(selectedClient, true);
            clientWindow.ShowDialog();
            if (clientWindow.DialogResult == true)
            {
                LoadClients();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            var selectedClient = ClientsDataGrid.SelectedItem as ClientInfoView;

            if (selectedClient == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите клиента для удаления");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите удалить клиента {selectedClient.LastName} {selectedClient.FirstName}", true);
            confirmMessageBox.ShowDialog();
            if (confirmMessageBox.DialogResult == true)
            {
                if (ClientRepository.DeleteClient(selectedClient))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Клиент успешно удален");
                    messageBoxWindow.ShowDialog();
                    LoadClients();
                }

            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientsDataGrid.SelectedItem as ClientInfoView;

            if (selectedClient == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите менеджера для сбрасывания пароля");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmMessageBox = new MessageBoxWindow($"Вы уверены что хотите сбросить пароля у менеджера {selectedClient.LastName} {selectedClient.FirstName}", true);
            confirmMessageBox.ShowDialog();

            if (confirmMessageBox.DialogResult == true)
            {
                if (ClientRepository.ResetPassword((int)selectedClient.ClientId))
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Пароль успешно сброшен");
                    messageBoxWindow.ShowDialog();
                }
            }
        }

        private void PrintConsentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientsDataGrid.SelectedItem as ClientInfoView;

            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для печати согласия.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                PassportWindow passportWindow = new PassportWindow();
                passportWindow.ShowDialog();
                if (passportWindow.DialogResult == true)
                {
                    string templatePath = @"..\..\..\blank-soglasie-na-obrabotku-personalnyh-dannyh.docx";

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Word Documents (*.docx)|*.docx";
                    saveFileDialog.Title = "Выберите место для сохранения согласия";
                    saveFileDialog.FileName = $"Согласие_На_Обработку_Данных_{selectedClient.LastName}_{selectedClient.FirstName}_{selectedClient.MiddleName}.docx";


                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string outputPath = saveFileDialog.FileName;

                        File.Copy(templatePath, outputPath, true);

                        Word.Application app = new Word.Application();

                        object missing = Type.Missing;

                        string[] data = { $"{selectedClient.LastName} {selectedClient.FirstName} {selectedClient.MiddleName}", passportWindow.Series, passportWindow.Number, passportWindow.IssuedBy, DateOnly.FromDateTime((DateTime)passportWindow.IssuedDate).ToString(), passportWindow.Address};
                        string[] finds = { "[ФИО]", "[CЕРИЯ]", "[НОМЕР]", "[КЕМ_ВЫДАН]", "[КОГДА_ВЫДАН]", "[АДРЕС_РЕГИСТРАЦИИ]" };

                        Word.Document doc;

                        for (int i = 0; i < finds.Length; ++i)
                        {
                            doc = app.Documents.Open(outputPath, missing, missing);
                            app.Selection.Find.Execute(finds[i], missing, missing, missing, missing, missing, missing, missing, missing, data[i]);
                            doc.Save();
                            doc.Close();
                        }

                        var process = new System.Diagnostics.Process();
                        process.StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = outputPath,
                            UseShellExecute = true,
                            Verb = "print"
                        };
                        process.Start();

                        MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Согласие подготовлено и отправлено на печать.");
                        messageBoxWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Вы не выбрали место для сохранения файла.");
                        messageBoxWindow.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow($"Ошибка при создании согласия: {ex.Message}");
                messageBoxWindow.ShowDialog();
            }
        }

    }
}
