using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace RoboBraille.WebApi.Models
{
    public class MSOfficeRepository : IRoboBrailleJob<MSOfficeJob>
    {
        private RoboBrailleDataContext _context;
        private static string FileDirectory = ConfigurationManager.AppSettings.Get("FileDirectory");

        public MSOfficeRepository()
        {
            _context = new RoboBrailleDataContext();
        }

        public MSOfficeRepository(RoboBrailleDataContext context)
        {
            _context = context;
        }
        public async System.Threading.Tasks.Task<Guid> SubmitWorkItem(MSOfficeJob job)
        {
            try
            {
                _context.Jobs.Add(job);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }

            var task = Task.Factory.StartNew(t =>
            {
                string sourceFilePath = Path.Combine(Path.GetTempPath(), job.Id + "." + job.FileExtension);
                bool success = true;
                try
                {
                    /*
                     * What file do you have: doc, docx, rtf, ppt, pptx, html, other
                     * What file do you want: pdf, txt, html, rtf
                     * How to process? 
                    */
                    File.WriteAllBytes(sourceFilePath, job.FileContent);
                    RtfHTMLProcessor rhp = new RtfHTMLProcessor();
                    switch (job.MSOfficeOutput)
                    {
                        case MSOfficeOutput.pdf:
                            var tempFile = Path.Combine(Path.GetTempPath(), job.Id.ToString() + ".pdf");
                            try
                            {
                                string[] args = new string[] { sourceFilePath, tempFile };
                                int status = OfficeToPDF.OfficeToPdfFacade.ConvertOfficeToPDF(args);
                                if (status == 0)
                                {
                                    job.ResultContent = File.ReadAllBytes(tempFile);
                                }
                                else success = false;
                            }
                            catch (Exception e)
                            {
                                success = false;
                            }
                            finally
                            {
                                if (File.Exists(tempFile))
                                    File.Delete(tempFile);
                            }
                            break;
                        case MSOfficeOutput.txt:
                            if (job.MimeType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var res = OfficeWordProcessor.ConvertWordDocument(sourceFilePath, MSOfficeOutput.txt);
                                if (res != null)
                                    job.ResultContent = res;
                                else success = false;
                            }
                            else if ((job.MimeType.Equals("application/msword", StringComparison.InvariantCultureIgnoreCase) || job.MimeType.Equals("application/rtf", StringComparison.InvariantCultureIgnoreCase)) && job.FileExtension.Equals("rtf"))
                            {
                                job.ResultContent = Encoding.UTF8.GetBytes(rhp.ConvertRtfToText(Encoding.UTF8.GetString(job.FileContent)));
                            }
                            else if (job.MimeType.Equals("application/vnd.openxmlformats-officedocument.presentationml.presentation", StringComparison.InvariantCultureIgnoreCase))
                            {
                                byte[] result = null;
                                if (!OfficePresentationProcessor.ContainsVideo(sourceFilePath))
                                    result = OfficePresentationProcessor.ProcessDocument(sourceFilePath, job);
                                else
                                    result = (new OfficePresentationProcessor().ProcessPPTXWithVideo(sourceFilePath, job));
                                if (result != null)
                                    job.ResultContent = result;
                                else success = false;
                            }
                            else success = false;
                            break;
                        case MSOfficeOutput.html:
                            byte[] html = null;
                            switch (job.MimeType.ToLowerInvariant())
                            {
                                case "application/rtf":
                                case "application/msword":
                                    if (job.FileExtension.EndsWith("rtf"))
                                    {
                                        html = Encoding.UTF8.GetBytes(rhp.ConvertRtfToHtml(Encoding.UTF8.GetString(job.FileContent)));
                                    }
                                    break;
                                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                                    html = OfficeWordProcessor.ConvertWordDocument(sourceFilePath, MSOfficeOutput.html);
                                    break;
                                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                                    html = OfficePresentationProcessor.ProcessDocument(sourceFilePath, job);
                                    break;
                                default:
                                    success = false;
                                    break;
                            }
                            if (html != null)
                                job.ResultContent = html;
                            else success = false;
                            break;
                        case MSOfficeOutput.rtf:
                            byte[] rtf = null;
                            switch (job.MimeType.ToLowerInvariant())
                            {
                                case "text/html":
                                case "application/xhtml+xml":
                                    //rtf = rhp.ConvertHtmlToRtf(Encoding.UTF8.GetString(job.FileContent));
                                    break;
                                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                                    rtf = OfficePresentationProcessor.ProcessDocument(sourceFilePath, job);
                                    break;
                                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                                    rtf = OfficeWordProcessor.ConvertWordDocument(sourceFilePath, MSOfficeOutput.rtf);
                                    break;
                                default:
                                    success = false;
                                    break;
                            }
                            if (rtf != null)
                                job.ResultContent = rtf;
                            else success = false;
                            break;
                        default:
                            success = false;
                            break;
                    }
                    if (success)
                    {
                        string mime = "text/plain";
                        string fileExtension = ".txt";
                        switch (job.MSOfficeOutput)
                        {
                            case MSOfficeOutput.pdf:
                                mime = "application/pdf";
                                fileExtension = ".pdf";
                                break;
                            case MSOfficeOutput.html:
                                mime = "text/html";
                                fileExtension = ".html";
                                break;
                            case MSOfficeOutput.rtf:
                                mime = "application/rtf";
                                fileExtension = ".rtf";
                                break;
                            case MSOfficeOutput.txt:
                                if (job.SubtitleFormat == null && job.SubtitleLangauge == null)
                                {
                                    mime = "text/plain";
                                    fileExtension = ".txt";
                                }
                                else
                                {
                                    mime = "application/zip";
                                    fileExtension = ".zip";
                                }
                                break;
                            default:
                                break;
                        }
                        job.DownloadCounter = 0;
                        job.ResultFileExtension = fileExtension;
                        job.ResultMimeType = mime;
                        job.Status = JobStatus.Done;
                        job.FinishTime = DateTime.Now;
                        
                        _context.Entry(job).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    //sometimes transaction exception coming from video conversion part https://msdn.microsoft.com/en-gb/data/dn456833
                    success = false;
                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                    Trace.WriteLine(ex.Message);
                    throw ex;
                }
                finally
                {
                    if (File.Exists(sourceFilePath))
                        File.Delete(sourceFilePath);
                }

                if (!success)
                {
                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                    throw new Exception("Job was unssuccessful!");
                }

            }, job);
            
            return job.Id;
        }

        public int GetWorkStatus(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var job = _context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
            if (job != null)
                return (int)job.Status;
            return (int)JobStatus.Error;
        }

        public FileResult GetResultContents(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                return null;

            var job = _context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
            if (job == null || job.ResultContent == null)
                return null;
            RoboBrailleProcessor.UpdateDownloadCounterInDb(job.Id, _context);
            FileResult result = null;
            try
            {
                result = new FileResult(job.ResultContent, job.ResultMimeType, job.FileName + job.ResultFileExtension);
            }
            catch (Exception)
            {
                // ignored
            }
            return result;
        }


        public RoboBrailleDataContext GetDataContext()
        {
            return _context;
        }
    }
}