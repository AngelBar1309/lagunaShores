using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LagunaShoreResort2.Models;
using LagunaShoreResort2.Models.ViewModels;

namespace LagunaShoreResort2.Controllers
{
    [Authorize(Roles =AccountRolesNames.ADMINISTRATOR)]
    public class CondoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Condo
        public ActionResult Index()
        {
            return View(db.Condoes.ToList());
        }

        // GET: Condo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condo condo = db.Condoes.Find(id);
            if (condo == null)
            {
                return HttpNotFound();
            }
            return View(condo);
        }

        // GET: Condo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Condo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "condoID,name,phase,block,bedrooms,description")] Condo condo)
        {
            if (ModelState.IsValid)
            {
                db.Condoes.Add(condo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(condo);
        }

        // GET: Condo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condo condo = db.Condoes.Find(id);
            if (condo == null)
            {
                return HttpNotFound();
            }
            return View(condo);
        }

        // POST: Condo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "condoID,name,phase,block,bedrooms,description")] Condo condo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(condo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(condo);
        }

        // GET: Condo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condo condo = db.Condoes.Find(id);
            if (condo == null)
            {
                return HttpNotFound();
            }
            return View(condo);
        }

        // POST: Condo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Condo condo = db.Condoes.Find(id);
            db.Condoes.Remove(condo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returns a information about the requested condo and
        /// its current owner in the same view model
        /// </summary>
        /// <param name="condoID">Unique integer identification requested condo.</param>
        /// <returns></returns>
        public JsonResult condoInfo(int condoID)
        {
            VMCondo_CurrentClient vmCondoClient = new VMCondo_CurrentClient(condoID);
            return Json(vmCondoClient, JsonRequestBehavior.AllowGet);
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
