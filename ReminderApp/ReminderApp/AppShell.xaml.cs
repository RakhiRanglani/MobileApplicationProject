
using ReminderApp.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ReminderApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
//Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
           Routing.RegisterRoute(nameof(ReminderEntryPage), typeof(ReminderEntryPage));
        }
    }
}
