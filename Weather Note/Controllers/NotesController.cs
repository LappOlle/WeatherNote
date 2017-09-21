using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Weather_Note.Models.Context;
using Weather_Note.Models.POCO;

namespace Weather_Note.Controllers
{
    public class NotesController : Controller
    {
        //DB Context, It's with this you to all the CRUD operation to the database.
        private NoteContext db = new NoteContext();

        /*My OpenWeatherService. You can send an other cityID or apiKey then the default.
         By default it's choosen Örnsköldsvik in sweden ="2686469"*/
        private OpenWeatherService weather = new OpenWeatherService();

        /// <summary>
        /// ActionResult for the index view.
        /// </summary>
        /// <param name="sortOrder">send what you want to sort by, "SortMessage" or "SortDate".
        /// It will toggle between Descending and Ascending if you press link.</param>
        /// <param name="searchString">Send the Message that you want to search for.</param>
        /// <returns>Returns a view with the choosen data.</returns>
        public async Task<ActionResult> Index(string sortOrder,string searchString)
        {
            foreach (var item in db.Notes)//For every item in Notes we Gets and set the max temp for that day.
            {
                item.MaxTemp = await weather.GetMaxTemp(item.Date);
            }
            //Toggle message sort by descending and ascending.
            ViewBag.SortMessage = !String.IsNullOrEmpty(sortOrder) ? "" : "message_desc";
            //Toggle date sort by descending and ascending.
            ViewBag.SortDate = sortOrder == "date_desc" ? "Date" : "date_desc";

            /*Load all the notes from database to an IQuerable collection
             * so we can do querys and return a temporary list to the view.*/
            var notes = from n in db.Notes select n;
            var notesToSend = new List<Note>();
            //If the searchString isn't null we search for message containing that text.
            if (!String.IsNullOrEmpty(searchString))
            {
                var arrayWithSearchWords = new string[Regex.Split(searchString, "[^a-zA-Z0-9]").Length];
                arrayWithSearchWords = Regex.Split(searchString.ToLower(), "[^a-zA-Z0-9]");

                foreach (var item in notes)
                {
                    var allWordsInMessage = new string[Regex.Split(item.Message, "[^a-zA-Z0-9]").Length];
                    allWordsInMessage = Regex.Split(item.Message.ToLower(), "[^a-zA-Z0-9]");

                    var both = allWordsInMessage.Intersect(arrayWithSearchWords) ;
                    if (both.Count() == arrayWithSearchWords.Length)
                    {
                        notesToSend.Add(item);
                    }
                }
                notes = notesToSend.AsQueryable();
            }

            /*Switching the choosen sortOrder and sort the temporary list by the case result
             * and then return it to the view.*/
            switch (sortOrder)
            {
                case "message_desc":
                    {
                        notes = notes.OrderByDescending(n => n.Message);
                        break;
                    }
                case "date_desc":
                    {
                        notes = notes.OrderByDescending(n => n.Date);
                        break;
                    }
                default:
                    {
                        notes = notes.OrderBy(n => n.Date);
                        break;
                    }
            }

            return View(notes.ToList());
        }

        /// <summary>
        /// Action result to display a choosen model details.
        /// </summary>
        /// <param name="id">Model ID</param>
        /// <returns>returns a model with the specific id.</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        /// <summary>
        /// ActionResult to show the Create view.
        /// </summary>
        /// <returns>Returns the create view to create new notes.</returns>
        public ActionResult Create()
        {
            return View();
        }

       /// <summary>
       /// ActionResult for save the model(post) and redirect to the Index View.
       /// </summary>
       /// <param name="note">The model that you want to save.</param>
       /// <returns>Redirect to Index view if everything went fine, else it displays what's missing in the 
       /// Create view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Date,Message")] Note note)
        {
            if (ModelState.IsValid)
            {
                db.Notes.Add(note);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(note);
        }

        /// <summary>
        /// ActionResult for editing an note model.
        /// </summary>
        /// <param name="id">The model ID.</param>
        /// <returns>Returns the Edit View with the model date and message.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        /// <summary>
        /// ActionResult for save the model that you have edit, and then return to Index View.
        /// </summary>
        /// <param name="note"></param>
        /// <returns>Redirect to Index view if everything went fine, else it displays what's missing in the 
        /// Edit view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Date,Message")] Note note)
        {
            if (ModelState.IsValid)
            {
                db.Entry(note).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(note);
        }

        /// <summary>
        /// ActionResult for delete a model.
        /// </summary>
        /// <param name="id">Model ID</param>
        /// <returns>Returns the delete view with the choosen model.</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        /// <summary>
        /// ActionResult for showing that everything went fine when deleting the note model.
        /// </summary>
        /// <param name="id">Model ID</param>
        /// <returns>Returns the Index View if everything went fine.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = db.Notes.Find(id);
            db.Notes.Remove(note);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
