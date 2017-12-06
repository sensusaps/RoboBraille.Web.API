using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Tesseract;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RoboBraille.WebApi.Models
{
    public class OcrConversionRepository : IRoboBrailleJob<OcrConversionJob>
    {
        private RoboBrailleDataContext _context;
        private static Dictionary<Language, string> supportedLangs;

        private static readonly string baseAddress = "http://160.40.50.183:80/";
        private static readonly string service_url = baseAddress+"P4All/certhOCR/";

        public OcrConversionRepository()
        {
            _context = new RoboBrailleDataContext();
            supportedLangs = new Dictionary<Language, string>();
            supportedLangs.Add(Language.enUS, "eng");
            supportedLangs.Add(Language.daDK, "dan");
        }

        public OcrConversionRepository(RoboBrailleDataContext context)
        {
            _context = context;
            supportedLangs = new Dictionary<Language, string>();
            supportedLangs.Add(Language.enUS, "eng");
            supportedLangs.Add(Language.daDK, "dan");
        }

        public static IEnumerable<string> getOcrLanguages()
        {
            IEnumerable<string> supportedLangs = new List<string>()
            {
                Enum.GetName(typeof(Language),Language.enUS),
                Enum.GetName(typeof(Language),Language.daDK)

            };

            return supportedLangs;
        }
        public async System.Threading.Tasks.Task<Guid> SubmitWorkItem(OcrConversionJob job)
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
                try
                {
                    string mime = "text/plain";
                    string fileExtension = ".txt";
                    if (job.HasTable)
                    {
                        job = PostToCerth(job).Result;
                        mime = "text/html";
                        fileExtension = ".html";
                    }
                    else
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
                        mime = "text/plain";
                        fileExtension = ".txt";
                    }
                    job.DownloadCounter = 0;
                    job.ResultFileExtension = fileExtension;
                    job.ResultMimeType = mime;
                    job.Status = JobStatus.Done;
                    job.FinishTime = DateTime.Now;
                    _context.Entry(job).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                    throw ex;
                }
            }, job);
            
            return job.Id;
        }

        public static async Task<OcrConversionJob> PostToCerth(OcrConversionJob job)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://160.40.50.183/");
                var bac = new ByteArrayContent(job.FileContent);
                bac.Headers.ContentType = MediaTypeHeaderValue.Parse("application/binary");
                var mfdc = new MultipartFormDataContent();
                mfdc.Add(bac, "fileToUpload", job.FileName);
                var response = await client.PostAsync("P4All/certhOCR/",mfdc);
                if (response.IsSuccessStatusCode)
                {
                    var htmlRes = await response.Content.ReadAsStringAsync();
                    job.ResultContent = Encoding.UTF8.GetBytes(htmlRes);
                }
                return job;
            }
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