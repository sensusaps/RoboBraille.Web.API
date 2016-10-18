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
    public class BrailleController : ApiController
    {
      private readonly IRoboBrailleJob<BrailleJob> _repository;

        public BrailleController()
        {
            _repository = new BrailleJobRepository();
        }

        public BrailleController(IRoboBrailleJob<BrailleJob> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get a list of translation tables used to brute force the web api to use liblouis
        /// </summary>
        /// <returns>A list of translation tables.</returns>
        [AllowAnonymous]
        [Route("api/braille/gettranslationtables")]
        public IEnumerable<string> GetTranslationTables()
        {
            return LouisFacade.GetTranslationTables();
        }

        [AllowAnonymous]
        [Route("api/braille/getcontractions")]
        public IEnumerable<string> GetContractions()
        {
            return Enum.GetNames(typeof(BrailleContraction));
        }

        /// <summary>
        /// POST a new BrailleJob.
        /// </summary>
        /// <param name="job">The BrailleJob class parameter</param>
        /// <returns>A unique job ID</returns>
        [Authorize]
        [Route("api/braille")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(BrailleJob job)
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
        /// GET braille job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/braille/getstatus")]
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
        [Route("api/braille/getresult")]
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