using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReminderApp.Model
{
    public class Reminder
    {
        public string Filename { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
