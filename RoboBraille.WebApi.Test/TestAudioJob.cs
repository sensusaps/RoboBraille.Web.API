using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using RoboBraille.WebApi.Models;
using System.Collections.Generic;
using System.Diagnostics;
using RoboBraille.WebApi.Controllers;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Http;
using Moq;
using System.Data.Entity;
using System.Text;
using System.Collections.Specialized;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;

namespace RoboBraille.WebApi.Test
{
    [TestClass]
    public class TestAudioJob
    {
        //private IEnumerable<string> installedLangs;
        //private AudioController auCont;
        //private readonly IRoboBrailleJob<AudioJob> _repository;
        //private static string filePath = @"C:\Users\Paul\Desktop\RoboBrailleWebApi Test Cases\A14.quick.txt";
        //private static string fileName = @"A14.quick.txt";

        public TestAudioJob()
        {
            //_repository = new AudioJobRepository();
            //auCont = new AudioController(_repository);
            //installedLangs = auCont.GetLangs();
        }



        [TestMethod]
        public async Task TestPostAudioJob()
        {
            //init
            var mockJobs = new Mock<DbSet<Job>>();
            var mockServiceUsers = new Mock<DbSet<ServiceUser>>();
            var mockContext = new Mock<RoboBrailleDataContext>();
            var mockAuSender = new Mock<IAudioJobSender>();

            // arrange
            var users = new List<ServiceUser> { 
                new ServiceUser
                {
                EmailAddress = "test@test.eu",
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                ApiKey = Encoding.UTF8.GetBytes("7b76ae41-def3-e411-8030-0c8bfd2336cd"),
                FromDate = new DateTime(2015, 1, 1),
                ToDate = new DateTime(2020, 1, 1),
                UserName = "TestUser",
                Jobs = null
                }
            }.AsQueryable();

            AudioJob auj = new AudioJob()
            {
                Id = Guid.NewGuid(),
                FileContent = new byte[512],
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".pdf",
                FileName = "test",
                MimeType = "application/pdf",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[64],
                AudioLanguage = Language.daDK,
                FormatOptions = AudioFormat.Mp3,
                SpeedOptions = AudioSpeed.Normal,
                VoicePropriety = new VoicePropriety[] { VoicePropriety.Anne }
            };
            AudioJob auj2 = new AudioJob()
            {
                Id = Guid.NewGuid(),
                FileContent = new byte[256],
                UserId = Guid.Parse("d2b87532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".txt",
                FileName = "test2",
                MimeType = "text/plain",
                Status = JobStatus.Done,
                SubmitTime = DateTime.Now,
                DownloadCounter = 2,
                InputFileHash = new byte[2]
            };
            var jobs = new List<AudioJob> { auj2 }.AsQueryable();

            mockJobs.As<IDbAsyncEnumerable<Job>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Job>(jobs.GetEnumerator()));
            mockJobs.As<IQueryable<Job>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Job>(jobs.Provider));

            mockJobs.As<IQueryable<Job>>().Setup(m => m.Expression).Returns(jobs.Expression);
            mockJobs.As<IQueryable<Job>>().Setup(m => m.ElementType).Returns(jobs.ElementType);
            mockJobs.As<IQueryable<Job>>().Setup(m => m.GetEnumerator()).Returns(jobs.GetEnumerator());

