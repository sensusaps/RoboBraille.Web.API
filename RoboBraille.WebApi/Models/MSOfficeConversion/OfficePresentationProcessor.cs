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
using System.Threading.Tasks;
using System.IO.Compression;
using System.Diagnostics;

namespace RoboBraille.WebApi.Models
{
    public class OfficePresentationProcessor
    {
        private static string FileDirectory = ConfigurationManager.AppSettings.Get("FileDirectory");
        private static string guid = null;
        private static RoboVideo.VideoConversionRepository vcr = new RoboVideo.VideoConversionRepository();
        public byte[] ProcessDocument(string source, MSOfficeJob job)
        {
            guid = job.Id.ToString();

            byte[] result = null;
            Application app = new Application();
            var pres = app.Presentations.Open(source, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
            if (job.MSOfficeOutput.Equals(MSOfficeOutput.html))
            {
                try
                {
                    pres.SaveCopyAs(FileDirectory + @"Temp\" + guid + ".html", PpSaveAsFileType.ppSaveAsHTML, MsoTriState.msoCTrue);
                    result = File.ReadAllBytes(FileDirectory + @"Temp\" + guid + ".html");
                    File.Delete(FileDirectory + @"Temp\" + guid + ".html");
                }
                catch
                {

                }
            }
            else
                if (job.MSOfficeOutput.Equals(MSOfficeOutput.rtf))
                {
                    pres.SaveCopyAs(FileDirectory + @"Temp\" + guid + ".rtf", PpSaveAsFileType.ppSaveAsRTF, MsoTriState.msoCTrue);
                    result = File.ReadAllBytes(FileDirectory + @"Temp\" + guid + ".rtf");
                    File.Delete(FileDirectory + @"Temp\" + guid + ".rtf");
                }
                else
                    if (job.MSOfficeOutput.Equals(MSOfficeOutput.txt))
                    {
                        if (job.SubtitleLangauge != null && job.SubtitleFormat != null)
                        {
                            //maybe surround this with a try catch
                            try
                            {
                                Directory.CreateDirectory(FileDirectory + "Temp\\" + guid);
                                string text = "";
                                foreach (KeyValuePair<int, string> val in ProcessPPTXWithVideo(source, job))
                                {
                                    text = text + val.Value + Environment.NewLine;
                                }
                                File.WriteAllText(FileDirectory + @"Temp\" + guid + "\\" + job.FileName + ".txt", text);
                                string ZipFilePath = FileDirectory + @"Temp\" + guid + ".zip";
                                ZipFile.CreateFromDirectory(FileDirectory + @"Temp\" + guid, ZipFilePath);
                                result = File.ReadAllBytes(ZipFilePath);
                                Directory.Delete(FileDirectory + "Temp\\" + guid,true);
                                File.Delete(ZipFilePath);
                            }
                            catch (Exception e)
                            {
                                
                            }
                        }
                        else
                        {
                            string text = "";
                            foreach (KeyValuePair<int, string> val in ExtractText(source))
                            {
                                text = text + val.Value + Environment.NewLine;
                            }
                            result = Encoding.UTF8.GetBytes(text);
                        }
                    }
            return result;
        }

        private static Dictionary<int, string> ExtractText(string source)
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
                    var videos = slide.Slide.Descendants<Video>();
                    var videos2 = slide.Slide.CommonSlideData.ShapeTree.Descendants<DocumentFormat.OpenXml.Drawing.VideoFromFile>();
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

        public bool ContainsVideo(string source)
        {
            using (PresentationDocument ppt = PresentationDocument.Open(source, false))
            {
                PresentationPart part = ppt.PresentationPart;
                foreach (SlideId slideID in part.Presentation.SlideIdList.ChildElements)
                {
                    if (((SlidePart)part.GetPartById(slideID.RelationshipId)).Slide.Descendants<A.VideoFromFile>().Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Consider two output types:
        /// .txt if pptx doesn't contain videos 
        /// .zip if it contains videos (a .txt file + the video files) 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Dictionary<int, string> ProcessPPTXWithVideo(string source, MSOfficeJob job)
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

                    //extract the videos and put placeholders
                    IEnumerable<A.VideoFromFile> videos = slide.Slide.Descendants<A.VideoFromFile>();
                    foreach (A.VideoFromFile vid in videos)
                    {
                        string videoRelId = vid.Link;
                        ReferenceRelationship rel = slide.GetReferenceRelationship(videoRelId);
                        Uri uri = rel.Uri;
                        var filename = uri.ToString().Split('/').Last();
                        var s = ppt.Package.GetPart(uri).GetStream();
                        byte[] videoBytes = null;
                        using (BinaryReader br = new BinaryReader(s))
                        {
                            videoBytes = br.ReadBytes((int)s.Length);
                        }
                        //write video to result directory
                        File.WriteAllBytes(FileDirectory + "Temp\\" + guid + "\\" + filename, videoBytes);

                        //send to amara
                        string langauge = "en-us";
                        if (ppt.PackageProperties.Language != null)
                            langauge = ppt.PackageProperties.Language;

                        RoboVideo.VideoJob vj = new RoboVideo.VideoJob()
                        {
                            SubmitTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                            Status = JobStatus.Started,
                            MimeType = "video/" + filename.Substring(filename.LastIndexOf('.') + 1), //define this properly
                            User = job.User,
                            UserId = job.UserId,
                            DownloadCounter = 0,
                            FileContent = videoBytes,
                            InputFileHash = RoboBrailleProcessor.GetInputFileHash(videoBytes),
                            SubtitleFormat = job.SubtitleFormat,
                            SubtitleLangauge = job.SubtitleLangauge,
                            FileName = guid + "-" + filename.Substring(0, filename.LastIndexOf('.')),
                            FileExtension = filename.Substring(filename.LastIndexOf('.') + 1)
                        };

                        //retrieve the message from amara
                        byte[] filebytes = null;
                        Guid subtitleJobId = vcr.SubmitWorkItem(vj).Result;
                        //try
                        //{
                            while (vcr.GetWorkStatus(subtitleJobId) == 2)
                            {
                                Task.Delay(2000); //don't think that this actually works
                            }
                        //}
                        //catch (Exception e)
                        //{
                        //    Trace.WriteLine(e);
                        //}
                        filebytes = vcr.GetResultContents(subtitleJobId).getFileContents();
                        File.WriteAllBytes(FileDirectory + "Temp\\" + guid + "\\" + filename.Substring(0, filename.LastIndexOf('.')) + vj.ResultFileExtension, filebytes);

                        slide.Slide.CommonSlideData.ShapeTree.AppendChild(
                            new ShapeProperties(
                                new TextBody(
                                    new A.Paragraph(
                                        new A.Run(
                                            new A.Text("Video file name = " + filename + " subtitle attached to video has id: " + vj.Id.ToString())
                                            )))));
                    }


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
                //until now there are: video files + subtitle files
                //at the end there will be: a txt file + video files + subtitle files (all zipped up)
            }
            return TextContent;
        }
    }
}