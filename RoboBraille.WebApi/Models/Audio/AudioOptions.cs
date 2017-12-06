namespace RoboBraille.WebApi.Models
{
    public enum AudioSpeed : int
    {
        TooFast = 10,
        StillFast = 9,
        Fastest = 8,
        ModerateFast = 7,
        Faster = 6,
        Fast5 = 5,
        Fast4 = 4,
        Fast = 3,
        NotSoFast = 2,
        KindOfFast = 1,
        Normal = 0,
        KindOfSlow = -1,
        NotSoSlow = -2,
        Slow = -3,
        Slow4 = -4,
        Slow5 = -5,
        Slower= -6,
        ModerateSlow= -7,
        Slowest = -8,
        StillSlow = -9,
        TooSlow = -10
    }
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
    public enum AudioFormat :int
    {
        Mp3 =1,
        Wav =2,
        Aac =8
    }
}