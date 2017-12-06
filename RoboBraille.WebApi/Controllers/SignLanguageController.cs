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
    public class SignLanguageController : ApiController
    {
        private readonly IRoboBrailleJob<SignLanguageJob> _repository;

        public SignLanguageController()
        {
            _repository = new SignLanguageRepository();
        }

        public SignLanguageController(IRoboBrailleJob<SignLanguageJob> jobRepository)
        {
            _repository = jobRepository;
        }

        [AllowAnonymous]
        [Route("api/signlanguage/getsupportedtranslations")]
        [ResponseType(typeof(string))]

        public IEnumerable<string> GetSupportedTranslations()
        {
            return null;
        }

        /// <summary>
        /// POST translation job
        /// </summary>
        /// <param name="job">TranslationJob job object</param>
        /// <returns>GUID jobid</returns>

        [Authorize]
        [Route("api/signlanguage")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(SignLanguageJob job)
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
        /// GET translation job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/signlanguage/getstatus")]
        [ResponseType(typeof(string))]

        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        /// <summary>
        /// GET translation result
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>The file result</returns>
        [HttpGet]
        [Route("api/signlanguage/getresult")]
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
        /// <returns></returns>
        [Authorize]
        [Route("api/signlanguage")]
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