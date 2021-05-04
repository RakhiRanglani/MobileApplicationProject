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
    public partial class ReminderEntryPage : ContentPage
    {
        INotificationManager notificationManager;
        int notificationNumber = 0;
        public string ItemId
        {
            set
            {
                LoadNote(value);
            }
        }

        public ReminderEntryPage()
        {
            InitializeComponent();

            // Set the BindingContext of the page to a new Note.
            BindingContext = new Reminder()
            {
                ExpiryDate = DateTime.Now,
            };

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
        }

        async void LoadNote(string itemId)
        {
            try
            {
                int id = Convert.ToInt32(itemId);

                // Retrieve the note and set it as the BindingContext of the page.
                Reminder note = await App.Database.GetNoteAsync(id);
                BindingContext = note;

                // After loading all the notes form the SQLlite activate Email and SMs Functionality.
                LoadAllReminder();
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load note.");
            }
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var note = (Reminder)BindingContext;
                note.Date = DateTime.UtcNow;
                if (note.IsSMS)
                {
                    note.selection = "Notify via SMS";
                }
                else if (note.IsEmail)
                {
                    note.selection = "Notify via Email";
                }
                else
                {
                    note.selection = "Notify via Reminder";
                }

                if (!string.IsNullOrWhiteSpace(note.Text))
                {
                    await App.Database.SaveNoteAsync(note);
                }
                // schedule Notification 
                if (note.IsReminderNotification)
                {
                    NotifyUser(note);
                }
                // Navigate backwards
                await Shell.Current.GoToAsync("..");
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
                foreach (var item in reminderlist)
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
                    }

                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load note.");
            }
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var note = (Reminder)BindingContext;
            await App.Database.DeleteNoteAsync(note);

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
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
