using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// The class representing the result of every conversion
    /// </summary>
    public class FileResult : IHttpActionResult
    {
        private readonly byte[] _fileContents;
        private readonly string _contentType;
        private readonly string _fileName ="RBresult";

        public FileResult(byte[] fileContents, string contentType)
        {
            _fileContents = fileContents;
            _contentType = contentType;
        }

        public FileResult(byte[] fileContents, string contentType, string fileName)
        {
            _fileContents = fileContents;
            _contentType = contentType;
            _fileName = fileName;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    
                    Content = new ByteArrayContent(_fileContents)
                };
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = _fileName
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
                response.Content.Headers.ContentEncoding.Add("UTF-8");
                return response;

            }, cancellationToken);
        }
    }
}