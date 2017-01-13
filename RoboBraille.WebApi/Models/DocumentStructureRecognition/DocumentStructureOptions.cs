using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models.DocumentStructureRecognition
{
    public enum DocumentElement : int
    {
        Title = 1,
        Subtitle = 2,
        paragraph = 3,
        quote = 4,
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
        Presentation = 23,
        PowerPoint = 24,
        Calendar = 25,
        Figures = 26,
        SmartArt = 27,
        Diagram = 28,
        Equation = 29,
        Spreadsheet = 30,
        Table = 31
    }
  
}