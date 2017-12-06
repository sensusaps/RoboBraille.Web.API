using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace RoboBraille.WebApi.Models
{
    public class RoboBrailleJobRepository : IRoboBrailleJobRepository<Job>
    {
        private RoboBrailleDataContext _context;

        public RoboBrailleJobRepository()
        {
            _context = new RoboBrailleDataContext();
        }

        public RoboBrailleJobRepository(RoboBrailleDataContext context)
        {
            _context = context;
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