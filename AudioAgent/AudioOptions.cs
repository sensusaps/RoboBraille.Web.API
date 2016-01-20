﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAgent
{
    public enum VoicePropriety : int
    {
        Male = 1,
        Female = 2,
        Older = 3,
        Younger = 4,
        Bilingual = 5,
        Cantonese = 6,
        Mandarin = 7,
        Taiwanese = 8,
        Castilian = 9,
        LatinAmerican = 10,
        Anne = 11,
        None = 0
    }
    public enum Language : uint
    {
        enUEB = 0x1,
        esCO = 0x240A,
        arEG = 0x0C01,
        klGL = 0x2,
        enGB = 0x0809,
        enUS = 0x0409,
        daDK = 0x0406,
        nnNO = 0x0814,
        nbNO = 0x0414,
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
        plPL = 0x0415,
        nlNL = 0x0813,
        ltLT = 0x0427,
        zhCN = 0x0804,
        zhHK = 0x0C04,
        zhTW = 0x0404,
        fiFI = 0x040B,
        ruRU = 0x0419,
        jaJP = 0x0411,
        koKR = 0x0412,
        ptBR = 0x0416,
        enAU = 0x0C09,
        enCA = 0x1009,
        enIN = 0x4,
        frCA = 0x0C0C,
        esMX = 0x080A,
        caES = 0x0403
    }
}
