namespace Weather_Note.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Weather_Note.Models.POCO;

    internal sealed class Configuration : DbMigrationsConfiguration<Weather_Note.Models.Context.NoteContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Weather_Note.Models.Context.NoteContext";
        }

        protected override void Seed(Models.Context.NoteContext context)
        {
        

        }
    }
}
