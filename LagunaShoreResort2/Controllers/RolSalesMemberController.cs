using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LagunaShoreResort2.Models;

namespace LagunaShoreResort2.Controllers
{
    [Authorize]
    public class RolSalesMemberController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /RolSalesMember/
        public ActionResult Index()
        {
            var rolsalesmembers = db.RolSalesMembers.Include(r => r.rol);
            return View(rolsalesmembers.ToList());
        }

        // GET: /RolSalesMember/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolSalesMember rolsalesmember = db.RolSalesMembers.Find(id);
            if (rolsalesmember == null)
            {
                return HttpNotFound();
            }
            return View(rolsalesmember);
        }

        // GET: /RolSalesMember/Create
        public ActionResult Create()
        {
            ViewBag.rolID = new SelectList(db.SalesMemberTypes, "rolID", "type");
            return View();
        }

        // POST: /RolSalesMember/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="rolSalesMemberID,rolID,salesMemeberID")] RolSalesMember rolsalesmember)
        {
            if (ModelState.IsValid)
            {
                db.RolSalesMembers.Add(rolsalesmember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.rolID = new SelectList(db.SalesMemberTypes, "rolID", "type", rolsalesmember.rolID);
            return View(rolsalesmember);
        }

        // GET: /RolSalesMember/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolSalesMember rolsalesmember = db.RolSalesMembers.Find(id);
            if (rolsalesmember == null)
            {
                return HttpNotFound();
            }
            ViewBag.rolID = new SelectList(db.SalesMemberTypes, "rolID", "type", rolsalesmember.rolID);
            return View(rolsalesmember);
        }

        // POST: /RolSalesMember/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="rolSalesMemberID,rolID,salesMemeberID")] RolSalesMember rolsalesmember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rolsalesmember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.rolID = new SelectList(db.SalesMemberTypes, "rolID", "type", rolsalesmember.rolID);
            return View(rolsalesmember);
        }

        // GET: /RolSalesMember/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolSalesMember rolsalesmember = db.RolSalesMembers.Find(id);
            if (rolsalesmember == null)
            {
                return HttpNotFound();
            }
            return View(rolsalesmember);
        }

        // POST: /RolSalesMember/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RolSalesMember rolsalesmember = db.RolSalesMembers.Find(id);
            db.RolSalesMembers.Remove(rolsalesmember);
            db.SaveChanges();
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
