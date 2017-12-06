using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RoboBraille.WebApi.Models;
using System.Speech.Synthesis;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;

namespace RoboBraille.WebApi.Controllers
{
    /// <summary>
    /// API controller for audio jobs
    /// </summary>
    public class AudioController : ApiController
    {
        private readonly IRoboBrailleJob<AudioJob> _repository;
        /// <summary>
        /// Initializes an AudioRepository
        /// </summary>
        public AudioController()
        {
            _repository = new AudioRepository();
        }

        /// <summary>
        /// Injects an AudioRepository for testing purposes
        /// </summary>
        /// <param name="repository"></param>
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
            //current supported langs are:
            IEnumerable<string> supportedLangs = new List<string>()
            {
                Enum.GetName(typeof(Language),Language.enUS),
                Enum.GetName(typeof(Language),Language.enGB),
                Enum.GetName(typeof(Language),Language.bgBG),
                Enum.GetName(typeof(Language),Language.zhCN),
                Enum.GetName(typeof(Language),Language.zhHK),
                Enum.GetName(typeof(Language),Language.zhTW),
                Enum.GetName(typeof(Language),Language.czCZ),
                Enum.GetName(typeof(Language),Language.daDK),
                Enum.GetName(typeof(Language),Language.nlNL),
                Enum.GetName(typeof(Language),Language.fiFI),
                Enum.GetName(typeof(Language),Language.frFR),
                Enum.GetName(typeof(Language),Language.deDE),
                Enum.GetName(typeof(Language),Language.elGR),
                Enum.GetName(typeof(Language),Language.klGL),
                Enum.GetName(typeof(Language),Language.huHU),
                Enum.GetName(typeof(Language),Language.isIS),
                Enum.GetName(typeof(Language),Language.jaJP),
                Enum.GetName(typeof(Language),Language.koKR),
                Enum.GetName(typeof(Language),Language.itIT),
                Enum.GetName(typeof(Language),Language.ltLT),
                Enum.GetName(typeof(Language),Language.nbNO),
                Enum.GetName(typeof(Language),Language.plPL),
                Enum.GetName(typeof(Language),Language.ptPT),
                Enum.GetName(typeof(Language),Language.roRO),
                Enum.GetName(typeof(Language),Language.ruRU),
                Enum.GetName(typeof(Language),Language.skSK),
                Enum.GetName(typeof(Language),Language.slSI),
                Enum.GetName(typeof(Language),Language.esES),
                Enum.GetName(typeof(Language),Language.esCO),
                Enum.GetName(typeof(Language),Language.svSE),
                Enum.GetName(typeof(Language),Language.cyGB)

            };

            return supportedLangs;
        }

        /// <summary>
        /// POST the Audio Job
        /// </summary>
        /// <param name="job">The Audio Job Object</param>
        /// <returns>The Job GUID</returns>
        [Authorize]
        [Route("api/audio")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(AudioJob job)
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
        /// GET audio job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/audio/getstatus")]
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
        [Route("api/audio/getresult")]
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
        [Route("api/audio")]
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