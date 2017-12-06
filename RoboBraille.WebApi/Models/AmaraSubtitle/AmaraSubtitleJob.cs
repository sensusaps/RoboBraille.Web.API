using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class AmaraSubtitleJob : Job
    {
        [Required]
        public string VideoUrl { get; set; }

        [Required]
        public string SubtitleLangauge { get; set; }

        [Required]
        public string SubtitleFormat { get; set; }

        [JsonIgnore]
        public string AmaraVideoId { get; set; }
    }
}