using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class MSOfficeJob : Job
    {
        public MSOfficeOutput MSOfficeOutput { get; set; }

        public MSOfficeJob()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
        }
    }
}