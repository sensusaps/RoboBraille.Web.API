using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RoboBraille.WebApi.Models;
using System.Collections.Generic;
using System.Net.Http;


namespace RoboBraille.WebApi.Controllers
{
    public class AccessibleConversionController : ApiController
    {
        private readonly IRoboBrailleJob<AccessibleConversionJob> _repository;

        /// <summary>
        /// Initializes a new repository object.
        /// </summary>
        public AccessibleConversionController()
        {
            _repository = new AccessibleConversionRepository();
        }

        /// <summary>
        /// Used for dependency injection of a job repository for testing
        /// </summary>
        /// <param name="jobRepository"></param>
        public AccessibleConversionController(IRoboBrailleJob<AccessibleConversionJob> jobRepository)
        {
            _repository = jobRepository;
        }

        /// <summary>
        /// Call this method to get a available target formats
        /// </summary>
        /// <returns>A list of targetformats</returns>
        [AllowAnonymous]
        [Route("api/accessibleconversion/getoutputformats")]
        [ResponseType(typeof(string))]

        public IEnumerable<string> GetOutputFormats()
        {
            return AccessibleConversionRepository.GetDestinationFormats();
        }

        /// <summary>
        /// POST accessible conversion job
        /// </summary>
        /// <param name="job">Accessible conversion job object</param>
        /// <returns>GUID jobid</returns>

        [Authorize]
        [Route("api/accessibleconversion")] 
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(AccessibleConversionJob job)
        {
            try
            {
                Guid userId = RoboBrailleProcessor.getUserIdFromJob(this.Request.Headers.Authorization.Parameter);
                job.UserId = userId;
                Guid jobId = await _repository.SubmitWorkItem(job);
                return Ok(jobId.ToString("D"));
            }
            catch (Exception e)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(string.Format("Internal error: {0}", e)),
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(resp);
            }
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

        /// <summary>
        /// Delete a job that you published. You must be the owner of the job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns>The deleted jobid</returns>
        [Authorize]
        [Route("api/accessibleconversion")]
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
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(resp);
            }
        }
    }
}