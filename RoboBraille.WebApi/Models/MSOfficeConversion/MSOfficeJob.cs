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

        [JsonIgnore]
        [NotMapped]
        public string SubtitleLangauge { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string SubtitleFormat { get; set; }

        public MSOfficeJob()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.Now;
            FinishTime = DateTime.Now;
        }
    }
}