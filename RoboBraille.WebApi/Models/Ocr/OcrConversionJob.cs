using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Tesseract;

namespace RoboBraille.WebApi.Models
{
    public class OcrConversionJob : Job
    {
        public Language OcrLanguage { get; set; }

        [JsonIgnore]
        [NotMapped]
        public bool HasTable { get; set; }

        public OcrConversionJob()
        {
            Init();
        }

        public OcrConversionJob(Language lang, bool hasTables)
        {
            Init();
            this.OcrLanguage = lang;
            this.HasTable = hasTables;
        }

        private void Init()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.Now;
            FinishTime = DateTime.Now;
        }
    }
}