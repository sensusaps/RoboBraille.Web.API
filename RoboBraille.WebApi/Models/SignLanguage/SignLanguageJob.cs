using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class SignLanguageJob : Job
    {
        [NotMapped]
        public string SourceTextLanguage { get; set; }

        [NotMapped]
        public string TargetSignLanguage { get; set; }

        [NotMapped]
        public SignLanguageType SignLanguageForm { get; set; }
    }
}