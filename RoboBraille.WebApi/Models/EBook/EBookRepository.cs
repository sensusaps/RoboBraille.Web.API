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
        public Task<Guid> SubmitWorkItem(EBookJob job)
        {
            string outputFormat = ".epub";

            if (job == null)
                return null;

            
            switch (job.EbookFormat) { 
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
                // TODO : REMOVE and use authenticated user id
                Guid uid;
                Guid.TryParse("d2b97532-e8c5-e411-8270-f0def103cfd0", out uid);
                job.UserId = uid;

                using (var context = new RoboBrailleDataContext())
                {
                    try
                    {
                        context.Jobs.Add(job);
                        context.SaveChanges();
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
                finally {
                    try
                    {
                        using (var context = new RoboBrailleDataContext())
                        {
                            ebJob.ResultContent = File.ReadAllBytes(tempfile + outputFormat);
                            ebJob.Status = JobStatus.Done;
                            context.Jobs.Attach(ebJob);
                            context.Entry(job).State = EntityState.Modified;
                            context.SaveChanges();
                            File.Delete(tempfile+".pdf");
                            File.Delete(tempfile + outputFormat);
                        }
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
                var job = (EBookJob)context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job == null || job.ResultContent == null)
                    return null;

                string mime = "application/epub+zip";
                EbookFormat fmtOptions = (EbookFormat)Enum.Parse(typeof(EbookFormat), Convert.ToString(job.EbookFormat));
                string fileName = job.FileName;
                switch (fmtOptions)
                {
                    case EbookFormat.epub:
                        mime = "application/epub+zip";
                        fileName += ".epub";
                        break;

                    case EbookFormat.mobi:
                        mime = "application/x-mobipocket-ebook";
                        fileName += ".prc";
                        break;
                    default:
                        mime = "application/epub+zip";
                        fileName += ".epub";
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