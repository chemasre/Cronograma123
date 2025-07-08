using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para CalendarEditor.xaml
    /// </summary>
    public partial class CalendarEditor : Window, EntityEditor<Calendar>
    {
        Calendar calendar;

        public CalendarEditor()
        {
            InitializeComponent();

        }

        public Calendar GetEntity()
        {
            return calendar;
        }

        public void SetEntity(Calendar entity)
        {
            calendar = entity;

            TextTitle.Text = entity.Title;

            DateStart.SelectedDate = entity.StartDay;
            DateEnd.SelectedDate = entity.EndDay;

            ListBoxFreeDays.Items.Clear();
            List<DateTime> sortedList = entity.FreeDays.ToList();
            sortedList.Sort();
            sortedList.ForEach(e => ListBoxFreeDays.Items.Add(e.ToString()));

            DateFreeDay.SelectedDate = entity.StartDay;

            TextTitle.TextChanged += TextTitle_TextChanged;
            DateStart.SelectedDateChanged += DateStart_SelectedDateChanged;
            DateEnd.SelectedDateChanged += DateEnd_SelectedDateChanged;
            DateFreeDay.SelectedDateChanged += DateFreeDay_SelectedDateChanged;
            ListBoxFreeDays.SelectionChanged += ListBoxFreeDays_SelectionChanged;

            Validate();

        }

        private void ListBoxFreeDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListBoxFreeDays.SelectedIndex >= 0)
            {
                DateFreeDay.SelectedDate = DateTime.Parse(ListBoxFreeDays.SelectedItem.ToString());
            }
        }

        private void UpdateEntity()
        {
            calendar.Title = TextTitle.Text.Trim();
            calendar.StartDay = DateStart.SelectedDate.GetValueOrDefault();
            calendar.EndDay = DateEnd.SelectedDate.GetValueOrDefault();

            calendar.FreeDays.Clear();
            foreach(var item in ListBoxFreeDays.Items) { calendar.FreeDays.Add(DateTime.Parse(item.ToString())); }

        }

        private void Validate()
        {
            Entity.ValidationResult result = calendar.Validate();

            if (result == Entity.ValidationResult.success)
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorValid"]);
                TextValidation.Text = "El calendario es correcto";
            }
            else
            {
                BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorInvalid"]);

                if (result == Entity.ValidationResult.titleEmpty)
                {                    
                    TextValidation.Text = "Tienes que escribir un título para el calendario";
                }
                else if(result == Entity.ValidationResult.freeDayOutsideCalendar)
                {
                    TextValidation.Text = "Todos los días festivos deben situarse entre el primer día del curso y el último (incluidos)";
                }
                else // result == Entity.ValidationResult.startDayAfterEndDay
                {
                    TextValidation.Text = "Tienes que situar el primer día del curso antes o coincidiendo con el último";
                }
            }

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            calendar.Save();

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
            UpdateEntity();
            Validate();
        }

        private void DateStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void DateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void ButtonFreeDayAdd_Click(object sender, RoutedEventArgs e)
        {
            if(DateFreeDay.SelectedDate != null)
            {
                SortedSet<DateTime> set = new();
                set.Add(DateFreeDay.SelectedDate.Value.Date);
                foreach(object? item in ListBoxFreeDays.Items) { set.Add(DateTime.Parse(item.ToString())); }
                ListBoxFreeDays.Items.Clear();
                foreach(DateTime date in set) { ListBoxFreeDays.Items.Add(date.ToShortDateString()); }

                DateFreeDay.SelectedDate = DateFreeDay.SelectedDate.Value.AddDays(1);

                UpdateEntity();
                Validate();
            }
        }

        private void ButtonFreeDayRemove_Click(object sender, RoutedEventArgs e)
        {
            UpdateEntity();
            Validate();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
