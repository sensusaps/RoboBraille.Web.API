using NUnit.Framework;
using RoboBraille.WebApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test.Workflows
{
    [TestFixtureSource(typeof(DaisyFixtureData), "FixtureParms")]
    class TestDaisyWorkflows
    {
        private static readonly string daisyTest = Path.GetTempPath() + @"\testDaisy.docx";

        private static string resultPath;
        private string format;

        public TestDaisyWorkflows(string format)
        {
            this.format = format;
        }

        [TestFixtureSetUp]
        public void Initialize()
        {
            //File.Create(daisyTest).Close();
        }

        [Test]
        public void TestDaisy()
        {

            //setup
            resultPath = InputSourceRepository.GetTestResultDirectory() + @"\Daisy-" + format;
            Directory.CreateDirectory(resultPath);
            InputSourceRepository isr = new InputSourceRepository();
            string testDocumentsPath = InputSourceRepository.GetTestDirectory() + "A1.Daisy.docx";
            File.Copy(testDocumentsPath+"", daisyTest);
            DaisyJob daisyj = new DaisyJob()
            {
                Id = Guid.NewGuid(),
                FileContent = File.ReadAllBytes(daisyTest),
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".txt",
                FileName = "testDaisy",
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8],
                DaisyOutput = DaisyOutput.TalkingBook                
            };

            string destFile = Guid.NewGuid().ToString() + ".txt";
            //act
            var apiTask = Task.Run(() => WebAPICall(daisyj));
            byte[] apiRes = apiTask.Result;
            //assert
            NUnit.Framework.Assert.IsNotNull(apiRes);
        }
        public async Task<byte[]> WebAPICall(DaisyJob dj)
        {
            //return Encoding.ASCII.GetBytes("test");
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

        [TestFixtureTearDown]
        public void Cleanup()
        {
            File.Delete(daisyTest);
        }
    }

    public class DaisyFixtureData
    {
        private static readonly string[] daisyOptions = new string[] {
            "Classic","New","Math"
        };
        public static IEnumerable FixtureParms
        {
            get
            {
                foreach (string f in daisyOptions)
                {
                    yield return new TestFixtureData(f);
                }
            }
        }
    }
}
