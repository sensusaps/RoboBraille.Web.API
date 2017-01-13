using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace RoboBraille.WebApi.Models
{
    public class AudioJob : Job
    {
        public AudioJob()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.Now;
            FinishTime = DateTime.Now;
        }
        /// <summary>
        /// [enUS,enGB]
        /// </summary>
        public Language AudioLanguage { get; set; }
        /// <summary>
        /// [Normal, Fast, Faster, Fastest, Slowest, Slower, Slow]
        /// </summary>
        [NotMapped]
        public AudioSpeed SpeedOptions { get; set; }
        /// <summary>
        /// [MP3, WAV, WMA, AAC]
        /// </summary>
        public AudioFormat FormatOptions { get; set; }

        [NotMapped]
        [JsonIgnore]
        public IEnumerable<VoicePropriety> VoicePropriety { get; set; }
    }
}