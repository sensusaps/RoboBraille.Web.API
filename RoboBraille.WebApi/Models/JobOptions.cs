using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public enum Language : uint
    {
        enUEB = 0x1,
        enGB = 0x0809,
        enUS = 0x0409,
        daDK = 0x0406,
        nnNO = 0x0814,
        isIS = 0x040F,
        ptPT = 0x0816,
        itIT = 0x0410,
        frFR = 0x040C,
        deDE = 0x0407,
        roRO = 0x0418,
        esES = 0x0C0A,
        slSI = 0x0424,
        huHU = 0x040E,
        bgBG = 0x0402,
        svSE = 0x041D,
        elGR = 0x0408,
        plPL = 0x0415
    }
}