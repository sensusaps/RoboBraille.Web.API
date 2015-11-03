namespace RoboBraille.WebApi.Migrations
{
    using RoboBraille.WebApi.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

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

            Guid uid;
            Guid.TryParse("7b76ae41-def3-e411-8030-0c8bfd2336cd", out uid);
            context.ServiceUsers.AddOrUpdate(new ServiceUser { EmailAddress = "source@sensus.dk", UserId = uid , ApiKey = uid.ToByteArray(), FromDate = new DateTime(2015, 1, 1), ToDate = new DateTime(2020,1,1), UserName ="TestUser", Jobs = null  });
                //context.People.AddOrUpdate(
                //  p => p.FullName,
                //  new Person { FullName = "Andrew Peters" },
                //  new Person { FullName = "Brice Lambson" },
                //  new Person { FullName = "Rowan Miller" }
                //);
            
        }
    }
}
