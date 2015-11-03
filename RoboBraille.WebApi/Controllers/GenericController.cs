using RoboBraille.WebApi.Models;
using RoboBraille.WebApi.Models.Sample;
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
    /// <summary>
    /// The following class is only for development purposes do not use in production environment
    /// </summary>
    public class GenericController : ApiController
    {
       private readonly IRoboBrailleJob<GenericJob> _repository;

        public GenericController()
        {
            _repository = new GenericJobRepository();
        }

        public GenericController(IRoboBrailleJob<GenericJob> repository)
        {
            _repository = repository;
        }
        
        /// <summary>
        /// POST the generic job
        /// </summary>
        /// <param name="job">A GenericJob class</param>
        /// <returns>a unique job ID GUID</returns>
        [Route("api/generic")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(GenericJob job)
        {
            Guid jobId = await _repository.SubmitWorkItem(job);
            return Ok(jobId.ToString("D"));
        }

        /// <summary>
        /// GET generic job status (Do not use, only for development purposes)
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/generic/getstatus")]
        [ResponseType(typeof (string))]
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
        [Route("api/generic/getresult")]
        [ResponseType(typeof (FileResult))]
        public Task<IHttpActionResult> GetJobResult([FromUri] Guid jobId)
        {
            var fr = _repository.GetResultContents(jobId);
            if (fr == null)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            return Task.FromResult<IHttpActionResult>(fr);
        }
    }
}
