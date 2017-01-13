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
        public Task<Guid> SubmitWorkItem(EBookJob job)
        {
            string outputFormat = ".epub";

            if (job == null)
                return null;


            switch (job.EbookFormat)
            {
                case EbookFormat.epub:
                    outputFormat = ".epub";
                    break;
                case EbookFormat.mobi:
                    outputFormat = ".mobi";
                    break;
                default:
                    outputFormat = ".epub";
                    break;

            }

            try
            {
                    try
                    {
                        _context.Jobs.Add(job);
                        _context.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var task = Task.Factory.StartNew(t =>
            {
                EBookJob ebJob = (EBookJob)t;
                string tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                File.WriteAllBytes(tempfile + ".pdf", job.FileContent);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = calibre;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;
                startInfo.FileName = "ebook-convert.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = tempfile + ".pdf " + tempfile + outputFormat + " --enable-heuristics";

                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch
                {
                    // Log error.
                }
                finally
                {
                    try
                    {
                        EbookFormat fmtOptions = job.EbookFormat;
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
                            default:
                                mime = "application/epub+zip";
                                fileExtension = ".epub";
                                break;
                        }

                            ebJob.ResultContent = File.ReadAllBytes(tempfile + outputFormat);
                            ebJob.DownloadCounter = 0;
                            ebJob.ResultMimeType = mime;
                            ebJob.ResultFileExtension = fileExtension;
                            ebJob.Status = JobStatus.Done;
                            ebJob.FinishTime = DateTime.Now;
                            _context.Jobs.Attach(ebJob);
                            _context.Entry(job).State = EntityState.Modified;
                            _context.SaveChanges();
                            File.Delete(tempfile + ".pdf");
                            File.Delete(tempfile + outputFormat);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }




            }, job).ContinueWith(t =>
            {

            }, TaskContinuationOptions.OnlyOnFaulted);

            return Task.FromResult(job.Id);

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