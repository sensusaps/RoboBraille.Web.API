using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RoboBraille.WebApi.Models;
using System.Collections.Generic;


namespace RoboBraille.WebApi.Controllers
{
    //[Authorize]
    public class AccessibleConversionController : ApiController
    {
        private readonly IRoboBrailleJob<AccessibleConversionJob> _repository;

        public AccessibleConversionController()
        {
            _repository = new AccessibleConversionRepository();
        }

        public AccessibleConversionController(IRoboBrailleJob<AccessibleConversionJob> jobRepository)
        {
            _repository = jobRepository;
        }

        [AllowAnonymous]
        [Route("api/accessibleconversion/getoutputformats")]
        [ResponseType(typeof(string))]

        public IEnumerable<string> GetOutputFormats()
        {
            return AccessibleConversionRepository.GetDestinationFormats();
        }

        [AllowAnonymous]
        [Route("api/accessibleconversion")]
        public string GetMessage()
        {
            string user = string.IsNullOrWhiteSpace(User.Identity.Name) ? "Anonymous" : User.Identity.Name;
            return string.Concat("RoboBraille Service User", " - ", user);
        }

        /// <summary>
        /// POST accessible conversion job
        /// </summary>
        /// <param name="job">Accessible conversion job object</param>
        /// <returns>GUID jobid</returns>
        
        [Route("api/accessibleconversion")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(AccessibleConversionJob job)
        {
            Guid jobId = await _repository.SubmitWorkItem(job);
            return Ok(jobId.ToString("D"));
        }

        /// <summary>
        /// GET accessible conversion job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/accessibleconversion/getstatus")]
        [ResponseType(typeof(string))]
        
        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        /// <summary>
        /// GET accessible conversion result
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>The file result</returns>
        [HttpGet]
        [Route("api/accessibleconversion/getresult")]
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