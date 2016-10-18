﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using RoboBraille.WebApi.Models;
using RoboBraille.WebApi.Controllers;
using RoboBraille.WebApi.ABBYY;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Web;
using System.Collections.Specialized;

namespace RoboBraille.WebApi.Test
{

    [TestClass]
    public class TestAccessibleConversion
    {
        [TestMethod]
        public async Task TestPostAccessibleConversion()
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
            
            AccessibleConversionJob acj = new AccessibleConversionJob() {
                Id = Guid.NewGuid(),
                FileContent = new byte[512],
                TargetDocumentFormat = OutputFileFormatEnum.OFF_PDF,
                UserId = Guid.Parse("d2b97532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".pdf",
                FileName = "test",
                MimeType = "application/pdf",
                Status = JobStatus.Started,
                SubmitTime = DateTime.Now,
                DownloadCounter = 0,
                InputFileHash = new byte[8]
            };
            AccessibleConversionJob acj2 = new AccessibleConversionJob() {
                Id = Guid.NewGuid(),
                FileContent = new byte[256],
                TargetDocumentFormat = OutputFileFormatEnum.OFF_MSWord,
                UserId = Guid.Parse("d2b87532-e8c5-e411-8270-f0def103cfd0"),
                FileExtension = ".txt",
                FileName = "test2",
                MimeType = "text/plain",
                Status = JobStatus.Done,
                SubmitTime = DateTime.Now,
                DownloadCounter = 2,
                InputFileHash = new byte[2]
            };
            var jobs = new List<AccessibleConversionJob>{acj2}.AsQueryable();

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

            var repo = new AccessibleConversionRepository(mockContext.Object);
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", "Hawk id=\"d2b97532-e8c5-e411-8270-f0def103cfd0\", ts=\"1470657024\", nonce=\"VkcMGB\", mac=\"hXW+BLRoqwlUaQZQtpPToOWnVAh5KbAXGGT5f8dLMVk=\"");
            var serviceController = new AccessibleConversionController(repo);
            serviceController.Request = request;
            //call
            await serviceController.Post(acj);
            //test
            mockJobs.Verify(m => m.Add(It.IsAny<Job>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}
