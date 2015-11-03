namespace RoboBraille.WebApi.Models
{
    //public enum BrailleLanguage : uint
    //{
    //    enUEB = 0x1,
    //    enGB = 0x0809,
    //    enUS = 0x0409,
    //    daDK = 0x0406,
    //    nnNO = 0x0814,
    //    isIS = 0x040F,
    //    ptPT = 0x0816,
    //    itIT = 0x0410,
    //    frFR = 0x040C,
    //    deDE = 0x0407,
    //    roRO = 0x0418,
    //    esES = 0x0C0A,
    //    slSI = 0x0424,
    //    huHU = 0x040E,
    //    bgBG = 0x0402,
    //    svSE = 0x041D,
    //    elGR = 0x0408,
    //    plPL = 0x0415
    //}

    public enum PageNumbering : int
    {
        none = 0,
        right = 1,
        left = 2
    }

    public enum BrailleFormat: int
    {
        sixdot =6,
        eightdot=8
    }

    public enum ConversionPath : int
    {
        texttobraille = 0,
        brailletotext = 1
    }

    public enum BrailleContraction : int
    {
        full =1,
        small =2,
        large =3,
        large2 =4,
        grade0 =5,
        grade1 =6,
        grade2b =7,
        grade2i =8,
        grade2 =9,
        level0 =10,
        level1 =11,
        level2 =12,
        level3 =13,
        user =14
    }

    public enum OutputFormat : int
    {
        None=0,
        OctoBraille = 1,
        Unicode= 2,
        Pef=3,
        NACB = 4
    }
}