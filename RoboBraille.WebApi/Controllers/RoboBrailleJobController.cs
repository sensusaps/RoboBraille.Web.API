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
    //[Authorize]
    public class RoboBrailleJobController : ApiController
    {  
            private readonly IRoboBrailleJobRepository<Job> _repository;

        public RoboBrailleJobController()
        {
            _repository = new RoboBrailleJobRepository();
        }

        public RoboBrailleJobController(IRoboBrailleJobRepository<Job> repository)
        {
            _repository = repository;
        }

        [Authorize]
        [Route("api/robobraillejob")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(Job job)
        {
            try
            {
                Guid userId = RoboBrailleProcessor.getUserIdFromJob(this.Request.Headers.Authorization.Parameter);
                job.UserId = userId;
                Guid jobId = Guid.Empty;
                switch (job.GetType().Name)
                {
                    case nameof(AccessibleConversionJob):
                        jobId = await (new AccessibleConversionRepository().SubmitWorkItem((AccessibleConversionJob)job));
                        break;
                    case nameof(AudioJob):
                        jobId = await (new AudioRepository().SubmitWorkItem((AudioJob)job));
                        break;
                    case nameof(BrailleJob):
                        jobId = await (new BrailleRepository().SubmitWorkItem((BrailleJob)job));
                        break;
                    case nameof(DaisyJob):
                        jobId = await (new DaisyRepository().SubmitWorkItem((DaisyJob)job));
                        break;
                    case nameof(DocumentStructureJob):
                        jobId = await (new DocumentStructureRepository().SubmitWorkItem((DocumentStructureJob)job));
                        break;
                    case nameof(EBookJob):
                        jobId = await (new EBookRepository().SubmitWorkItem((EBookJob)job));
                        break;
                    case nameof(HTMLtoPDFJob):
                        jobId = await (new HTMLtoPDFRepository().SubmitWorkItem((HTMLtoPDFJob)job));
                        break;
                    case nameof(HTMLToTextJob):
                        jobId = await (new HTMLToTextRepository().SubmitWorkItem((HTMLToTextJob)job));
                        break;
                    case nameof(MSOfficeJob):
                        jobId = await (new MSOfficeRepository().SubmitWorkItem((MSOfficeJob)job));
                        break;
                    case nameof(OcrConversionJob):
                        jobId = await (new OcrConversionRepository().SubmitWorkItem((OcrConversionJob)job));
                        break;
                    case nameof(SignLanguageJob): 
                        jobId = await (new SignLanguageRepository().SubmitWorkItem((SignLanguageJob)job));
                        break;
                    case nameof(TranslationJob):
                        jobId = await (new TranslationRepository().SubmitWorkItem((TranslationJob)job));
                        break;
                    case nameof(AmaraSubtitleJob):
                        jobId = await (new AmaraSubtitleRepository().SubmitWorkItem((AmaraSubtitleJob)job));
                        break;
                    default: break;
                }
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
        /// GET the job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/robobraillejob/getstatus")]
        [ResponseType(typeof (string))]
        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        /// <summary>
        /// Delete a job that you published. You must be the owner of the job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/robobraillejob/delete")]
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

        /// <summary>
        /// GET conversion result
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>The file result</returns>
        [AllowAnonymous]
        [Route("api/robobraillejob/getresult")]
        [ResponseType(typeof (FileResult))]
        public Task<IHttpActionResult> GetJobResult([FromUri] Guid jobId)
        {
            var fr = _repository.GetResultContents(jobId);
            if (fr == null)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            return Task.FromResult<IHttpActionResult>(fr);
        }

        [AllowAnonymous]
        [Route("api/robobraillejob")]
        public string GetMessage()
        {
            string user = string.IsNullOrWhiteSpace(User.Identity.Name) ? "Anonymous" : User.Identity.Name;
            return string.Concat("RoboBraille Service User", " - ", user);
        }
    }
}
