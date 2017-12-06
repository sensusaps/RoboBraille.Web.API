using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RoboBraille.WebApi.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace RoboBraille.WebApi.Controllers
{
    /// <summary>
    /// Web controller responsible for registering, login and logged in user details
    /// </summary>
    public class AccountController : Controller
    {
        private RoboBrailleDataContext _context = new RoboBrailleDataContext();
        private AccountProcessor accProc = new AccountProcessor();
        public async Task<ActionResult> Register()
        {
            return View("Register");
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterServiceUser model)
        {
            bool userExists = await _context.ServiceUsers.AnyAsync(u => u.EmailAddress.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase));
            if (userExists)
            {
                ModelState.AddModelError("Email","Email already exists! Please use another email address.");
            }
            if (ModelState.IsValid)
            {
                var user = new ServiceUser()
                {
                    UserName = model.UserName,
                    EmailAddress = model.Email,
                    ApiKey = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
                    Password = accProc.GeneratePasswordHash(model.Password),
                    IsApproved = false,
                    FromDate = DateTime.Now  
                };
                user.ToDate = user.FromDate;
                _context.ServiceUsers.Add(user);
                _context.SaveChanges();
                Session["userid"] = user.UserId.ToString();
                Session["role"] = user.UserRole;
                Session["username"] = user.UserName;
                return RedirectToAction("Details", new { id = user.UserId });
            }
            return View(model);
        }

        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceUser user = await _context.ServiceUsers.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.ReadableKey = Encoding.UTF8.GetString(user.ApiKey);
            return View(user);
        }

        public async Task<ActionResult> Login()
        {
            return View("Login");
        }

        public async Task<ActionResult> LogOut()
        {
            Session["userid"] = null;
            Session["role"] = null;
            Session["username"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginServiceUser model)
        {
            ServiceUser user = await _context.ServiceUsers.SingleOrDefaultAsync(u => u.EmailAddress.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase));
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email not found! Please register for an account first.");
            }
            if (ModelState.IsValid)
            {                
                if (AccountProcessor.VerifyPassword(model.Password, user.Password))
                {
                    Session["userid"] = user.UserId.ToString();
                    Session["role"] = user.UserRole;
                    Session["username"] = user.UserName;
                    return RedirectToAction("Details", new { id = user.UserId });
                } else
                {
                    return HttpNotFound("Password does not match!");
                }
            }
            return View(model);
        }
    }
}