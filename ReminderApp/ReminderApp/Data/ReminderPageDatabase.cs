using ReminderApp.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReminderApp.Data
{
    public class ReminderPageDatabase
    {
        readonly SQLiteAsyncConnection database;


        public ReminderPageDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Reminder>().Wait();
        }

        public Task<List<Reminder>> GetNotesAsync()
        {
            //Get all notes.
            return database.Table<Reminder>().ToListAsync();
        }

        public Task<Reminder> GetNoteAsync(int id)
        {
            // Get a specific note.
            return database.Table<Reminder>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

       
        public Task<int> SaveNoteAsync(Reminder note)
        {
            if (note.ID != 0)
            {
                // Update an existing note.
                return database.UpdateAsync(note);
            }
            else
            {
                // Save a new note.
                return database.InsertAsync(note);
            }
        }

        public Task<int> DeleteNoteAsync(Reminder note)
        {
            // Delete a note.
            return database.DeleteAsync(note);
        }
    }
}

