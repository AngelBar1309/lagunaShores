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
    public class ContractSalesMemberController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ContractSalesMember
        [Authorize(Roles = "Administrador,Capturista")]
        public ActionResult Index()
        {
            var contractSalesMembers = db.ContractSalesMembers.Include(c => c.salesContract).Include(c => c.salesMember).Include(c => c.rol);
            return View(contractSalesMembers.ToList());
        }

        // GET: ContractSalesMember/Details/5
        [Authorize(Roles = "Administrador,Capturista")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractSalesMember contractSalesMember = db.ContractSalesMembers.Find(id);
            if (contractSalesMember == null)
            {
                return HttpNotFound();
            }
            return View(contractSalesMember);
        }

        // GET: ContractSalesMember/Create
        [Authorize(Roles = "Administrador,Capturista")]
        public ActionResult Create()
        {
            ViewBag.salesContractID = new SelectList(db.SalesContracts, "salesContractID", "contractNumber");
            ViewBag.salesMemberID = new SelectList(db.SalesMembers, "salesMemberID", "legalName");
            ViewBag.salesMemberTypeID = new SelectList(db.SalesMemberTypes, "salesMemberTypeID", "type");
            return View();
        }

        // POST: ContractSalesMember/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador,Capturista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "contractSalesMemberID,salesContractID,salesMemberID,salesMemberTypeID")] ContractSalesMember contractSalesMember)
        {
            if (ModelState.IsValid)
            {
                db.ContractSalesMembers.Add(contractSalesMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.salesContractID = new SelectList(db.SalesContracts, "salesContractID", "contractNumber", contractSalesMember.salesContractID);
            ViewBag.salesMemberID = new SelectList(db.SalesMembers, "salesMemberID", "legalName", contractSalesMember.salesMemberID);
            ViewBag.salesMemberTypeID = new SelectList(db.SalesMemberTypes, "salesMemberTypeID", "type", contractSalesMember.rolID);
            return View(contractSalesMember);
        }

        // GET: ContractSalesMember/Edit/5
        [Authorize(Roles = "Administrador,Capturista")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractSalesMember contractSalesMember = db.ContractSalesMembers.Find(id);
            if (contractSalesMember == null)
            {
                return HttpNotFound();
            }
            ViewBag.salesContractID = new SelectList(db.SalesContracts, "salesContractID", "contractNumber", contractSalesMember.salesContractID);
            ViewBag.salesMemberID = new SelectList(db.SalesMembers, "salesMemberID", "legalName", contractSalesMember.salesMemberID);
            ViewBag.salesMemberTypeID = new SelectList(db.SalesMemberTypes, "salesMemberTypeID", "type", contractSalesMember.rolID);
            return View(contractSalesMember);
        }

        // POST: ContractSalesMember/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador,Capturista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "contractSalesMemberID,salesContractID,salesMemberID,salesMemberTypeID")] ContractSalesMember contractSalesMember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractSalesMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.salesContractID = new SelectList(db.SalesContracts, "salesContractID", "contractNumber", contractSalesMember.salesContractID);
            ViewBag.salesMemberID = new SelectList(db.SalesMembers, "salesMemberID", "legalName", contractSalesMember.salesMemberID);
            ViewBag.salesMemberTypeID = new SelectList(db.SalesMemberTypes, "salesMemberTypeID", "type", contractSalesMember.rolID);
            return View(contractSalesMember);
        }

        // GET: ContractSalesMember/Delete/5
        [Authorize(Roles = "Administrador,Capturista")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractSalesMember contractSalesMember = db.ContractSalesMembers.Find(id);
            if (contractSalesMember == null)
            {
                return HttpNotFound();
            }
            return View(contractSalesMember);
        }

        // POST: ContractSalesMember/Delete/5
        [Authorize(Roles = "Administrador,Capturista")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContractSalesMember contractSalesMember = db.ContractSalesMembers.Find(id);
            db.ContractSalesMembers.Remove(contractSalesMember);
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
