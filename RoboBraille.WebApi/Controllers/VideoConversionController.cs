using RoboBraille.WebApi.Models;
using RoboBraille.WebApi.Models.RoboVideo;
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
    public class VideoConversionController : ApiController
    {
        private readonly IRoboBrailleJob<VideoJob> _repository;

        public VideoConversionController()
        {
            _repository = new VideoConversionRepository();
        }

        public VideoConversionController(IRoboBrailleJob<VideoJob> jobRepository)
        {
            _repository = jobRepository;
        }

        [AllowAnonymous]
        [Route("api/videoconversion/getoutputformats")]
        [ResponseType(typeof(string))]

        public IEnumerable<string> GetOutputFormats()
        {
            return new string[] {"srt", "txt", "dfxp", "ssa", "sbv", "vtt"};
        }

        /// <summary>
        /// POST video conversion job
        /// </summary>
        /// <param name="job">Video conversion job object</param>
        /// <returns>GUID jobid</returns>

        [Authorize]
        [Route("api/videoconversion")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(VideoJob job)
        {
            Guid userId = RoboBrailleProcessor.getUserIdFromJob(this.Request.Headers.Authorization.Parameter);
            job.UserId = userId;
            //if (RoboBrailleProcessor.IsSameJobProcessing(job))
            //{
                //var resp = new HttpResponseMessage(HttpStatusCode.Conflict)
                //{
                //    Content = new StringContent(string.Format("The file with the name {0} is already being processed", job.FileName)),
                //    ReasonPhrase = "Job already processing"
                //};
                //throw new HttpResponseException(resp);
            //}
            Guid jobId = await _repository.SubmitWorkItem(job);
            return Ok(jobId.ToString("D"));
        }

        /// <summary>
        /// GET video conversion job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/videoconversion/getstatus")]
        [ResponseType(typeof(string))]
        
        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        /// <summary>
        /// GET video conversion result
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>The file result</returns>
        [HttpGet]
        [Route("api/videoconversion/getresult")]
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
