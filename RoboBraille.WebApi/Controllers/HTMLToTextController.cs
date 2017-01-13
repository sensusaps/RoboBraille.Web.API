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
    [Authorize]
    public class HTMLToTextController : ApiController
    {
            private readonly IRoboBrailleJob<HTMLToTextJob> _repository;


        public HTMLToTextController()
        {
            _repository = new HTMLToTextRepository();
        }

        public HTMLToTextController(IRoboBrailleJob<HTMLToTextJob> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// POST a new HTMLToTextJob.
        /// </summary>
        /// <param name="job">The HTMLToTextJob class parameter</param>
        /// <returns>A unique job ID</returns>
        [Authorize]
        [Route("api/htmltotext")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(HTMLToTextJob job)
        {
            try
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
        /// GET HtmlToText job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/htmltotext/getstatus")]
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
        [Route("api/htmltotext/getresult")]
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
