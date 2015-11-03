namespace RoboBraille.WebApi.Models
{
    public enum AudioSpeed : int
    {
        Fastest = 6,
        Faster = 4,
        Fast = 2,
        Normal = 0,
        Slow = -2,
        Slower= -4,
        Slowest = -6
    }
    //public enum AudioLanguage :uint
    //{
    //    enUS = 0x0409,
    //    enGB = 0x0809
    //   /* DanishAnne,
    //    DanishSara,
    //    DanishCarsten,
    //    Arabic,
    //    ArabicEnglish,
    //    Belgium,
    //    NetherlandsFemale,
    //    NetherlandsMale,
    //    French,
    //    German,
    //    Greek,
    //    HungarianFemale,
    //    HungarianMale,
    //    IslandicFemale,
    //    IslandicMale,
    //    Lituanian,
    //    Lativian,
    //    Polish,
    //    Portuguese,
    //    Romanian,
    //    Russian,
    //    Slovenian,
    //    SpanishCatalan,
    //    SpanishLatinAmerica*/
    //}

    public enum AudioFormat :int
    {
        Mp3 =1,
        Wav =2,
        Wma =4,
        Aac =8
    }
}