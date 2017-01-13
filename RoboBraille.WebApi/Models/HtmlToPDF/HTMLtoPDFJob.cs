using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class HTMLtoPDFJob : Job
    {
        public HTMLtoPDFJob()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.Now;
            FinishTime = DateTime.Now;
        }

        [NotMapped]
        public PaperSize paperSize { get; set; }
    }
}