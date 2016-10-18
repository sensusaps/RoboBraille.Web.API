using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class HTMLToTextJob : Job
    {
        public HTMLToTextJob()
        {
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
            DownloadCounter = 0;
        }
    }
}