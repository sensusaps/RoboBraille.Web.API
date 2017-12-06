using AmaraVideoClient;
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

namespace RoboBraille.WebApi.Models
{
    public class AmaraSubtitleRepository : IRoboBrailleJob<AmaraSubtitleJob>
    {
        private RoboBrailleDataContext _context;
        private AmaraSubtitleProcessor vjp = new AmaraSubtitleProcessor();

        public AmaraSubtitleRepository()
        {
            _context = new RoboBrailleDataContext();
        }

        public AmaraSubtitleRepository(RoboBrailleDataContext context)
        {
            _context = context;
        }
        public async System.Threading.Tasks.Task<Guid> SubmitWorkItem(AmaraSubtitleJob job)
        {
            try
            {
                if (job.FileContent != null)
                {
                    job.VideoUrl = vjp.CreateVideoUrl(job);
                }
                job.DownloadCounter = 0;
                job.Status = JobStatus.Started;
                try
                {
                    _context.Jobs.Add(job);
                    _context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                    throw new DbEntityValidationException(errorMessages);
                }
            }
            catch (Exception ex)
            {
                RoboBrailleProcessor.SetJobFaulted(job, _context);
                throw ex;
            }

            var task = Task.Factory.StartNew(t =>
            {
                AmaraSubtitleJob viJob = (AmaraSubtitleJob)t;
                try
                {
                    byte[] result = null;
                    if (!String.IsNullOrWhiteSpace(viJob.VideoUrl))
                    {
                        SubtitleInfo si = vjp.PostVideo(viJob);
                        viJob.AmaraVideoId = si.VideoId;
                        string mime = "text/plain";
                        string fileExtension = ".txt";
                        switch (si.Status)
                        {
                            case VideoSubtitleStatus.SubtitleRequested:
                                result = Encoding.UTF8.GetBytes("Video exists but subtitle does not. Contact amara.org to request a subtitle!" + Environment.NewLine +
                                    "Amara video id: " + viJob.AmaraVideoId + Environment.NewLine +
                                    "RoboBraille job id: " + viJob.Id
                                    );
                                viJob.ResultContent = result;
                                try
                                {
                                    viJob.DownloadCounter = 0;
                                    viJob.ResultFileExtension = fileExtension;
                                    viJob.ResultMimeType = mime;
                                    viJob.FinishTime = DateTime.Now;
                                    viJob.Status = JobStatus.Queued;
                                    _context.Jobs.Attach(viJob);
                                    _context.Entry(viJob).State = EntityState.Modified;
                                    _context.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Trace.WriteLine(ex.Message);
                                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                                    throw ex;
                                }
                                break;
                            case VideoSubtitleStatus.Error:
                                viJob.ResultContent = Encoding.UTF8.GetBytes("Subtitle could not be created. Error!");
                                RoboBrailleProcessor.SetJobFaulted(viJob, _context);
                                break;
                            case VideoSubtitleStatus.Exists:
                                SubtitleDownloadAndSave(viJob);
                                break;
                            case VideoSubtitleStatus.Submitted:
                                result = Encoding.UTF8.GetBytes("Video with id " + si.VideoId + " has been submitted for manual subtitling. Make sure to check when it's ready!");
                                viJob.ResultContent = result;
                                try
                                {
                                    viJob.DownloadCounter = 0;
                                    viJob.ResultFileExtension = fileExtension;
                                    viJob.ResultMimeType = mime;
                                    viJob.FinishTime = DateTime.Now;
                                    viJob.Status = JobStatus.Processing;
                                    _context.Jobs.Attach(viJob);
                                    _context.Entry(viJob).State = EntityState.Modified;
                                    _context.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Trace.WriteLine(ex.Message);
                                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                                    throw ex;
                                }
                                break;

                            case VideoSubtitleStatus.Complete:
                                viJob.ResultContent = Encoding.UTF8.GetBytes("Subtitle is complete.");
                                break; //not handled yet
                            case VideoSubtitleStatus.NotComplete:
                                result = Encoding.UTF8.GetBytes("Video with id " + si.VideoId + " already exists but the subtitle is not complete. Make sure to check when it's complete!");
                                viJob.ResultContent = result;
                                try
                                {
                                    viJob.DownloadCounter = 0;
                                    viJob.ResultFileExtension = fileExtension;
                                    viJob.ResultMimeType = mime;
                                    viJob.FinishTime = DateTime.Now;
                                    viJob.Status = JobStatus.Processing;
                                    _context.Jobs.Attach(viJob);
                                    _context.Entry(viJob).State = EntityState.Modified;
                                    _context.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Trace.WriteLine(ex.Message);
                                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                                    throw ex;
                                }
                                break;
                            default: break;
                        }
                    }
                    else
                    {
                        throw new Exception("No video url");
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    RoboBrailleProcessor.SetJobFaulted(job, _context);
                    throw ex;
                }

            }, job);

            return job.Id;
        }

        public int GetWorkStatus(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var job = (AmaraSubtitleJob)_context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
            if (job.Status != JobStatus.Processing && !String.IsNullOrWhiteSpace(job.AmaraVideoId) && !String.IsNullOrWhiteSpace(job.SubtitleLangauge))
            {
                VideoLanguageDetail vld = vjp.GetVideoInfo(job.AmaraVideoId, job.SubtitleLangauge);
                if (vld != null && vld.AreSubsComplete && vld.SubtitleCount > 0)
                {
                    job.Status = JobStatus.Done;
                    //can begin downloading the new subtitle and saving it to the database in a new task
                    var task = Task.Factory.StartNew(vj =>
                    {
                        SubtitleDownloadAndSave(job);
                    }, job);
                }
            }
            if (job != null)
                return (int)job.Status;
            return (int)JobStatus.Error;
        }

        private bool SubtitleDownloadAndSave(AmaraSubtitleJob viJob)
        {
            string mime = "text/plain";
            string fileExtension = ".txt";
            byte[] result = vjp.DownloadSubtitle(viJob);
            if (result == null)
            {
                RoboBrailleProcessor.SetJobFaulted(viJob, _context);
                return false;
            }
            else viJob.ResultContent = result;
            var fmtOptions = viJob.SubtitleFormat;

            switch (fmtOptions)
            {
                case "srt":
                    mime = "text/srt";
                    fileExtension = ".srt";
                    break;

                case "txt":
                    mime = "text/plain";
                    fileExtension = ".txt";
                    break;

                case "dfxp":
                    mime = "application/ttml+xml";
                    fileExtension = ".dfxp";
                    break;

                case "sbv":
                    mime = "text/sbv";
                    fileExtension = ".sbv";
                    break;
                case "ssa":
                    mime = "text/ssa";
                    fileExtension = ".ssa";
                    break;
                case "vtt":
                    mime = "text/vtt";
                    fileExtension = ".vtt";
                    break;
                default:
                    mime = "text/srt";
                    fileExtension = ".srt";
                    break;
            }
            try
            {
                viJob.DownloadCounter = 0;
                viJob.ResultFileExtension = fileExtension;
                viJob.ResultMimeType = mime;
                viJob.FinishTime = DateTime.Now;
                viJob.Status = JobStatus.Done;
                _context.Jobs.Attach(viJob);
                _context.Entry(viJob).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public FileResult GetResultContents(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                return null;

            var job = _context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
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


        public RoboBrailleDataContext GetDataContext()
        {
            return _context;
        }
    }
}