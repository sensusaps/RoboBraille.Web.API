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
    public class OcrConversionController : ApiController
    {
            private readonly IRoboBrailleJob<OcrConversionJob> _repository;

        public OcrConversionController()
        {
            _repository = new OcrConversionRepository();
        }

        public OcrConversionController(IRoboBrailleJob<OcrConversionJob> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Supported translation modes according to tessaract ocr 3.02
        /// </summary>
        /// <returns>A list of engine modes names.</returns>
        [AllowAnonymous]
        [Route("api/ocrconversion")]
        public IEnumerable<string> GetLangs()
        {
            return OcrConversionRepository.getOcrLanguages();
        }

        /// <summary>
        /// POST a new OCR Conversion job.
        /// </summary>
        /// <param name="job">The OcrConversionJob parameter</param>
        /// <returns>A unique job ID</returns>
        [Authorize]
        [Route("api/ocrconversion")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(OcrConversionJob job)
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
        /// Get the job status
        /// </summary>
        /// <param name="jobId">The JobId from the POST method.</param>
        /// <returns>An int representing the status code.</returns>
        [Route("api/ocrconversion/getstatus")]
        [ResponseType(typeof(string))]
        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        /// <summary>
        /// Get the result content
        /// </summary>
        /// <param name="jobId">The JobId from the POST method.</param>
        /// <returns>The result as application/text</returns>
        [Route("api/ocrconversion/getresult")]
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
