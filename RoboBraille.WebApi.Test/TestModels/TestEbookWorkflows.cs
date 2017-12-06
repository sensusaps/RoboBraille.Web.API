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
    /// <summary>
    /// docx,chm,
    /// </summary>
    class TestEbookWorkflows
    {
        private static readonly string timestamp = DateTime.Now.Ticks.ToString();
        private Dictionary<string, byte[]> inputFiles;

        [OneTimeSetUp]
        public void Initialize()
        {
            inputFiles = IOController.GetInputFiles();
        }

        [Test]
        public void TestEbookDocxEpub3()
        {

            RabbitMQCluster.ClusterConnect();
            var testDocumentsName = "A1.Daisy.docx";
            var apiFileContent = inputFiles.Where(x => x.Key.EndsWith(testDocumentsName)).Select(x => x.Value).First();
            var ebookj = new DaisyJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".docx",
                FileName = "testDaisyEPUB3WMO",
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                DaisyOutput = DaisyOutput.Epub3WMO
            };

            var apiTask = Task.Run(() => WebAPICall(ebookj));

            byte[] apiRes = apiTask.Result;

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-EPUB3WMO-API.epub");

            RabbitMQCluster.ClusterDisconnect();
        }

        [Test]
        public void TestEbookHtmlEpub()
        {
            //arrange
            string format = "EPUB";
            string testDocumentsName = "A4.testHTMLtotext.html";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Html" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookHtmlMobi()
        {
            //arrange
            string format = "MOBI";
            string testDocumentsName = "A4.testHTMLtotext.html";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Html-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookPdfEpub()
        {
            //arrange
            string format = "EPUB";
            string testDocumentsName = "A6.1.testTaggedPDF.pdf";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Pdf-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookPdfMobi()
        {
            //arrange
            string format = "MOBI";
            string testDocumentsName = "A6.1.testTaggedPDF.pdf";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Pdf-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookDocxEpub()
        {
            //arrange
            string format = "EPUB";
            string testDocumentsName = "A12testWordComplete.docx";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Docx-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookDocxMobi()
        {
            //arrange
            string format = "MOBI";
            string testDocumentsName = "A12testWordComplete.docx";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Docx-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookMobiEpub()
        {
            //arrange
            string format = "EPUB";
            string testDocumentsName = "A26.testMobi.mobi";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Mobi-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookEpubMobi()
        {
            //arrange
            string format = "MOBI";
            string testDocumentsName = "A25.testEpub.epub";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Epub-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookTxtEpub()
        {
            //arrange
            string format = "EPUB";
            string testDocumentsName = "A11.testUTF8.txt";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Txt-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookTxtMobi()
        {
            //arrange
            string format = "MOBI";
            string testDocumentsName = "A11.testUTF8.txt";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Txt-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookRtfEpub()
        {
            //arrange
            string format = "EPUB";
            string testDocumentsName = "A8.testRtf.rtf";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Rtf-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookEpubRtf()
        {
            //arrange
            string format = "RTF";
            string testDocumentsName = "A25.testEpub.epub";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Epub-" + format + "-API." + format.ToLowerInvariant());
        }

        /// <summary>
        /// Not supported in robobraille
        /// </summary>
        public void TestEbookEpubHtml()
        {
            //arrange
            string format = "HTML";
            string testDocumentsName = "A25.testEpub.epub";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Epub-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookEpubDocx()
        {
            //arrange
            string format = "docx";
            string testDocumentsName = "A25.testEpub.epub";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Epub-" + format + "-API." + format.ToLowerInvariant());
        }

        [Test]
        public void TestEbookEpubTxt()
        {
            //arrange
            string format = "txt";
            string testDocumentsName = "A25.testEpub.epub";

            //act
            var apiRes = TestEbookAPI(format, testDocumentsName);

            //assert
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Ebook-" + timestamp + "-Epub-" + format + "-API." + format.ToLowerInvariant());
        }

        public byte[] TestEbookAPI(string format, string testDocumentsName)
        {
            var apiFileContent = inputFiles.Where(x => x.Key.EndsWith(testDocumentsName)).Select(x => x.Value).First();
            var ftpFilePath = inputFiles.Where(x => x.Key.EndsWith(testDocumentsName)).Select(x => x.Key).First();
            var fileExtension = testDocumentsName.Substring(testDocumentsName.LastIndexOf(".")+1);

            EbookFormat ef = (EbookFormat) Enum.Parse(typeof(EbookFormat),format.ToLowerInvariant());
            switch (format)
            {
                case "EPUB": ef = EbookFormat.epub; break;
                case "MOBI": ef = EbookFormat.mobi; break;
                case "RTF": ef = EbookFormat.rtf; break;
                case "TXT": ef = EbookFormat.txt; break;
                default: break;
            }
            var ebookj = new EBookJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = fileExtension,
                FileName = "testEbook",
                MimeType = "plain/text",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                EbookFormat = ef
            };
            
            var apiTask = Task.Run(() => WebAPICall(ebookj));

            byte[] apiRes = apiTask.Result;

            return apiRes;
        }

       
        public async Task<byte[]> WebAPICall(Job accj)
        {
            //return Encoding.ASCII.GetBytes("test");
            byte[] byteRes = null;
            if (accj.GetType().Equals(typeof(DaisyJob)))
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

        [OneTimeTearDown]
        public void Cleanup()
        {
        }
    }
}
