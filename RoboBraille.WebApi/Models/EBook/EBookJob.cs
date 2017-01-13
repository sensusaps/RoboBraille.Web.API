using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace RoboBraille.WebApi.Models
{
    public class EBookJob : Job
    {
        public EBookJob()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.Now;
            FinishTime = DateTime.Now;
        }

        /// <summary>
        /// [mobi,epub]
        /// </summary>
       public EbookFormat EbookFormat{get; set;}
    }
}