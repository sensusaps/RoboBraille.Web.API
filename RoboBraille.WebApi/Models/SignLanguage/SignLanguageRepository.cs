using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace RoboBraille.WebApi.Models
{
    public class SignLanguageRepository : IRoboBrailleJob<SignLanguageJob>
    {
        private RoboBrailleDataContext _context;

        public SignLanguageRepository()
        {
            _context = new RoboBrailleDataContext();
        }

        public SignLanguageRepository(RoboBrailleDataContext context)
        {
            _context = context;
        }

        public async Task<Guid> SubmitWorkItem(SignLanguageJob job)
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
                    /*
                     * TODO process the job here
                     * if at any point it fails change the success flag to false (success=false)
                     */

                    //get text from txt file
                    string textToTranslate = Encoding.UTF8.GetString(job.FileContent);
                    //submit to translation engine/service/library
                    var result = MockTranslate(textToTranslate, job.SourceTextLanguage, job.TargetSignLanguage, job.SignLanguageForm);
                    //required input: source text, source langauge, destination language
                    //get result and save to database as video file

                    job.ResultContent = result;
                    job.DownloadCounter = 0;
                    job.ResultFileExtension = "mp4";
                    job.ResultMimeType = "video/mp4";
                    job.Status = JobStatus.Done;
                    job.FinishTime = DateTime.Now;
                    _context.Entry(job).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                    throw ex;
                }

            }, job);

            return job.Id;
        }

        private byte[] MockTranslate(string textToTranslate, string sourceTextLanguage, string targetSignLanguage, SignLanguageType signType)
        {
            //connect with a library that converts text to sign language video
            return Encoding.UTF8.GetBytes(textToTranslate);
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
            FileResult result = null;
            try
            {
                result = new FileResult(job.ResultContent, job.ResultMimeType, job.FileName + job.ResultFileExtension);
                RoboBrailleProcessor.UpdateDownloadCounterInDb(job.Id, _context);
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