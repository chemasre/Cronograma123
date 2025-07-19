using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para CalendarEditor.xaml
    /// </summary>
    public partial class CalendarEditor : Window, IEntityEditor<Calendar>
    {
        string? parentStorageId;
        Calendar calendar;

        HashSet<DateTime> freeDaysSet;
        List<DateTime> freeDaysList;

        public CalendarEditor()
        {
            InitializeComponent();

            freeDaysSet = new HashSet<DateTime>();
            freeDaysList = new List<DateTime>();

        }

        public Calendar GetEntity()
        {
            return calendar;
        }

        public void InitEditor(Calendar entity, string? _parentStorageId = null)
        {
            entity.Save(_parentStorageId);

            parentStorageId = _parentStorageId;
            calendar = entity;

            TextTitle.Text = entity.Title;

            DateStart.SelectedDate = entity.StartDay;
            DateEnd.SelectedDate = entity.EndDay;

            freeDaysList = calendar.FreeDays.ToList();
            freeDaysList.Sort();
            freeDaysSet = new HashSet<DateTime>(freeDaysList);

            ListBoxFreeDays.Items.Clear();
            foreach(DateTime d in freeDaysList) { ListBoxFreeDays.Items.Add(Utils.FormatDate(d)); }

            if(freeDaysSet.Count > 0)
            {
                DateFreeDay.SelectedDate = freeDaysSet.First<DateTime>();
                ListBoxFreeDays.SelectedIndex = 0;
            }
            else
            {
                DateFreeDay.SelectedDate = calendar.StartDay;
                ListBoxFreeDays.SelectedIndex = -1;
            }

            CalendarPreview.SelectedDate = null;

            TextTitle.TextChanged += TextTitle_TextChanged;
            DateStart.SelectedDateChanged += DateStart_SelectedDateChanged;
            DateEnd.SelectedDateChanged += DateEnd_SelectedDateChanged;
            DateFreeDay.SelectedDateChanged += DateFreeDay_SelectedDateChanged;
            ListBoxFreeDays.SelectionChanged += ListBoxFreeDays_SelectionChanged;

            CalendarPreview.Loaded += CalendarPreview_Loaded;

            UpdateCalendarPreview();
            Validate();


        }

        private void CalendarPreview_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCalendarPreview();
        }

        private void ListBoxFreeDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListBoxFreeDays.SelectedIndex >= 0)
            {
                DateFreeDay.SelectedDate = freeDaysList[ListBoxFreeDays.SelectedIndex];
            }
        }

        private void UpdateEntity()
        {
            calendar.Title = TextTitle.Text.Trim();
            calendar.StartDay = DateStart.SelectedDate.GetValueOrDefault();
            calendar.EndDay = DateEnd.SelectedDate.GetValueOrDefault();

            calendar.FreeDays.Clear();
            calendar.FreeDays.Add(freeDaysList);

            calendar.Save(parentStorageId);
        }

        private void Validate()
        {
            Entity.ValidationResult result = calendar.Validate();

            if (result == Entity.ValidationResult.success)
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorValid"]);
                TextValidation.Text = "El calendario es válido";
            }
            else
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorInvalid"]);

                if (result == Entity.ValidationResult.titleEmpty)
                {                    
                    TextValidation.Text = "Tienes que escribir un título para el calendario";
                }
                else if (result == Entity.ValidationResult.descriptionEmpty)
                {                    
                    TextValidation.Text = "Tienes que escribir una descripción para el calendario";
                }
                else if(result == Entity.ValidationResult.freeDayBeforeStartOrAfterEnd)
                {
                    TextValidation.Text = "Todos los días festivos deben situarse entre el primer día del curso y el último (incluidos)";
                }
                else if(result == Entity.ValidationResult.startDayAfterEndDay)
                {
                    TextValidation.Text = "Tienes que situar el primer día del curso antes o coincidiendo con el último";
                }
                else // result == Entity.ValidationResult.noSchoolDays
                {
                    TextValidation.Text = "Tiene que existir al menos un día lectivo";
                }
            }

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            //calendar.Save(parentStorageId);

            TextTitle.TextChanged -= TextTitle_TextChanged;
            DateStart.SelectedDateChanged -= DateStart_SelectedDateChanged;
            DateEnd.SelectedDateChanged -= DateEnd_SelectedDateChanged;
            DateFreeDay.SelectedDateChanged -= DateFreeDay_SelectedDateChanged;
            ListBoxFreeDays.SelectionChanged -= ListBoxFreeDays_SelectionChanged;


            Close();
        }

        private void TextTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEntity();
            Validate();

        }

        private void DateFreeDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxFreeDays.SelectedIndex = freeDaysList.FindIndex(e => e == DateFreeDay.SelectedDate);
            if(DateFreeDay.SelectedDate.HasValue) { CalendarPreview.DisplayDate = DateFreeDay.SelectedDate.Value; }
            UpdateEntity();
            Validate();
        }

        private void DateStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DateStart.SelectedDate.HasValue) { CalendarPreview.DisplayDate = DateStart.SelectedDate.Value; }
            UpdateCalendarPreview();
            UpdateEntity();
            Validate();
        }

        private void DateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DateEnd.SelectedDate.HasValue) { CalendarPreview.DisplayDate = DateEnd.SelectedDate.Value; }
            UpdateCalendarPreview();
            UpdateEntity();
            Validate();
        }

        private void ButtonFreeDayAdd_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(DateFreeDay.SelectedDate != null, "La fecha seleccionada no puede estar vacía");

            if(DateFreeDay.SelectedDate != null)
            {
                freeDaysSet.Add(DateFreeDay.SelectedDate.Value);
                freeDaysList = freeDaysSet.ToList<DateTime>();
                freeDaysList.Sort();

                ListBoxFreeDays.Items.Clear();
                foreach(DateTime d in freeDaysList) { ListBoxFreeDays.Items.Add(Utils.FormatDate(d)); }
                
                DateFreeDay.SelectedDate = DateFreeDay.SelectedDate.Value.AddDays(1);

                UpdateCalendarPreview();
                UpdateEntity();
                Validate();

            }
        }

        private void ButtonFreeDayRemove_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(DateFreeDay.SelectedDate != null, "La fecha seleccionada no puede estar vacía");

            if(DateFreeDay.SelectedDate != null)
            {
                if(freeDaysSet.Contains(DateFreeDay.SelectedDate.Value))
                {
                    freeDaysSet.Remove(DateFreeDay.SelectedDate.Value);

                    freeDaysList = freeDaysSet.ToList<DateTime>();
                    freeDaysList.Sort();

                    ListBoxFreeDays.Items.Clear();
                    foreach(DateTime d in freeDaysList) { ListBoxFreeDays.Items.Add(Utils.FormatDate(d)); }

                    DateFreeDay.SelectedDate = DateFreeDay.SelectedDate.Value.AddDays(1);

                    UpdateCalendarPreview();
                    UpdateEntity();
                    Validate();
                }

            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        void UpdateCalendarPreview()
        {
            // https://solutionfall.com/question/how-can-i-change-the-background-color-of-a-calendardaybutton-in-wpf/

            int month; 
            DependencyObject monthView;

            month = CalendarPreview.DisplayDate.Month;
            monthView = FindMonthView(CalendarPreview, month);

            if(monthView == null) { return; }

            int count = VisualTreeHelper.GetChildrenCount(monthView);

            for(int i = 0; i < count; i++)
            {
                DependencyObject childView = VisualTreeHelper.GetChild(monthView, i);

                if(childView is CalendarDayButton calendarDayButton)
                {
                    DateTime date = (DateTime)calendarDayButton.DataContext;

                    if(freeDaysSet.Contains(date.Date))
                    {
                        calendarDayButton.Background =  Brushes.PaleVioletRed;
                    }
                    else if(date.Date.DayOfWeek == DayOfWeek.Sunday || date.Date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        calendarDayButton.Background =  Brushes.LightBlue;
                    }
                    else if(date.Date == DateStart.SelectedDate.GetValueOrDefault())
                    {
                        calendarDayButton.Background = Brushes.LightGreen;
                    }
                    else if(date.Date == DateEnd.SelectedDate.GetValueOrDefault())
                    {
                        calendarDayButton.Background = Brushes.LightGreen;
                    }
                    else
                    {
                        calendarDayButton.Background = Brushes.Transparent;
                    }
                }
            }

        }

        private DependencyObject? FindMonthView(DependencyObject depObj, int targetMonth)
        {
            DependencyObject? result;
            int childrenCount;

            childrenCount = VisualTreeHelper.GetChildrenCount(depObj);

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                if (child is FrameworkElement frameworkElement)
                {
                    if(frameworkElement.Name == "PART_MonthView")
                    {
                        int month = CalendarPreview.DisplayDate.Month;
                        if (month == targetMonth)
                        {
                            return child;
                        }
                    }
                }

                result = FindMonthView(child, targetMonth);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private void CalendarPreview_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            UpdateCalendarPreview();
        }

        private void DateStart_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(DateStart.SelectedDate.HasValue) { CalendarPreview.DisplayDate = DateStart.SelectedDate.Value; }
        }

        private void DateEnd_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(DateEnd.SelectedDate.HasValue) { CalendarPreview.DisplayDate = DateEnd.SelectedDate.Value; }
        }

        private void DateFreeDay_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(DateFreeDay.SelectedDate.HasValue) { CalendarPreview.DisplayDate = DateFreeDay.SelectedDate.Value; }
        }

        public void SetEntityTitleEditable(bool editable)
        {
            TextTitle.IsReadOnly = !editable;
            TextTitle.IsReadOnlyCaretVisible = false;
        }

        public void SetEditorTitle(string title)
        {
            TextEditorTitle.Content = title;
        }
    }
}
