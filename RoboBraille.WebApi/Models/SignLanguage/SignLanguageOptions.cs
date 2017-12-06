using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public enum SignLanguageType : int
    {
        DeafSignLanguage = 0,
        AuxiliarySignLanguage = 1,
        SignedMode = 2,
        SpokenLanguage = 3,
        UserDefined = 4
    }
}