using System;
using RoboBraille.WebApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace RoboBraille.WebApi.Test
{
    public class TestHtmlToTextWorkflow
    {
        private static readonly string timestamp = DateTime.Now.Ticks.ToString();
        private Dictionary<string, byte[]> inputFiles;


        [OneTimeSetUp]
        public void Initialize()
        {
            inputFiles = IOController.GetInputFiles();
        }

        [Test]
        public void TestHtmlToTextFull()
        {
            //arrange
            string format = "txt";
            string fileName = "A4.testHTMLtotext.html";

            //act
            var apiRes = TestAPI(fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "HtmlToTextFull-" + timestamp + "-API." + format);
        }

        [Test]
        public void TestHtmlToTextTable()
        {
            //arrange
            string format = "txt";
            string fileName = "A3.testHTMLtable.html";

            //act
            var apiRes = TestAPI(fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "HtmlToTextTable-" + timestamp + "-API." + format);
        }

        public byte[] TestAPI(string inputFileName)
        {
            //setup
            byte[] apiFileContent = inputFiles.Where(x => x.Key.EndsWith(inputFileName)).Select(x => x.Value).First();

            HTMLToTextJob htmlToTextJob = new HTMLToTextJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "html",
                FileName = "testHtmlToText",
                MimeType = "text/html",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent)
            };

            var apiTask = Task.Run(() => WebAPICall(htmlToTextJob));
            return apiTask.Result;
        }
        public async Task<byte[]> WebAPICall(HTMLToTextJob htmlToTextJ)
        {
            byte[] byteRes = null;
            HTMLToTextRepository repo = new HTMLToTextRepository();
            Guid jobID = await repo.SubmitWorkItem(htmlToTextJ);
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
