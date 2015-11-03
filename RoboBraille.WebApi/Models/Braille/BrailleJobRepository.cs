using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using RoboBraille.WebApi.Models;
using System.Text;
using System.Diagnostics;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
//using Sensus.Braille;
using System.Configuration;



namespace RoboBraille.WebApi.Models
{
    public class BrailleJobRepository : IRoboBrailleJob<BrailleJob>
    {
        public Task<Guid> SubmitWorkItem(BrailleJob job)
        {
            if (job == null)
                return null;

            // TODO : REMOVE and use authenticated user id
            Guid uid;
            Guid.TryParse("d2b97532-e8c5-e411-8270-f0def103cfd0", out uid);
            job.UserId = uid;

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
                    string strBraille = null;
                    if (!string.IsNullOrWhiteSpace(job.TranslationTable))
                    {
                        strBraille = this.LouisBrailleConvert(job);
                    }
                    else
                        switch (job.BrailleLanguage)
                        {
                            case Language.daDK:
                            case Language.enUS:
                            case Language.enGB:
                            case Language.nnNO:
                            case Language.isIS:
                            case Language.svSE:
                                //strBraille = this.SensusBrailleConvert(job);
                                //break;
                            default:
                                strBraille = this.LouisBrailleConvert(job);
                                break;
                        }

                    if (strBraille != null)
                    {
                        strBraille = RoboBrailleTextProcessor.FormatBraille(strBraille, job);
                        job.ResultContent = Encoding.UTF8.GetBytes(strBraille);
                    }
                    else
                    {
                        this.SetBrailleTaskFaulted(job);
                    }
                    using (var context = new RoboBrailleDataContext())
                    {
                        job.Status = JobStatus.Done;
                        job.FinishTime = DateTime.UtcNow.Date;
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
                            job.FinishTime = DateTime.UtcNow.Date;
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

        private string DecodeBrailleString(BrailleJob job)
        {
            Encoding enc = RoboBrailleTextProcessor.GetEncoding(job.FileContent);
            if (enc.Equals(Encoding.ASCII))
                enc = RoboBrailleTextProcessor.GetEncodingByCountryCode(job.BrailleLanguage);
           return enc.GetString(job.FileContent);
        }

        private string LouisBrailleConvert(BrailleJob job)
        {
            //int[] translationModeValues = { 1, 2, 4, 8, 16, 32, 64, 128 };
            int mode = 0;
            if (job.BrailleFormat.Equals(BrailleFormat.eightdot))
                mode = mode + 8;
            if (job.Contraction.Equals(BrailleContraction.grade1))
                mode = mode + 1;

            string str = DecodeBrailleString(job);
            String result = null;
            using (LouisFacade louFacade = new LouisFacade())
            {
                string louTable = null;
                if (string.IsNullOrWhiteSpace(job.TranslationTable))
                    louTable = louFacade.getTranslationTable(job.BrailleLanguage, job.Contraction,job.BrailleFormat);
                else louTable = job.TranslationTable;
                if (job.ConversionPath.Equals(ConversionPath.brailletotext))
                {
                    result = louFacade.BackTranslateString(str, louTable, mode);
                }
                else
                {
                    result = louFacade.TranslateString(str, louTable, mode);
                }
            }
            return result;
        }
        //Uncomment this part for implementing Sensus Braille conversions with SB4
        /*
        private string SensusBrailleConvert(BrailleJob job)
        {
            string result = null;
            try
            {
                string holder = Environment.CurrentDirectory;
                Environment.CurrentDirectory = ConfigurationManager.AppSettings.Get("bindirectory");
                SensusWrapper contractor = new SensusWrapper(job.Language, job.Contraction, job.BrailleFormat);
                string str = DecodeBrailleString(job);
                result = contractor.ConvertText(str);
                Environment.CurrentDirectory = holder;
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
        */
        private void SetBrailleTaskFaulted(BrailleJob job)
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
        public FileResult GetResultContents(Guid jobId)
        {
            if (jobId.Equals(Guid.Empty))
                return null;

            using (var context = new RoboBrailleDataContext())
            {

                var job = (BrailleJob)context.Jobs.FirstOrDefault(e => jobId.Equals(e.Id));
                if (job == null || job.ResultContent == null)
                    return null;

                FileResult result = null;
                var outputFormat = (OutputFormat)job.OutputFormat;

                switch (outputFormat)
                {
                    case OutputFormat.Pef:
                        result = new FileResult(job.ResultContent, "application/x-pef",job.FileName+".pef");
                        break;
                    default:
                        result = new FileResult(job.ResultContent, "text/plain",job.FileName+".txt");
                        break;

                }
                return result;
            }
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
    }
}