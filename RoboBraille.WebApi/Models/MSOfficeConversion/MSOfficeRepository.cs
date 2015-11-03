﻿using System;
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
        private static string FileDirectory = ConfigurationManager.AppSettings.Get("FileDirectory");
        public System.Threading.Tasks.Task<Guid> SubmitWorkItem(MSOfficeJob job)
        {
            if (job == null)
                return null;

            // TODO : REMOVE and use authenticated user id
            Guid uid;
            Guid.TryParse("d2b97532-e8c5-e411-8270-f0def103cfd0", out uid);
            job.UserId = uid;

            try
            {
                using (var context = new RoboBrailleDataContext())
                {
                    context.Jobs.Add(job);
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }

            var task = Task.Factory.StartNew(t =>
            {
                string sourceFilePath = FileDirectory+@"\Source\" + job.Id + job.FileName+"."+job.FileExtension;
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
                            string[] args = new string[] { sourceFilePath, FileDirectory + @"OfficeToPdfResults\" + job.Id.ToString() + ".pdf" };
                            int status = OfficeToPDF.OfficeToPdfFacade.ConvertOfficeToPDF(args);
                            if (status == 0)
                            {
                                job.ResultContent = File.ReadAllBytes(FileDirectory + @"OfficeToPdfResults\" + job.Id.ToString() + ".pdf");
                                //delete the file
                            }
                            else success = false;
                            break;
                        case MSOfficeOutput.txt:
                            if (job.MimeType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document", StringComparison.InvariantCultureIgnoreCase))
                            {
                                OfficeWordProcessor owp = new OfficeWordProcessor();
                                owp.ProcessWordDocument(sourceFilePath);
                                Dictionary<string, Object> result = owp.GetProcessedDocument();
                                string text = null;
                                foreach (KeyValuePair<string, Object> val in result)
                                {
                                    switch (val.Key.Substring(0, 4))
                                    {
                                        case "MATH":
                                        case "IMAG":
                                        case "TABL":
                                            break;
                                        default:
                                            text = text + val.Value + Environment.NewLine;
                                            break;
                                    }
                                }
                                if (text != null)
                                    job.ResultContent = Encoding.UTF8.GetBytes(text);
                                else success = false;
                            }
                            else if ((job.MimeType.Equals("application/msword", StringComparison.InvariantCultureIgnoreCase) || job.MimeType.Equals("application/rtf", StringComparison.InvariantCultureIgnoreCase)) && job.FileExtension.Equals("rtf"))
                            {
                                job.ResultContent = Encoding.UTF8.GetBytes(rhp.convertRtfToText(Encoding.UTF8.GetString(job.FileContent)));
                            }
                            else if (job.MimeType.Equals("application/vnd.openxmlformats-officedocument.presentationml.presentation", StringComparison.InvariantCultureIgnoreCase))
                            {
                                OfficePresentationProcessor opp = new OfficePresentationProcessor();
                                Dictionary<int, string> pptText = opp.ProcessPresentationText(sourceFilePath);
                                string text = null;
                                foreach (KeyValuePair<int, string> val in pptText)
                                {
                                    text = text + val.Value + Environment.NewLine;
                                }
                                if (text != null)
                                    job.ResultContent = Encoding.UTF8.GetBytes(text);
                                else success = false;
                            }
                            else success = false;
                            break;
                        case MSOfficeOutput.html:
                            string html = null;
                            switch (job.MimeType.ToLowerInvariant())
                            {
                                case "application/msword":
                                    if (job.FileExtension.Equals("rtf"))
                                    {
                                        html = rhp.ConvertRtfToHtml(Encoding.UTF8.GetString(job.FileContent));
                                    }
                                    break;
                                case "application/rtf":
                                    html = rhp.ConvertRtfToHtml(Encoding.UTF8.GetString(job.FileContent));
                                    break;
                                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                                    html = OfficeWordProcessor.ConvertToHtml(job.FileContent);
                                    break;
                                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                                    html = OfficePresentationProcessor.ConvertPptx(sourceFilePath, job.MSOfficeOutput,job.Id.ToString());
                                    break;
                                default:
                                    success = false;
                                    break;
                            }
                            if (html != null)
                                job.ResultContent = Encoding.UTF8.GetBytes(html);
                            else success = false;
                            break;
                        case MSOfficeOutput.rtf:
                            string rtf = null;
                            switch (job.MimeType.ToLowerInvariant())
                            { 
                                case "text/html":
                                case "application/xhtml+xml":
                                    rtf = rhp.ConvertHtmlToRtf(Encoding.UTF8.GetString(job.FileContent));
                                    break;
                                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                                    rtf = OfficePresentationProcessor.ConvertPptx(sourceFilePath, job.MSOfficeOutput, job.Id.ToString());
                                    break;
                                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                                    rtf = OfficeWordProcessor.ConvertWordToRtf(sourceFilePath, job.Id.ToString());
                                    break;
                                default:
                                    success = false;
                                    break;
                            }
                            if (rtf != null)
                                job.ResultContent = Encoding.UTF8.GetBytes(rtf);
                            else success = false;
                            break;
                        default:
                            success = false;
                            break;
                    }
                    if (success)
                    {
                        using (var context = new RoboBrailleDataContext())
                        {
                            job.Status = JobStatus.Done;
                            job.FinishTime = DateTime.UtcNow.Date;
                            context.Entry(job).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    Trace.WriteLine(ex.Message);
                }
                finally {
                    if (File.Exists(sourceFilePath))
                        File.Delete(sourceFilePath);
                }

                if (!success)
                {
                    try
                    {
                        using (var context = new RoboBrailleDataContext())
                        {
                            job.Status = JobStatus.Error;
                            job.FinishTime = DateTime.UtcNow.Date;
                            context.Entry(job).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }

            }, job);

            return Task.FromResult(job.Id);
        }

        public int GetWorkStatus(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            using (var context = new RoboBrailleDataContext())
            {
                var job = context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job != null)
                    return (int)job.Status;
            }
            return (int)JobStatus.Error;
        }

        public FileResult GetResultContents(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                return null;

            using (var context = new RoboBrailleDataContext())
            {
                var job = (MSOfficeJob)context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job == null || job.ResultContent == null)
                    return null;
                string fileName = job.FileName;
                string mime = "text/plain";
                switch (job.MSOfficeOutput)
                {
                    case MSOfficeOutput.pdf:
                        mime = "application/pdf";
                        fileName += ".pdf";
                        break;
                    case MSOfficeOutput.html:
                        mime = "text/html";
                        fileName += ".html";
                        break;
                    case MSOfficeOutput.rtf:
                        mime = "application/rtf";
                        fileName += ".rtf";
                        break;
                    case MSOfficeOutput.txt:
                    default:
                        fileName += ".txt";
                        break;
                }
                FileResult result = null;
                try
                {
                    result = new FileResult(job.ResultContent, mime, fileName);
                }
                catch (Exception)
                {
                    // ignored
                }
                return result;
            }
        }
    }
}