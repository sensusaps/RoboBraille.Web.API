using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class HTMLtoPDFJob : Job
    {
        public HTMLtoPDFJob()
        {
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
        }

        public PaperSize size { get; set; }
    }
}