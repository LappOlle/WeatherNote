using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Weather_Note.Models.Initializers;

namespace Weather_Note.Models.Context
{
    public class NoteContext : DbContext
    {
        public NoteContext() : base("name=NoteContext")
        {
            //Set the Initializer to my own custom Initializer. It has a seed method that runs to.
            Database.SetInitializer(new WheatherNoteInitializer());
        }

        public DbSet<POCO.Note> Notes { get; set; }
    }
}
