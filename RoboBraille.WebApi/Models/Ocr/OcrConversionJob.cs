using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tesseract;

namespace RoboBraille.WebApi.Models
{
    public class OcrConversionJob : Job
    {
        public Language OcrLanguage { get; set; }
        
        public OcrConversionJob()
        {
            Init();
        }

        public OcrConversionJob(Language lang)
        {
            Init();
            this.OcrLanguage = lang;
        }

        private void Init()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
        }
    }
}