using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public enum DocumentElement : int
    {
        Title = 1,
        Subtitle = 2,
        Paragraph = 3,
        Quote = 4,
        Preface = 5,
        Appendix = 6,
        Bulleted_List = 7,
        Numbered_List = 8,
        Email = 9,
        Commentary = 10,
        Link = 11,
        Reference = 12,
        TableOfContent = 13,
        Bookmark = 14,
        Footer = 15,
        Header = 16,
        Page_Number = 17,
        Column = 18,
        Symbol = 19,
        Object = 20,
        Image = 21,
        Video = 22,
        Calendar = 23,
        Figures = 24,
        SmartArt = 25,
        Diagram = 26,
        Equation = 27,
        Table = 28
    }
  
}