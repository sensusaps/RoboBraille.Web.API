﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// Not used in this version
    /// </summary>
    public class ServiceUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }
        [Required]
        [StringLength(32)]
        public string UserName { get; set; }
        [Required]
        public byte[] ApiKey { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}