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
    [Authorize]
    public class SalesMemberController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SalesMember
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER + "," + AccountRolesNames.VLO)]
        public ActionResult Index(bool showDroppedOut=false)
        {
            //Show only active sales members by default
            if (!showDroppedOut)
            {
                var salesMembers = db.SalesMembers.Where(sm => sm.droppedOutDate == null);
                return View(salesMembers.ToList());
            }
            else
            {
                return View(db.SalesMembers.ToList());
            }
            
        }

        // GET: SalesMember/Details/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER + "," + AccountRolesNames.VLO)]
        public ActionResult Details(int? id, VMFilterContractReport vmFilter)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesMember salesMember = db.SalesMembers.Find(id);
            if (salesMember == null)
            {
                return HttpNotFound();
            }

            //Get all sales contracts related with this member
            var query = from csm in salesMember.contractSalesMembers
                        select csm;
            //Get all sales contracts related with this member
            var query2 = from csm2 in salesMember.trialSalesMembers
                        select csm2;

            //Apply time filter
            var salesMemberContractsReport = (from csm in query.ToList()
                                              select new VMSalesContract(csm)).
                         Where(contract => contract.contractDate >= vmFilter.start && contract.contractDate <= vmFilter.end);

            //Apply time filter
            var salesMemberTrialMembershipsReport = (from csm2 in query2.ToList()
                                              select new VMTrialMemberships(csm2)).
                         Where(contract => contract.contractDate >= vmFilter.start && contract.contractDate <= vmFilter.end);
            
            if (vmFilter.commissionPaymentStatus != "All") //Activate filter by completed downpayment
            {
                Boolean isdownPaymentPaid = false;//Have NOT downpayment completed 
                if (vmFilter.commissionPaymentStatus == "Downpayment Paid") isdownPaymentPaid = true;//Have downpayment completed 
                salesMemberContractsReport = salesMemberContractsReport.Where(contract => contract.downPaymentPaid == isdownPaymentPaid);//Filter
                salesMemberTrialMembershipsReport = salesMemberTrialMembershipsReport.Where(contract => contract.downPaymentPaid == isdownPaymentPaid);//Filter
            }

            ViewBag.salesMemberContractsReport = salesMemberContractsReport;
            ViewBag.salesMemberTrialMembershipsReport = salesMemberTrialMembershipsReport;
            ViewBag.vmFilter = vmFilter;
            return View(salesMember);
        }

        // GET: SalesMember/Create
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult Create()
        {
            //ViewBag.SalesMemberTypes = db.SalesMemberTypes.ToList();
            VMMemberQualificationSelection vmqSel = new VMMemberQualificationSelection();
            vmqSel.fillWithMemberTypesDB();
            ViewBag.SalesMembertTypesCheckBox = vmqSel;
            return View();
        }

        // POST: SalesMember/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesMember salesMember, VMMemberQualificationSelection qualifications)
        {
            if (ModelState.IsValid)
            {
                db.SalesMembers.Add(salesMember);//Saves new sales member data
                int createdRegs = db.SaveChanges();
                if (createdRegs > 0) { //If new sales member was saved successfully
                    List<RolSalesMember> qualificationsToRegister = salesMember.generateRolSalesMemberList(qualifications);

                    db.RolSalesMembers.AddRange(qualificationsToRegister);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            //ViewBag.SalesMemberTypes = db.SalesMemberTypes.ToList();
            VMMemberQualificationSelection vmqSel = new VMMemberQualificationSelection();
            vmqSel.fillWithMemberTypesDB();
            ViewBag.SalesMembertTypesCheckBox = vmqSel;
            return View(salesMember);
        }

        // GET: SalesMember/Edit/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesMember salesMember = db.SalesMembers.Find(id);
            if (salesMember == null)
            {
                return HttpNotFound();
            }

            //Prepares a new memberType list from database and select the current member qualifications
            VMMemberQualificationSelection vmqSel = new VMMemberQualificationSelection();
            vmqSel.fillWithMemberTypesDB();//Fill list
            vmqSel.selectMemberTypes(salesMember.rolSalesMembers.ToList());//Select current qualifications
            ViewBag.SalesMembertTypesCheckBox = vmqSel;

            return View(salesMember);
        }

        // POST: SalesMember/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SalesMember salesMember, VMMemberQualificationSelection qualifications)
        {
            if (ModelState.IsValid && isQualificationsSelectionValid(qualifications))
            {
                db.Entry(salesMember).State = EntityState.Modified;
                int createdRegs = db.SaveChanges();

                if (createdRegs > 0) {
                    var registeredRolesForMember = db.RolSalesMembers.Where(rol => rol.salesMemberID == salesMember.salesMemberID);
                    db.RolSalesMembers.RemoveRange(registeredRolesForMember); //If new sales member was saved successfully

                    //Prepare a qualifications list related to current sales member and saves in database.
                        List<RolSalesMember> qualificationsToRegister = salesMember.generateRolSalesMemberList(qualifications);
                        db.RolSalesMembers.AddRange(qualificationsToRegister);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
            }

            //Prepares a new memberType list from database and select the current member qualifications
            salesMember = db.SalesMembers.Find(salesMember.salesMemberID);
            VMMemberQualificationSelection vmqSel = new VMMemberQualificationSelection();
            vmqSel.fillWithMemberTypesDB();//Fill list
            vmqSel.selectMemberTypes(salesMember.rolSalesMembers.ToList());//Select current qualifications
            ViewBag.SalesMembertTypesCheckBox = vmqSel;

            return View(salesMember);
        }

        private bool isQualificationsSelectionValid(VMMemberQualificationSelection qualifications)
        {
            bool valid = true;
            if (qualifications.postedMemberTypes == null || qualifications.postedMemberTypes.members.Count() == 0)
            {
                ModelState.AddModelError("INVALID_ROLE_SALES_MEMBERS_SELECTION", "Verify you have selected at least one role for this member.");
                valid = false;
            }
            return valid;
        }

        // GET: SalesMember/Delete/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesMember salesMember = db.SalesMembers.Find(id);
            if (salesMember == null)
            {
                return HttpNotFound();
            }
            return View(salesMember);
        }

        // POST: SalesMember/Delete/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SalesMember salesMember = db.SalesMembers.Find(id);
            db.SalesMembers.Remove(salesMember);
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
