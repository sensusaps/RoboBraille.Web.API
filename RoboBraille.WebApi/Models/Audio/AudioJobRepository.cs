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

        public AudioJobRepository(RoboBrailleDataContext context,IAudioJobSender auSender)
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
        public async Task<Guid> SubmitWorkItem(AudioJob job)
        {
            if (job == null)
                throw new Exception("audio job is null");

            //VoiceGender gender = VoiceGender.NotSet;
            //CultureInfo ci = CultureInfo.CurrentCulture;
            //EncodingFormat eformat = EncodingFormat.Pcm;
            //string voiceName = String.Empty;
            //VoicePropriety voicePropriety = job.VoicePropriety.First();
            //    switch (job.AudioLanguage)
            //    {
            //        case Language.daDK:
            //            ci = CultureInfo.GetCultureInfo("da-DK");
            //            if (voicePropriety.Equals(VoicePropriety.Male))
            //            {
            //                gender = VoiceGender.Male;
            //                voiceName = "Carsten";
            //            }
            //            else
            //            {
            //                gender = VoiceGender.Female;
            //                if (job.VoicePropriety.Contains(VoicePropriety.Anne))
            //                    voiceName = "Anne";
            //                else
            //                    voiceName = "Sara";
            //            }
            //            break;
            //    case Language.ltLT:
            //        ci = CultureInfo.GetCultureInfo("lt-LT");
            //        if (voicePropriety.Equals(VoicePropriety.Male))
            //        {
            //            gender = VoiceGender.Male;
            //            if (job.VoicePropriety.Contains(VoicePropriety.Older))
            //                voiceName = "Vladas";
            //            else voiceName = "Edvardas";
            //        }
            //        else
            //        {
            //            gender = VoiceGender.Female;
            //            if (job.VoicePropriety.Contains(VoicePropriety.Older))
            //                voiceName = "Regina";
            //            else voiceName = "Aiste";
            //        }
            //        break;
            //    //case Language.arEG:
            //    //    ci = CultureInfo.GetCultureInfo("ar-EG");
            //    //    if (voicePropriety.Equals(VoicePropriety.Male))
            //    //    {
            //    //        gender = VoiceGender.Male;
            //    //        voiceName = "Sakhr Voice One";
            //    //    }
            //    //    else
            //    //    {
            //    //        gender = VoiceGender.Female;
            //    //        voiceName = "Sakhr Voice Two"; //Three, Four, Five, Six
            //    //    }
            //    //    break;
            //    case Language.huHU: ci = CultureInfo.GetCultureInfo("hu-HU");
            //        if (voicePropriety.Equals(VoicePropriety.Male))
            //        {
            //            gender = VoiceGender.Male;
            //            voiceName = "Gabor";
            //        }
            //        else gender = VoiceGender.Female;
            //            voiceName = "Eszter";
            //        break;
            //    case Language.isIS: ci = CultureInfo.GetCultureInfo("is-IS");
            //        if (voicePropriety.Equals(VoicePropriety.Male))
            //        {
            //            gender = VoiceGender.Male;
            //            voiceName = "IVONA 2 Karl";
            //        }
            //        else
            //        {
            //            gender = VoiceGender.Female;
            //            voiceName = "IVONA 2 Dóra"; 
            //        }
            //        break;
            //    case Language.nlNL: ci = CultureInfo.GetCultureInfo("nl-NL");
            //        if (voicePropriety.Equals(VoicePropriety.Male))
            //        {
            //            gender = VoiceGender.Male;
            //            voiceName = "Arthur";
            //        }
            //        else
            //        {
            //            gender = VoiceGender.Female;
            //            voiceName = "Janneke";
            //        };
            //        break;
            //    case Language.enUS: ci = CultureInfo.GetCultureInfo("en-US"); gender = VoiceGender.Female; voiceName = "IVONA 2 Jennifer"; break;
            //    case Language.enGB: ci = CultureInfo.GetCultureInfo("en-GB"); gender = VoiceGender.Female; voiceName = "Kate"; break;
            //    case Language.frFR: ci = CultureInfo.GetCultureInfo("fr-FR"); gender = VoiceGender.Female; voiceName = "ScanSoft Virginie_Full_22kHz"; break;
            //    case Language.deDE: ci = CultureInfo.GetCultureInfo("de-DE"); gender = VoiceGender.Male; voiceName = "Stefan"; break;
            //    case Language.esES: ci = CultureInfo.GetCultureInfo("es-ES"); gender = VoiceGender.Male; voiceName = "Jorge"; break;
            //    case Language.esCO: ci = CultureInfo.GetCultureInfo("es-CO"); gender = VoiceGender.Female; voiceName = "Ximena"; break;
            //    case Language.bgBG: ci = CultureInfo.GetCultureInfo("bg-BG"); gender = VoiceGender.Female; voiceName = "Gergana"; break;
            //    case Language.itIT: ci = CultureInfo.GetCultureInfo("it-IT"); gender = VoiceGender.Female; voiceName = "Paola"; break;
            //    case Language.nbNO: ci = CultureInfo.GetCultureInfo("nb-NO"); break;
            //    case Language.roRO: ci = CultureInfo.GetCultureInfo("ro-RO"); gender = VoiceGender.Female; voiceName = "IVONA 2 Carmen"; break;
            //    case Language.svSE: ci = CultureInfo.GetCultureInfo("sv-SE"); break;
            //    case Language.plPL: ci = CultureInfo.GetCultureInfo("pl-PL"); gender = VoiceGender.Male; voiceName = "Krzysztof"; break;
            //    case Language.ptBR: ci = CultureInfo.GetCultureInfo("pt-BR"); break;
            //    case Language.enAU: ci = CultureInfo.GetCultureInfo("en-AU"); break;
            //    case Language.frCA: ci = CultureInfo.GetCultureInfo("fr-CA"); break;
            //    case Language.ptPT: ci = CultureInfo.GetCultureInfo("pt-PT"); gender = VoiceGender.Female; voiceName = "Amalia"; break;
            //    case Language.klGL: ci = CultureInfo.GetCultureInfo("kl-GL"); gender = VoiceGender.Female; voiceName = "Martha"; break;
            //    case Language.elGR: ci = CultureInfo.GetCultureInfo("el-GR"); gender = VoiceGender.Female; voiceName = "Maria"; break;
            //    case Language.slSI: ci = CultureInfo.GetCultureInfo("sl-SI"); gender = VoiceGender.Male; voiceName = "Matej Govorec"; break;
            //    case Language.jaJP: ci = CultureInfo.GetCultureInfo("ja-JP"); break;
            //    case Language.koKR: ci = CultureInfo.GetCultureInfo("ko-KR"); break;
            //    case Language.zhCN: ci = CultureInfo.GetCultureInfo("zh-CN"); break;
            //    case Language.zhHK: ci = CultureInfo.GetCultureInfo("zh-HK"); break;
            //    case Language.zhTW: ci = CultureInfo.GetCultureInfo("zh-TW"); break;
            //    case Language.fiFI: ci = CultureInfo.GetCultureInfo("fi-FI"); break;
            //    case Language.esMX: ci = CultureInfo.GetCultureInfo("es-MX"); break;
            //    case Language.caES: ci = CultureInfo.GetCultureInfo("ca-ES"); break;
            //    case Language.ruRU: ci = CultureInfo.GetCultureInfo("ru-RU"); gender = VoiceGender.Female; voiceName = "IVONA 2 Tatyana"; break;
            //    default: ci = CultureInfo.GetCultureInfo("en-US"); gender = VoiceGender.Female; voiceName = "IVONA 2 Jennifer"; break;
            //}
            try
            {
                        _context.Jobs.Add(job);
                        _context.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }

            var task = Task.Factory.StartNew(t =>
            {
                AudioJob auJob = (AudioJob)t;
                try
                {
                    //if (isVoiceInstalled(ci, gender, voiceName) && !"Gergana".Equals(voiceName))
                    //{
                    //    string tempfile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                    //    int rate = (int)Enum.Parse(typeof(AudioSpeed), Convert.ToString(auJob.SpeedOptions));

                    //    switch (auJob.FormatOptions)
                    //    {
                    //        case AudioFormat.Mp3:
                    //            tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");
                    //            break;
                    //        case AudioFormat.Wav:
                    //            tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
                    //            break;
                    //        case AudioFormat.Wma:
                    //            tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wma");
                    //            break;
                    //        case AudioFormat.Aac:
                    //            tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".aac");
                    //            break;
                    //    }

                    //    var speech = new SpeechSynthesizer();
                    //    speech.Rate = rate;
                    //    speech.SelectVoice(voiceName);
                    //    if (speech.Voice.Equals(null))
                    //    {
                    //        speech.SelectVoiceByHints(gender, VoiceAge.Adult, 1, ci);
                    //        if (speech.Voice.Equals(null))
                    //        {
                    //            //return a message saying the voice is not installed on the system
                    //            RoboBrailleProcessor.SetJobFaulted(auJob);
                    //            return;
                    //        }
                    //    }
                    //    var safi = new SpeechAudioFormatInfo(eformat, 44100, 16, 2, 44100 * 4, 4, null);
                    //    speech.SetOutputToWaveFile(tempfile, safi);
                    //    Encoding enc = RoboBrailleProcessor.GetEncoding(auJob.FileContent);
                    //    if (enc.Equals(Encoding.ASCII))
                    //        enc = RoboBrailleProcessor.GetEncodingByCountryCode(auJob.AudioLanguage);
                    //    speech.Speak(enc.GetString(auJob.FileContent));
                    //    speech.SetOutputToNull();
                    //    auJob.ResultContent = File.ReadAllBytes(tempfile);
                    //    if (File.Exists(tempfile))
                    //        File.Delete(tempfile);
                    //}
                    //else
                    //{
                    //send job to rabbitmq cluster
                    byte[] result = _auSender.SendAudioJobToQueue(auJob);

                    //get file from \Temp file . this is where the AudioAgent placed the result
                    string outputPath = Encoding.UTF8.GetString(result);

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
                        RoboBrailleProcessor.SetJobFaulted(auJob, _context);
                        return;
                    }
                    else auJob.ResultContent = result;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    RoboBrailleProcessor.SetJobFaulted(auJob, _context);
                    throw ex;
                    //return;
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
                    //using (var context = new RoboBrailleDataContext())
                    //{
                        auJob.DownloadCounter = 0;
                        auJob.ResultFileExtension = fileExtension;
                        auJob.ResultMimeType = mime;
                        auJob.FinishTime = DateTime.Now;
                        auJob.Status = JobStatus.Done;
                        
                        _context.Jobs.Attach(auJob);
                        _context.Entry(auJob).State = EntityState.Modified;
                        _context.SaveChanges();
                    //}
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    throw ex;
                }

            }, job);/*.ContinueWith(t =>
            {

            }, TaskContinuationOptions.OnlyOnFaulted);
            */
            await task;
            return job.Id;
            //return Task.FromResult(job.Id);

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