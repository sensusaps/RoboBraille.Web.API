using System;
using System.Threading.Tasks;
using RoboBraille.WebApi.Models;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Collections;

namespace RoboBraille.WebApi.Test
{
    /// <summary>
    /// All braille tests are done using grade 1 and unicode
    /// with no formating or pagination
    /// </summary>
    public class TestBrailleWorkflows {

        private static string timestamp = DateTime.Now.Ticks.ToString();
        
        [OneTimeSetUp]
        public void Initialize()
        {
        }

        [Test]
        public void TestBrailleEnUS6G1()
        {
            string language="enUS";
            string contraction="grade1";
            string format="none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction,dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format +"-"+ dots +"-API.txt");
        }

        [Test]
        public void TestBrailleEnUS6G2()
        {
            string language = "enUS";
            string contraction = "grade2";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleEnGB8G1()
        {
            string language = "enGB";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 8;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleEnGB8G2()
        {
            string language = "enGB";
            string contraction = "grade2";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 8;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleEnGB6G1()
        {
            string language = "enGB";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleEnGB6G2()
        {
            string language = "enGB";
            string contraction = "grade2";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleBulgarian6G1()
        {
            string language = "bgBG";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleDanish8FullOcto()
        {
            string language = "daDK";
            string contraction = "full";
            string format = "octobraille";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 8;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleDanish8SmallOcto()
        {
            string language = "daDK";
            string contraction = "small";
            string format = "octobraille";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 8;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleDanish8LargeOcto()
        {
            string language = "daDK";
            string contraction = "large";
            string format = "octobraille";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 8;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleFrench6G1()
        {
            string language = "frFR";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleGerman6G1()
        {
            string language = "deDE";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleGreek6G1()
        {
            string language = "elGR";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleHungarian6G1()
        {
            string language = "huHU";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleIcelandic6G1()
        {
            string language = "isIS";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleItalian6G1()
        {
            string language = "itIT";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }
        
        [Test]
        public void TestBrailleNorwegian6Level0()
        {
            string language = "nbNO";
            string contraction = "level0";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleNorwegian6Level1()
        {
            string language = "nbNO";
            string contraction = "level1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleNorwegian6Level2()
        {
            string language = "nbNO";
            string contraction = "level2";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBraillePolish6G1()
        {
            string language = "plPL";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBraillePortuguese6G1()
        {
            string language = "ptPT";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleRomanian6G1()
        {
            string language = "roRO";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleSlovenian6G1()
        {
            string language = "slSI";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        [Test]
        public void TestBrailleSpanish6G1()
        {
            string language = "esES";
            string contraction = "grade1";
            string format = "none";
            int charsPerLine = 0, linesPerPage = 0;
            int dots = 6;
            var apiRes = TestBrailleAPI(language, format, charsPerLine, linesPerPage, contraction, dots);
            NUnit.Framework.Assert.IsNotNull(apiRes);
            Assert.IsNotEmpty(apiRes);
            IOController.SaveTestResultFile(apiRes, "Braille-" + timestamp + "-" + language + "-" + contraction + "-" + format + "-" + dots + "-API.txt");
        }

        //TODO test different formatting options: unicode, octobraille, nacb where applicable

        public byte[] TestBrailleAPI(string language, string format, int charsPerLine, int linesPerPage, string contraction,int dots)
        {
            Encoding enc = RoboBrailleProcessor.GetEncodingByCountryCode((Language)Enum.Parse(typeof(Language), language, true));
            var apiFileContents = enc.GetBytes(IOController.GetTestForLanguage(language));

            PageNumbering pn = PageNumbering.none;

            var brj = new BrailleJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContents,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".txt",
                FileName = "testBraille",
                MimeType = "plaint/text",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContents),
                BrailleLanguage = (Language)Enum.Parse(typeof(Language), language, true),
                BrailleFormat = (BrailleFormat)Enum.Parse(typeof(BrailleFormat),dots.ToString(),true),
                CharactersPerLine = charsPerLine,
                LinesPerPage = linesPerPage,
                Contraction = (BrailleContraction)Enum.Parse(typeof(BrailleContraction), contraction, true),
                ConversionPath = ConversionPath.texttobraille,
                OutputFormat = (OutputFormat)Enum.Parse(typeof(OutputFormat), format, true),
                PageNumbering = pn
            };
            
            var apiTask = Task.Run(() => WebAPICall(brj));
            
            byte[] apiRes = apiTask.Result;
            return apiRes;
        }

        static async Task<byte[]> WebAPICall(BrailleJob brj)
        {
            byte[] byteRes = null;
            var brJobRepo = new BrailleRepository();
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
            }
            else
            {
                //fail
            }
            return byteRes;
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
        }
    }
}
