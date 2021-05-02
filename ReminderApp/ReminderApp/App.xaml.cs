using ReminderApp.Data;
using ReminderApp.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReminderApp
{
    public partial class App : Application
    {
        //static ReminderPageDatabase database;

        //// Create the database connection as a singleton.
        //public static ReminderPageDatabase Database
        //{
        //    get
        //    {
        //        if (database == null)
        //        {
        //            database = new ReminderPageDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
        //        }
        //        return database;
        //    }
        //}

        public static string FolderPath { get; internal set; }

        public App()
        {
            InitializeComponent();
            FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
