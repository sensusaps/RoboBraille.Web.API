using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Diagnostics;
using Spire.Presentation;

namespace RoboBraille.WebApi.Models
{
    public class OfficePresentationProcessor
    {
        //private static string FileDirectory = ConfigurationManager.AppSettings.Get("FileDirectory");
        private AmaraSubtitleRepository vcr;

        public OfficePresentationProcessor()
        {
            vcr = new AmaraSubtitleRepository();
        }

        public static void SavePPtxAsHtml(string sourceFile, string targetFile)
        {
            using (Spire.Presentation.Presentation ppt = new Spire.Presentation.Presentation())
            {
                ppt.LoadFromFile(sourceFile);
                ppt.SaveToFile(targetFile, FileFormat.Html);
            }
            string newHtml = File.ReadAllText(targetFile);
            string toMatch = "Evaluation Warning : The document was created with  Spire.Presentation for .NET";
            newHtml = newHtml.Replace(toMatch, "");
            File.WriteAllText(targetFile, newHtml);
        }

        public static byte[] ProcessDocument(string source, MSOfficeJob job)
        {
            var guid = job.Id.ToString();

            byte[] result = null;
            if (job.MSOfficeOutput.Equals(MSOfficeOutput.html))
            {
                var tempDirectory = Path.Combine(Path.GetTempPath(), guid);
                var tempZipFile = Path.Combine(Path.GetTempPath(), guid + ".zip");
                try
                {
                    Application app = new Application();
                    app.DisplayAlerts = PpAlertLevel.ppAlertsNone;
                    var pres = app.Presentations.Open(source, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
                    pres.SaveCopyAs(tempDirectory, PpSaveAsFileType.ppSaveAsHTML, MsoTriState.msoCTrue);
                    pres.Close();
                    app.Quit();
                    app = null;

                    ZipFile.CreateFromDirectory(tempDirectory, tempZipFile);
                    result = File.ReadAllBytes(tempZipFile);
                }
                catch (Exception e)
                {
                    //an exception is thrown if the powerpoint version is too new and does not support save as HTML
                    //instead use spire presentation
                    var tempFile = Path.Combine(Path.GetTempPath(), guid + ".html");
                    SavePPtxAsHtml(source, tempFile);
                    result = File.ReadAllBytes(tempFile);
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                finally
                {
                    if (Directory.Exists(tempDirectory))
                        Directory.Delete(tempDirectory, true);
                    if (File.Exists(tempZipFile))
                        File.Delete(tempZipFile);
                }
            }
            else if (job.MSOfficeOutput.Equals(MSOfficeOutput.rtf))
            {
                var tempFile = Path.Combine(Path.GetTempPath(), guid + ".rtf");
                try
                {
                    Application app = new Application();
                    app.DisplayAlerts = PpAlertLevel.ppAlertsNone;
                    var pres = app.Presentations.Open(source, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
                    pres.SaveCopyAs(tempFile, PpSaveAsFileType.ppSaveAsRTF, MsoTriState.msoCTrue);
                    pres.Close();
                    app.Quit();
                    app = null;
                    result = File.ReadAllBytes(tempFile);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
                finally
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
            }
            else if (job.MSOfficeOutput.Equals(MSOfficeOutput.txt))
            {
                string text = "";
                foreach (KeyValuePair<int, string> val in ExtractText(source))
                {
                    text = text + val.Value + Environment.NewLine;
                }
                result = Encoding.UTF8.GetBytes(text);
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

        public static bool ContainsVideo(string source)
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

        public byte[] ProcessPPTXWithVideo(string source, MSOfficeJob job)
        {
            var guid = job.Id.ToString();
            byte[] result = null;
            var tempDirectory = Path.Combine(Path.GetTempPath(), guid);
            var tempZipFile = Path.Combine(Path.GetTempPath(), guid + ".zip");
            try
            {

                Directory.CreateDirectory(tempDirectory);
                string text = "";
                foreach (KeyValuePair<int, string> val in ProcessVideoParts(source, job,tempDirectory).Result)
                {
                    text = text + val.Value + Environment.NewLine;
                }

                File.WriteAllText(Path.Combine(tempDirectory, job.FileName + ".txt"), text);

                ZipFile.CreateFromDirectory(tempDirectory, tempZipFile);
                result = File.ReadAllBytes(tempZipFile);
            }
            catch (Exception e)
            {
                result = null;
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                    Directory.Delete(tempDirectory, true);
                if (File.Exists(tempZipFile))
                    File.Delete(tempZipFile);
            }
            return result;
        }

        /// <summary>
        /// Consider two output types:
        /// .txt if pptx doesn't contain videos 
        /// .zip if it contains videos (a .txt file + the video files) 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<Dictionary<int, string>> ProcessVideoParts(string source, MSOfficeJob job,string tempDirectory)
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
                        File.WriteAllBytes(Path.Combine(tempDirectory, filename), videoBytes);

                        //send to amara
                        string langauge = "en-us";
                        if (ppt.PackageProperties.Language != null)
                            langauge = ppt.PackageProperties.Language;

                        AmaraSubtitleJob vj = new AmaraSubtitleJob()
                        {
                            SubmitTime = DateTime.Now,
                            FinishTime = DateTime.Now,
                            Status = JobStatus.Started,
                            MimeType = "video/" + filename.Substring(filename.LastIndexOf('.') + 1), //define this properly
                            User = job.User,
                            UserId = job.UserId,
                            DownloadCounter = 0,
                            FileContent = videoBytes,
                            InputFileHash = RoboBrailleProcessor.GetMD5Hash(videoBytes),
                            SubtitleFormat = job.SubtitleFormat,
                            SubtitleLangauge = job.SubtitleLangauge,
                            FileName = job.Id.ToString() + "-" + filename.Substring(0, filename.LastIndexOf('.')),
                            FileExtension = filename.Substring(filename.LastIndexOf('.') + 1)
                        };

                        //retrieve the message from amara
                        byte[] filebytes = null;
                        Guid subtitleJobId = vcr.SubmitWorkItem(vj).Result;
                        while (vcr.GetWorkStatus(subtitleJobId) == 2)
                        {
                            await Task.Delay(2000); 
                        }
                        filebytes = vcr.GetResultContents(subtitleJobId).getFileContents();
                        File.WriteAllBytes(Path.Combine(tempDirectory,filename.Substring(0, filename.LastIndexOf('.')) + vj.ResultFileExtension), filebytes);

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