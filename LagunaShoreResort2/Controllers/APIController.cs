using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LagunaShoreResort2.Models;
using LagunaShoreResort2.Models.ViewModels;

namespace LagunaShoreResort2.Controllers
{
    public class APIController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: API
        public ActionResult Index()
        {
            return View();
        }

        // GET: API/getQualifications/salesMemeberID
        public JsonResult getQualifications(int salesMemeberID = 0)
        {
            var qualifications = from roleMemeber in db.RolSalesMembers.ToList()
                                 where roleMemeber.salesMemberID == salesMemeberID
                                 select new Rol(roleMemeber.rolID, roleMemeber.rol.type);

            return Json(qualifications.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}