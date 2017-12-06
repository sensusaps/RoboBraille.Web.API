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

namespace RoboBraille.WebApi.Controllers
{
    public class JobsController : Controller
    {
        private RoboBrailleDataContext db = new RoboBrailleDataContext();

        // GET: Jobs
        public async Task<ActionResult> Index()
        {
            Guid usersjobs = Guid.Empty;
            string sessionUserId = "";
            if (Session["userid"] != null)
                sessionUserId = Session["userid"].ToString();
            if (Guid.TryParse(sessionUserId, out usersjobs))
            {
                var jobs = db.Jobs.Include(j => j.User).Where(j => j.UserId == usersjobs);
                return View(await jobs.ToListAsync());
            } else
            {
                return View(new List<Job>());
            }
        }

        // GET: Jobs based on UserId
        [HttpPost]
        public async Task<ActionResult> Index(string UserId)
        {
            Guid usersjobs = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                if (Guid.TryParse((string) UserId, out usersjobs))
                {
                    Session["UserId"] = UserId;
                    var jobs = db.Jobs.Include(j => j.User).Where(j => j.UserId == usersjobs).OrderByDescending(j => j.FinishTime);
                    return View(await jobs.ToListAsync());
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            } else
            {
                var jobs = db.Jobs.Include(j => j.User).Where(j => j.UserId == usersjobs);
                return View(await jobs.ToListAsync());
            }
        }

        // GET: Jobs/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = await db.Jobs.FindAsync(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = await db.Jobs.FindAsync(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Job job = await db.Jobs.FindAsync(id);
            db.Jobs.Remove(job);
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
