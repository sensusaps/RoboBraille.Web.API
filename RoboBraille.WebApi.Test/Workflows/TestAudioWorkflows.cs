using NUnit.Framework;
using RoboBraille.WebApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test
{
    [TestFixtureSource(typeof(AudioFixtureData), "FixtureParms")]
    public class TestAudioWorkflows    {
        private static readonly string audioTest = Path.GetTempPath() + @"\testAudio.txt";
        private static string resultPath;

        private string language;

        public TestAudioWorkflows(string lang)
        {
            this.language = lang;
        }

        [TestFixtureSetUp]
        public void Initialize()
        {
            File.Create(audioTest).Close();
        }

        [Test]
        public void TestAudio()
        {
            //setup
            Language lang = (Language)Enum.Parse(typeof(Language), language, true);
            resultPath = InputSourceRepository.GetTestResultDirectory() + @"\Audio-" + language;
            Directory.CreateDirectory(resultPath);
            InputSourceRepository isr = new InputSourceRepository();
            File.WriteAllBytes(audioTest, RoboBrailleProcessor.GetEncodingByCountryCode(lang).GetBytes(isr.GetTestForLanguage(language)));

            AudioJob auj = new AudioJob()
            {
                Id = Guid.NewGuid(),
                FileContent = File.ReadAllBytes(audioTest),
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".txt",
                FileName = "testAudio",
                MimeType = "plaint/text",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8],
                AudioLanguage = lang,
                FormatOptions = AudioFormat.Mp3,
                SpeedOptions = AudioSpeed.Normal,
                VoicePropriety = new VoicePropriety[] { VoicePropriety.None }
            };

            string destFile = Guid.NewGuid().ToString() + ".txt";
            //act
            var apiTask = Task.Run(() => WebAPICall(auj));

            byte[] apiRes = apiTask.Result;

            //assert
            NUnit.Framework.Assert.IsNotNull(apiRes);
        }

        public async Task<byte[]> WebAPICall(AudioJob auj)
        {
            //return Encoding.ASCII.GetBytes("test");
            byte[] byteRes = null;
            AudioJobRepository audioJobRepo = new AudioJobRepository();
            Guid jobID = await audioJobRepo.SubmitWorkItem(auj);
            while (audioJobRepo.GetWorkStatus(jobID) == 2)
            {
                //wait
                await Task.Delay(200);
            }
            if (audioJobRepo.GetWorkStatus(jobID) == 1)
            {
                //sucess
                FileResult result = audioJobRepo.GetResultContents(jobID);
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
            File.Delete(audioTest);
        }
    }

    public class AudioFixtureData
    {
        private static readonly string[] audioLangs = new string[] {
            "arAR","bgBG","zhCN","zhHK","zhTW","nlNL","enGB","enUS","fiFI","frFR","deDE","elGR","klKL","huHU","isIS","itIT","jaJP","koKR","ltLT","nbNO","plPL","ptPT","roRO","ruRU","slSL","esES","esMX","svSE"
        }; //use arAR==arEG for api nbNO and nnNO may be the same 
        public static IEnumerable FixtureParms
        {
            get
            {
                foreach (string a in audioLangs)
                {
                    yield return new TestFixtureData(a);
                }
            }
        }
    }
}
