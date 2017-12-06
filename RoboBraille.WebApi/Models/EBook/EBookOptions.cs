namespace RoboBraille.WebApi.Models
{
        public enum EbookFormat :int 
        {
            mobi =1,
            epub =2,
            txt = 3, 
            rtf = 4,
            docx = 5,
            html = 7,
            chm = 8
        }

    public enum EbookBaseFontSize:int
    {
        LARGE = 16,
        XLARGE = 24,
        HUGE = 40
    }
}