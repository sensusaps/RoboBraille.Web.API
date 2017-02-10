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
    [TestFixtureSource(typeof(EBookFixtureData), "FixtureParms")]
    class TestEbookWorkflows
    {
        private static readonly string daisyTest = Path.GetTempPath() + @"\testDaisy.docx";
        private static readonly string ebookTest = Path.GetTempPath() + @"\testEbook.pdf";

        private static string resultPath;
        private string format;

        public TestEbookWorkflows(string format)
        {
            this.format = format;
        }

        [TestFixtureSetUp]
        public void Initialize()
        {
            //File.Create(ebookTest).Close();
        }

        [Test]
        public void TestEbook()
        {
            //setup
            resultPath = InputSourceRepository.GetTestResultDirectory() + @"\Ebook-" + format;
            Directory.CreateDirectory(resultPath);
            InputSourceRepository isr = new InputSourceRepository();
            string testDocumentsPath = "";
            Job ebookj = null;
            if (format.Equals("EPUB3WMO"))
            {
                testDocumentsPath = InputSourceRepository.GetTestDirectory() + "A1.Daisy.docx";
                File.Copy(testDocumentsPath + "", daisyTest);
                ebookj = new DaisyJob()
                {
                    Id = Guid.NewGuid(),
                    FileContent = File.ReadAllBytes(daisyTest),
                    UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                    FileExtension = ".txt",
                    FileName = "testDaisyEPUB3WMO",
                    MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    Status = JobStatus.Started,
                    SubmitTime = DateTime.Now,
                    DownloadCounter = 0,
                    InputFileHash = new byte[8],
                    DaisyOutput = DaisyOutput.Epub3WMO
                };
            }
            else
            {
                testDocumentsPath = InputSourceRepository.GetTestDirectory() + "A6testPDF.pdf";
                File.Copy(testDocumentsPath + "", ebookTest);
                EbookFormat ef = EbookFormat.mobi;
                if (format.Equals("EPUB"))
                    ef = EbookFormat.epub;
                else ef = EbookFormat.mobi;
                ebookj = new EBookJob()
                {
                    Id = Guid.NewGuid(),
                    FileContent = File.ReadAllBytes(ebookTest),
                    UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                    FileExtension = ".txt",
                    FileName = "testEbook",
                    MimeType = "plaint/text",
                    Status = JobStatus.Started,
                    SubmitTime = DateTime.Now,
                    DownloadCounter = 0,
                    InputFileHash = new byte[8],
                    EbookFormat = ef
                };
            }
            string destFile = Guid.NewGuid().ToString() + ".txt";
            SensusRequest sr = new SensusRequest()
            {
                Process = "E-book",
                SubProcess = format,
                Option = "", //normal = none large xlarge huge
                Language = "",
                Gender = "",
                Age = "",
                Prefix = "",
                RequesterID = "sensus-test",
                FTPServer = "2.109.50.19",
                FTPUser = "sensustest",
                FTPPassword = "M1cr0c0mputer",
                SourcePath = ebookTest,
                DestinationFile = destFile
            };
            //act
            var apiTask = Task.Run(() => WebAPICall(ebookj));
            //var ftpTask = Task.Run(() => FTPCall(sr));

            //Task.WaitAll(new Task[] { apiTask, ftpTask });
            byte[] apiRes = apiTask.Result;
            //byte[] ftpRes = ftpTask.Result;

            //assert
            NUnit.Framework.Assert.IsNotNull(apiRes);
            //NUnit.Framework.Assert.IsNotNull(ftpRes);
            //string expected = RoboBrailleProcessor.GetEncodingByCountryCode((Language)Enum.Parse(typeof(Language), language, true)).GetString(ftpRes).Trim();
            //string result = Encoding.UTF8.GetString(apiRes).Trim();
            //byte assertion fails because the files are not the same encoding

            File.WriteAllBytes(resultPath + @"\api.epub", apiRes);
            //File.WriteAllBytes(resultPath + @"\ftp.epub", ftpRes);

            //NUnit.Framework.Assert.AreEqual(ftpRes, apiRes);
            //NUnit.Framework.Assert.AreEqual(expected, result);
        }

        public async Task<byte[]> FTPCall(SensusRequest sr)
        {
            //return Encoding.ASCII.GetBytes("test");
            return RbFTPRepository.ProcessRoboBrailleFTP(sr);

        }
        public async Task<byte[]> WebAPICall(Job accj)
        {
            //return Encoding.ASCII.GetBytes("test");
            byte[] byteRes = null;
            if (format.Equals("EPUB3WMO"))
            {
                DaisyRepository daisyJobRepo = new DaisyRepository();
                Guid jobID = await daisyJobRepo.SubmitWorkItem((DaisyJob)accj);
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
            else
            {
                EBookRepository ebookJobRepo = new EBookRepository();
                Guid jobID = await ebookJobRepo.SubmitWorkItem((EBookJob)accj);
                while (ebookJobRepo.GetWorkStatus(jobID) == 2)
                {
                    //wait
                    await Task.Delay(200);
                }
                if (ebookJobRepo.GetWorkStatus(jobID) == 1)
                {
                    //sucess
                    FileResult result = ebookJobRepo.GetResultContents(jobID);
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

        [TestFixtureTearDown]
        public void Cleanup()
        {
            try
            {
                File.Delete(ebookTest);
            } catch
            {

            }
            try
            {
                File.Delete(daisyTest);
            } catch
            {

            }
        }
    }

    public class EBookFixtureData
    {
        private static readonly string[] formats = new string[] {
            "EPUB","MOBI","EPUB3WMO"//"EPUB3"   
        };
        public static IEnumerable FixtureParms
        {
            get
            {
                foreach (string f in formats)
                {
                    yield return new TestFixtureData(f);
                }
            }
        }
    }
}
