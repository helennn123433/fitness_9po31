using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using fitness.DBAccess;
using fitness.Models;
using fitness.Windows;
using fitness.Windows.ActionWindows;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace fitness.Pages.DataPages
{
    public partial class SchedulePage : Page
    {
        private readonly string[] days = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };
        private readonly TimeSpan startTime = TimeSpan.FromHours(6);
        private readonly TimeSpan endTime = TimeSpan.FromHours(22);
        private readonly TimeSpan interval = TimeSpan.FromMinutes(30);
        private const double rowHeight = 50;
        private int? selectedLessonId = null;
        private DateTime currentWeekStart;
        private List<LessonScheduleView> lessons;

        private readonly List<Brush> lessonBrushes = new List<Brush>
        {
            Brushes.DarkSlateGray,
            Brushes.SteelBlue,
            Brushes.Teal,
            Brushes.Orange,
            Brushes.DarkSeaGreen,
            Brushes.MediumPurple,
            Brushes.CadetBlue,
            Brushes.DarkGoldenrod,
            Brushes.IndianRed,
            Brushes.DarkOliveGreen
        };

        public SchedulePage()
        {
            InitializeComponent();
            currentWeekStart = GetStartOfWeek(DateTime.Today);
            UpdateWeekRangeText();
            Loaded += SchedulePage_Loaded;
            SizeChanged += SchedulePage_SizeChanged;
            var userId = LoginWindow.currentUser.UserId;
            if (!UserRepository.IsManager(userId) && !UserRepository.IsTrainer(userId))
                ActionBar.Visibility = Visibility.Collapsed;

            LoadLessonTypes();
            LoadTrainers();

        }

        private void LoadTrainers()
        {
            trainersComboBox.ItemsSource = TrainerRepository.GetAllTrainer();
        }

        private void LoadLessonTypes()
        {
            lessonTypesComboBox.ItemsSource = LessonRepository.GetAllLessonsType();
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return date.AddDays(-6);
            return date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
        }

        private void UpdateWeekRangeText()
        {
            DateTime endOfWeek = currentWeekStart.AddDays(6);
            WeekRangeText.Text = $"{currentWeekStart:dd.MM.yyyy} - {endOfWeek:dd.MM.yyyy}";
        }

        private void SchedulePage_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTimeColumn();
            DrawScheduleGrid();
            AddLessonsFromDatabase();
        }

        private void SchedulePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawScheduleGrid();
            AddLessonsFromDatabase();
        }

        private void InitializeTimeColumn()
        {
            var timeList = new List<string>();
            for (var time = startTime; time < endTime; time += interval)
            {
                timeList.Add(time.ToString(@"hh\:mm"));
            }
            TimeColumn.ItemsSource = timeList;
        }

        private void DrawScheduleGrid()
        {
            ScheduleCanvas.Children.Clear();

            double columnWidth = (ScheduleGrid.ActualWidth - 80) / 7;
            int rowCount = (int)((endTime - startTime).TotalMinutes / interval.TotalMinutes);

            for (int i = 0; i <= 7; i++)
            {
                var line = new Line
                {
                    X1 = i * columnWidth,
                    X2 = i * columnWidth,
                    Y1 = 0,
                    Y2 = rowCount * rowHeight,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 0.5
                };
                ScheduleCanvas.Children.Add(line);
            }

            for (int i = 0; i <= rowCount; i++)
            {
                var line = new Line
                {
                    X1 = 0,
                    X2 = 7 * columnWidth,
                    Y1 = i * rowHeight,
                    Y2 = i * rowHeight,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 0.5
                };
                ScheduleCanvas.Children.Add(line);
            }

            ScheduleCanvas.Width = 7 * columnWidth;
            ScheduleCanvas.Height = rowCount * rowHeight;
        }

        private string GetRussianDayOfWeek(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Понедельник",
                DayOfWeek.Tuesday => "Вторник",
                DayOfWeek.Wednesday => "Среда",
                DayOfWeek.Thursday => "Четверг",
                DayOfWeek.Friday => "Пятница",
                DayOfWeek.Saturday => "Суббота",
                DayOfWeek.Sunday => "Воскресенье",
                _ => "Понедельник"
            };
        }

        private Brush GetContrastTextColor(Brush background)
        {
            if (background is SolidColorBrush solidColorBrush)
            {
                double brightness = (solidColorBrush.Color.R * 0.299 +
                                   solidColorBrush.Color.G * 0.587 +
                                   solidColorBrush.Color.B * 0.114) / 255;

                return brightness > 0.5 ? Brushes.Black : Brushes.White;
            }
            return Brushes.White;
        }

        private void AddLesson(string title, string trainer, string start, string end, string day,
                     Brush background, Brush foreground, int? lessonId = null)
        {
            TimeSpan startTimeSpan = TimeSpan.Parse(start);
            TimeSpan endTimeSpan = TimeSpan.Parse(end);

            double startRow = (startTimeSpan - startTime).TotalMinutes / interval.TotalMinutes;
            double endRow = (endTimeSpan - startTime).TotalMinutes / interval.TotalMinutes;

            int columnIndex = Array.IndexOf(days, day);
            if (columnIndex == -1 || startRow < 0 || endRow <= startRow)
                return;

            double columnWidth = (ScheduleGrid.ActualWidth - 80) / 7;
            double width = columnWidth - 2;
            double height = (endRow - startRow) * rowHeight - 2;

            var originalBrush = background;
            var hoverBrush = background is SolidColorBrush scb
                ? new SolidColorBrush(DarkenColor(scb.Color, 0.1f))
                : Brushes.Gray;
            var selectedBrush = background is SolidColorBrush scb2
                ? new SolidColorBrush(DarkenColor(scb2.Color, 0.2f))
                : Brushes.DarkGray;

            Rectangle lessonBlock = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = originalBrush,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                RadiusX = 5,
                RadiusY = 5,
                Tag = lessonId
            };

            Canvas.SetLeft(lessonBlock, columnIndex * columnWidth + 1);
            Canvas.SetTop(lessonBlock, startRow * rowHeight + 1);

            lessonBlock.MouseEnter += (sender, e) =>
            {
                if ((int?)lessonBlock.Tag != selectedLessonId)
                    lessonBlock.Fill = hoverBrush;
            };

            lessonBlock.MouseLeave += (sender, e) =>
            {
                if ((int?)lessonBlock.Tag != selectedLessonId)
                    lessonBlock.Fill = originalBrush;
            };

            lessonBlock.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount == 1)
                {
                    foreach (var child in ScheduleCanvas.Children)
                    {
                        if (child is Rectangle rect && rect.Tag is int id && id == selectedLessonId)
                        {
                            var brush = lessonBrushes[id % lessonBrushes.Count];
                            rect.Fill = brush;
                        }
                    }

                    lessonBlock.Fill = selectedBrush;
                    selectedLessonId = (int?)lessonBlock.Tag;
                }
            };

            lessonBlock.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount == 2)
                {
                    if (lessonBlock.Tag is int id)
                        ShowLessonInfo(id);
                }
            };

            ScheduleCanvas.Children.Add(lessonBlock);

            TextBlock lessonText = new TextBlock
            {
                Text = $"{title}\n{trainer}",
                Foreground = foreground,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Width = width - 10,
                TextAlignment = TextAlignment.Center
            };

            lessonText.MouseEnter += (sender, e) =>
            {
                if ((int?)lessonBlock.Tag != selectedLessonId)
                    lessonBlock.Fill = hoverBrush;
            };

            lessonText.MouseLeave += (sender, e) =>
            {
                if ((int?)lessonBlock.Tag != selectedLessonId)
                    lessonBlock.Fill = originalBrush;
            };

            lessonText.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount == 1)
                {
                    foreach (var child in ScheduleCanvas.Children)
                    {
                        if (child is Rectangle rect && rect.Tag is int id && id == selectedLessonId)
                        {
                            var brush = lessonBrushes[id % lessonBrushes.Count];
                            rect.Fill = brush;
                        }
                    }

                    lessonBlock.Fill = selectedBrush;
                    selectedLessonId = (int?)lessonBlock.Tag;
                }
                else if (e.ClickCount == 2)
                {
                    if (lessonBlock.Tag is int id)
                        ShowLessonInfo(id);
                }
            };

            Canvas.SetLeft(lessonText, columnIndex * columnWidth + 5);
            Canvas.SetTop(lessonText, startRow * rowHeight + height / 2 - 15);
            ScheduleCanvas.Children.Add(lessonText);
        }

        private Color DarkenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                (byte)(color.R * (1 - factor)),
                (byte)(color.G * (1 - factor)),
                (byte)(color.B * (1 - factor)));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            LessonWindow lessonWindow = new LessonWindow();
            lessonWindow.ShowDialog();
            if (lessonWindow.DialogResult == true)
            {
                AddLessonsFromDatabase();
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Тренировка успешно добавлена");
                messageBoxWindow.ShowDialog();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLessonId == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите тренировку для редактирования");
                messageBoxWindow.ShowDialog();
                return;
            }

            LessonWindow lessonWindow = new LessonWindow(selectedLessonId, true);
            lessonWindow.ShowDialog();
            if (lessonWindow.DialogResult == true)
            {
                AddLessonsFromDatabase();
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Тренировка успешно обновлена");
                messageBoxWindow.ShowDialog();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLessonId == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите тренировку для редактирования");
                messageBoxWindow.ShowDialog();
                return;
            }

            MessageBoxWindow confirmWindow = new MessageBoxWindow("Вы уверены что хотите удалить занятие?", true);
            confirmWindow.ShowDialog();
            if (confirmWindow.DialogResult == true)
            {
                if (LessonRepository.DeleteLesson((int)selectedLessonId))
                {
                    AddLessonsFromDatabase();
                    MessageBoxWindow messageBoxWindow2 = new MessageBoxWindow("Тренировка успешно удалена");
                    messageBoxWindow2.ShowDialog();
                }
            }
        }

        private void PrevWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(-7);
            UpdateWeekRangeText();
            AddLessonsFromDatabase();
        }

        private void CurrentWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = GetStartOfWeek(DateTime.Today);
            UpdateWeekRangeText();
            AddLessonsFromDatabase();
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(7);
            UpdateWeekRangeText();
            AddLessonsFromDatabase();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            lessonTypesComboBox.SelectedItem = null;
            trainersComboBox.SelectedItem = null;
        }

        private void lessonTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddLessonsFromDatabase();
        }

        private void trainersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddLessonsFromDatabase();
        }

        private void AddLessonsFromDatabase()
        {
            ScheduleCanvas.Children.Clear();
            DrawScheduleGrid();

            DateTime endOfWeek = currentWeekStart.AddDays(6);

            var selectedLessonType = lessonTypesComboBox.SelectedItem as LessonType;
            var selectedTrainer = trainersComboBox.SelectedItem as TrainerDetail;

            var type = selectedLessonType != null ? selectedLessonType?.TypeLessonName : null;
            var name = selectedTrainer != null ? $"{selectedTrainer?.LastName} {selectedTrainer?.FirstName}" : null;

            lessons = LessonRepository.GetLessonsForWeek(currentWeekStart, endOfWeek, type, name);
            foreach (var lesson in lessons)
            {
                Brush background = lessonBrushes[(int)lesson.IdLesson % lessonBrushes.Count];
                Brush foreground = GetContrastTextColor(background);

                string dayOfWeek = GetRussianDayOfWeek(lesson.LessonDate.Value.DayOfWeek);
                string startTime = lesson.LessonStart.Value.ToString("HH:mm");
                string endTime = lesson.LessonEnd.Value.ToString("HH:mm");

                AddLesson(
                    lesson.LessonTypeName ?? "Занятие",
                    lesson.TrainerFullName ?? "Тренер",
                    startTime,
                    endTime,
                    dayOfWeek,
                    background,
                    foreground,
                    lesson.IdLesson
                );
            }
        }

        private void ExportScheduleToExcel()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx",
                FileName = "Расписание.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExcelPackage.License.SetNonCommercialPersonal("My Name");

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Расписание");

                        using (var headerRange = worksheet.Cells[1, 1, 1, 6])
                        {
                            headerRange.Style.Font.Bold = true;
                            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
                            headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }

                        worksheet.Cells[1, 1].Value = "Название занятия";
                        worksheet.Cells[1, 2].Value = "Тренер";
                        worksheet.Cells[1, 3].Value = "День недели";
                        worksheet.Cells[1, 4].Value = "Время начала";
                        worksheet.Cells[1, 5].Value = "Время окончания";
                        worksheet.Cells[1, 6].Value = "Дата";

                        for (int i = 0; i < lessons.Count; i++)
                        {
                            int row = i + 2;

                            worksheet.Cells[row, 1].Value = lessons[i].LessonTypeName ?? "Не указано";
                            worksheet.Cells[row, 2].Value = lessons[i].TrainerFullName ?? "Не указан";

                            if (lessons[i].LessonDate.HasValue)
                            {
                                worksheet.Cells[row, 3].Value = GetRussianDayOfWeek(lessons[i].LessonDate.Value.DayOfWeek);
                                worksheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            if (lessons[i].LessonStart.HasValue)
                            {
                                worksheet.Cells[row, 4].Value = lessons[i].LessonStart.Value.ToString(@"hh\:mm");
                                worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            if (lessons[i].LessonEnd.HasValue)
                            {
                                worksheet.Cells[row, 5].Value = lessons[i].LessonEnd.Value.ToString(@"hh\:mm");
                                worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            if (lessons[i].LessonDate.HasValue)
                            {
                                worksheet.Cells[row, 6].Value = lessons[i].LessonDate.Value.ToString("dd.MM.yyyy");
                                worksheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[row, 6].Style.Numberformat.Format = "dd.mm.yyyy";
                            }
                        }

                        using (var dataRange = worksheet.Cells[1, 1, lessons.Count + 1, 6])
                        {
                            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }

                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        package.SaveAs(new FileInfo(saveFileDialog.FileName));

                        MessageBox.Show("Расписание успешно сохранено в Excel!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            ExportScheduleToExcel();
        }

        private void ShowLessonInfo(int lessonId)
        {
            ReadOnlyLessonWindow lessonWindow = new ReadOnlyLessonWindow(lessonId);
            lessonWindow.ShowDialog();
        }
    }
}