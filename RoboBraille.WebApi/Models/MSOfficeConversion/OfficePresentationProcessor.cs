using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Configuration;
using System.IO;

namespace RoboBraille.WebApi.Models
{
    public class OfficePresentationProcessor
    {
        private static string FileDirectory = ConfigurationManager.AppSettings.Get("FileDirectory");
        public static string ConvertPptx(string source, MSOfficeOutput msOut,string guid) {
            string result = null;
            Application app = new Application();
            var pres = app.Presentations.Open(source, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
            if (msOut.Equals(MSOfficeOutput.html))
            {
                try
                {
                    pres.SaveCopyAs(FileDirectory+@"Temp\"+guid+".html", PpSaveAsFileType.ppSaveAsHTML, MsoTriState.msoCTrue);
                    result = File.ReadAllText(FileDirectory + @"Temp\" + guid + ".html",Encoding.UTF8);
                    File.Delete(FileDirectory + @"Temp\" + guid + ".html");
                }
                catch {

                }
            }
            if (msOut.Equals(MSOfficeOutput.rtf))
            {
                pres.SaveCopyAs(FileDirectory + @"Temp\" + guid + ".rtf", PpSaveAsFileType.ppSaveAsRTF, MsoTriState.msoCTrue);
                result = File.ReadAllText(FileDirectory + @"Temp\" + guid + ".rtf",Encoding.UTF8);
                File.Delete(FileDirectory + @"Temp\" + guid + ".rtf");
            }
            return result;
        }
        public Dictionary<int, string> ProcessPresentationText(string source)
        {
            Dictionary<int, string> TextContent = new Dictionary<int, string>();
            int index = 1;
            using (PresentationDocument ppt = PresentationDocument.Open(source, false))
            {
                // Get the relationship ID of the first slide.
                PresentationPart part = ppt.PresentationPart;
                OpenXmlElementList slideIds = part.Presentation.SlideIdList.ChildElements;
                foreach (SlideId slideID in slideIds)
                {
                    //get the right content according to type. for now only text is getting through
                    string relId = slideID.RelationshipId;

                    // Get the slide part from the relationship ID.
                    SlidePart slide = (SlidePart)part.GetPartById(relId);

                    // Build a StringBuilder object.
                    StringBuilder paragraphText = new StringBuilder();

                    // Get the inner text of the slide:
                    IEnumerable<A.Text> texts = slide.Slide.Descendants<A.Text>();

                    foreach (A.Text text in texts)
                    {
                        if (text.Text.Length > 1)
                            paragraphText.Append(text.Text + " ");
                        else
                            paragraphText.Append(text.Text);
                    }
                    string slideText = paragraphText.ToString();
                    TextContent.Add(index, slideText);
                    index++;
                }
            }
            return TextContent;
        }
    }
}