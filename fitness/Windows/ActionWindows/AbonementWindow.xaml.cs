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

namespace fitness.Windows.ActionWindows
{
    /// <summary>
    /// Логика взаимодействия для AbonementWindow.xaml
    /// </summary>
    public partial class AbonementWindow : Window
    {
        public Abonement CurrentAbonement { get; set; }
        public bool IsEditMode { get; }

        public AbonementWindow(Abonement? abonement = null, bool isEditMode = false)
        {
            InitializeComponent();

            CurrentAbonement = abonement ?? new Abonement();
            IsEditMode = isEditMode;

            Title = IsEditMode ? "Редактирование услуги" : "Добавление услуги";
            WindowTitle.Text = Title;


            if (CurrentAbonement.IdAbonement != 0)
            {
                abonementName.Text = CurrentAbonement.AbonementName;
                abonementPrice.Text = CurrentAbonement.AbonementPrice.ToString();
                abonementDays.Text = CurrentAbonement.AbonementLong.ToString();
                abonementFreeze.Text = CurrentAbonement.AbonementFreeze.ToString();
                abonementDesc.Text = CurrentAbonement.AbonementDescription;
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(abonementName.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите название абонемента");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(abonementPrice.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите стоимость абонемента");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(abonementDays.Text))
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Введите длительность абонемента в днях");
                messageBoxWindow.ShowDialog();
                return;
            }


            CurrentAbonement.AbonementName = abonementName.Text;
            CurrentAbonement.AbonementPrice = Convert.ToInt32(abonementPrice.Text);
            CurrentAbonement.AbonementLong = Convert.ToInt32(abonementDays.Text);
            CurrentAbonement.AbonementFreeze = Convert.ToInt32(abonementFreeze.Text);
            CurrentAbonement.AbonementDescription = abonementDesc.Text;

            bool success = false;
            if (!IsEditMode)
            {
                success = AbonementRepository.AddAbonement(CurrentAbonement);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Абонемент успешно добавлен");
                    messageBoxWindow.ShowDialog();
                }
            }
            else
            {
                success = AbonementRepository.UpdateAbonement(CurrentAbonement);
                if (success)
                {
                    MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Абонемент успешно обновлен");
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
    }
}
