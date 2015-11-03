using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace RoboBraille.WebApi.Models
{
    public class AudioJob : Job
    {
        public AudioJob()
        {
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
        }
        /// <summary>
        /// [EnglishUS,EnglishUK]
        /// </summary>
        public Language AudioLanguage { get; set; }
        /// <summary>
        /// [Normal, Fast, Faster, Fastest, Slowest, Slower, Slow]
        /// </summary>
        public AudioSpeed SpeedOptions { get; set; }
        /// <summary>
        /// [MP3, WAV, WMA, AAC]
        /// </summary>
        public AudioFormat FormatOptions { get; set; }
    }
}