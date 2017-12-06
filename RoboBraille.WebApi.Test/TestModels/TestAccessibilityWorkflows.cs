using NUnit.Framework;
using RoboBraille.WebApi.ABBYY;
using RoboBraille.WebApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test
{
    /// <summary>
    /// Tests the following
    /// pdf/png to text (unicode, ansi, utf8)
    /// pdf/png to docx
    /// pdf/png to tagged pdf
    /// pdf/png to doc/rtf/csv/html/xls/xlsx 
    /// </summary>
    class TestAccessibilityWorkflows
    {
        private static readonly string timestamp = DateTime.Now.Ticks.ToString();
        private Dictionary<string, byte[]> inputFiles;


        [OneTimeSetUp]
        public void Initialize()
        {
            inputFiles = IOController.GetInputFiles();
        }

        [Test]
        public void TestAccessibilityPdfToTaggedPdf()
        {
            //arrange
            string format = "pdf";
            string fileName = "A6.testPDF.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format.Substring(0,3));
        }

        [Test]
        public void TestAccessibilityPdfToText()
        {
            //arrange
            string format = "txt";
            string fileName = "A6.testPDF.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityPngToText()
        {
            //arrange
            string format = "txt";
            string fileName = "A5.testOcr.png";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImagePng-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityJpgToText()
        {
            //arrange
            string format = "txt";
            string fileName = "A17.testOcr.jpg";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImageJpg-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityPdfToDocx()
        {
            //arrange
            string format = "docx";
            string fileName = "A6.testPDF.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityImageToDocx()
        {
            //arrange
            string format = "docx";
            string fileName = "A17.testOcr.jpg";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityPdfToDoc()
        {
            //arrange
            string format = "doc";
            string fileName = "A6.testPDF.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityImageToDoc()
        {
            //arrange
            string format = "doc";
            string fileName = "A17.testOcr.jpg";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityPdfToRtf()
        {
            //arrange
            string format = "rtf";
            string fileName = "A6.testPDF.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityImageToRtf()
        {
            //arrange
            string format = "rtf";
            string fileName = "A17.testOcr.jpg";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityScanPdfToTaggedPdf()
        {
            //arrange
            string format = "pdf";
            string fileName = "A28.testScanDoc.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format.Substring(0,3));
        }

        [Test]
        public void TestAccessibilityImageToTaggedPdf()
        {
            //arrange
            string format = "pdf";
            string fileName = "A17.testOcr.jpg";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format.Substring(0, 3));
        }

        [Test]
        public void TestAccessibilityImageTo1APdf()
        {
            //arrange
            string format = "pdfa";
            string fileName = "A17.testOcr.jpg";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format.Substring(0,3));
        }

        [Test]
        public void TestAccessibilityPdfToXls()
        {
            //arrange
            string format = "xls";
            string fileName = "A28.testScanDoc.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityImageToXls()
        {
            //arrange
            string format = "xls";
            string fileName = "A27.testTableImage.png";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityPdfToXlsx()
        {
            //arrange
            string format = "xlsx";
            string fileName = "A28.testScanDoc.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityImageToXlsx()
        {
            //arrange
            string format = "xlsx";
            string fileName = "A27.testTableImage.png";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityPdfToCSV()
        {
            //arrange
            string format = "csv";
            string fileName = "A28.testScanDoc.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityImageToCSV()
        {
            //arrange
            string format = "csv";
            string fileName = "A27.testTableImage.png";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityPdfToHtm()
        {
            //arrange
            string format = "htm";
            string fileName = "A28.testScanDoc.pdf";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityPdf-" + timestamp + "-" + format + "-API." + format);
        }

        [Test]
        public void TestAccessibilityImageToHtm()
        {
            //arrange
            string format = "htm";
            string fileName = "A27.testTableImage.png";

            //act
            var apiRes = TestAccessibilityAPI(format, fileName);

            //assert and save to result dir
            NUnit.Framework.Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "AccessibilityImage-" + timestamp + "-" + format + "-API." + format);
        }

        public byte[] TestAccessibilityAPI(string format, string inputFileName)
        {
            //setup
            var of = (OutputFileFormatEnum)Enum.Parse(typeof(OutputFileFormatEnum), Convert.ToString("OFF_DOCX"));
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
            byte[] apiFileContent = inputFiles.Where(x => x.Key.EndsWith(inputFileName)).Select(x => x.Value).First();
            switch (format)
            {
                case "doc":
                    of = OutputFileFormatEnum.OFF_MSWord;
                    break;
                case "docx":
                    of = OutputFileFormatEnum.OFF_DOCX;
                    break;
                case "rtf":
                    of = OutputFileFormatEnum.OFF_RTF;
                    break;
                case "pdf":
                    of = OutputFileFormatEnum.OFF_PDF;
                    break;
                case "pdfa":
                    of = OutputFileFormatEnum.OFF_PDFA;
                    break;
                case "xls":
                    of = OutputFileFormatEnum.OFF_MSExcel;
                    break;
                case "xlsx":
                    of = OutputFileFormatEnum.OFF_XLSX;
                    break;
                case "csv":
                    of = OutputFileFormatEnum.OFF_CSV;
                    break;
                case "txt":
                    of = OutputFileFormatEnum.OFF_Text;
                    break;
                case "htm":
                    of = OutputFileFormatEnum.OFF_HTML;
                    break;
                default:
                    of = OutputFileFormatEnum.OFF_Text;
                    break;
            }


            AccessibleConversionJob accj = new AccessibleConversionJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = extension,
                FileName = "testAccessibility",
                MimeType = mime,
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                TargetDocumentFormat = of
            };

            var apiTask = Task.Run(() => WebAPICall(accj));
            return apiTask.Result;
        }

        public async Task<byte[]> WebAPICall(AccessibleConversionJob accj)
        {
            //return Encoding.ASCII.GetBytes("test");
            byte[] byteRes = null;
            AccessibleConversionRepository accJobRepo = new AccessibleConversionRepository();
            Guid jobID = await accJobRepo.SubmitWorkItem(accj);
            while (accJobRepo.GetWorkStatus(jobID) == 2)
            {
                //wait
                await Task.Delay(2000);
            }
            if (accJobRepo.GetWorkStatus(jobID) == 1)
            {
                //sucess
                FileResult result = accJobRepo.GetResultContents(jobID);
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
        }
    }
}
