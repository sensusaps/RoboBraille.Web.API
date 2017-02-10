using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using iTextSharp.text;

namespace RoboBraille.WebApi.Models.DocumentStructureRecognition
{
    public class DocumentStructurerepository : IRoboBrailleJob<DocumentStructureJob>
    {
        private RoboBrailleDataContext _context;
        public System.Threading.Tasks.Task<Guid> SubmitWorkItem(DocumentStructureJob job)
        {
            if (job == null)
                return null;

            try
            {
                using (var context = new RoboBrailleDataContext())
                {
                    context.Jobs.Add(job);
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }

            var task = Task.Factory.StartNew(t =>
            {
                bool success = true;
                try
                {
                    /*
                     * TODO process the job here
                     * if at any point it fails change the success flag to false (success=false)
                     */

                    //get the document
                    byte[] sourceDocument = job.FileContent;

                    //submit to structure recognition engine/service/library
                    Tuple<DocumentElement, string>[] elements = MockRecognizeStructure(sourceDocument);

                    //get result and save to database
                    string result = null;
                    foreach(Tuple<DocumentElement,string> tuple in elements) {
                        result += tuple.Item1.ToString() + " = " + tuple.Item2 + Environment.NewLine;
                    }
                    job.ResultContent = Encoding.UTF8.GetBytes(result);
                    using (var context = new RoboBrailleDataContext())
                    {
                        job.Status = JobStatus.Done;
                        job.FinishTime = DateTime.Now;
                        context.Entry(job).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    Trace.WriteLine(ex.Message);
                }

                if (!success)
                {
                    try
                    {
                        using (var context = new RoboBrailleDataContext())
                        {
                            job.Status = JobStatus.Error;
                            job.FinishTime = DateTime.UtcNow;
                            context.Entry(job).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }

            }, job);

            return Task.FromResult(job.Id);
        }

        /// <summary>
        /// A mock example of how a structure recognition job might be processed. SplitDocumentInParts and IsAccurateMatch could be combined to work in one single method.
        /// </summary>
        /// <param name="scannedDocument"></param>
        /// <returns></returns>
        public Tuple<DocumentElement,string>[] MockRecognizeStructure(byte[] scannedDocument)
        {
            Dictionary<int, string> parts = SplitDocumentInParts(scannedDocument);
            Tuple<DocumentElement, string>[] result = new Tuple<DocumentElement, string>[parts.Count];
            foreach (KeyValuePair<int, string> part in parts)
            {
                foreach (DocumentElement de in Enum.GetValues(typeof(DocumentElement)))
                {
                    if (IsAccurateMatch(part, de))
                    {
                        result[part.Key] = new Tuple<DocumentElement, string>(de, part.Value);
                    }
                }
            }
            return result.ToArray();

        }

        /// <summary>
        /// Use a solution to determine with some degree of accuract if the selected part is that type of document element.
        /// This is an unkown factor of the conversion
        /// </summary>
        /// <param name="part"></param>
        /// <param name="de"></param>
        /// <returns></returns>
        private bool IsAccurateMatch(KeyValuePair<int, string> part, DocumentElement de)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use a solution to determine the individual parts of the document, based on some sort of classification.
        /// This is an unkown factor of the conversion
        /// </summary>
        /// <param name="scannedDocument"></param>
        /// <returns></returns>
        private Dictionary<int, string> SplitDocumentInParts(byte[] scannedDocument)
        {
            throw new NotImplementedException();
        }


        public int GetWorkStatus(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            using (var context = new RoboBrailleDataContext())
            {
                var job = context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job != null)
                    return (int)job.Status;
            }
            return (int)JobStatus.Error;
        }
        public FileResult GetResultContents(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                return null;

            using (var context = new RoboBrailleDataContext())
            {
                var job = context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job == null || job.ResultContent == null)
                    return null;
                RoboBrailleProcessor.UpdateDownloadCounterInDb(job.Id, _context);
                FileResult result = null;
                try
                {
                    result = new FileResult(job.ResultContent, job.ResultMimeType, job.FileName + job.ResultFileExtension);
                }
                catch (Exception)
                {
                    // ignored
                }
                return result;
            }
        }


        public RoboBrailleDataContext GetDataContext()
        {
            throw new NotImplementedException();
        }
    }
}