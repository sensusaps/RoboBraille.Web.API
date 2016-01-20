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
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
        }

        [NotMapped]
        public PaperSize paperSize { get; set; }
    }
}