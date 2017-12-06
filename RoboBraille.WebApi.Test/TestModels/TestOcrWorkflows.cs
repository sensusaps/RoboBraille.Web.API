using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using RoboBraille.WebApi.Models;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test
{
    [TestClass]
    public class TestOcrWorkflows
    {
        private static readonly string timestamp = DateTime.Now.Ticks.ToString();
        private Dictionary<string, byte[]> inputFiles;


        [OneTimeSetUp]
        public void Initialize()
        {
            inputFiles = IOController.GetInputFiles();
        }

        [Test]
        public void TestCerthOcrJpgToHtml()
        {
            //arrange
            string format = "html";
            string fileName = "A17.testOcr.jpg";

            //act
            var apiRes = TestCerthOcrAPI(true, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "CerthOcrJpg-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestCerthOcrPngToHtml()
        {
            //arrange
            string format = "html";
            string fileName = "A27.testTableImage.png";

            //act
            var apiRes = TestCerthOcrAPI(true, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "CerthOcrPng-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestCerthOcrPdfToHtml()
        {
            //arrange
            string format = "html";
            string fileName = "A6.testPDF.pdf";

            //act
            var apiRes = TestCerthOcrAPI(true, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "CerthOcrPdf-" + timestamp + "-" + format + "-API." + format);
        }

        public byte[] TestCerthOcrAPI(bool hasTable, string inputFileName)
        {
            byte[] apiFileContent = inputFiles.Where(x => x.Key.EndsWith(inputFileName)).Select(x => x.Value).First();
            var extension = ".pdf";
            var mime = "application/pdf";
            if (inputFileName.EndsWith("png"))
            {
                extension = ".png";
                mime = "image/png";
            }
            if (inputFileName.EndsWith("jpg"))
            {
                extension = ".jpg";
                mime = "image/jpeg";
            }
            var ocrJob = new OcrConversionJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = extension,
                FileName = "testOcr",
                MimeType = mime,
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                HasTable = hasTable,
                OcrLanguage = Language.enUS
            };

            return WebAPICall(ocrJob).Result;
        }

        public async Task<byte[]> WebAPICall(OcrConversionJob ocrj)
        {
            byte[] byteRes = null;
            OcrConversionRepository ocrJobRepo = new OcrConversionRepository();
            Guid jobID = await ocrJobRepo.SubmitWorkItem(ocrj);
            while (ocrJobRepo.GetWorkStatus(jobID) == 2)
            {
                //wait
                await Task.Delay(200);
            }
            if (ocrJobRepo.GetWorkStatus(jobID) == 1)
            {
                //sucess
                FileResult result = ocrJobRepo.GetResultContents(jobID);
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
