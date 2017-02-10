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
    public class HTMLToTextRepository : IRoboBrailleJob<HTMLToTextJob>
    {
        private RoboBrailleDataContext _context;

        public HTMLToTextRepository()
        {
            _context = new RoboBrailleDataContext();
        }

        public HTMLToTextRepository(RoboBrailleDataContext context)
        {
            _context = context;
        }
        public Task<Guid> SubmitWorkItem(HTMLToTextJob job)
        {
            if (job == null)
                return null;

            // TODO : REMOVE and use authenticated user id
            //Guid uid;
            //Guid.TryParse("d2b97532-e8c5-e411-8270-f0def103cfd0", out uid);
            //job.UserId = uid;

            try
            {
                //using (var context = new RoboBrailleDataContext())
                //{
                    _context.Jobs.Add(job);
                    _context.SaveChanges();
                //}
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
                    string mime = "text/plain";
                    string fileExtension = ".txt";
                    string res = HTMLToTextProcessor.StripHTML(Encoding.UTF8.GetString(job.FileContent));
                    job.ResultContent = Encoding.UTF8.GetBytes(res);
                    //using (var context = new RoboBrailleDataContext())
                    //{
                        job.DownloadCounter = 0;
                        job.ResultFileExtension = fileExtension;
                        job.ResultMimeType = mime;
                        job.Status = JobStatus.Done;
                        job.FinishTime = DateTime.Now;
                        _context.Entry(job).State = EntityState.Modified;
                        _context.SaveChanges();
                    //}
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
                        RoboBrailleProcessor.SetJobFaulted(job, _context);
                        //using (var context = new RoboBrailleDataContext())
                        //{
                            //job.Status = JobStatus.Error;
                            //job.FinishTime = DateTime.UtcNow;
                            //_context.Entry(job).State = EntityState.Modified;
                            //_context.SaveChanges();
                        //}
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

            //using (var context = new RoboBrailleDataContext())
            //{
                var job = _context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job != null)
                    return (int)job.Status;
            //}
            return (int)JobStatus.Error;
        }

        public FileResult GetResultContents(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                return null;

            //using (var context = new RoboBrailleDataContext())
            //{
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
            //}
        }


        public RoboBrailleDataContext GetDataContext()
        {
            return _context;
        }
    }
}