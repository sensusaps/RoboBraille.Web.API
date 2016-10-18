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

namespace RoboBraille.WebApi.Models.LanguageTranslation
{
    public class TranslationRepository : IRoboBrailleJob<TranslationJob>
    {
        private RoboBrailleDataContext _context;
        /*
         Investigate on using Google Translate API and make it into a more general mock-up to support any language to language translations
         */
        public System.Threading.Tasks.Task<Guid> SubmitWorkItem(TranslationJob job)
        {
            if (job == null)
                return null;

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
                    /*
                     * TODO process the job here
                     * if at any point it fails change the success flag to false (success=false)
                     */

                    //extract text from document if not already in text form
                    string textToTranslate = Encoding.UTF8.GetString(job.FileContent);
                    //submit to translation engine/service/library
                    string result = MockTranslate(textToTranslate, job.SourceLanguage, job.TargetLanguage);
                    //required input: source text, source langauge, destination language
                    //get result and save to database as simple text

                    job.ResultContent = Encoding.UTF8.GetBytes(result);
                    using (var context = new RoboBrailleDataContext())
                    {
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

        private string MockTranslate(string textToTranslate, string sourceLang, string targetLang)
        {
            //if translation fails, return same text
            string res = textToTranslate;
            //remember to validate with the supported library languages
            //optional: langs can be an enumerator to make validation easier
            //do actual translation, don't worry about exceptions, they are caught above
            return res;
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
        }


        public RoboBrailleDataContext GetDataContext()
        {
            throw new NotImplementedException();
        }
    }
}