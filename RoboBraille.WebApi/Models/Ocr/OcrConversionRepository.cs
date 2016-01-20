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
using Tesseract;

namespace RoboBraille.WebApi.Models
{
    public class OcrConversionRepository : IRoboBrailleJob<OcrConversionJob>
    {
        private static Dictionary<Language, string> supportedLangs = new Dictionary<Language, string>();

        public static string[] getOcrLanguages()
        {
            return Enum.GetNames(typeof(Language));
        }
        public System.Threading.Tasks.Task<Guid> SubmitWorkItem(OcrConversionJob job)
        {
            supportedLangs.Add(Language.enUS, "eng");
            supportedLangs.Add(Language.daDK, "dan");
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
                bool success = true;
                try
                {
                    if (!supportedLangs.ContainsKey(job.OcrLanguage))
                    {
                        throw new Exception("The input language is not supported by the ocr conversion tool");
                    }
                    using (var engine = new TesseractEngine(ConfigurationManager.AppSettings["TessDataPath"], supportedLangs[job.OcrLanguage], EngineMode.Default))
                    {
                        MemoryStream stream = new MemoryStream();
                        stream.Write(job.FileContent, 0, job.FileContent.Length);
                        // have to load Pix via a bitmap since Pix doesn't support loading a stream.
                        using (var image = new System.Drawing.Bitmap(stream))
                        {
                            using (var pix = PixConverter.ToPix(image))
                            {
                                using (var page = engine.Process(pix))
                                {
                                    job.ResultContent = Encoding.UTF8.GetBytes(page.GetText());
                                }
                            }
                        }
                    }
                    string mime = "text/plain";
                    string fileExtension = ".txt";
                    using (var context = new RoboBrailleDataContext())
                    {
                        job.DownloadCounter = 0;
                        job.ResultFileExtension = fileExtension;
                        job.ResultMimeType = mime;
                        job.Status = JobStatus.Done;
                        job.FinishTime = DateTime.UtcNow.Date;
                        context.Entry(job).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    Trace.WriteLine(ex.Message);
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
                var job = context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job == null || job.ResultContent == null)
                    return null;
                RoboBrailleProcessor rbp = new RoboBrailleProcessor();
                rbp.UpdateDownloadCounterInDb(job.Id);
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
        }
    }
}