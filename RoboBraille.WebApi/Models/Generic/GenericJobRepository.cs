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
    public class GenericJobRepository : IRoboBrailleJob<GenericJob>
    {
        private RoboBrailleDataContext _context;

        public GenericJobRepository()
        {
            _context = new RoboBrailleDataContext();
        }

        public GenericJobRepository(RoboBrailleDataContext context)
        {
            _context = context;
        }

        public async Task<Guid> SubmitWorkItem(GenericJob job)
        {
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
                try
                {
                    //TODO process the job here
                    Task.Delay(10000);

                    job.ResultContent = Encoding.UTF8.GetBytes("This is a sample job that ran for 10 seconds.");
                    job.ResultFileExtension = "txt";
                    job.ResultMimeType = "text/plain";
                    job.DownloadCounter = 0;
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
            //info: add "await task;" here to make it run synced
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

            var job = (GenericJob)_context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
            if (job == null || job.ResultContent == null)
                return null;
            
            FileResult result = null;
            try
            {
                result = new FileResult(job.ResultContent, "text/plain", "SampleResponse.txt");
                RoboBrailleProcessor.UpdateDownloadCounterInDb(job.Id, _context);
            } catch
            {
                //ignored
            }
            return result;
        }
        
        public RoboBrailleDataContext GetDataContext()
        {
            return _context;
        }
    }
}