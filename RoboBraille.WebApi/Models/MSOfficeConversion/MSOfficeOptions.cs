using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public enum MSOfficeOutput : int
    {
        pdf =1,
        txt=2,
        html=4,
        rtf=8
    }
}