using RoboBraille.WebApi.Models;
using RoboBraille.WebApi.Models.LanguageTranslation;
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
    public class TranslationController : ApiController
    {
        private readonly IRoboBrailleJob<TranslationJob> _repository;

        public TranslationController()
        {
            _repository = new TranslationRepository();
        }

        public TranslationController(IRoboBrailleJob<TranslationJob> jobRepository)
        {
            _repository = jobRepository;
        }

        [AllowAnonymous]
        [Route("api/translation/getsupportedtranslations")]
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
        [Route("api/translation")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(TranslationJob job)
        {
            Guid userId = RoboBrailleProcessor.getUserIdFromJob(this.Request.Headers.Authorization.Parameter);
            job.UserId = userId;
            if (RoboBrailleProcessor.IsSameJobProcessing(job, _repository.GetDataContext()))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(string.Format("The file with the name {0} is already being processed", job.FileName)),
                    ReasonPhrase = "Job already processing"
                };
                throw new HttpResponseException(resp);
            }
            Guid jobId = await _repository.SubmitWorkItem(job);
            return Ok(jobId.ToString("D"));
        }

        /// <summary>
        /// GET translation job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/translation/getstatus")]
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
        [Route("api/translation/getresult")]
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
        [Route("api/translation")]
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
    }
}
