using ReminderApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReminderApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReminderPage : ContentPage
    {
        INotificationManager notificationManager;
        int notificationNumber = 0;
        public ReminderPage()
        {
            InitializeComponent();
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
            LoadAllReminder();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Retrieve all the notes from the database, and set them as the
            // data source for the CollectionView.
            collectionView.ItemsSource = await App.Database.GetNotesAsync();
        }

        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                // Navigate to the NoteEntryPage, passing the ID as a query parameter.
                Reminder note = (Reminder)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"{nameof(ReminderEntryPage)}?{nameof(ReminderEntryPage.ItemId)}={note.ID.ToString()}");
            }
        }
        async void OnAddClicked(object sender, EventArgs e)
        {
            // Navigate to the NoteEntryPage, without passing any data.
            await Shell.Current.GoToAsync(nameof(ReminderEntryPage));
        }

        async void OnDeleteAllClick(object sender, EventArgs e)
        {
            try
            {
                List<Reminder> reminderlist = await App.Database.GetNotesAsync();
                for (var i = 0; i <= reminderlist.Count; i++)
                {
                    var item = new Reminder();

                    item.ID = reminderlist[i].ID;
                    item.Text = reminderlist[i].Text;
                    item.ExpiryDate = reminderlist[i].ExpiryDate;
                    item.emailId = reminderlist[i].emailId;
                    item.IsEmail = reminderlist[i].IsEmail;
                    item.IsReminderNotification = reminderlist[i].IsReminderNotification;
                    item.IsSMS = reminderlist[i].IsSMS;
                    item.Date = reminderlist[i].Date;
                    item.phonenumber = reminderlist[i].phonenumber;
                    item.selection = reminderlist[i].selection;
                    await App.Database.DeleteNoteAsync(item);
                }
                await Shell.Current.GoToAsync(nameof(ReminderEntryPage));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

        }

        async void LoadAllReminder()
        {
            try
            {
                // Retrieve the note and set it as the BindingContext of the page.
                List<Reminder> reminderlist = await App.Database.GetNotesAsync();
                List<string> recepientList = new List<string>();
                if (reminderlist.Count() > 0)
                {
                    foreach (var item in reminderlist.Where(x => x != null))
                    {
                        recepientList.Add(item.emailId);
                        if (DateTime.Now.Date < item.ExpiryDate)
                        {
                            if (item.IsEmail)
                            {
                                try
                                {
                                    var message = new EmailMessage
                                    {
                                        Subject = "Expiry Alert",
                                        Body = "Your Product" + item.Text + "is about to get Expired. The expiry date is " + item.ExpiryDate.ToShortDateString() + "Utlize your Product before it is too late.",
                                        To = recepientList
                                    };
                                    await Email.ComposeAsync(message);

                                }
                                catch (FeatureNotSupportedException fbsEx)
                                {
                                    Console.WriteLine("{0} Exception caught.", fbsEx);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("{0} Exception caught.", ex);
                                }

                            }
                            else if (item.IsSMS)
                            {
                                try
                                {
                                    var messageText = "Your Product is about to get Expired. The expiry date is " + item.ExpiryDate.ToShortDateString() + "Utlize your Product before it is too late.";
                                    var message = new SmsMessage(messageText, new[] { Convert.ToString(item.phonenumber) });
                                    await Sms.ComposeAsync(message);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("{0} Exception caught.", ex);
                                }
                            }
                            else if (item.IsReminderNotification)
                            {
                                try
                                {
                                    NotifyUser(item);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("{0} Exception caught.", ex);
                                }

                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load note.");
            }
        }
        private void NotifyUser(Reminder note)
        {
            try
            {
                var sendDate = note.ExpiryDate.Date.AddDays(-1);
                notificationNumber++;
                string title = $"Be Alert #{notificationNumber}";
                string message = $"Your Product {note.Text} is Expiring on {note.ExpiryDate}!";
                notificationManager.SendNotification(title, message, sendDate);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }
        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
                //stackLayout.Children.Add(msg);
            });
        }
    }
}
