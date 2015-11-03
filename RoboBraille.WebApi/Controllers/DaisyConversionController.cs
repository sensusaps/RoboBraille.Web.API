using RoboBraille.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RoboBraille.WebApi.Controllers
{
    public class DaisyConversionController : ApiController
    {
         private readonly IRoboBrailleJob<DaisyJob> _repository;

        public DaisyConversionController()
        {
            _repository = new DaisyRepository();
        }

        public DaisyConversionController(IRoboBrailleJob<DaisyJob> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// POST a new DaisyJob.
        /// </summary>
        /// <param name="job">The DaisyJob class parameter</param>
        /// <returns>A unique job ID</returns>
        [Route("api/daisy")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(DaisyJob job)
        {
            Guid jobId = await _repository.SubmitWorkItem(job);
            return Ok(jobId.ToString("D"));
        }

        /// <summary>
        /// GET daisy job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/daisy/getstatus")]
        [ResponseType(typeof(string))]
        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        /// <summary>
        /// GET conversion result
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>The file result</returns>
        [Route("api/daisy/getresult")]
        [ResponseType(typeof(FileResult))]
        public Task<IHttpActionResult> GetJobResult([FromUri] Guid jobId)
        {
            var fr = _repository.GetResultContents(jobId);
            if (fr == null)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            return Task.FromResult<IHttpActionResult>(fr);
        }
    }
}
