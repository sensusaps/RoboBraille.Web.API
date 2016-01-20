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
        public System.Threading.Tasks.Task<Guid> SubmitWorkItem(DaisyJob job)
        {
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
                    byte[] res = null;
                    using (DaisyRpcCall drp = new DaisyRpcCall())
                    {
                        res = drp.Call(job.FileContent, DaisyOutput.Epub3WMO.Equals(job.DaisyOutput), job.Id.ToString());
                    }
                    //DaisyPipelineConverter dpc = new DaisyPipelineConverter(job.Id.ToString());
                    //res = dpc.ManageDaisyConversion(job.FileContent,DaisyOutput.Epub3WMO.Equals(job.DaisyOutput));
                    if (res != null)
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
                    using (var context = new RoboBrailleDataContext())
                    {
                        if (!success)
                        {
                            job.Status = JobStatus.Error;
                            job.FinishTime = DateTime.UtcNow.Date;
                            context.Entry(job).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            job.ResultFileExtension = fileExtension;
                            job.ResultMimeType = mime;
                            job.DownloadCounter = 0;
                            job.Status = JobStatus.Done;
                            job.FinishTime = DateTime.UtcNow.Date;
                            context.Entry(job).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    RoboBrailleProcessor.SetJobFaulted(job);
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