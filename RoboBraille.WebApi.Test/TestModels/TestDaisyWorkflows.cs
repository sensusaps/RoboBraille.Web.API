using NUnit.Framework;
using RoboBraille.WebApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test
{
    class TestDaisyWorkflows
    {
        private static readonly string timestamp = DateTime.Now.Ticks.ToString();
        private Dictionary<string, byte[]> inputFiles;
        
        [OneTimeSetUp]
        public void Initialize()
        {
            inputFiles = IOController.GetInputFiles();
            RabbitMQCluster.ClusterConnect();
        }

        [Test]
        public void TestDaisyDocx()
        {
            //arrange
            var testDocumentsName = "A1.Daisy.docx";
            var apiFileContent = inputFiles.Where(x => x.Key.EndsWith(testDocumentsName)).Select(x => x.Value).First();

            DaisyJob daisyj = new DaisyJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".docx",
                FileName = "testDaisy",
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                DaisyOutput = DaisyOutput.TalkingBook
            };


            //act
            var apiTask = Task.Run(() => WebAPICall(daisyj));
            byte[] apiRes = apiTask.Result;

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Daisy-" + timestamp + "-API.zip");
        }
        
        public async Task<byte[]> WebAPICall(DaisyJob dj)
        {
            byte[] byteRes = null;
            DaisyRepository daisyJobRepo = new DaisyRepository();
            Guid jobID = await daisyJobRepo.SubmitWorkItem(dj);
            while (daisyJobRepo.GetWorkStatus(jobID) == 2)
            {
                //wait
                await Task.Delay(200);
            }
            if (daisyJobRepo.GetWorkStatus(jobID) == 1)
            {
                //sucess
                FileResult result = daisyJobRepo.GetResultContents(jobID);
                byteRes = result.getFileContents();
            }
            else
            {
                //fail
                throw new Exception("Task with job ID: " + jobID + " failed");
            }
            return byteRes;
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            RabbitMQCluster.ClusterDisconnect();
        }
    }
}
