using System;
using System.ComponentModel.DataAnnotations.Schema;
using RoboBraille.WebApi.ABBYY;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// A class instance of an AccessibleConversionJob it extends the abstract Job class
    /// </summary>
    public class AccessibleConversionJob : Job
    {
        /// <summary>
        /// Creates an accessible conversion job
        /// </summary>
        public AccessibleConversionJob()
        {
            DownloadCounter = 0;
            SubmitTime = DateTime.Now;
            FinishTime = DateTime.Now;
        }
        
        /// <summary>
        /// Use array value or index to specify file source format
        /// </summary>
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