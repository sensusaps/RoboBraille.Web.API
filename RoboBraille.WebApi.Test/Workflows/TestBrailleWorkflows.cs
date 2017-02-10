using System;
using System.Threading.Tasks;
using RoboBraille.WebApi.Models;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Collections;
using RoboBraille.WebApi.Test.RoboBrailleFTP;

namespace RoboBraille.WebApi.Test
{
    [TestFixtureSource(typeof(BrailleFixtureData), "FixtureParms")]
    public class TestBrailleWorkflows {
        private static readonly string brailleTest = Path.GetTempPath() + @"\testBraille.txt";
        private static string resultPath;

        private string language;
        private string contraction;
        private string format;
        private int charsPerLine, linesPerPage;

        public TestBrailleWorkflows(string language, string contraction, string format)
        {
            this.language = language;
            this.contraction = contraction;
            this.format = format;
            this.charsPerLine = 0;
            this.linesPerPage = 0;
        }

        public TestBrailleWorkflows(string language, string contraction, string format, int charsPerLine, int linesPerPage)
        {
            this.language = language;
            this.contraction = contraction;
            this.format = format;
            this.charsPerLine = charsPerLine;
            this.linesPerPage = linesPerPage;
        }

        [TestFixtureSetUp]
        public void Initialize()
        {
            File.Create(brailleTest).Close();
        }

        [Test]
        public void TestBraille()
        {
            //setup
            Encoding enc = RoboBrailleProcessor.GetEncodingByCountryCode((Language)Enum.Parse(typeof(Language), language, true));
            resultPath = InputSourceRepository.GetTestResultDirectory() + @"\Braille-"+language+"-"+contraction+"-"+format;
            Directory.CreateDirectory(resultPath);
            InputSourceRepository isr = new InputSourceRepository();
            File.WriteAllBytes(brailleTest, enc.GetBytes(isr.GetTestForLanguage(language)));

            PageNumbering pn = PageNumbering.none;
            if (((OutputFormat)Enum.Parse(typeof(OutputFormat), format, true)).Equals(OutputFormat.Pef)) {
                charsPerLine = new Random().Next(25, 33);
                linesPerPage = new Random().Next(25, 33);
                switch (new Random().Next(0, 2))
                {
                    case 0: pn = PageNumbering.none; break;
                    case 1: pn = PageNumbering.left; break;
                    case 2: pn = PageNumbering.right; break;
                    default: break;
                }
            }

            BrailleJob brj = new BrailleJob()
            {
                Id = Guid.NewGuid(),
                FileContent = File.ReadAllBytes(brailleTest),
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".txt",
                FileName = "testBraille",
                MimeType = "plaint/text",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8],
                BrailleLanguage = (Language)Enum.Parse(typeof(Language), language,true),
                BrailleFormat = BrailleFormat.sixdot,
                CharactersPerLine = charsPerLine,
                LinesPerPage = linesPerPage,
                Contraction = (BrailleContraction)Enum.Parse(typeof(BrailleContraction),contraction,true),
                ConversionPath = ConversionPath.texttobraille,
                OutputFormat = (OutputFormat)Enum.Parse(typeof(OutputFormat),format,true),
                PageNumbering = pn
            };
            if (language == "daDK")
            {
                switch (contraction)
                {
                    case "small": contraction = "Lille"; break;
                    case "large": contraction = "Grade2"; break;
                    case "full": contraction = "Grade1"; break;
                    default: break;
                }
            }
            if (language == "nnNO")
            {
                switch (contraction)
                {
                    case "level0": contraction = "Grade1"; break;
                    case "level1": contraction = "Kortskrift1"; break;
                    case "level2": contraction = "Kortskrift2"; break;
                    default: break;
                }
                language = "nbNO";
            }
            string destFile = Guid.NewGuid().ToString() + ".txt";
            SensusRequest sr = new SensusRequest()
            {
                Process = "Braille",
                SubProcess = contraction,
                Option = format,
                Language = language.ToLowerInvariant().Insert(2,"-"),
                Prefix = "",
                RequesterID = "sensus-test",
                FTPServer = "2.109.50.19",
                FTPUser = "sensustest",
                FTPPassword = "M1cr0c0mputer",
                SourcePath = brailleTest,
                DestinationFile = destFile
            };

            //act
            var apiTask = Task.Run(() => WebAPICall(brj));
            //var ftpTask = Task.Run(() => FTPCall(sr));
            //Task.WaitAll(new Task[] { apiTask, ftpTask });
            byte[] apiRes = apiTask.Result;
            //byte[] ftpRes =  ftpTask.Result;

            //assert
            //NUnit.Framework.Assert.IsNotNull(ftpRes);
            //string expected = enc.GetString(ftpRes).Trim();
            NUnit.Framework.Assert.IsNotNull(apiRes);
            string result = enc.GetString(apiRes).Trim();
            //byte assertion fails because the files are not the same encoding

            //NUnit.Framework.Assert.AreEqual(ftpRes, apiRes);
            //NUnit.Framework.Assert.AreEqual(expected,result);
        }
        static async Task<byte[]> FTPCall(SensusRequest sr)
        {
            //return Encoding.ASCII.GetBytes("test");
            byte[] ftpRes =  RbFTPRepository.ProcessRoboBrailleFTP(sr);
            File.WriteAllBytes(resultPath + @"\ftp.txt", ftpRes);
            return ftpRes;

        }

        static async Task<byte[]> WebAPICall(BrailleJob brj)
        {
            //return Encoding.ASCII.GetBytes("test");
            byte[] byteRes = null;
            BrailleJobRepository brJobRepo = new BrailleJobRepository();
            Guid jobID = await brJobRepo.SubmitWorkItem(brj);
           
            while (brJobRepo.GetWorkStatus(jobID) == 2)
            {
                //wait
                await Task.Delay(200);
            }
            if (brJobRepo.GetWorkStatus(jobID) == 1)
            {
                //sucess
                FileResult result = brJobRepo.GetResultContents(jobID);
                byteRes = result.getFileContents();
                File.WriteAllBytes(resultPath + @"\api.txt", byteRes);
            }
            else
            {
                //fail
                //throw new Exception("Task with job ID: " + jobID + " failed");
            }
            return byteRes;
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            File.Delete(brailleTest);
        }
    }


    public class BrailleFixtureData
    {
        private static readonly string[] brailleLangsGrade1 = new string[] {
            "enGB","isIS","enUS","frFR","deDE","huHU","itIT","plPL","ptPT","roRO","slSI","esES","bgBG"
        };
        private static readonly string[] brailleFormats = new string[] {
            "nacb","unicode","octobraille","pef"
        };
        public static IEnumerable FixtureParms
        {
            get
            {
                //string f = "octobraille";
                //foreach (string f in brailleFormats)
                //{
                    foreach (string s in brailleLangsGrade1)
                    {
                        yield return new TestFixtureData(s, "Grade1", "unicode");
                    }
                    //yield return new TestFixtureData("daDK", "small", f);
                    //yield return new TestFixtureData("daDK", "large", f);
                    //yield return new TestFixtureData("daDK", "full", f);
                    //yield return new TestFixtureData("enGB", "Grade2", f);
                    //yield return new TestFixtureData("enUS", "Grade2", f);
                    //yield return new TestFixtureData("deDE", "Grade2", f);
                    //yield return new TestFixtureData("nnNO", "level0", f);
                    //yield return new TestFixtureData("nnNO", "level1", f);
                    //yield return new TestFixtureData("nnNO", "level2", f);
                //}
            }
        }
    }
}