            mockServiceUsers.As<IDbAsyncEnumerable<ServiceUser>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<ServiceUser>(users.GetEnumerator()));
            mockServiceUsers.As<IQueryable<ServiceUser>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ServiceUser>(users.Provider));
            mockServiceUsers.As<IQueryable<ServiceUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockServiceUsers.As<IQueryable<ServiceUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockServiceUsers.As<IQueryable<ServiceUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            mockContext.Setup(m => m.Jobs).Returns(mockJobs.Object);
            mockContext.Setup(m => m.ServiceUsers).Returns(mockServiceUsers.Object);
            mockAuSender.Setup(m => m.SendAudioJobToQueue(auj)).Returns(new byte[512]);

            var repo = new AudioJobRepository(mockContext.Object, mockAuSender.Object);

            var request = new HttpRequestMessage();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", "Hawk id=\"d2b97532-e8c5-e411-8270-f0def103cfd0\", ts=\"1470657024\", nonce=\"VkcMGB\", mac=\"hXW+BLRoqwlUaQZQtpPToOWnVAh5KbAXGGT5f8dLMVk=\"");
            var serviceController = new AudioController(repo);
            serviceController.Request = request;
            //call
            await serviceController.Post(auj);

            //test
            mockJobs.Verify(m => m.Add(It.IsAny<Job>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
            mockAuSender.Verify(m => m.SendAudioJobToQueue(It.IsAny<AudioJob>()), Times.Once());
        }
        
        //[TestMethod]
        //public async Task TestLanguages()
        //{
        //    AudioSpeed rate = AudioSpeed.Normal;
        //    foreach (string lang in installedLangs)
        //    {
        //        Language language = (Language)Enum.Parse(typeof(Language), lang);
        //        AudioJob job = new AudioJob()
        //        {
        //            AudioLanguage = language,
        //            SpeedOptions = rate,
        //            FormatOptions = AudioFormat.Mp3
        //        };
        //        job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.None };
        //        job.FileContent = File.ReadAllBytes(filePath);
        //        job.FileName = fileName.Substring(0, fileName.LastIndexOf("."));
        //        job.FileExtension = fileName.Substring(fileName.LastIndexOf(".") + 1);
        //        job.MimeType = @"text/plain";


        //        //Male and Female for: NL,IS,HU
        //        if (Language.nlNL.Equals(language) || Language.isIS.Equals(language) || Language.huHU.Equals(language))
        //        {
        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Male };
        //            Guid idmale = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idmale, "Test config: " + lang + " Sex: M");

        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Female };
        //            Guid idfemale = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idfemale, "Test config: " + lang + " Sex: F");
        //        } else
        //        //daDK special case
        //        if (Language.daDK.Equals(language))
        //        {
        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Male };
        //            Guid idmale = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idmale, "Test config: " + lang + " Sex: M");

        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Female };
        //            Guid idfemale = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idfemale, "Test config: " + lang + " Sex: F");

        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Female, VoicePropriety.Anne };
        //            Guid idfemaleanne = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idfemaleanne, "Test config: " + lang + " Sex: F Anne");
        //        } else
        //        //lt special case
        //        if (Language.ltLT.Equals(language))
        //        {
        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Male };
        //            Guid idmale = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idmale, "Test config: " + lang + " Sex: M");

        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Male, VoicePropriety.Older };
        //            Guid idmaleolder = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idmaleolder, "Test config: " + lang + " Sex: M Older");

        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Female };
        //            Guid idfemale = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idfemale, "Test config: " + lang + " Sex: F");

        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.Female, VoicePropriety.Older };
        //            Guid idfemaleolder = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idfemaleolder, "Test config: " + lang + " Sex: F Older");
        //        }
        //        else
        //        {
        //            job.VoicePropriety = new List<VoicePropriety>() { VoicePropriety.None };
        //            Guid idsimple = await _repository.SubmitWorkItem(job);
        //            this.WaitAndGetResult(idsimple, "Test config: " + lang + " Sex: None");
        //        }
        //        Assert.IsNotNull(job);
        //    }
        //}

        //private void WaitAndGetResult(Guid id,string message)
        //{
        //    Task t1 = Task.Factory.StartNew(() => this.ResultNewThread(id,message));
        //    Task.WaitAll(t1);
        //}

        //private void ResultNewThread(Guid id,string message)
        //{
        //    bool wait = true;
        //    while (wait)
        //    {
        //        Thread.Sleep(1500);
        //        int status = _repository.GetWorkStatus(id);
        //        if (status < 2)
        //        {
        //            wait = false;
        //            if (status == 1)
        //            {
        //                var result = _repository.GetResultContents(id);
        //                if (result != null)
        //                {
        //                    //success
        //                    Console.WriteLine("Success for: ID: " + id.ToString("D") + " " + message);
        //                    Assert.IsNotNull(result);
        //                }
        //            }
        //            else
        //            {
        //                Assert.AreEqual(0, status);
        //                //error
        //                Console.WriteLine("ERROR on: ID: " + id.ToString("D") + " " + message);
        //            }
        //        }
        //    }
        //}
    }
}
