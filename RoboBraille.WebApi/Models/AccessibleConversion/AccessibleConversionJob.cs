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
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
        }


        
        /// <summary>
        /// Use array value or index to specify file source format
        /// </summary>
        /// <remarks>TESTSETSET</remarks>
        [Required]
        [JsonIgnore]
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