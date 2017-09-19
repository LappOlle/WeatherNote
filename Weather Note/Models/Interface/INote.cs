using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weather_Note.Models.Interface
{
    public interface INote
    {
        int ID { get; set; }

        DateTime Date { get; set; }

        string Message { get; set; }
    }
}