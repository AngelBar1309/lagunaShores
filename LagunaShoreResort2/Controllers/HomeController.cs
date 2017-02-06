using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LagunaShoreResort2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Action to receive and save in session the aside toggle status to keep it in every screen
        /// </summary>
        /// <param name="asideStatus">Boolean status, false if open and true if closed.</param>
        [HttpPost]
        public void saveAsideStatus(Boolean asideStatus)
        {
           Session["asideStatus"] = asideStatus;
        }
    }
}