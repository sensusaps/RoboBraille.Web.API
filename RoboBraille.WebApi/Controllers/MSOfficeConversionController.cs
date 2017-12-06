using RoboBraille.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RoboBraille.WebApi.Controllers
{
    //[Authorize]
    public class MSOfficeConversionController : ApiController
    {
        private readonly IRoboBrailleJob<MSOfficeJob> _repository;

        public MSOfficeConversionController()
        {
            _repository = new MSOfficeRepository();
        }

        public MSOfficeConversionController(IRoboBrailleJob<MSOfficeJob> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// POST a new MSOffice Conversion job.
        /// </summary>
        /// <param name="job">The MSOfficeJob class parameter</param>
        /// <returns>A unique job ID</returns>
        [Authorize]
        [Route("api/office")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(MSOfficeJob job)
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
        /// GET MSOfficeConversion job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/office/getstatus")]
        [ResponseType(typeof (string))]
        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        [Route("api/office/getcontainsvideo")]
        [ResponseType(typeof(bool))]
        [ActionName("ContainsVideo")]
        public async Task<IHttpActionResult> PostContainsVideo()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            try
            {
                bool containsVideo = false;
                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                MultipartFileData file = provider.FileData.First();
                string name = file.Headers.ContentDisposition.FileName;
                string path = file.LocalFileName;
                if (name.EndsWith("pptx"))
                {
                    containsVideo = OfficePresentationProcessor.ContainsVideo(path + name);
                }
                else if (name.EndsWith("docx"))
                {
                    OfficeWordProcessor owp = new OfficeWordProcessor();
                    containsVideo = owp.ContainsVideo(path + name);
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.NotImplemented);
                }
                File.Delete(path + name);
                return Ok(containsVideo.ToString());
            }
            catch (System.Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// GET conversion result
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>The file result</returns>
        [Route("api/office/getresult")]
        [ResponseType(typeof (FileResult))]
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
        [Route("api/office")]
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
