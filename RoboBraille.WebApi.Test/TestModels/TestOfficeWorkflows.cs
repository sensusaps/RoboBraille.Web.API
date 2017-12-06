using System;
using RoboBraille.WebApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace RoboBraille.WebApi.Test.TestModels
{
    public class TestOfficeWorkflows
    {
        private static readonly string timestamp = DateTime.Now.Ticks.ToString();
        private Dictionary<string, byte[]> inputFiles;

        [OneTimeSetUp]
        public void Initialize()
        {
            inputFiles = IOController.GetInputFiles();
        }

        [Test]
        public void TestOfficeDocxToPdf()
        {
            string output = "pdf";
            string fileName = "A12testWordComplete.docx";
            var apiRes = TestAPI(fileName, output, false,"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeDocx-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeDocxToTxt()
        {
            string output = "txt";
            string fileName = "A12testWordComplete.docx";
            var apiRes = TestAPI(fileName, output, false, "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeDocx-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeDocxToRtf()
        {
            string output = "rtf";
            string fileName = "A12testWordComplete.docx";
            var apiRes = TestAPI(fileName, output, false, "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeDocx-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeDocxToHtml()
        {
            string output = "html";
            string fileName = "A12testWordComplete.docx";
            var apiRes = TestAPI(fileName, output, false, "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeDocx-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeDocToPdf()
        {
            string output = "pdf";
            string filename = "A16.testWordComplete.doc";
            var apiRes = TestAPI(filename, output, false,"doc", "application/msword");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeDoc-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficePptToPdf()
        {
            string output = "pdf";
            string filename = "A7.testPPTX.ppt";
            var apiRes = TestAPI(filename, output, false,"ppt", "application/vnd.ms-powerpoint");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficePpt-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficePptxToPdf()
        {
            string output = "pdf";
            string filename = "A20.testPPTX.pptx";
            var apiRes = TestAPI(filename, output, false,"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficePptx-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficePptxToTxt()
        {
            string output = "txt";
            string filename = "A20.testPPTX.pptx";
            var apiRes = TestAPI(filename, output, false, "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficePptx-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficePptxVideoToTxt()
        {
            string output = "txt";
            string filename = "A21.testVideo.pptx";
            var apiRes = TestAPI(filename, output, true, "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficePptx-" + timestamp + "-" + output + "-API.zip");
        }

        [Test]
        public void TestOfficePptxToHtml()
        {
            string output = "html";
            string filename = "A20.testPPTX.pptx";
            var apiRes = TestAPI(filename, output, false, "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficePptx-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeXlsToPdf()
        {
            string output = "pdf";
            string filename = "A18.RoboBrailleWebApi.TestCases.xls";
            var apiRes = TestAPI(filename, output, false, "xls", "application/vnd.ms-excel");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeXls-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeXlsxToPdf()
        {
            string output = "pdf";
            string filename = "A0-RoboBrailleWebApi.TestCases.xlsx";
            var apiRes = TestAPI(filename, output, false,"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeXlsx-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeRtfToTxt()
        {
            string output = "txt";
            string filename = "A8.testRtf.rtf";
            var apiRes = TestAPI(filename, output, false,"rtf", "application/rtf");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeRtf-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeRtfToPdf()
        {
            string output = "pdf";
            string filename = "A8.testRtf.rtf";
            var apiRes = TestAPI(filename, output, false, "rtf", "application/rtf");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeRtf-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        [Test]
        public void TestOfficeRtfToHtml()
        {
            string output = "html";
            string filename = "A8.testRtf.rtf";
            var apiRes = TestAPI(filename, output, false, "rtf", "application/rtf");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeRtf-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }
        
        /// <summary>
        /// Not supported in robobraille
        /// </summary>
        public void TestOfficeHtmlToRtf()
        {
            string output = "rtf";
            string filename = "A4.testHTMLtotext.html";
            var apiRes = TestAPI(filename, output, false,"html","text/html");

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "MSOfficeHtml-" + timestamp + "-" + output + "-API." + output.Substring(0, 3));
        }

        public byte[] TestAPI(string inputFileName,string destFormat,bool hasVideo,string extension="html",string mime="text/html")
        {
            //setup
            MSOfficeOutput msOutput = (MSOfficeOutput)Enum.Parse(typeof(MSOfficeOutput), destFormat);
            byte[] apiFileContent = inputFiles.Where(x => x.Key.EndsWith(inputFileName)).Select(x => x.Value).First();
            
            MSOfficeJob OfficeJob = new MSOfficeJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = extension,
                FileName = "testOfficeJob",
                MimeType = mime,
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                MSOfficeOutput = msOutput
            };
            if(hasVideo)
            {
                //TODO maybe mock Amara call
                OfficeJob.SubtitleLangauge = "en-US";
                OfficeJob.SubtitleFormat = "srt";
            }

            var apiTask = Task.Run(() => WebAPICall(OfficeJob));
            return apiTask.Result;
        }
        public async Task<byte[]> WebAPICall(MSOfficeJob OfficeJob)
        {
            byte[] byteRes = null;
            MSOfficeRepository repo = new MSOfficeRepository();
            Guid jobID = await repo.SubmitWorkItem(OfficeJob);
            while (repo.GetWorkStatus(jobID) == 2)
            {
                //wait
                await Task.Delay(2000);
            }
            if (repo.GetWorkStatus(jobID) == 1)
            {
                //sucess
                FileResult result = repo.GetResultContents(jobID);
                byteRes = result.getFileContents();
            }
            else
            {
                //fail
                throw new Exception("Task with job ID: " + jobID + " failed");
            }
            return byteRes;
        }
    }
}
