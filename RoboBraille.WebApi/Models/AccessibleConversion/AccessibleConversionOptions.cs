using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SourceFormat
    {
        //text = 0,
        doc = 1,
        docx = 2,
        html = 3,
        htm = 4,
        rtf = 5,
        tiff = 6,
        tif = 7,
        gif = 8,
        jpg = 9,
        png = 10,
        bmp = 11,
        pcx = 12,
        dcx = 13,
        j2k = 14,
        jp2 = 15,
        jpx = 16,
        djv = 17,
        pdf = 18,
        xls = 19,
        xlsx = 20,
        csv = 21,
        ppt = 22,
        pptx = 23
    }
}