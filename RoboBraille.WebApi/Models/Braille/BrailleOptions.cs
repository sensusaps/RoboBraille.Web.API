namespace RoboBraille.WebApi.Models
{

    public enum PageNumbering : int
    {
        none = 0,
        right = 1,
        left = 2
    }

    public enum BrailleFormat : int
    {
        sixdot = 6,
        eightdot = 8
    }

    public enum ConversionPath : int
    {
        texttobraille = 0,
        brailletotext = 1
    }

    public enum BrailleContraction : int
    {
        full = 1,
        small = 2,
        large = 3,
        grade1 = 4,
        grade2 = 5,
        level0 = 6,
        level1 = 7,
        level2 = 8,
    }

    public enum OutputFormat : int
    {
        None = 0,
        OctoBraille = 1,
        Unicode = 2,
        Pef = 3,
        NACB = 4
    }
}