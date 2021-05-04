using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ReminderApp.Model
{
    public class Reminder
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string selection { get; set; }
        public bool IsSMS { get; set; }

        public bool IsEmail { get; set; }

        public string emailId { get; set; }
        public string phonenumber { get; set; }
        public bool IsReminderNotification { get; set; }


    }
}
