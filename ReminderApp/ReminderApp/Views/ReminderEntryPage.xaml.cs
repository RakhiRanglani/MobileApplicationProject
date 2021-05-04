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
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class ReminderEntryPage : ContentPage
    {
        
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

        }

        async void LoadNote(string itemId)
        {
            try
            {
                Reminder note = new Reminder()
                {
                    emptytext = null,
                };
                if (itemId != null)
                {
                    int id = Convert.ToInt32(itemId);

                    // Retrieve the note and set it as the BindingContext of the page.
                     note = await App.Database.GetNoteAsync(id);
                     BindingContext = note;

                }
                else
                {
                    note.emptytext = "There is no Product added by you.";
                    BindingContext = note;
                }
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
               
                // Navigate backwards
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }
        }

       

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var note = (Reminder)BindingContext;
            await App.Database.DeleteNoteAsync(note);

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }
      
    }
}
