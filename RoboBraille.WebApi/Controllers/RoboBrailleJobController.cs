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
    //[Authorize]
    public class RoboBrailleJobController : ApiController
    {  
            private readonly IRoboBrailleJobRepository<Job> _repository;

        public RoboBrailleJobController()
        {
            _repository = new RoboBrailleJobRepository();
        }

        public RoboBrailleJobController(IRoboBrailleJobRepository<Job> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET the job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/robobraillejob/getstatus")]
        [ResponseType(typeof (string))]
        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        /// <summary>
        /// Delete a job that you published. You must be the owner of the job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/robobraillejob/delete")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete([FromUri] Guid jobId)
        {
            try
            {
                Guid userId = RoboBrailleProcessor.getUserIdFromJob(this.Request.Headers.Authorization.Parameter);
                jobId = await RoboBrailleProcessor.DeleteJobFromDb(jobId, userId, _repository.GetDataContext());
                return Ok(jobId.ToString("D"));
            }
            catch (Exception e)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(string.Format("Internal error: {0}", e)),
                    ReasonPhrase = "Job already processing or " + e.Message
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// GET conversion result
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>The file result</returns>
        [AllowAnonymous]
        [Route("api/robobraillejob/getresult")]
        [ResponseType(typeof (FileResult))]
        public Task<IHttpActionResult> GetJobResult([FromUri] Guid jobId)
        {
            var fr = _repository.GetResultContents(jobId);
            if (fr == null)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            return Task.FromResult<IHttpActionResult>(fr);
        }

        [AllowAnonymous]
        [Route("api/robobraillejob")]
        public string GetMessage()
        {
            string user = string.IsNullOrWhiteSpace(User.Identity.Name) ? "Anonymous" : User.Identity.Name;
            return string.Concat("RoboBraille Service User", " - ", user);
        }
    }
}
