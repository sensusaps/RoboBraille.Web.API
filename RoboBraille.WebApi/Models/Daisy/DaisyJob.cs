﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class DaisyJob : Job
    {
        public DaisyOutput DaisyOutput { get; set; }

        public DaisyJob()
        {

        }
    }
}