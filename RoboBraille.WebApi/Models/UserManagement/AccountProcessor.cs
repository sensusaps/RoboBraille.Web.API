using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class AccountProcessor
    {
        public AccountProcessor()
        {

        }

        internal string GeneratePasswordHash(string password)
        {
            return Encrypt.CreateHash(password);
        }

        internal static bool VerifyPassword(string password, string hash)
        {
            return Encrypt.ValidatePassword(password, hash);
        }
    }
}