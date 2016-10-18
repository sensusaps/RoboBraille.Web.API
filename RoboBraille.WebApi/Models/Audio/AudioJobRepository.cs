using System;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Diagnostics;
using System.Data.Entity.Validation;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RoboBraille.WebApi.Models
{
    public class AudioJobRepository : IRoboBrailleJob<AudioJob>
    {

        private RoboBrailleDataContext _context;
        private IAudioJobSender _auSender;

        public AudioJobRepository()
        {
            _context = new RoboBrailleDataContext();
            _auSender = new AudioJobSender();
        }

        public AudioJobRepository(RoboBrailleDataContext context, IAudioJobSender auSender)
        {
            _context = context;
            _auSender = auSender;
        }
        public static List<string> GetInstalledLangs()
        {
            List<string> result = new List<string>();
            var speech = new SpeechSynthesizer();

            var voices = speech.GetInstalledVoices();
            foreach (InstalledVoice vc in voices)
            {
                result.Add(vc.VoiceInfo.Culture.DisplayName);
            }
            return result;
        }
        public Task<Guid> SubmitWorkItem(AudioJob job)
        {
            if (job == null)
                return null;

            //optionally here you can check for voices and convert to audio without the use of rabbitmq, if the voices are installed on the same server

                    try
                    {
                        _context.Jobs.Add(job);
                        _context.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }
                
            
            var task = Task.Factory.StartNew(t =>
            {
                AudioJob auJob = (AudioJob)t;
                try
                {
                    //here do the conversion to audio or
                    //send job to rabbitmq cluster
                    byte[] result = (new AudioJobSender()).SendAudioJobToQueue(auJob);

                    //get file from the source where it was converted.
                    string outputPath = Encoding.UTF8.GetString(result);
                    outputPath = outputPath.Replace(@"C:", @"\\SERVERPATH"); //SERVERPATH is the name of your machine that converts audio on your local network
                    if (File.Exists(outputPath))
                    {
                        result = File.ReadAllBytes(outputPath);
                        File.Delete(outputPath);
                    }
                    else
                    {
                        result = null;
                    }

                    if (result == null)
                    {
                        RoboBrailleProcessor.SetJobFaulted(auJob,_context);
                        return;
                    }
                    else auJob.ResultContent = result;
                    //}
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    RoboBrailleProcessor.SetJobFaulted(auJob,_context);
                    return;
                }

                string mime = "audio/wav";
                string fileExtension = ".wav";
                AudioFormat fmtOptions = auJob.FormatOptions;

                switch (fmtOptions)
                {
                    case AudioFormat.Mp3:
                        mime = "audio/mpeg3";
                        fileExtension = ".mp3";
                        break;

                    case AudioFormat.Wav:
                        mime = "audio/wav";
                        fileExtension = ".wav";
                        break;

                    case AudioFormat.Aac:
                        mime = "audio/aac";
                        fileExtension = ".aac";
                        break;

                    case AudioFormat.Wma:
                        mime = "audio/wma";
                        fileExtension = ".wma";
                        break;

                    default:
                        mime = "audio/wav";
                        fileExtension = ".wav";
                        break;
                }
                try
                {
                        auJob.DownloadCounter = 0;
                        auJob.ResultFileExtension = fileExtension;
                        auJob.ResultMimeType = mime;
                        auJob.FinishTime = DateTime.UtcNow;
                        auJob.Status = JobStatus.Done;
                        _context.Jobs.Attach(auJob);
                        _context.Entry(auJob).State = EntityState.Modified;
                        _context.SaveChanges();
                    
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }

            }, job).ContinueWith(t =>
            {

            }, TaskContinuationOptions.OnlyOnFaulted);

            return Task.FromResult(job.Id);

        }


        public int GetWorkStatus(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            //using (var context = new RoboBrailleDataContext())
            //{
            var job = _context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
            if (job != null)
                return (int)job.Status;
            //}
            return (int)JobStatus.Error;
        }

        public FileResult GetResultContents(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                return null;

            //using (var context = new RoboBrailleDataContext())
            //{
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
            //}
        }

        public RoboBrailleDataContext GetDataContext()
        {
            return _context;
        }
    }
}