using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace RoboBraille.WebApi.Models
{
    public class EBookJob : Job
    {
        public EBookJob() {
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
        }

        /// <summary>
        /// [mobi,epub]
        /// </summary>
       public EbookFormat EbookFormat{get; set;}
    }
}