using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using RoboBraille.WebApi.ABBYY;
using System.Data.Entity.Validation;
using System.Text;

namespace RoboBraille.WebApi.Models
{
    public class AccessibleConversionRepository : IRoboBrailleJob<AccessibleConversionJob>
    {
        public static string[] GetDestinationFormats()
        {
            return Enum.GetNames(typeof(OutputFileFormatEnum));
        }
        public async Task<Guid> SubmitWorkItem(AccessibleConversionJob accessibleJob)
        {
            using (var context = new RoboBrailleDataContext())
            {
                try
                {
                    /*
                    var serviceUser = await context.ServiceUsers.FirstOrDefaultAsync(e => e.UserName.Equals(User.Identity.Name));
                    if (serviceUser != null)
                    {
                        acm.UserId = serviceUser.UserId;
                    }
                    else
                    {
                        throw new HttpResponseException(HttpStatusCode.Forbidden);
                    }
                    */
                    // TODO : REMOVE
                    Guid uid;
                    Guid.TryParse("d2b97532-e8c5-e411-8270-f0def103cfd0", out uid);
                    accessibleJob.UserId = uid;

                    context.Jobs.Add(accessibleJob);
                    context.SaveChanges();
                }
                /*catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    Trace.WriteLine(ex.InnerException);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }*/
                catch (DbEntityValidationException ex)
                {
                    string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                    throw new DbEntityValidationException(errorMessages);
                }
            }

            // send the job to OCR server
            // send the job to OCR server
            var task = Task.Factory.StartNew(j =>
            {
                var job = (AccessibleConversionJob)j;
                RSSoapServiceSoapClient wsClient;
                string flowName;

                string wsUri = WebConfigurationManager.AppSettings["AbbyyOCRServer"];
                string wName = WebConfigurationManager.AppSettings["RBWorkflowName"];

                try
                {
                    wsClient = new RSSoapServiceSoapClient();
                    // enumerate all workflows
                    var workflows = wsClient.GetWorkflows(wsUri);
                    flowName = workflows.FirstOrDefault(e => e.Equals(wName));
                }
                catch (Exception)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(flowName))
                    return;

                var fileContainer = new FileContainer { FileContents = job.FileContent };

                var ticket = wsClient.CreateTicket(wsUri, flowName);
                var infile = new InputFile { FileData = fileContainer };

                var fileformat = (OutputFileFormatEnum)Enum.Parse(typeof(OutputFileFormatEnum), Convert.ToString(job.TargetDocumentFormat));
                //var fileformat = (OutputFileFormatEnum) job.TargetDocumentFormat;

                OutputFormatSettings ofs;

                switch (fileformat)
                {
                    case OutputFileFormatEnum.OFF_PDF:
                        ofs = new PDFExportSettings
                        {
                            PDFExportMode = PDFExportModeEnum.PEM_ImageOnText,
                            PictureResolution = -1,
                            Quality = 70,
                            UseOriginalPaperSize = true,
                            WriteTaggedPdf = true,
                            IsEncryptionRequested = false,
                            FileFormat = OutputFileFormatEnum.OFF_PDF,
                        };
                        break;

                    case OutputFileFormatEnum.OFF_PDFA:
                        ofs = new PDFAExportSettings
                        {
                            Write1ACompliant = true,
                            UseOriginalPaperSize = true,
                            Quality = 70,
                            PictureResolution = -1,
                            PDFExportMode = PDFExportModeEnum.PEM_ImageOnText,
                            FileFormat = OutputFileFormatEnum.OFF_PDFA
                        };
                        break;

                    case OutputFileFormatEnum.OFF_RTF:
                        ofs = new RTFExportSettings
                        {
                            ForceFixedPageSize = false,
                            HighlightErrorsWithBackgroundColor = false,
                            PaperHeight = 16834,
                            PaperWidth = 11909,
                            RTFSynthesisMode = RTFSynthesisModeEnum.RSM_EditableCopy,
                            WritePictures = true,
                            FileFormat = OutputFileFormatEnum.OFF_RTF
                        };
                        break;

                    case OutputFileFormatEnum.OFF_Text:
                        ofs = new TextExportSettings
                        {
                            //
                            EncodingType = TextEncodingTypeEnum.TET_UTF8,
                            FileFormat = OutputFileFormatEnum.OFF_Text
                        };
                        break;

                    case OutputFileFormatEnum.OFF_MSWord:
                        ofs = new MSWordExportSettings
                        {
                            ForceFixedPageSize = false,
                            HighlightErrorsWithBackgroundColor = false,
                            RTFSynthesisMode = RTFSynthesisModeEnum.RSM_EditableCopy,
                            WritePictures = true,
                            PaperHeight = 16834,
                            PaperWidth = 11909,
                            FileFormat = OutputFileFormatEnum.OFF_MSWord
                        };
                        break;

                    case OutputFileFormatEnum.OFF_HTML:
                        ofs = new HTMLExportSettings
                        {
                            EncodingType = TextEncodingTypeEnum.TET_UTF8,
                            HTMLSynthesisMode = HTMLSynthesisModeEnum.HSM_PageLayout,
                            AllowCss = true,
                            CodePage = CodePageEnum.CP_Latin,
                            WritePictures = true,
                            FileFormat = OutputFileFormatEnum.OFF_HTML
                        };
                        break;

                    case OutputFileFormatEnum.OFF_CSV:
                        ofs = new CSVExportSettings
                        {
                            CodePage = CodePageEnum.CP_Latin,
                            EncodingType = TextEncodingTypeEnum.TET_UTF8,
                            IgnoreTextOutsideTables = false,
                            TabSeparator = ",",
                            UsePageBreaks = false,
                            FileFormat = OutputFileFormatEnum.OFF_CSV
                        };
                        break;

                    case OutputFileFormatEnum.OFF_DOCX:
                        ofs = new DOCXExportSettings
                        {
                            ForceFixedPageSize = false,
                            HighlightErrorsWithBackgroundColor = false,
                            PaperHeight = 16834,
                            PaperWidth = 11909,
                            RTFSynthesisMode = RTFSynthesisModeEnum.RSM_EditableCopy,
                            WritePictures = true,
                            FileFormat = OutputFileFormatEnum.OFF_DOCX
                        };
                        break;

                    case OutputFileFormatEnum.OFF_EPUB:
                        ofs = new EpubExportSettings
                        {
                            PictureResolution = -1,
                            Quality = 70,
                            FileFormat = OutputFileFormatEnum.OFF_EPUB
                        };
                        break;

                    case OutputFileFormatEnum.OFF_MSExcel:
                        ofs = new XLExportSettings
                        {
                            ConvertNumericValuesToNumbers = true,
                            IgnoreTextOutsideTables = false,
                            FileFormat = OutputFileFormatEnum.OFF_MSExcel
                        };
                        break;

                    case OutputFileFormatEnum.OFF_XLSX:
                        ofs = new XLSXExportSettings
                        {
                            ConvertNumericValuesToNumbers = true,
                            IgnoreTextOutsideTables = false,
                            FileFormat = OutputFileFormatEnum.OFF_XLSX
                        };
                        break;

                    case OutputFileFormatEnum.OFF_XML:
                        ofs = new XMLExportSettings
                        {
                            PagesPerFile = 512,
                            WriteCharactersFormatting = false,
                            WriteCharAttributes = false,
                            WriteExtendedCharAttributes = false,
                            WriteNonDeskewedCoordinates = false,
                            FileFormat = OutputFileFormatEnum.OFF_XML
                        };
                        break;

                    case OutputFileFormatEnum.OFF_TIFF:
                        ofs = new TiffExportSettings
                        {
                            ColorMode = ImageColorModeEnum.ICM_AsIs,
                            Compression = ImageCompressionTypeEnum.ICT_Jpeg,
                            Resolution = -1,
                            FileFormat = OutputFileFormatEnum.OFF_TIFF
                        };
                        break;

                    case OutputFileFormatEnum.OFF_JPG:
                        ofs = new JpegExportSettings
                        {
                            ColorMode = ImageColorModeEnum.ICM_AsIs,
                            Quality = 70,
                            Resolution = -1,
                            FileFormat = OutputFileFormatEnum.OFF_JPG
                        };
                        break;

                    default:
                        ofs = new TextExportSettings
                        {
                            EncodingType = TextEncodingTypeEnum.TET_UTF8,
                            FileFormat = OutputFileFormatEnum.OFF_Text
                        };
                        break;
                }

                var formatset = new[] { ofs };
                var inputFiles = new[] { infile };

                ticket.ExportParams.Formats = formatset;
                ticket.InputFiles = inputFiles;
                ticket.Priority = job.Priority;

                XmlResult result;
                try
                {
                    result = wsClient.ProcessTicket(wsUri, flowName, ticket);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    SetOCRTaskFaulted(job);
                    return;
                }

                if (result.IsFailed)
                {
                    SetOCRTaskFaulted(job);
                    return;
                }

                byte[] contents = null;
                foreach (var ifile in result.InputFiles)
                {
                    foreach (var odoc in ifile.OutputDocuments)
                    {
                        foreach (var ofile in odoc.Files)
                        {
                            contents = ofile.FileContents;
                        }
                    }
                }

                if (contents == null)
                {
                    SetOCRTaskFaulted(job);
                    return;
                }
                string mime;
                string fileExtension = ".txt";
                switch (fileformat)
                {
                    case OutputFileFormatEnum.OFF_PDF:
                        mime = "application/pdf";
                        fileExtension = ".pdf";
                        break;

                    case OutputFileFormatEnum.OFF_PDFA:
                        mime = "application/pdf";
                        fileExtension = ".pdf";
                        break;

                    case OutputFileFormatEnum.OFF_RTF:
                        mime = "text/rtf";
                        fileExtension = ".rtf";
                        break;

                    case OutputFileFormatEnum.OFF_Text:
                        mime = "text/plain";
                        fileExtension = ".txt";
                        break;

                    case OutputFileFormatEnum.OFF_MSWord:
                        mime = "application/msword";
                        fileExtension = ".doc";
                        break;

                    case OutputFileFormatEnum.OFF_HTML:
                        mime = "text/html";
                        fileExtension = ".html";
                        break;

                    case OutputFileFormatEnum.OFF_CSV:
                        mime = "text/csv";
                        fileExtension = ".csv";
                        break;

                    case OutputFileFormatEnum.OFF_DOCX:
                        mime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        fileExtension = ".docx";
                        break;

                    case OutputFileFormatEnum.OFF_EPUB:
                        mime = "application/epub+zip";
                        fileExtension = ".epub";
                        break;

                    case OutputFileFormatEnum.OFF_MSExcel:
                        mime = "application/vnd.ms-excel";
                        fileExtension = ".xls";
                        break;

                    case OutputFileFormatEnum.OFF_XLSX:
                        mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileExtension = ".xlsx";
                        break;

                    case OutputFileFormatEnum.OFF_XML:
                        mime = "application/xml";
                        fileExtension = ".xml";
                        break;
                    case OutputFileFormatEnum.OFF_JPG:
                        mime = "image/jpeg";
                        fileExtension = ".jpg";
                        break;
                    case OutputFileFormatEnum.OFF_TIFF:
                        mime = "image/tiff";
                        fileExtension = ".tiff";
                        break;

                    default:
                        mime = "text/plain";
                        fileExtension = ".txt";
                        break;
                }

                using (var context = new RoboBrailleDataContext())
                {
                    try
                    {
                        job.DownloadCounter = 0;
                        job.ResultFileExtension = fileExtension;
                        job.ResultMimeType = mime;
                        job.ResultContent = contents;
                        job.Status = JobStatus.Done;
                        context.Jobs.Attach(job);
                        context.Entry(job).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        SetOCRTaskFaulted(job);
                    }
                }

            }, accessibleJob);

            await task;

            // send the job id
            return accessibleJob.Id;
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

        private void SetOCRTaskFaulted(AccessibleConversionJob job)
        {
            using (var context = new RoboBrailleDataContext())
            {
                try
                {
                    job.Status = JobStatus.Error;
                    context.Jobs.Attach(job);
                    context.Entry(job).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}