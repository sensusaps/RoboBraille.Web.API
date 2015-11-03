using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;




namespace RoboBraille.WebApi.Models
{
    public class BrailleJob : Job
    {
        public BrailleJob() {
            LinesPerPage = 0;
            CharactersPerLine = 0;
            ConversionPath = ConversionPath.texttobraille;
            PageNumbering = PageNumbering.none;
        }
                
        public BrailleFormat BrailleFormat { get; set; }
        public BrailleContraction Contraction { get; set; }
        public Language BrailleLanguage { get; set; }
        public OutputFormat OutputFormat { get; set; }

        [NotMapped]
        public ConversionPath ConversionPath { get; set; }

        [NotMapped]
        public int LinesPerPage { get; set; }

        [NotMapped]
        public int CharactersPerLine { get; set; }

        [JsonIgnore]
        [NotMapped]
        internal string TranslationTable { get; set; }

        [JsonIgnore]
        [NotMapped]
        internal PageNumbering PageNumbering { get; set; }
    }
}