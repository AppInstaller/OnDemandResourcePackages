using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using CoffeeUniversal.Helpers;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;

namespace CoffeeUniversal.Pages
{
    public sealed partial class CalendarPage : Page
    {

        #region Init

        private AppointmentStore appointmentStore;
        private IReadOnlyList<AppointmentCalendar> calendars;
        private int newAppointmentCount = 1;
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public CalendarPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("CalendarPage_OnNavigatedFrom");
            navigationHelper.OnNavigatedFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            try
            {
                appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);
                if (appointmentStore != null)
                {
                    status.Log(LocalizableStrings.CALENDAR_APPOINTMENTSTORE_FIND_SUCCESS);
                    ShowAllCalendars();
                    ShowAllAppointments();
                    Add.IsEnabled = true;
                    Refresh.IsEnabled = true;
                }
                else
                {
                    status.Log(LocalizableStrings.CALENDAR_APPOINTMENTSTORE_FIND_FAIL);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalendarPage.OnNavigatedTo: " +ex.ToString());
                status.Log(ex.GetType().ToString());
            }
        }

        #endregion


        #region Show Calendars/Appointments

        async private void ShowAllCalendars()
        {
            try
            {
                int calendarsCount = 0;
                calendars = await appointmentStore.FindAppointmentCalendarsAsync(FindAppointmentCalendarsOptions.IncludeHidden);
                if (calendars != null)
                {
                    calendarsCount = calendars.Count;
                    if (calendarsCount > 0)
                    {
                        CalendarsListBox.ItemsSource = calendars;
                    }
                }
                status.Log(string.Format(CultureInfo.CurrentCulture,
                    LocalizableStrings.CALENDAR_FIND_CALENDARS_RESULT, calendarsCount));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalendarPage.ShowAllCalendars: " +ex.ToString());
                status.Log(ex.GetType().ToString());
            }
        }

        async private void ShowAllAppointments()
        {
            try
            {
                FindAppointmentsOptions findOptions = new FindAppointmentsOptions();
                findOptions.MaxCount = 20;
                findOptions.FetchProperties.Add(AppointmentProperties.Subject);
                findOptions.FetchProperties.Add(AppointmentProperties.StartTime);
                findOptions.FetchProperties.Add(AppointmentProperties.Location);
                findOptions.FetchProperties.Add(AppointmentProperties.Details);

                int appointmentsCount = 0;
                IReadOnlyList<Appointment> appointments = await appointmentStore.FindAppointmentsAsync(DateTime.Now, TimeSpan.FromDays(5), findOptions);
                if (appointments != null)
                {
                    appointmentsCount = appointments.Count;
                    if (appointmentsCount > 0)
                    {
                        AppointmentListBox.ItemsSource = appointments;
                        AppointmentListBox.SelectionChanged += AppointmentListBox_SelectionChanged;
                    }
                }
                status.Log(string.Format(CultureInfo.CurrentCulture,
                    LocalizableStrings.CALENDAR_FIND_APPOINTMENTS_RESULT, appointmentsCount));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalendarPage.ShowAllAppointments: " +ex.ToString());
                status.Log(ex.GetType().ToString());
            }
        }

        private void AppointmentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppointmentListBox.SelectedItem != null)
            {
                Remove.IsEnabled = true;
                Appointment appt = AppointmentListBox.SelectedItem as Appointment;
                string text = string.IsNullOrEmpty(appt.Details) ? "(none)" : appt.Details.Trim();
                status.Log(string.Format(CultureInfo.CurrentCulture,
                    LocalizableStrings.CALENDAR_APPOINTMENT_REPORT, text));
            }
            else
            {
                Remove.IsEnabled = false;
            }
        }

        #endregion


        #region Add/Remove

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Appointment appointment = new Appointment();
                appointment.Subject = string.Format(CultureInfo.CurrentCulture,
                    LocalizableStrings.CALENDAR_APPOINTMENT_SUBJECT, newAppointmentCount++);
                appointment.StartTime = DateTime.Now.AddHours(1);
                appointment.Duration = TimeSpan.FromHours(1);
                appointment.Location = LocalizableStrings.CALENDAR_APPOINTMENT_LOCATION;
                appointment.Details = LocalizableStrings.CALENDAR_APPOINTMENT_DETAILS;

                string appointmentId = await appointmentStore.ShowAddAppointmentAsync(appointment, new Rect());
                if (!string.IsNullOrEmpty(appointmentId))
                {
                    status.Log(LocalizableStrings.CALENDAR_APPOINTMENT_ADD_SUCCESS);
                    ShowAllAppointments();
                }
                else
                {
                    status.Log(LocalizableStrings.CALENDAR_APPOINTMENT_ADD_FAIL);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CalendarPage.AddButton_Click: " +ex.ToString());
                status.Log(ex.GetType().ToString());
            }
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Appointment appointment = AppointmentListBox.SelectedItem as Appointment;
                bool isSuccess = await appointmentStore.ShowRemoveAppointmentAsync(appointment.LocalId, new Rect());
                if (isSuccess)
                {
                    status.Log(LocalizableStrings.CALENDAR_APPOINTMENT_REMOVE_SUCCESS);
                    ShowAllAppointments();
                }
                else
                {
                    status.Log(LocalizableStrings.CALENDAR_APPOINTMENT_REMOVE_FAIL);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                status.Log(ex.GetType().ToString());
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ShowAllAppointments();
        }

        #endregion
    }
}
