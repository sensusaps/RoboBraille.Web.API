using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace RoboBraille.WebApi.Controllers
{
    public class RoboBrailleSPAController : Controller
    {
        // GET: RoboBrailleSPA
        public ActionResult Index()
        {
            return View();
        }
    }
}