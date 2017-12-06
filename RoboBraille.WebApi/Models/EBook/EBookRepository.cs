using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace RoboBraille.WebApi.Models
{
    public class EBookRepository : IRoboBrailleJob<EBookJob>
    {
        private readonly string calibre = ConfigurationSettings.AppSettings["CalibrePath"];
        private RoboBrailleDataContext _context;

        public EBookRepository()
        {
            _context = new RoboBrailleDataContext();
        }
        public EBookRepository(RoboBrailleDataContext roboBrailleDataContext)
        {
            _context = roboBrailleDataContext;
        }
        public async Task<Guid> SubmitWorkItem(EBookJob job)
        {
            var outputFormat = "." + job.EbookFormat.ToString().ToLowerInvariant();

            try
            {
                _context.Jobs.Add(job);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var task = Task.Factory.StartNew(t =>
            {
                EBookJob ebJob = (EBookJob)t;
                string tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                File.WriteAllBytes(tempfile + "." + ebJob.FileExtension, ebJob.FileContent);

                string cmdArgs = " --book-producer=\"Sensus\" --change-justification=\"left\"";
                if (ebJob.EbookFormat.Equals(EbookFormat.epub))
                {
                    cmdArgs += " --preserve-cover-aspect-ratio";
                }
                if (ebJob.EbookFormat.Equals(EbookFormat.mobi))
                {
                    cmdArgs += " --enable-heuristics";
                }
                if (ebJob.EbookFormat.Equals(EbookFormat.html))
                {
                    cmdArgs = "";
                }
                switch (ebJob.BaseFontSize)
                {
                    case EbookBaseFontSize.LARGE:
                        cmdArgs += " --base-font-size=\"16\"  --font-size-mapping=\"12,14,16,18,20,22,24,28\"";
                        break;
                    case EbookBaseFontSize.XLARGE:
                        cmdArgs += " --base-font-size=\"24\"  --font-size-mapping=\"18,20,24,26,28,30,32,36\"";
                        break;
                    case EbookBaseFontSize.HUGE:
                        cmdArgs += " --base-font-size=\"40\"  --font-size-mapping=\"32,36,40,42,48,56,60,72\"";
                        break;
                    default:
                        break;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = calibre;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;
                startInfo.FileName = "ebook-convert.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = tempfile + "." + ebJob.FileExtension + " " + tempfile + outputFormat + cmdArgs;

                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch (Exception e)
                {
                    RoboBrailleProcessor.SetJobFaulted(ebJob, _context);
                    throw e;
                }
                finally
                {
                    try
                    {
                        EbookFormat fmtOptions = ebJob.EbookFormat;
                        string mime = "application/epub+zip";
                        string fileExtension = ".epub";
                        switch (fmtOptions)
                        {
                            case EbookFormat.epub:
                                mime = "application/epub+zip";
                                fileExtension = ".epub";
                                break;
                            case EbookFormat.mobi:
                                mime = "application/x-mobipocket-ebook";
                                fileExtension = ".prc";
                                break;
                            case EbookFormat.txt:
                                mime = "plain/text";
                                fileExtension = ".txt";
                                break;
                            case EbookFormat.rtf:
                                mime = "application/rtf";
                                fileExtension = ".rtf";
                                break;
                            case EbookFormat.html:
                                mime = "text/html";
                                fileExtension = ".html";
                                break;
                            case EbookFormat.chm:
                                mime = "application/vnd.ms-htmlhelp";
                                fileExtension = ".chm";
                                break;
                            case EbookFormat.docx:
                                mime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                                fileExtension = ".docx";
                                break;
                            default:
                                mime = "application/epub+zip";
                                fileExtension = ".epub";
                                break;
                        }
                        if (File.Exists(tempfile + outputFormat))
                        {
                            ebJob.ResultContent = File.ReadAllBytes(tempfile + outputFormat);
                            ebJob.DownloadCounter = 0;
                            ebJob.ResultMimeType = mime;
                            ebJob.ResultFileExtension = fileExtension;
                            ebJob.Status = JobStatus.Done;
                            ebJob.FinishTime = DateTime.Now;
                            _context.Jobs.Attach(ebJob);
                            _context.Entry(ebJob).State = EntityState.Modified;
                            _context.SaveChanges();
                            File.Delete(tempfile + outputFormat);
                        }
                        else
                        {
                            RoboBrailleProcessor.SetJobFaulted(ebJob, _context);
                            throw new Exception("Result file does not exist!");
                        }
                        if (File.Exists(tempfile + "." + ebJob.FileExtension))
                        {
                            File.Delete(tempfile + "." + ebJob.FileExtension);
                        }
                    }
                    catch (Exception ex)
                    {
                        RoboBrailleProcessor.SetJobFaulted(ebJob, _context);
                        throw ex;
                    }
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