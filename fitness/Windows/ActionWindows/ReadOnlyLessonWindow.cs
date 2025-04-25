using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace fitness.Windows.ActionWindows
{
    public partial class ReadOnlyLessonWindow : LessonWindow
    {
        public ReadOnlyLessonWindow(int lessonId) : base(lessonId, true)
        {
            Title = "Информация о занятии";
            WindowTitle.Text = Title;

            SetControlsReadOnly();

            ModifyButtons();
        }

        private void SetControlsReadOnly()
        {
            hallComboBox.IsEnabled = false;
            datePicker.IsEnabled = false;
            startTimeComboBox.IsEnabled = false;
            endTimeComboBox.IsEnabled = false;
            trainerComboBox.IsEnabled = false;
            lessonTypeComboBox.IsEnabled = false;
            clientComboBox.IsEnabled = false;

            isIndividualCheckBox.Visibility = Visibility.Collapsed;
            cbText.Visibility = Visibility.Collapsed;
        }

        private void ModifyButtons()
        {
            var stackPanel = this.FindName("ButtonsStackPanel") as StackPanel;
            if (stackPanel != null)
            {
                stackPanel.Children.Clear();

                // Добавляем только кнопку "Закрыть"
                var closeButton = new Button
                {
                    Content = "Закрыть",
                    Width = 120,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                closeButton.Click += (s, e) => this.Close();

                stackPanel.Children.Add(closeButton);
            }
        }
    }
}