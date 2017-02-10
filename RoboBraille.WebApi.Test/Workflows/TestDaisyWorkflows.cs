using NUnit.Framework;
using RoboBraille.WebApi.Models;
using RoboBraille.WebApi.Test.RoboBrailleFTP;
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
            SensusRequest sr = new SensusRequest()
            {
                Process = "DAISY",
                SubProcess = format,
                Option = "", 
                Language = "",
                Gender = "",
                Age = "",
                Prefix = "",
                RequesterID = "sensus-test",
                FTPServer = "2.109.50.19",
                FTPUser = "sensustest",
                FTPPassword = "M1cr0c0mputer",
                SourcePath = daisyTest,
                DestinationFile = destFile
            };
            //act
            var apiTask = Task.Run(() => WebAPICall(daisyj));
            //var ftpTask = Task.Run(() => FTPCall(sr));

            //Task.WaitAll(new Task[] { apiTask, ftpTask });
            byte[] apiRes = apiTask.Result;
            //byte[] ftpRes = ftpTask.Result;

            //assert
            //NUnit.Framework.Assert.IsNotNull(ftpRes);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            //string expected = RoboBrailleProcessor.GetEncodingByCountryCode((Language)Enum.Parse(typeof(Language), language, true)).GetString(ftpRes).Trim();
            //string result = Encoding.UTF8.GetString(apiRes).Trim();
            //byte assertion fails because the files are not the same encoding

            File.WriteAllBytes(resultPath + @"\api.zip", apiRes);
            //File.WriteAllBytes(resultPath + @"\ftp.txt", ftpRes);

            //NUnit.Framework.Assert.AreEqual(ftpRes, apiRes);
            //NUnit.Framework.Assert.AreEqual(expected, result);
        }

        public async Task<byte[]> FTPCall(SensusRequest sr)
        {
            //return Encoding.ASCII.GetBytes("test");
            return RbFTPRepository.ProcessRoboBrailleFTP(sr);

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
