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

        }

        public OcrConversionJob(Language lang)
        {
            this.OcrLanguage = lang;
        }
    }
}