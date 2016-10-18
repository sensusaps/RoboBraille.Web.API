using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models.LanguageTranslation
{
    public class TranslationJob : Job
    {
        [NotMapped]
        public string SourceLanguage { get; set; }
        [NotMapped]
        public string TargetLanguage { get; set; }
    }
}