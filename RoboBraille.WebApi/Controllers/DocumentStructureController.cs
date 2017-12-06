using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using RoboBraille.WebApi.Models;
using System.Web.Http.Description;

namespace RoboBraille.WebApi.Controllers
{


    public class DocumentStructureController : ApiController
    {
        private readonly IRoboBrailleJob<DocumentStructureJob> _repository;

        public DocumentStructureController()
        {
            _repository = new DocumentStructureRepository();
        }
        public DocumentStructureController(IRoboBrailleJob<DocumentStructureJob> jobRepository)
        {
            _repository = new DocumentStructureRepository();
        }

        [AllowAnonymous]
        [Route("api/documentstructure/getdocumentelements")]
        [ResponseType(typeof(string))]

        public IEnumerable<string> GetDocumentElements()
        {
            return null;
        }

        [Authorize]
        [Route("api/documentstructure")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(DocumentStructureJob job)
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

        [Route("api/documentstructure/getstatus")]
        [ResponseType(typeof(string))]

        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }
        [HttpGet]
        [Route("api/documentstructure/getresult")]
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
        [Route("api/documentstructure")]
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
