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

namespace fitness.Windows.ActionWindows
{
    /// <summary>
    /// Логика взаимодействия для HallWindow.xaml
    /// </summary>
    public partial class HallWindow : Window
    {
        private string _defaultImagePath = @"..\..\Images\picture.jpg";
        private Hall CurrentHall { get; set; }
        private bool IsEditMode { get; }
        public HallWindow(int? hallId = 0, bool isEditMode = false)
        {
            InitializeComponent();
            CurrentHall = HallRepository.GetHallById((int)hallId) ?? new Hall();
            IsEditMode = isEditMode;

            Title = IsEditMode ? "Редактирование существующего зала" : "Добавление нового зала";
            if (CurrentHall.IdHall != 0 && !IsEditMode)
            {
                Title = "Просмотр зала";
            }
            WindowTitle.Text = Title;

            LoadHallTypes();

            if (CurrentHall.IdHall != 0)
            {
                capacityTextBox.Text = CurrentHall.CapacityHall.ToString();
                areaTextBox.Text = CurrentHall.Area.ToString();
                isActive.IsChecked = CurrentHall.IsActive;
                string type_hall = CurrentHall.IdTypeHallNavigation.NameTypeHall;
                var selectedHall = hallTypeComboBox.Items
                    .Cast<HallType>()
                    .FirstOrDefault(ht => ht.NameTypeHall == type_hall);

                if (selectedHall != null)
                    hallTypeComboBox.SelectedItem = selectedHall;

                if (!string.IsNullOrEmpty(CurrentHall.ImagePath))
                {
                    LoadImage(CurrentHall.ImagePath);
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

        private void LoadHallTypes()
        {
            hallTypeComboBox.ItemsSource = HallRepository.GetHallTypes();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(capacityTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите количество вмещаемых людей");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrWhiteSpace(areaTextBox.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите площадь зала");
                messageBoxWindow.ShowDialog();
                return;
            }

            if(hallTypeComboBox.SelectedItem is null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Необходимо выбрать тип зала");
                messageBoxWindow.ShowDialog();
                return;
            }

            CurrentHall.CapacityHall = Convert.ToInt32(capacityTextBox.Text);
            CurrentHall.Area = Convert.ToInt32(areaTextBox.Text);
            CurrentHall.IdTypeHall = (hallTypeComboBox.SelectedItem as HallType).IdTypeHall;
            CurrentHall.IsActive = isActive.IsChecked.Value;

            bool success = false;
            if (!IsEditMode)
            {
                success = HallRepository.AddHall(CurrentHall);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Зал успешно добавлен");
                    messageBoxWindow.ShowDialog();
                }
            }
            else
            {
                success = HallRepository.UpdateHall(CurrentHall);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Зал успешно обновлен");
                    messageBoxWindow.ShowDialog();
                }
            }

            if (success)
            {
                DialogResult = true;
                Close();
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
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
                hallImage.Source = bitmap;

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
                hallImage.Source = defaultBitmap;
                removeImageBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Title = "Выберите изображение зала"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);

                    string targetDirectory = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        @"..\..\..\Images\Halls");

                    Directory.CreateDirectory(targetDirectory);

                    string targetPath = System.IO.Path.Combine(targetDirectory, fileName);


                    File.Copy(openFileDialog.FileName, targetPath, overwrite: true);

                    CurrentHall.ImagePath = System.IO.Path.Combine(@"Images\Halls", fileName);

                    LoadImage(targetPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            CurrentHall.ImagePath = null;
            LoadImage(_defaultImagePath);
        }
    }
}
