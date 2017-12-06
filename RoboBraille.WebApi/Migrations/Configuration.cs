namespace RoboBraille.WebApi.Migrations
{
    using RoboBraille.WebApi.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<RoboBraille.WebApi.Models.RoboBrailleDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

        }

        protected override void Seed(RoboBraille.WebApi.Models.RoboBrailleDataContext context)
        {
              //This method will be called after migrating to the latest version.

              //You can use the DbSet<T>.AddOrUpdate() helper extension method 
              //to avoid creating duplicate seed data. E.g.
              
            Guid apiGuid = Guid.NewGuid();
            String apiKey = apiGuid.ToString();
            byte[] apiKeyByteArray = Encoding.UTF8.GetBytes(apiKey);
            //when creating a new user save the uid and apiKey strings to pass to the user so that he may access the db
            //context.ServiceUsers.AddOrUpdate(new ServiceUser { EmailAddress = "source@sensus.dk", UserId = Guid.NewGuid(), ApiKey = Encoding.UTF8.GetBytes("7b76ae41-def3-e411-8030-0c8bfd2336cd"), FromDate = new DateTime(2015, 1, 1), ToDate = new DateTime(2020, 1, 1), UserName = "TestUser", Jobs = null });
            context.ServiceUsers.AddOrUpdate(new ServiceUser { EmailAddress = "paul@sensus.dk", Password=Encrypt.CreateHash("Fishb0ne"), UserId = Guid.NewGuid(), ApiKey = apiKeyByteArray, FromDate = DateTime.Now, ToDate = DateTime.Now.AddYears(100), UserName = "TestAdmin",IsApproved=true,UserRole=UserRole.Administrator, Jobs = null });
            context.ServiceUsers.AddOrUpdate(new ServiceUser { EmailAddress = "test@sensus.dk", Password = Encrypt.CreateHash("Fishb0ne"), UserId = Guid.NewGuid(), ApiKey = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()), FromDate = DateTime.Now, ToDate = DateTime.Now.AddYears(100), UserName = "TestUser", IsApproved = true, UserRole = UserRole.User, Jobs = null });

        }
    }
}
