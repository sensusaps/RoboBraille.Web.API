using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using iTextSharp.text;
using RoboBraille.WebApi.ABBYY;
using System.Collections.Generic;
using System.Text;
using System.Net;
using RoboBraille.WebApi.Models.LanguageTranslation;




namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// Class responsible for reading the content from the POST requests that arrive to the WEB API. Only valid requests pass through.
    /// </summary>
    public class MultipartFormDataFormatter : MediaTypeFormatter
    {
        public MultipartFormDataFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        /// <summary>
        /// Checks that the job type exists
        /// </summary>
        /// <param name="type">The job type</param>
        /// <returns>true if exists, false otherwise</returns>
        public override bool CanReadType(Type type)
        {
            if (type == typeof(AccessibleConversionJob) ||
                type == typeof(HTMLtoPDFJob) ||
                type == typeof(AudioJob) ||
                type == typeof(OcrConversionJob) ||
                type == typeof(EBookJob) ||
                type == typeof(DaisyJob) ||
                type == typeof(MSOfficeJob) ||
                type == typeof(BrailleJob) ||
                type == typeof(HTMLToTextJob) ||
                type == typeof(RoboVideo.VideoJob) ||
                type == typeof(TranslationJob))
            {
                return true;
            }

            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        /// <summary>
        /// Reads the input from the POST requests and creates the appropriate job instance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="readStream"></param>
        /// <param name="content"></param>
        /// <param name="formatterLogger"></param>
        /// <returns></returns>
        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (content == null)
                return null;

            var tempStorage = Path.GetTempPath();
            var tempStorageProvider = new MultipartFormDataStreamProvider(tempStorage);
            var msp = await content.ReadAsMultipartAsync(tempStorageProvider);
            Job job = null;

            if (type == typeof(AccessibleConversionJob))
            {
                job = new AccessibleConversionJob
                {
                    //SourceDocumnetFormat = (SourceFormat)Enum.Parse(typeof(SourceFormat), msp.FormData["sourceformat"]),//Convert.ToInt32(msp.FormData["sourceformat"]),
                    TargetDocumentFormat = (OutputFileFormatEnum)Enum.Parse(typeof(OutputFileFormatEnum), msp.FormData["targetdocumentformat"])//Convert.ToInt32(msp.FormData["targetformat"]),
                };

                if (msp.FormData.AllKeys.Contains("priority"))
                {
                    ((AccessibleConversionJob)job).Priority = (PriorityEnum)Enum.Parse(typeof(PriorityEnum), msp.FormData["priority"]);
                }
                else
                {
                    ((AccessibleConversionJob)job).Priority = PriorityEnum.P_Normal;
                }
            }
            else if (type == typeof(AudioJob))
            {
                job = new AudioJob()
                {
                    AudioLanguage = (Language)Enum.Parse(typeof(Language), msp.FormData["audiolanguage"]),
                    SpeedOptions = (AudioSpeed)Enum.Parse(typeof(AudioSpeed), msp.FormData["speedoptions"]),
                    FormatOptions = (AudioFormat)Enum.Parse(typeof(AudioFormat), msp.FormData["formatoptions"])
                };
                if (msp.FormData.AllKeys.Contains("voicepropriety"))
                {
                    string[] props = msp.FormData["voicepropriety"].Split(':');
                    List<VoicePropriety> propList = new List<VoicePropriety>();
                    foreach (string prop in props)
                    {
                        propList.Add((VoicePropriety)Enum.Parse(typeof(VoicePropriety), prop));
                    }
                    ((AudioJob)job).VoicePropriety = propList;
                }
                else ((AudioJob)job).VoicePropriety = new List<VoicePropriety>() { VoicePropriety.None };
            }
            else if (type == typeof(BrailleJob))
            {
                job = new BrailleJob
                {
                    BrailleFormat = (BrailleFormat)Enum.Parse(typeof(BrailleFormat), msp.FormData["brailleformat"]),
                    BrailleLanguage = (Language)Enum.Parse(typeof(Language), msp.FormData["language"]),
                    Contraction = (BrailleContraction)Enum.Parse(typeof(BrailleContraction), msp.FormData["contraction"]),
                    OutputFormat = (OutputFormat)Enum.Parse(typeof(OutputFormat), msp.FormData["outputformat"]),
                    ConversionPath = (ConversionPath)int.Parse(msp.FormData["conversionpath"]),
                    CharactersPerLine = int.Parse(msp.FormData["charactersperline"]),
                    LinesPerPage = int.Parse(msp.FormData["linesperpage"])
                };

                if (msp.FormData.AllKeys.Contains("pagenumbering"))
                {
                    ((BrailleJob)job).PageNumbering = (PageNumbering)int.Parse(msp.FormData["pagenumbering"]);
                }
                if (msp.FormData.AllKeys.Contains("translationtable"))
                {
                    ((BrailleJob)job).TranslationTable = msp.FormData["translationtable"];
                }
            }
            else if (type == typeof(DaisyJob))
            {
                job = new DaisyJob
                {
                    DaisyOutput = (DaisyOutput)Enum.Parse(typeof(DaisyOutput), msp.FormData["daisyoutput"])
                };
            }
            else if (type == typeof(EBookJob))
            {
                job = new EBookJob
                {
                    EbookFormat = (EbookFormat)Enum.Parse(typeof(EbookFormat), msp.FormData["format"])
                };
            }
            else if (type == typeof(HTMLtoPDFJob))
            {
                job = new HTMLtoPDFJob
                {
                    paperSize = (PaperSize)Enum.Parse(typeof(PaperSize), msp.FormData["size"])
                };
            }
            else if (type == typeof(MSOfficeJob))
            {
                job = new MSOfficeJob
                {
                    MSOfficeOutput = (MSOfficeOutput)Enum.Parse(typeof(MSOfficeOutput), msp.FormData["msofficeoutput"])
                };
            }
            else if (type == typeof(OcrConversionJob))
            {
                job = new OcrConversionJob
                {
                    OcrLanguage = (Language)Enum.Parse(typeof(Language), msp.FormData["language"])
                };
            }
            else if (type == typeof(HTMLToTextJob))
            {
                job = new HTMLToTextJob();
            }
            else if (type == typeof(RoboVideo.VideoJob))
            {
                job = new RoboVideo.VideoJob()
                {
                    SubtitleLangauge = msp.FormData["language"],
                    SubtitleFormat = msp.FormData["format"]
                };
                if (msp.FormData.AllKeys.Contains("videourl"))
                {
                    ((RoboVideo.VideoJob)job).VideoUrl = msp.FormData["videourl"];
                }
            }
            else //if (type == typeof(TranslationJob))
            {
                job = new TranslationJob()
                {
                    SourceLanguage = msp.FormData["sourcelanguage"],
                    TargetLanguage = msp.FormData["targetlanguage"]
                };
            }

            if (job == null)
            {
                Console.WriteLine("cum pula mea");
                return null;
            }
            else
            {
                if (msp.FileData.Count > 0)
                {
                    job.FileContent = File.ReadAllBytes(msp.FileData[0].LocalFileName);
                    string fileName = msp.FileData[0].Headers.ContentDisposition.FileName.Replace("\"", "").ToString();
                    job.FileName = fileName.Substring(0, fileName.LastIndexOf("."));
                    job.FileExtension = fileName.Substring(fileName.LastIndexOf(".") + 1);
                    job.MimeType = msp.FileData[0].Headers.ContentType.MediaType;
                }
                else if (msp.FormData["lastjobid"] != null)
                {
                    //1) check if a guid is present and exists in the database. 2) get the byte content from the database and use it in the new job
                    Guid previousJobId = Guid.Parse(msp.FormData["lastjobid"]);
                    RoboBrailleDataContext _context = new RoboBrailleDataContext();
                    Job previousJob = RoboBrailleProcessor.CheckForJobInDatabase(previousJobId, _context);
                    if (previousJob != null)
                    {
                        job.FileContent = previousJob.ResultContent;
                        job.FileName = previousJob.FileName;
                        job.FileExtension = previousJob.ResultFileExtension;
                        job.MimeType = previousJob.ResultMimeType;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    //if not assume it is a URL
                    if (!String.IsNullOrWhiteSpace(msp.FormData["FileContent"]))
                    {
                        string sorh = msp.FormData["FileContent"];
                        Uri uriResult;
                        bool result = Uri.TryCreate(sorh, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        if (result)
                        {
                            var webRequest = WebRequest.Create(uriResult);

                            using (var response = webRequest.GetResponse())
                            using (var httpcontent = response.GetResponseStream())
                            using (var reader = new StreamReader(httpcontent))
                            {
                                job.FileContent = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                                job.FileName = DateTime.Now.Ticks + "-RoboFile";
                                job.FileExtension = ".html";
                                job.MimeType = "text/html";
                            }
                        }
                        else //else it must be a file
                        {
                            job.FileContent = Encoding.UTF8.GetBytes(sorh);
                            job.FileName = DateTime.Now.Ticks + "-RoboFile";
                            job.FileExtension = ".txt";
                            job.MimeType = "text/plain";
                        }
                    }
                    else
                    {
                        //it's a video job
                    }
                }
                if (type != typeof(RoboVideo.VideoJob))
                {
                    job.InputFileHash = RoboBrailleProcessor.GetInputFileHash(job.FileContent);
                }
                job.Status = JobStatus.Started;
                job.SubmitTime = DateTime.Now;
                return job;
            }
        }
    }
}