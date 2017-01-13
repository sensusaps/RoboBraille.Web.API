using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models.UserManagement
{
    public class ServiceUserSummary
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        public bool IsLifeTimeLicense { get; set; }

        [DisplayName("Expiration Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }

        [Required(ErrorMessage = "Administrator password is required")]
        public string ClearTextPassPhrase { get; set; }

        [DataType(DataType.MultilineText)]
        public string StatusMessage { get; set; }

        public ServiceUserSummary()
        {

        }

        public ServiceUserSummary(string uName, string email, bool isLifeTimeLicense, DateTime expirationDate)
        {
            this.UserName = uName;
            this.EmailAddress = email;
            this.IsLifeTimeLicense = isLifeTimeLicense;
            this.ExpirationDate = expirationDate;
        }
    }
}