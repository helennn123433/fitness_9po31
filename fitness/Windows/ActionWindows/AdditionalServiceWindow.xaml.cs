using fitness.DBAccess;
using fitness.Models;
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
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme;

namespace fitness.Windows.ActionWindows
{
    /// <summary>
    /// Логика взаимодействия для AdditionalServiceWindow.xaml
    /// </summary>
    public partial class AdditionalServiceWindow : Window
    {
        private string _defaultImagePath = @"..\..\Images\picture.jpg";
        public AdditionalService CurrentService { get; set; }
        public bool IsEditMode { get; }
        public AdditionalServiceWindow(AdditionalService? service = null, bool isEditMode = false)
        {
            InitializeComponent();
            CurrentService = service ?? new AdditionalService();
            IsEditMode = isEditMode;

            Title = IsEditMode ? "Редактирование услуги" : "Добавление услуги";
            if (CurrentService.IdServices != 0 && !IsEditMode) 
            {
                Title = "Просмотр услуги: " + CurrentService.ServicesName;
            }
            WindowTitle.Text = Title;


            if (CurrentService.IdServices != 0)
            {
                serviceName.Text = CurrentService.ServicesName;
                servicePrice.Text = CurrentService.ServicesPrice.ToString();
                serviceDesc.Text = CurrentService.ServicesDescription;

                if (!string.IsNullOrEmpty(CurrentService.ImagePath))
                {
                    LoadImage(CurrentService.ImagePath);
                }
                else
                {
                    LoadImage(_defaultImagePath);
                }

                if (!IsEditMode)
                {
                    ButtonsPanel.Visibility = Visibility.Collapsed;
                    loadButton.Visibility = Visibility.Collapsed;
                    removeImageBtn.Visibility = Visibility.Collapsed;
                    closeBtn.Content = "ОК";
                }
            }
            else
            {
                LoadImage(_defaultImagePath);
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(serviceName.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите название услуги");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (Convert.ToInt32(servicePrice.Text) <= 0)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Цена должна быть больше нуля");
                messageBoxWindow.ShowDialog();
                return;
            }

            CurrentService.ServicesName = serviceName.Text;
            CurrentService.ServicesPrice = Convert.ToInt32(servicePrice.Text);
            CurrentService.ServicesDescription = serviceDesc.Text;

            bool success = false;
            if (!IsEditMode)
            {
                success = AdditionalServiceRepository.AddAdditionalService(CurrentService);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Услуга успешно добавлена");
                    messageBoxWindow.ShowDialog();
                }
            }
            else
            {
                success = AdditionalServiceRepository.UpdateAdditionalService(CurrentService);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Услуга успешно обновлена");
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

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            CurrentService.ImagePath = null;
            LoadImage(_defaultImagePath);
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Title = "Выберите изображение услуги"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);

                    string targetDirectory = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        @"..\..\..\Images\AdditionalService");

                    Directory.CreateDirectory(targetDirectory);

                    string targetPath = System.IO.Path.Combine(targetDirectory, fileName);

                    File.Copy(openFileDialog.FileName, targetPath, overwrite: true);

                    CurrentService.ImagePath = System.IO.Path.Combine(@"Images\AdditionalService", fileName);

                    LoadImage(targetPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadImage(string imagePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();

                if (!System.IO.Path.IsPathRooted(imagePath))
                {
                    string fullPath = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        @"..\..\..\",
                        imagePath);
                    bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(fullPath));
                }
                else
                    bitmap.UriSource = new Uri(imagePath);

                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                serviceImage.Source = bitmap;

                removeImageBtn.Visibility =
                    imagePath != _defaultImagePath ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                var defaultBitmap = new BitmapImage(new Uri(
                    System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        @"..\..\..\Images\picture.jpg"),
                    UriKind.RelativeOrAbsolute));
                serviceImage.Source = defaultBitmap;
                removeImageBtn.Visibility = Visibility.Collapsed;
            }
        }
    }
}
