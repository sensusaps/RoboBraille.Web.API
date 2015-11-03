using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RoboBraille.WebApi.Models;
using System.Speech.Synthesis;
using System.Collections.ObjectModel;

namespace RoboBraille.WebApi.Controllers
{
    //[Authorize]
    public class AudioController : ApiController
    {
        private readonly IRoboBrailleJob<AudioJob> _repository;

        public AudioController()
        {
            _repository = new AudioJobRepository();
        }

        public AudioController(IRoboBrailleJob<AudioJob> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET the installed audio languages
        /// </summary>
        /// <returns>A list of installed languages</returns>
        [AllowAnonymous]
        [Route("api/audio")]
        public IEnumerable<string> GetLangs()
        {
            return new string[] {Enum.GetName(typeof(Language),Language.enUS),Enum.GetName(typeof(Language),Language.enGB) }; //Enum.GetNames(typeof(Language));
        }

        /// <summary>
        /// POST the Audio Job
        /// </summary>
        /// <param name="job">The Audio Job Object</param>
        /// <returns>The Job GUID</returns>
        [Route("api/audio")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(AudioJob job)
        {
            Guid jobId = await _repository.SubmitWorkItem(job);
            return Ok(jobId.ToString("D"));
        }

        /// <summary>
        /// GET audio job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/audio/getstatus")]
        [ResponseType(typeof (string))]
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
        [Route("api/audio/getresult")]
        [ResponseType(typeof (FileResult))]
        public Task<IHttpActionResult> GetJobResult([FromUri] Guid jobId)
        {
            var fr = _repository.GetResultContents(jobId);
            if (fr == null)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            return Task.FromResult<IHttpActionResult>(fr);
        }
    }
}