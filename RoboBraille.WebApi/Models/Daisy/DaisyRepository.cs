using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace RoboBraille.WebApi.Models
{
    public class DaisyRepository : IRoboBrailleJob<DaisyJob>
    {
        private RoboBrailleDataContext _context;
        private IDaisyRpcCall _daisyCall;

        public DaisyRepository()
        {
            _context = new RoboBrailleDataContext();
            _daisyCall = new DaisyRpcCall();
        }

        public DaisyRepository(RoboBrailleDataContext context, IDaisyRpcCall daisyCall)
        {
            _context = context;
            _daisyCall = daisyCall;
        }
        public System.Threading.Tasks.Task<Guid> SubmitWorkItem(DaisyJob job)
        {
            if (job == null)
                return null;
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
                bool success = true;
                try
                {
                    byte[] res = null;
                    res = _daisyCall.Call(job.FileContent, DaisyOutput.Epub3WMO.Equals(job.DaisyOutput), job.Id.ToString());
                    if (res != null && res.Length>0)
                        job.ResultContent = res;
                    else success = false;

                    string mime = "application/zip";
                    string fileName = job.FileName;
                    string fileExtension = ".zip";
                    if (DaisyOutput.Epub3WMO.Equals(job.DaisyOutput))
                    {
                        mime = "application/epub+zip";
                        fileExtension = ".epub";
                    }
                        if (!success)
                        {
                            RoboBrailleProcessor.SetJobFaulted(job, _context);
                        }
                        else
                        {
                            job.ResultFileExtension = fileExtension;
                            job.ResultMimeType = mime;
                            job.DownloadCounter = 0;
                            job.Status = JobStatus.Done;
                            job.FinishTime = DateTime.Now;
                            _context.Entry(job).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                }
            }, job);

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