using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using System.Data.Entity;
using RoboBraille.WebApi.Models;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using RoboBraille.WebApi.Controllers;

namespace RoboBraille.WebApi.Test
{
    [TestClass]
    public class TestHtmlToText
    {
        [TestMethod]
        public async Task TestHtmlToTextConversion()
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

            HTMLToTextJob htj = new HTMLToTextJob()
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
                InputFileHash = new byte[8],
            };
            HTMLToTextJob htj2 = new HTMLToTextJob()
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
            var jobs = new List<HTMLToTextJob> { htj2 }.AsQueryable();

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

            var repo = new HTMLToTextRepository(mockContext.Object);
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", "Hawk id=\"d2b97532-e8c5-e411-8270-f0def103cfd0\", ts=\"1470657024\", nonce=\"VkcMGB\", mac=\"hXW+BLRoqwlUaQZQtpPToOWnVAh5KbAXGGT5f8dLMVk=\"");
            var serviceController = new HTMLToTextController(repo);
            serviceController.Request = request;
            //call
            await serviceController.Post(htj);
            //test
            mockJobs.Verify(m => m.Add(It.IsAny<Job>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}
