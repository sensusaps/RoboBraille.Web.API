//using RoboBraille.WebApi.Models;
//using RoboBraille.WebApi.Models.UserManagement;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;

//namespace RoboBraille.WebApi.Controllers
//{
//    public class UserManagementController : Controller
//    {
//        private RoboBrailleDataContext context = new RoboBrailleDataContext();
//        // GET: UserManagement
//        public ActionResult Index()
//        {
//            return View();
//        }


//        [HttpPost]
//        public ActionResult Create(ServiceUserSummary su)
//        {
//            try
//            {
//                string clearTextPp = su.ClearTextPassPhrase;
//                if (Encrypt.ValidatePassword(clearTextPp, ConfigurationManager.AppSettings.Get("passphrase")))
//                {
//                    UserManagementRepository umr = new UserManagementRepository();
//                    string apiKey = Guid.NewGuid().ToString();
//                    ServiceUser sUser = new ServiceUser()
//                    {
//                        UserName = su.UserName,
//                        EmailAddress = su.EmailAddress,
//                        FromDate = DateTime.UtcNow,
//                        ApiKey = Encoding.UTF8.GetBytes(apiKey)
//                    };
//                    if (su.IsLifeTimeLicense)
//                    {
//                        sUser.ToDate = sUser.FromDate;
//                    }
//                    else
//                    {
//                        if (su.ExpirationDate > DateTime.UtcNow)
//                            sUser.ToDate = su.ExpirationDate;
//                        else sUser.ToDate = DateTime.UtcNow.AddYears(1);
//                    }
//                    sUser = umr.CreateNewUser(sUser);
//                    //send an email to the new user with the token and user ID, or just send the userID and retrieve the token from a weblink

//                    su.StatusMessage = "New user created with the following credentials: " + Environment.NewLine +
//                        "User ID: " + sUser.UserId + Environment.NewLine +
//                        "User Name: " + sUser.UserName + Environment.NewLine +
//                        "User Email: " + sUser.EmailAddress + Environment.NewLine +
//                        "User API Key: " + Encoding.UTF8.GetString(sUser.ApiKey) + Environment.NewLine +
//                        "Valid From: " + sUser.FromDate + Environment.NewLine +
//                        "Valid Until: " + sUser.ToDate + Environment.NewLine;
//                }
//                else
//                {
//                    su.StatusMessage = "Administrator password is not correct.";
//                }
//            }
//            catch (Exception e)
//            {
//                su.StatusMessage = "An unexpected error occured. Please try again.";
//            }
//            return View("Index", su);

//        }
//    }
//}