﻿using RoboBraille.WebApi.Models;
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
    public class HTMLtoPDFController : ApiController
    {
        private readonly IRoboBrailleJob<HTMLtoPDFJob> _repository;


        public HTMLtoPDFController()
        {
            _repository = new HTMLtoPDFRepository();
        }

        public HTMLtoPDFController(IRoboBrailleJob<HTMLtoPDFJob> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// POST a new HTMLtoPDFJob.
        /// </summary>
        /// <param name="job">The HTMLtoPDFJob class parameter</param>
        /// <returns>A unique job ID</returns>
        [Route("api/htmltopdf")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(HTMLtoPDFJob job)
        {
            Guid jobId = await _repository.SubmitWorkItem(job);
            return Ok(jobId.ToString("D"));
        }

        /// <summary>
        /// GET HtmlToPDF job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/htmltopdf/getstatus")]
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
        [Route("api/htmltopdf/getresult")]
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