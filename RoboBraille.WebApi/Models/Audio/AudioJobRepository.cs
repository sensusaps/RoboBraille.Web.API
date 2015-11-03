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
            foreach (InstalledVoice vc in voices) {
                result.Add(vc.VoiceInfo.Culture.DisplayName);
            }
            return result;
        } 
        public Task<Guid> SubmitWorkItem(AudioJob job)
        {
            if (job == null)
                return null;

            VoiceGender gender = VoiceGender.NotSet;
            CultureInfo ci = CultureInfo.CurrentCulture;
            EncodingFormat eformat = EncodingFormat.Pcm;
                switch (job.AudioLanguage)
                {
                   /* case AudioLanguage.DanishSara:
                        gender = VoiceGender.Female;
                        ci = CultureInfo.GetCultureInfo("da-DK");
                        break;

                    case AudioLanguage.DanishAnne:
                        gender = VoiceGender.Female;
                        ci = CultureInfo.GetCultureInfo("da-DK");
                        break;

                    case AudioLanguage.DanishCarsten:
                        gender = VoiceGender.Female;
                        ci = CultureInfo.GetCultureInfo("da-DK");
                        break;*/

                    case Language.enGB:
                        gender = VoiceGender.Female;
                        ci = CultureInfo.GetCultureInfo("en-GB");
                        break;

                    case Language.enUS:
                        gender = VoiceGender.Female;
                        ci = CultureInfo.GetCultureInfo("en-US");
                        break;

                    default:
                        gender = VoiceGender.Female;
                        ci = CultureInfo.GetCultureInfo("en-US");
                        break;
                }
            try 
            {
                // TODO : REMOVE and use authenticated user id
                Guid uid;
                Guid.TryParse("d2b97532-e8c5-e411-8270-f0def103cfd0", out uid);
                job.UserId = uid;

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
                string tempfile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                int rate = (int)Enum.Parse(typeof(AudioSpeed), Convert.ToString(auJob.SpeedOptions));

                switch (auJob.FormatOptions) { 
                    case AudioFormat.Mp3:
                        tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");
                        break;
                    case AudioFormat.Wav:
                        tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
                        break;
                    case AudioFormat.Wma:
                        tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wma");
                        break;
                    case AudioFormat.Aac: 
                        tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".aac");
                        break;
                }

                 

                try
                {
                    var speech = new SpeechSynthesizer();
                    speech.Rate = rate;
                    speech.SelectVoiceByHints(gender, VoiceAge.Adult, 1, ci);

                    var voices = speech.GetInstalledVoices();
                    //Console.WriteLine("Installed voices: "+voices.Count);
                    var safi = new SpeechAudioFormatInfo(eformat, 44100, 16, 2, 44100 * 4, 4, null);
                    speech.SetOutputToWaveFile(tempfile, safi);
                    Encoding enc = RoboBrailleTextProcessor.GetEncoding(job.FileContent);
                    if (enc.Equals(Encoding.ASCII))
                        enc = RoboBrailleTextProcessor.GetEncodingByCountryCode(job.AudioLanguage);
                    speech.Speak(Encoding.UTF8.GetString(job.FileContent));
                    speech.SetOutputToNull();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }

                try
                {
                    using (var context = new RoboBrailleDataContext())
                    {
                        auJob.ResultContent = File.ReadAllBytes(tempfile);
                        auJob.Status = JobStatus.Done;
                        context.Jobs.Attach(auJob);
                        context.Entry(job).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }

                //MediaFoundationApi.Startup();
                //MediaType mediaType = null;
                //byte[] audioBuffer = File.ReadAllBytes(tempfile);

                //switch (auJob.FormatOptions)
                //{
                //    case AudioFormatOptions.Mp3:
                //        using (var reader = new MediaFoundationReader(tempfile))
                //        {
                //            var tempMp3File = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");
                //            MediaFoundationEncoder.EncodeToMp3(reader, tempMp3File);
                //            audioBuffer = File.ReadAllBytes(tempMp3File);
                //        }
                //        /*
                //        mediaType = MediaFoundationEncoder.SelectMediaType(AudioSubtypes.MFAudioFormat_MP3,
                //                                                           new Mp3WaveFormat(44100, 2, 0, 128), 
                //                                                           44100);
                //        if (mediaType != null)
                //        {
                //            var wavfmt = new WaveFormat(44100, 16, 2);
                //            using (var mp3Stream = new MemoryStream())
                //            using (var wavStream = new MemoryStream(auJob.FileContent))
                //            using (var wavReader = new RawSourceWaveStream(wavStream, wavfmt))
                //            using (var mp3Writer = new LameMP3FileWriter(mp3Stream, 
                //                                                         wavReader.WaveFormat,      
                //                                                         LAMEPreset.MEDIUM))
                //            {
                //                wavReader.CopyTo(mp3Writer);
                //                audioBuffer = mp3Stream.GetBuffer();
                //            }
                //        }
                //        */
                //        break;

                //    case AudioFormatOptions.Aac:
                //        using (var reader = new MediaFoundationReader(tempfile))
                //        {
                //            var tempAacfile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                //            MediaFoundationEncoder.EncodeToAac(reader, tempAacfile);
                //            audioBuffer = File.ReadAllBytes(tempAacfile);
                //        }
                //        break;

                //    case AudioFormatOptions.Wma:
                //        using (var reader = new MediaFoundationReader(tempfile))
                //        {
                //            var tempWmaFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                //            MediaFoundationEncoder.EncodeToWma(reader, tempWmaFile);
                //            audioBuffer = File.ReadAllBytes(tempWmaFile);
                //        }
                //        break;
                //}

                //if (audioBuffer != null)
                //{
                //    auJob.ResultContent = audioBuffer;
                //    auJob.Status = JobStatus.Done;
                //    auJob.FinishTime = DateTime.UtcNow.Date;

                //    try
                //    {
                //        using (var context = new RoboBrailleDataContext())
                //        {
                //            context.Jobs.Attach(auJob);
                //            context.Entry(auJob).State = EntityState.Modified;
                //            context.SaveChanges();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.WriteLine(ex.Message);
                //    }
                //}

                //MediaFoundationApi.Shutdown();

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
                var job = (AudioJob)context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job == null || job.ResultContent == null)
                    return null;

                string mime = "audio/wav";
                string fileName = job.FileName;
                AudioFormat fmtOptions =(AudioFormat)Enum.Parse(typeof (AudioFormat), Convert.ToString(job.FormatOptions));

                switch (fmtOptions)
                {
                    case AudioFormat.Mp3:
                        mime = "audio/mpeg3";
                        fileName += ".mp3";
                        break;

                    case AudioFormat.Wav:
                        mime = "audio/wav";
                        fileName += ".wav";
                        break;

                    case AudioFormat.Aac:
                        mime = "audio/mp4";
                        fileName += ".mp4";
                        break;

                    case AudioFormat.Wma:
                        mime = "audio/wma";
                        fileName += ".wma";
                        break;

                    default:
                        mime = "audio/wav";
                        fileName += ".wav";
                        break;
                }

                FileResult result = null;
                try
                {
                    result = new FileResult(job.ResultContent, mime, fileName);
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