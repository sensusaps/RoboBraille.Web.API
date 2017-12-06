using System;
using RoboBraille.WebApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace RoboBraille.WebApi.Test
{
    public class TestHtmlToPdfWorkflow
    {
        private static readonly string timestamp = DateTime.Now.Ticks.ToString();
        private Dictionary<string, byte[]> inputFiles;


        [OneTimeSetUp]
        public void Initialize()
        {
            inputFiles = IOController.GetInputFiles();
        }
        
        /// <summary>
        /// Does not exist as an option it works only with tables
        /// </summary>
        public void TestHtmlToPdfFull()
        {
            //arrange
            string format = "pdf";
            string fileName = "A4.testHTMLtotext.html";

            //act
            var apiRes = TestAPI(fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "HtmlToPdfFull-" + timestamp + "-API." + format);
        }

        [Test]
        public void TestHtmlToPdfTable()
        {
            //arrange
            string format = "pdf";
            string fileName = "A3.testHTMLtable.html";

            //act
            var apiRes = TestAPI(fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "HtmlToPdfTable-" + timestamp + "-API." + format);
        }

        public byte[] TestAPI(string inputFileName)
        {
            //setup
            byte[] apiFileContent = inputFiles.Where(x => x.Key.EndsWith(inputFileName)).Select(x => x.Value).First();

            HTMLtoPDFJob htmlToPdfJob = new HTMLtoPDFJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "html",
                FileName = "testHtmlToPdf",
                MimeType = "text/html",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                paperSize = PaperSize.a4
            };

            var apiTask = Task.Run(() => WebAPICall(htmlToPdfJob));
            return apiTask.Result;
        }
        public async Task<byte[]> WebAPICall(HTMLtoPDFJob htmlToPdfJ)
        {
            byte[] byteRes = null;
            HTMLtoPDFRepository repo = new HTMLtoPDFRepository();
            Guid jobID = await repo.SubmitWorkItem(htmlToPdfJ);
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
