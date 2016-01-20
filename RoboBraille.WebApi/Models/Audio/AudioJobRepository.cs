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
                // TODO : REMOVE and use authenticated user id
                //Guid uid;
                //Guid.TryParse("d2b97532-e8c5-e411-8270-f0def103cfd0", out uid);
                //job.UserId = uid;
                using (var context = new RoboBrailleDataContext())
                {
                    try
                    {
                        context.Jobs.Add(job);
                        context.SaveChanges();
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

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var task = Task.Factory.StartNew(t =>
            {
                AudioJob auJob = (AudioJob)t;
                try
                {
                    //here do the conversion to audio or
                    //send job to rabbitmq cluster
                    byte[] result = AudioJobSender.SendAudioJobToQueue(auJob);

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
                        RoboBrailleProcessor.SetJobFaulted(auJob);
                        return;
                    }
                    else auJob.ResultContent = result;
                    //}
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    RoboBrailleProcessor.SetJobFaulted(auJob);
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
                    using (var context = new RoboBrailleDataContext())
                    {
                        auJob.DownloadCounter = 0;
                        auJob.ResultFileExtension = fileExtension;
                        auJob.ResultMimeType = mime;
                        auJob.FinishTime = DateTime.UtcNow;
                        auJob.Status = JobStatus.Done;
                        context.Jobs.Attach(auJob);
                        context.Entry(auJob).State = EntityState.Modified;
                        context.SaveChanges();
                    }
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
                RoboBrailleProcessor rbp = new RoboBrailleProcessor();
                rbp.UpdateDownloadCounterInDb(job.Id);
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
    }
}