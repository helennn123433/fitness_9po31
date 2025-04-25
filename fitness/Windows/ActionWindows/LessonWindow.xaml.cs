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
    /// Логика взаимодействия для LessonWindow.xaml
    /// </summary>
    public partial class LessonWindow : Window
    {
        private Lesson CurrentLesson { get; set; }
        private bool IsEditMode { get; }
        public LessonWindow(int? lessonId = null, bool isEditMode = false)
        {
            InitializeComponent();
            CurrentLesson = lessonId != null
                            ? LessonRepository.GetLessonById(lessonId)
                            : new Lesson();
            IsEditMode = isEditMode;

            Title = IsEditMode ? "Редактирование существующего занятия" : "Добавление нового занятия";
            WindowTitle.Text = Title;

            LoadHalls();
            LoadLessonTypes();

            if (CurrentLesson.IdLesson != null && CurrentLesson.IdLesson != 0)
            {
                hallComboBox.SelectedItem = hallComboBox.Items
                    .Cast<HallDetail>()
                    .FirstOrDefault(hd => hd.IdHall == CurrentLesson.IdHall);
                hallComboBox_SelectionChanged(null, null);

                datePicker.SelectedDate = CurrentLesson.LessonDate.ToDateTime(TimeOnly.MinValue);
                datePicker_SelectedDateChanged(null, null);

                UpdateAvailableTimes();
                startTimeComboBox.SelectedItem = startTimeComboBox.Items
                    .Cast<string>()
                    .FirstOrDefault(st => st == CurrentLesson.StartTime?.ToString(@"HH\:mm"));
                endTimeComboBox.SelectedItem = endTimeComboBox.Items
                    .Cast<string>()
                    .FirstOrDefault(et => et == CurrentLesson.EndTime?.ToString(@"HH\:mm"));
                endTimeComboBox_SelectionChanged(null, null);

                trainerComboBox.SelectedItem = trainerComboBox.Items
                    .Cast<TrainerDetail>()
                    .FirstOrDefault(td => td.IdTrainer == CurrentLesson.IdTrainer);

                lessonTypeComboBox.SelectedItem = lessonTypeComboBox.Items
                    .Cast<LessonType>()
                    .FirstOrDefault(lt => lt.IdTypeLesson == CurrentLesson.IdTypeLesson);

                if (CurrentLesson.IdAbonementClient != null)
                {
                    isIndividualCheckBox.IsChecked = true;
                    var args = new RoutedEventArgs(CheckBox.ClickEvent);
                    isIndividualCheckBox.RaiseEvent(args);

                    clientComboBox.SelectedItem = clientComboBox.Items
                        .Cast<ClientInfoView>()
                        .FirstOrDefault(civ => civ.IdAbonementClient == CurrentLesson.IdAbonementClient);
                }
            }

        }

        private void LoadClients()
        {
            throw new NotImplementedException();
        }

        private void LoadLessonTypes()
        {
            lessonTypeComboBox.ItemsSource = LessonRepository.GetAllLessonsType();
        }

        private void LoadHalls()
        {
            hallComboBox.ItemsSource = HallRepository.GetActiveHalls();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (hallComboBox.SelectedItem == null) {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите зал для занятия");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (datePicker.SelectedDate == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите дату занятия");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (startTimeComboBox.SelectedItem == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите время начала занятия");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (endTimeComboBox.SelectedItem == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите время конца занятия");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (trainerComboBox.SelectedItem == null)
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите тренера для занятия");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (lessonTypeComboBox.SelectedItem == null) 
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите тип занятия");
                messageBoxWindow.ShowDialog();
                return;
            }

            if (isIndividualCheckBox.IsChecked == true && clientComboBox.SelectedItem == null) 
            {
                MessageBoxWindow messageBoxWindow = new MessageBoxWindow("Выберите клиента для индивидуального занятия");
                messageBoxWindow.ShowDialog();
                return;
            }

            CurrentLesson.IdHall = (int)(hallComboBox.SelectedItem as HallDetail).IdHall;
            CurrentLesson.LessonDate = DateOnly.FromDateTime((DateTime)datePicker.SelectedDate);
            CurrentLesson.IdTrainer = (int)(trainerComboBox.SelectedItem as TrainerDetail).IdTrainer;
            CurrentLesson.IdTypeLesson = (lessonTypeComboBox.SelectedItem as LessonType).IdTypeLesson;
            CurrentLesson.StartTime = TimeOnly.Parse(startTimeComboBox.SelectedItem.ToString());
            CurrentLesson.EndTime = TimeOnly.Parse(endTimeComboBox.SelectedItem.ToString());
            if (isIndividualCheckBox.IsChecked == true)
            {
                CurrentLesson.IdAbonementClient = (clientComboBox.SelectedItem as ClientInfoView).IdAbonementClient;
            }

            bool success = false;
            if (!IsEditMode)
                success = LessonRepository.AddLesson(CurrentLesson);
            else
                success = LessonRepository.UpdateLesson(CurrentLesson);
            if (success) {
                DialogResult = true;
                Close();
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAvailableTimes();
        }

        private void hallComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAvailableTimes();
        }

        private void UpdateAvailableTimes()
        {
            if (datePicker.SelectedDate == null || hallComboBox.SelectedItem == null)
            {
                startTimeComboBox.ItemsSource = null;
                endTimeComboBox.ItemsSource = null;
                return;
            }

            startTimeComboBox.SelectedItem = null;
            endTimeComboBox.SelectedItem = null;

            DateTime selectedDate = datePicker.SelectedDate.Value;
            var selectedHall = hallComboBox.SelectedItem as HallDetail;
            
            var lessons = LessonRepository.GetLessonsByHallAndDate((int)selectedHall.IdHall, selectedDate)
                            .OrderBy(l => l.StartTime)
                            .ToList();

            var startTimeSlots = GenerateStartTimeSlots();
            var endTimeSlots = GenerateEndTimeSlots();

            var availableStartTimes = FilterAvailableStartTimes(startTimeSlots, lessons);
            startTimeComboBox.ItemsSource = availableStartTimes.Select(t => t.ToString(@"hh\:mm"));

            startTimeComboBox.SelectionChanged += (sender, e) =>
            {
                if (startTimeComboBox.SelectedItem == null) return;

                TimeSpan selectedStartTime = TimeSpan.Parse(startTimeComboBox.SelectedItem.ToString());
                UpdateEndTimeComboBox(selectedStartTime, lessons, endTimeSlots);
            };
        }

        private List<TimeSpan> GenerateStartTimeSlots()
        {
            var timeSlots = new List<TimeSpan>();
            TimeSpan startTime = new TimeSpan(6, 0, 0);  
            TimeSpan endTime = new TimeSpan(21, 30, 0);  
            TimeSpan interval = new TimeSpan(0, 30, 0);  

            while (startTime <= endTime)
            {
                timeSlots.Add(startTime);
                startTime = startTime.Add(interval);
            }
            return timeSlots;
        }

        private List<TimeSpan> GenerateEndTimeSlots()
        {
            var timeSlots = new List<TimeSpan>();
            TimeSpan startTime = new TimeSpan(6, 30, 0);
            TimeSpan endTime = new TimeSpan(22, 0, 0);  
            TimeSpan interval = new TimeSpan(0, 30, 0); 

            while (startTime <= endTime)
            {
                timeSlots.Add(startTime);
                startTime = startTime.Add(interval);
            }
            return timeSlots;
        }

        private List<TimeSpan> FilterAvailableStartTimes(List<TimeSpan> startSlots, List<Lesson> lessons)
        {
            var availableSlots = new List<TimeSpan>(startSlots);

            foreach (var lesson in lessons)
            {
                if (!IsEditMode || CurrentLesson.IdLesson != lesson.IdLesson)
                {
                    TimeSpan lessonStart = lesson.StartTime.Value.ToTimeSpan();
                    TimeSpan lessonEnd = lesson.EndTime.Value.ToTimeSpan();

                    availableSlots.RemoveAll(slot =>
                        slot >= lessonStart && slot < lessonEnd);
                }
            }

            return availableSlots;
        }
        private void UpdateEndTimeComboBox(TimeSpan selectedStartTime, List<Lesson> lessons, List<TimeSpan> endTimeSlots)
        {
            var nextLesson = lessons
                .FirstOrDefault(l => l.StartTime.Value.ToTimeSpan() > selectedStartTime);

            TimeSpan maxEndTime = nextLesson != null
                ? nextLesson.StartTime.Value.ToTimeSpan()
                : new TimeSpan(22, 0, 0);

            var availableEndTimes = endTimeSlots
                .Where(t => t > selectedStartTime && t <= maxEndTime)
                .Select(t => t.ToString(@"hh\:mm"))
                .ToList();

            endTimeComboBox.ItemsSource = availableEndTimes;

            if (availableEndTimes.Any())
            {
                TimeSpan defaultEndTime = selectedStartTime.Add(new TimeSpan(1, 30, 0));
                string closestEndTime = availableEndTimes
                    .FirstOrDefault(t => TimeSpan.Parse(t) >= defaultEndTime) ?? availableEndTimes.Last();

                endTimeComboBox.SelectedItem = closestEndTime;
            }
            else
                endTimeComboBox.SelectedItem = null;
        }

        private void endTimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTrainerComboBox();

            if (isIndividualCheckBox.IsChecked == true)
                UpdateClientsComboBox();
        }

        private void UpdateTrainerComboBox()
        {
            if (datePicker.SelectedDate == null ||
                startTimeComboBox.SelectedItem == null ||
                endTimeComboBox.SelectedItem == null)
            {
                trainerComboBox.ItemsSource = null;
                return;
            }

            DateTime selectedDate = datePicker.SelectedDate.Value;
            TimeSpan startTime = TimeSpan.Parse(startTimeComboBox.SelectedItem.ToString());
            TimeSpan endTime = TimeSpan.Parse(endTimeComboBox.SelectedItem.ToString());

            LoadAvailableTrainers(selectedDate, startTime, endTime);
        }

        private void LoadAvailableTrainers(DateTime selectedDate, TimeSpan startTime, TimeSpan endTime)
        {
            var availableTrainers = TrainerRepository.GetAvailableTrainers(
                selectedDate,
                TimeOnly.FromTimeSpan(startTime),
                TimeOnly.FromTimeSpan(endTime),
                CurrentLesson.IdTrainer
            );
            trainerComboBox.ItemsSource = availableTrainers;
        }

        private void UpdateClientsComboBox()
        {
            if (datePicker.SelectedDate == null ||
                startTimeComboBox.SelectedItem == null ||
                endTimeComboBox.SelectedItem == null)
            {
                clientComboBox.ItemsSource = null;
                return;
            }

            DateTime selectedDate = datePicker.SelectedDate.Value;
            TimeSpan startTime = TimeSpan.Parse(startTimeComboBox.SelectedItem.ToString());
            TimeSpan endTime = TimeSpan.Parse(endTimeComboBox.SelectedItem.ToString());

            LoadAvailableClient(selectedDate, startTime, endTime);
        }

        private void LoadAvailableClient(DateTime selectedDate, TimeSpan startTime, TimeSpan endTime)
        {
            var availableClients = ClientRepository.GetAvailableClients(
                selectedDate,
                TimeOnly.FromTimeSpan(startTime),
                TimeOnly.FromTimeSpan(endTime),
                CurrentLesson.IdAbonementClient
            );
            clientComboBox.ItemsSource = availableClients;
        }

        private void isIndividualCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = isIndividualCheckBox.IsChecked == true;
            clientComboBox.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
            clientLabel.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;

            if (isChecked)
            {
                UpdateClientsComboBox();
            }
        }
    }
}
