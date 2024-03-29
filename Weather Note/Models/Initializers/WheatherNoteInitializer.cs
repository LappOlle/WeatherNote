﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Weather_Note.Models.Context;
using System.Data.Entity.Migrations;
using Weather_Note.Models.POCO;

namespace Weather_Note.Models.Initializers
{
    /*My own Initializer to create a database if it not exist, and then seed it with some default data.*/
    public class WheatherNoteInitializer:CreateDatabaseIfNotExists<NoteContext>
    {
        protected override void Seed(NoteContext context)
        {
            var dateToday = DateTime.Now;
            var seedMessages = new List<string>() {
            "Oh no i was planning to go fishing.",
            "Yessss, Finally i can go bathing with my family.",
            "I think I'm born with unluck.",
            "Perfect day for barbeque!",
            "Why can i never go outside.",
            "Where are you fortuna!",
            "I miss the winter, I want minus now."};

            for (int i = 0; i < seedMessages.Count(); i++)
            {
                //I use the AddOrUpdate It's only add if the data doesn't already exist.
                context.Notes.AddOrUpdate(
                new Note { ID = i, Date = dateToday, Message = seedMessages[i] });
                dateToday = dateToday.AddDays(1);
            }
            context.SaveChanges();
        }
    }
}