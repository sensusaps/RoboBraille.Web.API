using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// Base class for all Jobs
    /// </summary>
    public abstract class Job
    {
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [JsonIgnore]
        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }


        [JsonIgnore]
        [Required]
        [StringLength(512)]
        public string FileName { get; set; }

        [JsonIgnore]
        [Required]
        [StringLength(512)]
        public string FileExtension { get; set; }

        [JsonIgnore]
        [Required]
        public string MimeType { get; set; }


        [JsonIgnore]
        [Required]
        public JobStatus Status { get; set; }


        [JsonIgnore]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SubmitTime { get; set; }


        [JsonIgnore]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// File to be uploaded
        /// </summary>
        
        //[Required]
        [NotMapped]
        [DataType(DataType.Upload)]
        public byte[] FileContent { get; set; }

        [JsonIgnore]
        public byte[] InputFileHash { get; set; }

        [JsonIgnore]
        public byte[] ResultContent { get; set; }
        
        [JsonIgnore]
        public virtual ServiceUser User { get; set; }

        //added 03.12.2015
        [JsonIgnore]
        public int DownloadCounter { get; set; }
        
        [JsonIgnore]
        public string ResultFileExtension { get; set; }

        [JsonIgnore]
        public string ResultMimeType { get; set; }

        public Job()
        {
            SubmitTime = DateTime.UtcNow;
            FinishTime = DateTime.UtcNow;
            DownloadCounter = 0;
        }
    }
}