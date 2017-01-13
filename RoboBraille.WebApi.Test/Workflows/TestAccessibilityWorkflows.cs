using NUnit.Framework;
using RoboBraille.WebApi.ABBYY;
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
    [TestFixtureSource(typeof(AccessibilityFixtureData), "FixtureParms")]
    class TestAccessibilityWorkflows
    {
        private static readonly string accessibilityTest = Path.GetTempPath() + @"\testAccessibility";

        private static string resultPath;
        private string format;

        public TestAccessibilityWorkflows(string format)
        {
            this.format = format;
        }

        [TestFixtureSetUp]
        public void Initialize()
        {
            File.Create(accessibilityTest).Close();
        }

        [Test]
        public void TestAccessibility()
        {
            //setup
            resultPath = InputSourceRepository.GetTestResultDirectory() + @"\Accessibility-" + format;
            InputSourceRepository isr = new InputSourceRepository();
            var of = (OutputFileFormatEnum)Enum.Parse(typeof(OutputFileFormatEnum), Convert.ToString("OFF_DOCX"));
            string sourceFile = "A6testPDF.pdf";
            switch (format)
            {
                case "doc": of = OutputFileFormatEnum.OFF_MSWord;
                    sourceFile = "A6testPDF.pdf";
                    break;
                case "docx": of = OutputFileFormatEnum.OFF_DOCX;
                    sourceFile = "A6testPDF.pdf";
                    break;
                case "rtf": of = OutputFileFormatEnum.OFF_RTF;
                    sourceFile = "A2.testANSI.txt";
                    break;
                case "pdf": of = OutputFileFormatEnum.OFF_PDFA;
                    sourceFile = "A13.testWordSimplified.docx";
                    break;
                case "xls": of = OutputFileFormatEnum.OFF_MSExcel;
                    //?
                    break;
                case "xlsx": of = OutputFileFormatEnum.OFF_XLSX; 
                    //?
                    break;
                case "csv": of = OutputFileFormatEnum.OFF_CSV; 
                    //?
                    break;
                case "txt": of = OutputFileFormatEnum.OFF_Text;
                    sourceFile = "A5.testOcr.png";
                    break;
                case "htm": of = OutputFileFormatEnum.OFF_HTML;
                    //?
                    break;
                default: of = OutputFileFormatEnum.OFF_Text; 
                    break;
            }


            AccessibleConversionJob accj = new AccessibleConversionJob()
            {
                Id = Guid.NewGuid(),
                FileContent = File.ReadAllBytes(accessibilityTest),
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".txt",
                FileName = "testAccessibility",
                MimeType = "plaint/text",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8],
                TargetDocumentFormat = of
            };

            string destFile = Guid.NewGuid().ToString() + ".txt";
            //act
            var apiTask = Task.Run(() => WebAPICall(accj));

            byte[] apiRes = apiTask.Result;

            //assert
            NUnit.Framework.Assert.IsNotNull(apiRes);
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
                await Task.Delay(200);
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

        [TestFixtureTearDown]
        public void Cleanup()
        {
            File.Delete(accessibilityTest);
        }
    }

    public class AccessibilityFixtureData
    {
        private static readonly string[] formats = new string[] {
            "doc","docx","rtf","pdf","xls","xlsx","csv","txt","htm"       
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
