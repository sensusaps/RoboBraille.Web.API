using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RoboBraille.WebApi.Models;
using System.Text;
using RoboBraille.WebApi.Models.UserManagement;

namespace RoboBraille.WebApi.Controllers
{
    public class ServiceUsersController : Controller
    {
        private RoboBrailleDataContext db = new RoboBrailleDataContext();

        // GET: ServiceUsers
        public async Task<ActionResult> Index()
        {
            return View(await db.ServiceUsers.ToListAsync());
        }

        // GET: ServiceUsers/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceUser serviceUser = await db.ServiceUsers.FindAsync(id);
            if (serviceUser == null)
            {
                return HttpNotFound();
            }
            serviceUser.ReadableKey = Encoding.UTF8.GetString(serviceUser.ApiKey);
            return View(serviceUser);
        }

        // GET: ServiceUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ServiceUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Exclude="UserId,ApiKey")]ServiceUser serviceUser)
        {
            serviceUser.UserId = Guid.NewGuid();
            serviceUser.ApiKey = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            TryUpdateModel(serviceUser);
            if (ModelState.IsValid || serviceUser.ApiKey!=null)
            {
                db.ServiceUsers.Add(serviceUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Details",new { id = serviceUser.UserId });
            }

            return View(serviceUser);
        }

        // GET: ServiceUsers/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceUser serviceUser = await db.ServiceUsers.FindAsync(id);
            if (serviceUser == null)
            {
                return HttpNotFound();
            }
            return View(serviceUser);
        }

        // POST: ServiceUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserId,UserName,ApiKey,FromDate,ToDate,EmailAddress")] ServiceUser serviceUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(serviceUser);
        }

        // GET: ServiceUsers/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceUser serviceUser = await db.ServiceUsers.FindAsync(id);
            if (serviceUser == null)
            {
                return HttpNotFound();
            }
            return View(serviceUser);
        }

        // POST: ServiceUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            ServiceUser serviceUser = await db.ServiceUsers.FindAsync(id);
            db.ServiceUsers.Remove(serviceUser);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
