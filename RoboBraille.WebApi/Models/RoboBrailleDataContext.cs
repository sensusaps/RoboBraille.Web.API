using System;
using System.Data.Entity;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// Class used for instantiating the database
    /// </summary>
    public class RoboBrailleDataContext : DbContext
    {
        public RoboBrailleDataContext() : base("RoboBrailleJobDB")
        {
            try
            {
                Database.SetInitializer(new CreateDatabaseIfNotExists<RoboBrailleDataContext>());
            }
            catch (Exception e) {
                throw new Exception(e.Message + " - " + e.InnerException);
            }
        }
        //21.09.2016 added virtual
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<ServiceUser> ServiceUsers { get; set; }
    }

}