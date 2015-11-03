﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace RoboBraille.WebApi.Models
{
    public class HTMLtoPDFRepository : IRoboBrailleJob<HTMLtoPDFJob>
    {

        public Task<Guid> SubmitWorkItem(HTMLtoPDFJob job)
        {
            iTextSharp.text.Rectangle pageSize = PageSize.A4;

            if (job == null)
                return null;

            switch (job.size) { 
                case PaperSize.a1:
                    pageSize = PageSize.A1;
                    break;
                case PaperSize.a2:
                    pageSize = PageSize.A2;
                    break;
                case PaperSize.a3:
                    pageSize = PageSize.A3;
                    break;
                case PaperSize.a4:
                    pageSize = PageSize.A4;
                    break;
                case PaperSize.a5:
                    pageSize = PageSize.A5;
                    break;
                case PaperSize.a6:
                    pageSize = PageSize.A6;
                    break;
                case PaperSize.a7:
                    pageSize = PageSize.A7;
                    break;
                case PaperSize.a8:
                    pageSize = PageSize.A8;
                    break;
                case PaperSize.a9:
                    pageSize = PageSize.A9;
                    break;
                case PaperSize.letter:
                    pageSize = PageSize.LETTER;
                    break;
                default:
                    pageSize = PageSize.A4;
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
                HTMLtoPDFJob ebJob = (HTMLtoPDFJob)t;
                string htmlStream = "";
                string outputFileName = Guid.NewGuid().ToString("N") + ".pdf";
                string tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());



                try
                {

                    File.WriteAllBytes(tempfile + ".html", job.FileContent);
                    htmlStream = new StreamReader(tempfile + ".html").ReadToEnd();
                    

                    if (!string.IsNullOrEmpty(htmlStream))
                    {

                        var fsOut = new MemoryStream();
                        var stringReader = new StringReader(htmlStream);
                        Document document = new Document(pageSize, 5, 5, 5, 5);
                            try
                            {
                                //Document setup
                                
                                var pdfWriter = PdfAWriter.GetInstance(document, fsOut);
                                pdfWriter.SetTagged();
                                pdfWriter.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
                                pdfWriter.CreateXmpMetadata();
                                pdfWriter.AddViewerPreference(PdfName.DISPLAYDOCTITLE, PdfBoolean.PDFTRUE);
                                pdfWriter.Info.Put(new PdfName("Producer"), new PdfString("©" + DateTime.Now.Year.ToString() + " Robobraille.org"));


                                //PDF/A-1A Conformance
                                //pdfWriter = PdfAWriter.GetInstance(document, fsOut, PdfAConformanceLevel.PDF_A_1A);
                                

                                document.Open();
                                document.AddCreationDate();


                                //Custom tag processor
                                var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                                tagProcessors.AddProcessor(HTML.Tag.HTML, new HTMLTagProcessor(document));
                                tagProcessors.AddProcessor(HTML.Tag.TITLE, new HTMLTagProcessor(document));
                                tagProcessors.AddProcessor(HTML.Tag.TABLE, new TableTagProcessor());
                                tagProcessors.AddProcessor(HTML.Tag.TH, new THTagProcessor());

                                string colorProfilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/colors/sRGB.profile");
                                string defaultCSSPath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/css/default.css");

                                //Setup color profile
                                ICC_Profile icc = ICC_Profile.GetInstance(new FileStream(colorProfilePath, FileMode.Open, FileAccess.Read));
                                pdfWriter.SetOutputIntents("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", icc);
                                var xmlWorkerHelper = XMLWorkerHelper.GetInstance();
                                var cssResolver = new StyleAttrCSSResolver();
                                cssResolver.AddCssFile(defaultCSSPath, true);// CSS with default style for all converted docs



                                //Register system fonts
                                var xmlWorkerFontProvider = new XMLWorkerFontProvider();
                                Environment.SpecialFolder specialFolder = Environment.SpecialFolder.Fonts;
                                string path = Environment.GetFolderPath(specialFolder);
                                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                                FileInfo[] fontFiles = directoryInfo.GetFiles();
                                foreach (var fontFile in fontFiles)
                                {
                                    if (fontFile.Extension == ".ttf")
                                    {
                                        xmlWorkerFontProvider.Register(fontFile.FullName);
                                    }
                                }


                                //HTML & CSS parsing
                                var cssAppliers = new CssAppliersImpl(xmlWorkerFontProvider);
                                var htmlContext = new HtmlPipelineContext(cssAppliers);
                                htmlContext.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors);
                                PdfWriterPipeline pdfWriterPipeline = new PdfWriterPipeline(document, pdfWriter);
                                HtmlPipeline htmlPipeline = new HtmlPipeline(htmlContext, pdfWriterPipeline);
                                CssResolverPipeline cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                                XMLWorker xmlWorker = new XMLWorker(cssResolverPipeline, true);
                                XMLParser xmlParser = new XMLParser(xmlWorker);
                                xmlParser.Parse(stringReader);
                                document.Close();
                            }
                            catch
                            {
                                //ignore
                            }


                            

                            
  

                            //Data as byte array
                            byte[] fileData = fsOut.ToArray();

                            //For Test. Save file to temp
                            //BinaryWriter Writer = new BinaryWriter(File.OpenWrite(tempfile + ".pdf"));             
                            //Writer.Write(fileData);
                            //Writer.Flush();
                            //Writer.Close();

                            try
                            {
                                using (var context = new RoboBrailleDataContext())
                                {
                                    ebJob.ResultContent = fileData;
                                    ebJob.Status = JobStatus.Done;
                                    context.Jobs.Attach(ebJob);
                                    context.Entry(job).State = EntityState.Modified;
                                    context.SaveChanges();
                                    

                                }
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(ex.Message);
                            }


                        

                    }
                    else
                    {
                        //Error No HTML file to convert
                    }



                }
                catch (Exception e)
                {
                    //ignore
                }
                finally {
                   // File.Delete(tempfile + ".html");
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
                var job = (HTMLtoPDFJob)context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job == null || job.ResultContent == null)
                    return null;

                string mime = "application/pdf";
                
                FileResult result = null;
                try
                {
                    result = new FileResult(job.ResultContent, mime,job.FileName+".pdf");
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