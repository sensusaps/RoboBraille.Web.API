using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RoboBraille.WebApi.Models.UserManagement
{
    public class UserManagementRepository
    {
        public UserManagementRepository()
        {

        }

        /// <summary>
        /// The ServiceUserSummary instance provided must contain only the UserName, a bool to notify if the user has lifetime license, license expiration date and email address.
        /// For lifetime license the ToDate is equal to the FromDate otherwise if no ExpirationDate is provided the license is issued for one year.
        /// </summary>
        public ServiceUser CreateNewUser(ServiceUser sUser)
        {
            try
            {
                //save to database
                using (var context = new RoboBrailleDataContext())
                {
                    context.ServiceUsers.Add(sUser);
                    context.SaveChanges();
                }
                return sUser;
            }
            catch (Exception dbEx)
            {
                return null;
            }

        }

        public Task<List<Job>> GetUserCompletedJobs(Guid userId)
        {
            List<Job> userJobs = new List<Job>();
           var task = Task.Factory.StartNew(t =>
           {
               using (var context = new RoboBrailleDataContext())
               {
                   userJobs = (from j in context.Jobs where j.UserId==userId && j.Status==JobStatus.Done select j).ToList();
               }
           }, userId);

           return Task.FromResult(userJobs);
        }


    }
}