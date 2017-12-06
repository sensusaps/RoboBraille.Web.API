using MarkupConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/cc488002(v=vs.120).aspx
    /// https://code.msdn.microsoft.com/windowsdesktop/Converting-between-RTF-and-aaa02a6e
    /// </summary>
    public class RtfHTMLProcessor
    {
        private IMarkupConverter markupConverter;

        public RtfHTMLProcessor()
        {
            markupConverter = new MarkupConverter.MarkupConverter();
        }

        public string ConvertRtfToText(string input)
        {
            var thread = new Thread(ConvertRtfTextInSTAThread);
            var threadData = new ConvertRtfThreadData { RtfText = input };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(threadData);
            thread.Join();
            return threadData.HtmlText;
        }

        private void ConvertRtfTextInSTAThread(object rtf)
        {
            var threadData = rtf as ConvertRtfThreadData;
            System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox()
            {
                Rtf = threadData.RtfText
            };
            threadData.HtmlText = rtBox.Text;
        }
        
        public string ConvertHtmlToRtf(string htmlText)
        {
            var thread = new Thread(ConvertHtmlInSTAThread);
            var threadData = new ConvertRtfThreadData { HtmlText = htmlText };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(threadData);
            thread.Join();
            return threadData.RtfText;
        }

        private void ConvertHtmlInSTAThread(object html)
        {
            var threadData = html as ConvertRtfThreadData;
            threadData.RtfText = markupConverter.ConvertHtmlToRtf(threadData.HtmlText);
        }

        public string ConvertRtfToHtml(string rtfText)
        {
            var thread = new Thread(ConvertRtfInSTAThread);
            var threadData = new ConvertRtfThreadData { RtfText = rtfText };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(threadData);
            thread.Join();
            return threadData.HtmlText;
        }

        private void ConvertRtfInSTAThread(object rtf)
        {
            var threadData = rtf as ConvertRtfThreadData;
            threadData.HtmlText = markupConverter.ConvertRtfToHtml(threadData.RtfText);
        }

        private class ConvertRtfThreadData
        {
            public string RtfText { get; set; }
            public string HtmlText { get; set; }
        }
    }
}