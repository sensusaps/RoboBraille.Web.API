using System;
using NUnit.Framework;
using Moq;
using System.Data.Entity;
using RoboBraille.WebApi.Models;
using System.Text;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using RoboBraille.WebApi.Controllers;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test
{
    public class TestInterfaces
    {
        [Test]
        public void TestPostLanguageToLanguage()
        {
            //init
            var mockJobs = new Mock<DbSet<Job>>();
            var mockServiceUsers = new Mock<DbSet<ServiceUser>>();
            var mockContext = new Mock<RoboBrailleDataContext>();

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

            TranslationJob trj = new TranslationJob()
            {
                Id = Guid.NewGuid(),
                FileContent = Encoding.UTF8.GetBytes("This is the first translation job."),
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "txt",
                FileName = "test",
                MimeType = "text/plain",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8],
                SourceLanguage = Language.enGB.ToString(),
                TargetLanguage = Language.daDK.ToString()
            };
            TranslationJob trj2 = new TranslationJob()
            {
                Id = Guid.NewGuid(),
                FileContent = Encoding.UTF8.GetBytes("This is the second translation job."),
                UserId = Guid.Parse("d2b87532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "txt",
                FileName = "test2",
                MimeType = "text/plain",
                Status = JobStatus.Done,
                SubmitTime = DateTime.Now,
                DownloadCounter = 2,
                InputFileHash = new byte[2],
                SourceLanguage = Language.enGB.ToString(),
                TargetLanguage = Language.svSE.ToString()
            };
            var jobs = new List<TranslationJob> { trj2 }.AsQueryable();

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

            var repo = new TranslationRepository(mockContext.Object);
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", "Hawk id=\"d2b97532-e8c5-e411-8270-f0def103cfd0\", ts=\"1470657024\", nonce=\"VkcMGB\", mac=\"hXW+BLRoqwlUaQZQtpPToOWnVAh5KbAXGGT5f8dLMVk=\"");
            var serviceController = new TranslationController(repo);
            serviceController.Request = request;
            //call
            var jobid = serviceController.Post(trj).Result;
            //test
            mockJobs.Verify(m => m.Add(It.IsAny<Job>()), Times.Once());

            mockContext.Verify(m => m.SaveChanges(), Times.Exactly(1));
            //twice if it is synced
            //mockContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void TestModelLanguageToLanguage()
        {
            TranslationJob trj = new TranslationJob()
            {
                Id = Guid.NewGuid(),
                FileContent = Encoding.UTF8.GetBytes("This is the first translation job."),
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "txt",
                FileName = "test",
                MimeType = "text/plain",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8],
                SourceLanguage = Language.enGB.ToString(),
                TargetLanguage = Language.daDK.ToString()
            };
            var repo = new TranslationRepository();

            var jobID = repo.SubmitWorkItem(trj).Result;
            while (repo.GetWorkStatus(jobID) == 2)
            {
                //wait
                Task.Delay(200);
            }
            byte[] res = null;
            if (repo.GetWorkStatus(jobID) == 1)
            {
                //sucess
                FileResult result = repo.GetResultContents(jobID);
                res = result.getFileContents();
            }
            else
            {
                //fail
                throw new Exception("Task with job ID: " + jobID + " failed");
            }

            NUnit.Framework.Assert.AreEqual("This is the first translation job.", Encoding.UTF8.GetString(res));
        }
        
        [Test]
        public void TestPostTextToSign()
        {
            //init
            var mockJobs = new Mock<DbSet<Job>>();
            var mockServiceUsers = new Mock<DbSet<ServiceUser>>();
            var mockContext = new Mock<RoboBrailleDataContext>();

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

            SignLanguageJob sgnj = new SignLanguageJob()
            {
                Id = Guid.NewGuid(),
                FileContent = Encoding.UTF8.GetBytes("This is the first translation job."),
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "txt",
                FileName = "test",
                MimeType = "text/plain",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8],
                SourceTextLanguage = Language.enGB.ToString(),
                TargetSignLanguage = Language.enGB.ToString(),
                SignLanguageForm = SignLanguageType.DeafSignLanguage
            };
            SignLanguageJob sgnj2 = new SignLanguageJob()
            {
                Id = Guid.NewGuid(),
                FileContent = Encoding.UTF8.GetBytes("This is the second translation job."),
                UserId = Guid.Parse("d2b87532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "txt",
                FileName = "test2",
                MimeType = "text/plain",
                Status = JobStatus.Done,
                SubmitTime = DateTime.Now,
                DownloadCounter = 2,
                InputFileHash = new byte[2],
                SourceTextLanguage = Language.enUS.ToString(),
                TargetSignLanguage = Language.enUS.ToString(),
                SignLanguageForm = SignLanguageType.SignedMode
            };
            var jobs = new List<SignLanguageJob> { sgnj2 }.AsQueryable();

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

            var repo = new SignLanguageRepository(mockContext.Object);
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", "Hawk id=\"d2b97532-e8c5-e411-8270-f0def103cfd0\", ts=\"1470657024\", nonce=\"VkcMGB\", mac=\"hXW+BLRoqwlUaQZQtpPToOWnVAh5KbAXGGT5f8dLMVk=\"");
            var serviceController = new SignLanguageController(repo);
            serviceController.Request = request;
            //call
            var jobID = serviceController.Post(sgnj).Result;
            //test
            mockJobs.Verify(m => m.Add(It.IsAny<Job>()), Times.Once());

            mockContext.Verify(m => m.SaveChanges(), Times.Exactly(1));
            //twice if it is synced
            //mockContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void TestModelTextToSign()
        {
            SignLanguageJob sgnj = new SignLanguageJob()
            {
                Id = Guid.NewGuid(),
                FileContent = Encoding.UTF8.GetBytes("This is the first translation job."),
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = "txt",
                FileName = "test",
                MimeType = "text/plain",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8],
                SourceTextLanguage = Language.enGB.ToString(),
                TargetSignLanguage = Language.enGB.ToString(),
                SignLanguageForm = SignLanguageType.DeafSignLanguage
            };
            var repo = new SignLanguageRepository();

            var jobID = repo.SubmitWorkItem(sgnj).Result;
            while (repo.GetWorkStatus(jobID) == 2)
            {
                //wait
                Task.Delay(200);
            }
            byte[] res = null;
            if (repo.GetWorkStatus(jobID) == 1)
            {
                //sucess
                FileResult result = repo.GetResultContents(jobID);
                res = result.getFileContents();
            }
            else
            {
                //fail
                throw new Exception("Task with job ID: " + jobID + " failed");
            }

            NUnit.Framework.Assert.AreEqual("This is the first translation job.", Encoding.UTF8.GetString(res));
        }
    }
}
