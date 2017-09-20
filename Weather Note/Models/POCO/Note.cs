using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Weather_Note.Models.Interface;

namespace Weather_Note.Models.POCO
{
    public class Note:INote
    {
        public int ID { get; set; }

        //[DataType(DataType.Date)] When i had this i got one extra date picker from some framework. 
        //I only want my JQueryUi datepicker.
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        public string Message { get; set; }

        [NotMapped]//Data annotation for not mapping this propertie to the database with Entity Framework.
        public string MaxTemp { get; set; }
    }
}