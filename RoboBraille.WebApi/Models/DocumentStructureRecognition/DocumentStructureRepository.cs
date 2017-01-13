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
        /*
         Investigate on using Google Translate API and make it into a more general mock-up to support any language to language translations
         */
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

       

                    //extract text from document if not already in text form
                    string TextStructureElements = Encoding.UTF8.GetString(job.FileContent);
                    //submit to translation engine/service/library
                    string result = MockStructure(TextStructureElements,job);
                    //required input: source text, source langauge, destination language
                    //get result and save to database as simple text

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

        public string MockStructure(string TextStructureElements, DocumentStructureJob job)
        {

            var Element = job.DocumentElement;
            string result = null;
            string [] processing =TextStructureElements.ToLower().Split(';');
            foreach (string  word in processing)
            {
                switch (Element){
                    case DocumentElement.Title:
                        result += "title ";
                        break;
                    case DocumentElement.Subtitle:
                        result += "subtitle ";
                        break;
                    case DocumentElement.paragraph:
                        result += "paragraph ";
                        break;
                    case DocumentElement.quote:
                        result += "quote ";
                        break;
                    case DocumentElement.Preface:
                        result += "preface ";
                        break;
                    case DocumentElement.Appendix:
                        result += "appendix ";
                        break;
                    case DocumentElement.Bulleted_List:
                        result += "bulletedlist ";
                        break;
                    case DocumentElement.Numbered_List:
                        result += "numberedlist ";
                        break;
                    case DocumentElement.Email:
                        result += "email ";
                        break;
                    case DocumentElement.Commentary:
                        result += "commentary ";
                        break;
                    case DocumentElement.Link:
                        result += "link ";
                        break;
                    case DocumentElement.Reference:
                        result += "reference ";
                        break;
                    case DocumentElement.TableOfContent:
                        result += "tableofcontent ";
                        break;
                    case DocumentElement.Bookmark:
                        result += "bookmark ";
                        break;
                    case DocumentElement.Footer:
                        result += "footer ";
                        break;
                    case DocumentElement.Header:
                        result += "header ";
                        break;
                    case DocumentElement.Page_Number:
                        result += "pagenumber ";
                        break;
                    case DocumentElement.Column:
                        result += "column ";
                        break;
                    case DocumentElement.Symbol:
                        result += "symbol ";
                        break;
                    case DocumentElement.Object:
                        result += "object ";
                        break;
                    case DocumentElement.Image:
                        result += "image ";
                        break;
                    case DocumentElement.Video:
                        result += "video ";
                        break;
                    case DocumentElement.Presentation:
                        result += "presentation ";
                        break;
                    case DocumentElement.PowerPoint:
                        result += "powerpoint ";
                        break;
                    case DocumentElement.Calendar:
                        result += "calendar ";
                        break;
                    case DocumentElement.Figures:
                        result += "figures ";
                        break;
                    case DocumentElement.SmartArt:
                        result += "smartart ";
                        break;
                    case DocumentElement.Diagram:
                        result += "diagram ";
                        break;
                    case DocumentElement.Equation:
                        result += "eguation ";
                        break;
                    case DocumentElement.Spreadsheet:
                        result += "spreadsheet ";
                        break;
                    case DocumentElement.Table:
                        result += "table ";
                        break;
                    default:
                        result += "Unknown ";
                        break;

                }
          
            }
            return result;

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