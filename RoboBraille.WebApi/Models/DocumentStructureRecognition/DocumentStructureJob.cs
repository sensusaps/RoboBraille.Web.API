using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models.DocumentStructureRecognition
{
    public class DocumentStructureJob : Job
    {
        [NotMapped]
        public DocumentElement DocumentElement { get; set; }

    }
}