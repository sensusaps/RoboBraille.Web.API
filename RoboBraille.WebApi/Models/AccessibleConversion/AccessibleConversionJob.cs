using System;
using System.ComponentModel.DataAnnotations.Schema;
using RoboBraille.WebApi.ABBYY;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web.Script.Serialization;

namespace RoboBraille.WebApi.Models
{
    public class AccessibleConversionJob : Job
    {
        public AccessibleConversionJob()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.Now;
            FinishTime = DateTime.Now;
        }


        
        /// <summary>
        /// Use array value or index to specify file source format
        /// </summary>
        /// <remarks>TESTSETSET</remarks>
        [Required]
        [JsonIgnore]
        [NotMapped]
        public SourceFormat SourceDocumnetFormat { get; set; }
        
        /// <summary>
        /// Use array value or index to specify file target format
        /// </summary>
        [ScriptIgnore]
        [Required]
        public OutputFileFormatEnum TargetDocumentFormat { get; set; }



        [JsonIgnore]
        [NotMapped]
        internal PriorityEnum Priority { get; set; }
    }
}