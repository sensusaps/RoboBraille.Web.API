using NUnit.Framework;
using RoboBraille.WebApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test
{
    public class TestAudioWorkflows    {

        private static string timestamp = DateTime.Now.Ticks.ToString();
        public TestAudioWorkflows()
        {

        }

        [OneTimeSetUp]
        public void Initialize()
        {
            //connect with rabbitmq (must be on the same local network as the audio agents) and rabbitmqctl must be on path
            /* through cmd connect
             * Step 1) rabbitmqctl stop_app
             * Step 2) rabbitmqctl join_cluster robobraille
             * Step 3) rabbitmqctl start_app
             */
            RabbitMQCluster.ClusterConnect();
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioEnGB()
        {
            //arrange
            string language = Language.enGB.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioEnUS()
        {
            //arrange
            string language = Language.enUS.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        [Test]
        public void TestAudioEnUSModeratelyFast()
        {
            //arrange
            string language = Language.enUS.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender,AudioSpeed.ModerateFast);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-Faster-API" + ".mp3");
        }

        [Test]
        public void TestAudioEnUSSlow()
        {
            //arrange
            string language = Language.enUS.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender, AudioSpeed.Slow);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-Slow-API" + ".mp3");
        }

        [Test]
        public void TestAudioEnUSAAC()
        {
            //arrange
            string language = Language.enUS.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender, AudioSpeed.Normal,AudioFormat.Aac);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-aac-API" + ".mp3");
        }

        [Test]
        public void TestAudioEnUSWav()
        {
            //arrange
            string language = Language.enUS.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender, AudioSpeed.Normal, AudioFormat.Wav);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-wav-API" + ".mp3");
        }

        [Test]
        public void TestAudioArabic()
        {
            throw new NotImplementedException("Voice not installed");
            //arrange
            string language = Language.arEG.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        [Test]
        public void TestAudioArabicEnglish()
        {
            throw new NotImplementedException("Voice not installed");
            //arrange
            string language = Language.arEG.ToString();
            string age = "";
            string gender = "";
            List<VoicePropriety> properties = new List<VoicePropriety>() { VoicePropriety.Bilingual };
            //act
            var apiRes = TestAudioAPI(language, properties, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + VoicePropriety.Bilingual + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioBulgarian()
        {

            //arrange
            string language = Language.bgBG.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        [Test]
        public void TestAudioChineseCantonese()
        {

            //arrange
            string language = Language.zhHK.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        [Test]
        public void TestAudioChineseMandarin()
        {

            //arrange
            string language = Language.zhCN.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }
        

        [Test]
        public void TestAudioChineseTaiwanese()
        {
            //arrange
            string language = Language.zhTW.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        [Test]
        public void TestAudioCzechFemaleZuzana()
        {
            //arrange
            string language = Language.czCZ.ToString();
            string age = "";
            string gender = "female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        [Test]
        public void TestAudioSlovakFemaleLaura()
        {
            //arrange
            string language = Language.skSK.ToString();
            string age = "";
            string gender = "female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioDanishFemaleSara()
        {
            //arrange
            string language = Language.daDK.ToString();
            string age = "";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioDanishMaleCarsten()
        {
            //arrange
            string language = Language.daDK.ToString();
            string age = "";
            string gender = "Male";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioDutchFemale()
        {

            //arrange
            string language = Language.nlNL.ToString();
            string age = "";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioDutchMale()
        {

            //arrange
            string language = Language.nlNL.ToString();
            string age = "";
            string gender = "Male";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioFinnish()
        {

            //arrange
            string language = Language.fiFI.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// voice activated. working
        /// </summary>
        [Test]
        public void TestAudioFrench()
        {

            //arrange
            string language = Language.frFR.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioGerman()
        {

            //arrange
            string language = Language.deDE.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioGreek()
        {
            //arrange
            string language = Language.elGR.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioGreenlandic()
        {
            //arrange
            string language = Language.klGL.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioHungarianFemale()
        {
            //arrange
            string language = Language.huHU.ToString();
            string age = "";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioHungarianMale()
        {
            //arrange
            string language = Language.huHU.ToString();
            string age = "";
            string gender = "Male";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioIcelandicFemale()
        {
            //arrange
            string language = Language.isIS.ToString();
            string age = "";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioIcelandicMale()
        {
            //arrange
            string language = Language.isIS.ToString();
            string age = "";
            string gender = "Male";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioItalian()
        {
            //arrange
            string language = Language.itIT.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }
        /// <summary>
        /// making sounds. don't know if they are right
        /// </summary>
        [Test]
        public void TestAudioJapanese()
        {
            //arrange
            string language = Language.jaJP.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// making sounds. don't know if they are right
        /// </summary>
        [Test]
        public void TestAudioKorean()
        {
            //arrange
            string language = Language.koKR.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioLithuanianFemaleYounger()
        {
            //arrange
            string language = Language.ltLT.ToString();
            string age = "Young";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioLithuanianFemaleOlder()
        {
            //arrange
            string language = Language.ltLT.ToString();
            string age = "Old";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioLithuanianMaleYounger()
        {
            //arrange
            string language = Language.ltLT.ToString();
            string age = "Young";
            string gender = "Male";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioLithuanianMaleOlder()
        {
            //arrange
            string language = Language.ltLT.ToString();
            string age = "Old";
            string gender = "Male";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        /// <summary>
        /// working. passing. voice is probably the swedish one.
        /// </summary>
        [Test]
        public void TestAudioNorwegian()
        {
            //arrange
            string language = Language.nbNO.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //var ftpRes = GetAudioGroundTruth(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
            //Assert.IsNotNull(ftpRes);
            //IOController.SaveTestResultFile(ftpRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-FTP" + ".mp3");
            //Assert.AreEqual(RoboBrailleProcessor.GetMD5Hash(ftpRes),RoboBrailleProcessor.GetMD5Hash(apiRes));
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioPolish()
        {
            //arrange
            string language = Language.plPL.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //var ftpRes = GetAudioGroundTruth(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
            //Assert.IsNotNull(ftpRes);
            //IOController.SaveTestResultFile(ftpRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-FTP" + ".mp3");
            //Assert.AreEqual(RoboBrailleProcessor.GetMD5Hash(ftpRes), RoboBrailleProcessor.GetMD5Hash(apiRes));
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioPortuguese()
        {
            //arrange
            string language = Language.ptPT.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //var ftpRes = GetAudioGroundTruth(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
            //Assert.IsNotNull(ftpRes);
            //IOController.SaveTestResultFile(ftpRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-FTP" + ".mp3");
            //Assert.AreEqual(RoboBrailleProcessor.GetMD5Hash(ftpRes), RoboBrailleProcessor.GetMD5Hash(apiRes));
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioRomanian()
        {
            //arrange
            string language = Language.roRO.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //var ftpRes = GetAudioGroundTruth(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
            //Assert.IsNotNull(ftpRes);
            //IOController.SaveTestResultFile(ftpRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-FTP" + ".mp3");
            //Assert.AreEqual(RoboBrailleProcessor.GetMD5Hash(ftpRes), RoboBrailleProcessor.GetMD5Hash(apiRes));
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioRussian()
        {
            //arrange
            string language = Language.ruRU.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //var ftpRes = GetAudioGroundTruth(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
            //Assert.IsNotNull(ftpRes);
            //IOController.SaveTestResultFile(ftpRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-FTP" + ".mp3");
            //Assert.AreEqual(RoboBrailleProcessor.GetMD5Hash(ftpRes), RoboBrailleProcessor.GetMD5Hash(apiRes));
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioSlovenian()
        {
            //arrange
            string language = Language.slSI.ToString();
            string age = "";
            string gender = "Male";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioSpanishCastilian()
        {
            //arrange
            string language = Language.esES.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioSpanishLatinAmerican()
        {
            //arrange
            string language = Language.esCO.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        /// <summary>
        /// working. passing. voice correct.
        /// </summary>
        [Test]
        public void TestAudioSwedish()
        {
            //arrange
            string language = Language.svSE.ToString();
            string age = "";
            string gender = "";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API"+ ".mp3");
        }

        [Test]
        public void TestAudioWelshFemale()
        {
            //arrange
            string language = Language.cyGB.ToString();
            string age = "";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        [Test]
        public void TestAudioWelshMale()
        {
            //arrange
            string language = Language.cyGB.ToString();
            string age = "";
            string gender = "Male";
            //act
            var apiRes = TestAudioAPI(language, null, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + ".mp3");
        }

        //takes 12 minutes
        //[Test]
        public void TestAudioDanishFemaleSaraLargeText()
        {
            //arrange
            string file = "A29.testLargeTextDK.txt";
            string language = "daDK";
            string age = "";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPIFile(file, language, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + "-Large.mp3");
        }

        //takes 1too long
        [Test]
        public void TestAudioEnGBSpacedText()
        {
            //arrange
            string file = "A30.testSpacedTextEN.txt";
            string language = "enGB";
            string age = "";
            string gender = "Female";
            //act
            var apiRes = TestAudioAPIFile(file, language, age, gender);
            //assert
            Assert.IsNotNull(apiRes);
            IOController.SaveTestResultFile(apiRes, "Audio-" + timestamp + "-" + language + "-" + age + "-" + gender + "-API" + "-Spaced.mp3");
        }

        public byte[] TestAudioAPIFile(string inputFileName, string language, string age = "", string gender = "")
        {
            var outputPath = IOController.GetOutputDirectory();
            Language lang = Language.enGB;
            byte[] apiFileContent = null;
            if (Enum.IsDefined(typeof(Language), language.Substring(0, 4)))
            {
                apiFileContent = IOController.GetInputFiles().Where(x => x.Key.EndsWith(inputFileName)).Select(x => x.Value).First();
                lang = (Language)Enum.Parse(typeof(Language), language.Substring(0, 4), true);
            }
            var voiceProps = new List<VoicePropriety>();
            if (age.Equals("Young"))
                voiceProps.Add(VoicePropriety.Younger);
            if (age.Equals("Old"))
                voiceProps.Add(VoicePropriety.Older);
            if (gender.Equals("Male"))
                voiceProps.Add(VoicePropriety.Male);
            if (gender.Equals("Female"))
                voiceProps.Add(VoicePropriety.Female);
            AudioJob auj = new AudioJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "" + ".mp3",
                FileName = "testAudio",
                MimeType = "plain/text",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                AudioLanguage = lang,
                FormatOptions = AudioFormat.Mp3,
                SpeedOptions = AudioSpeed.Normal,
                VoicePropriety = voiceProps.ToArray()
            };

            return WebAPICall(auj).Result;
        }

        public byte[] TestAudioAPI(string language, List<VoicePropriety> voicepropriety, string age = "", string gender = "", AudioSpeed audioSpeed = AudioSpeed.Normal, AudioFormat audioFormat = AudioFormat.Mp3)
        {
            var outputPath = IOController.GetOutputDirectory();
            Language lang = Language.enGB;
            byte[] apiFileContent = null;
            if (Enum.IsDefined(typeof(Language), language.Substring(0, 4)))
            {
                apiFileContent = RoboBrailleProcessor.GetEncodingByCountryCode(lang).GetBytes(IOController.GetTestForLanguage(language));
                lang = (Language)Enum.Parse(typeof(Language), language.Substring(0, 4), true);
            }
            var voiceProps = new List<VoicePropriety>();
            if (age.Equals("Young"))
                voiceProps.Add(VoicePropriety.Younger);
            if (age.Equals("Old"))
                voiceProps.Add(VoicePropriety.Older);
            if (gender.Equals("Male"))
                voiceProps.Add(VoicePropriety.Male);
            if (gender.Equals("Female"))
                voiceProps.Add(VoicePropriety.Female);
            if (voicepropriety != null)
                voiceProps.AddRange(voicepropriety);
            AudioJob auj = new AudioJob()
            {
                Id = Guid.NewGuid(),
                FileContent = apiFileContent,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ""+ ".mp3",
                FileName = "testAudio",
                MimeType = "plain/text",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = RoboBrailleProcessor.GetMD5Hash(apiFileContent),
                AudioLanguage = lang,
                FormatOptions = audioFormat,
                SpeedOptions = audioSpeed,
                VoicePropriety = voiceProps.ToArray()
            };
            
            return WebAPICall(auj).Result;
        }
        public async Task<byte[]> WebAPICall(AudioJob auj)
        {
            //return Encoding.ASCII.GetBytes("test");
            byte[] byteRes = null;
            AudioRepository audioJobRepo = new AudioRepository();
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

        [OneTimeTearDown]
        public void Cleanup()
        {
            //dissconnect from rabbitmq or close audio agent test instance
            /* through cmd connect
             * Step 1) rabbitmqctl stop_app
             * Step 2) rabbitmqctl reset
             * Step 3) rabbitmqctl start_app
             */
            RabbitMQCluster.ClusterDisconnect();
        }
    }
}
