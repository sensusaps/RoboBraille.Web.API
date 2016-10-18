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

namespace RoboBraille.WebApi.Models.Sample
{
    public class GenericJobRepository : IRoboBrailleJob<GenericJob>
    {
        private RoboBrailleDataContext _context;
        public System.Threading.Tasks.Task<Guid> SubmitWorkItem(GenericJob job)
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
                    job.ResultContent = Encoding.UTF8.GetBytes("This is a sample job.");
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

                var job = (GenericJob)context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job == null || job.ResultContent == null)
                    return null;

                FileResult result = null;
                /*
                 * TODO process the final output if necessary
                 * TODO define the best MIME type for your result content
                 * TODO register that the job has been downloaded
                 */
                result = new FileResult(job.ResultContent, "text/plain","SampleResponse.txt");
                return result;
            }
        }


        public RoboBrailleDataContext GetDataContext()
        {
            throw new NotImplementedException();
        }
    }
}