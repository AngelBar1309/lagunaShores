using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualBasic;
using LagunaShoreResort2.Models;
using LagunaShoreResort2.Models.ViewModels;
using Novacode;
using CommissionPaymentStatus = LagunaShoreResort2.Models.ViewModels.VMFilterContractReport.CommissionPaymentStatus;
using Microsoft.AspNet.Identity;


namespace LagunaShoreResort2.Controllers
{
    [Authorize]
    public class TrialMembershipsController : Controller
    {
        //private Contexto db = new Contexto();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TrialMemberships
        public ActionResult Index(string buscadorContratos, int clientID = 0)
        {
            var trialMemberships = db.trialMemberships;
            String contractType = Request["contractType"];


            if (!String.IsNullOrEmpty(buscadorContratos))
            {
                var result = from c in trialMemberships
                             where
                                 c.contractNumberTM.Contains(buscadorContratos) ||
                                 c.client.legalName.Contains(buscadorContratos) ||
                                 c.client.secondLegalName.Contains(buscadorContratos)
                             select c;
                result = result.OrderByDescending(c => c.tmContractDate);
                if (contractType == "ContratosCancelados")
                {
                    result = result.Where(c => c.tmCanceledContract == true);
                }
                else if (contractType == "ContratosNoVerificados")
                { result = result.Where(c => c.tmVerifiedByAdmin == false); }

                if (User.IsInRole(AccountRolesNames.ACCOUNTANT))
                    result = result.Where(c => c.tmRequestToAccountat);

                return View(result.ToList().Take(30));
            }
            else
            {
                List<TrialMemberships> defaultResult = trialMemberships.OrderBy(c => c.tmContractDate).ToList();
                if (User.IsInRole(AccountRolesNames.ACCOUNTANT))
                {
                    defaultResult = trialMemberships.Where(c => c.tmRequestToAccountat).Take(30).ToList();
                }
                else
                {
                    if (contractType == "ContratosCancelados")
                    {
                        defaultResult = trialMemberships.Where(c => c.tmCanceledContract == true).ToList();
                    }
                    else if (contractType == "ContratosNoVerificados")
                    {
                        defaultResult = trialMemberships.Where(c => c.tmVerifiedByAdmin == false).ToList();
                    }
                    //SdefaultResult = salesContracts.OrderBy(c => c.contractDate).Take(30).ToList();
                }

                return View(defaultResult.Take(30));
            }
        }

        // GET: TrialMemberships/Details/5
        [Authorize(Roles = "Administrador,CEO,Concierge,ContractManager,ContractOfficer,PaymentsReservations,VLO,Accountant,SalesManagers")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrialMemberships trialMemberships = db.trialMemberships.Find(id);
            if (trialMemberships == null)
            {
                return HttpNotFound();
            }
            //ICollection<PaymentDeposits> pays =salesContract.getDepositsInPayments();
            ICollection<PaymentDeposits> pays = Deposit.getDepositsInPayments(null, trialMemberships, null);
            return View(trialMemberships);
        }
        // GET: SalesContract/RequestComission/salesContractID
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER
            + "," + AccountRolesNames.ACCOUNTANT)]
        public ActionResult RequestComission(int? id)
        {                  
            if (id == null)//Invalid ID
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrialMemberships trialMemberships = db.trialMemberships.Find(id);

            if (trialMemberships == null)//SalesContract not found
            {
                return HttpNotFound();
            }

            if (Deposit.isDownPaymentCompleted(null, trialMemberships,null))//If commission can be requested
            {
                VMComissionRequestTM request = new VMComissionRequestTM(trialMemberships);
                return View(request);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: SalesContract/ConfirmRequestComission/salesContractID
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("RequestComission")]
        public ActionResult ConfirmRequestComission(int? trialMemberhsipID)
        {
            if (trialMemberhsipID == null)//Invalid ID
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrialMemberships trialMembership = db.trialMemberships.Find(trialMemberhsipID);

            if (trialMembership == null)//SalesContract not found
            {
                return HttpNotFound();
            }
            if ((bool)Deposit.isDownPaymentCompleted(null, trialMembership, null) && (bool)trialMembership.tmVerifiedByAdmin && !trialMembership.tmRequestToAccountat)//If commission can be requested
            {
                trialMembership.tmRequestToAccountat = true;
                trialMembership.tmRequestToAccountantDate = DateTime.Now;
                try
                {
                    db.Entry(trialMembership).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Report", new { downPaymentPaid = VMFilterSalesContractReport.CommissionPaymentStatus.COMMISSION_REQUESTED });//See Requested Commissions Report
                    return RedirectToAction("RequestComission", new { id = trialMemberhsipID });//See Requested Commissions Report
                }
                catch (Exception exc)
                {
                    VMComissionRequestTM request = new VMComissionRequestTM(trialMembership);
                    ViewBag.errorMessage = "Contact technical support: " + exc.Message;
                    return View(request);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: SalesContract/ConfirmRequestComission/salesContractID
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.ACCOUNTANT)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult confirmCommissionPayment(int? trialMembershipID)
        {
            if (trialMembershipID == null)//Invalid ID
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrialMemberships trialMembership = db.trialMemberships.Find(trialMembershipID);

            if (trialMembership == null)//SalesContract not found
            {
                return HttpNotFound();
            }
            if ((bool)Deposit.isDownPaymentCompleted(null, trialMembership, null) && (bool)trialMembership.tmRequestToAccountat && !(bool)trialMembership.tmCommissionPaid)//If commission can be requested
            {
                trialMembership.tmCommissionPaid = true;
                trialMembership.tmCommissionPaidDate = DateTime.Now;
                try
                {
                    db.Entry(trialMembership).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Report", new { downPaymentPaid = VMFilterSalesContractReport.CommissionPaymentStatus.COMMISSION_PAID });//See Requested Commissions Report
                    return RedirectToAction("RequestComission", new { id = trialMembershipID });//See Requested Commissions Report
                }
                catch (Exception exc)
                {
                    VMComissionRequestTM request = new VMComissionRequestTM(trialMembership);
                    ViewBag.errorMessage = "Contact technical support: " + exc.Message;
                    return View(request);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: SalesContract/cancelRequestCommission/salesContractID
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult cancelRequestCommission(int? trialMemberhsipID)
        {
            if (trialMemberhsipID == null)//Invalid ID
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrialMemberships trialMembership = db.trialMemberships.Find(trialMemberhsipID);

            if (trialMembership == null)//SalesContract not found
            {
                return HttpNotFound();
            }

            trialMembership.tmRequestToAccountat = false;
            trialMembership.tmRequestToAccountantDate = null;

            trialMembership.tmCommissionPaid = false;
            trialMembership.tmCommissionPaidDate = null;

            try
            {
                db.Entry(trialMembership).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Report", new { downPaymentPaid = VMFilterSalesContractReport.CommissionPaymentStatus.COMMISSION_PAID });//See Requested Commissions Report
                return RedirectToAction("RequestComission", new { id = trialMemberhsipID });//See Requested Commissions Report
            }
            catch (Exception exc)
            {
                VMComissionRequestTM request = new VMComissionRequestTM(trialMembership);
                ViewBag.errorMessage = "Contact technical support: " + exc.Message;
                return View(request);
            }

        }

        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult CanceledContracts()
        {
            var query = from c in db.trialMemberships
                        where c.tmCanceledContract == true
                        select c;

            return View(query.ToList());
        }

        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult ContractsUnverified()
        {
            var query = from c in db.trialMemberships
                        where c.tmVerifiedByAdmin == false
                        select c;
            return View(query.ToList());
        }
        // GET: TrialMemberships/Create
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER)]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Se obtiene los datos del cliente nesesarios para realizar el contrato//
            var cliente = db.Clients.Find(id);
            ViewBag.legalName = cliente.legalName;
            ViewBag.slegalName = cliente.secondLegalName;
            ViewBag.clientID = cliente.clientID;
            ViewBag.modoPartialView = "Create";
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.Client = cliente;
            //Roles Selections
            ViewBag.SalesMemberTypes = new SelectList(db.SalesMemberTypes, "rolID", "type");
            //Sales Members Selections
            ViewBag.SalesMembers = new SelectList(
                from member in db.SalesMembers
                select new { salesMemberID = member.salesMemberID, legalName = member.firtName + " " + member.lastName },
                "salesMemberID", "legalName");

            return View();

        }
        // POST: TrialMemberships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrialMemberships trialMemberships, int[] SalesMemberTypes, int[] SalesMembers)
        {
            //Se asignan valores nulos 
            trialMemberships.tmCanceledContractDate = DateTime.Parse("01/01/1800");
            trialMemberships.tmCommissionPaidDate = DateTime.Parse("01/01/1800");
            trialMemberships.tmVerificationDate = DateTime.Parse("01/01/1800");
            //trialMemberships.tmPaymentsQuantity = int.Parse(Request["tmpaymentsQuantity"]);

            trialMemberships.tmVerifiedByAdmin = false;
            trialMemberships.tmRequestToAccountat=false;
            trialMemberships.tmCanceledContract = false;
            trialMemberships.tmCommissionPaid=false;
            Boolean validMemberSelection = validateSalesMemberSelector(SalesMemberTypes, SalesMembers);
            if (validMemberSelection && ModelState.IsValid)
            {
                //Se guarda el id del usurio que esta creando el contrato//
                trialMemberships.userCreateContract = User.Identity.GetUserId();     
                //Generate ContractNumber//
                DateTime dateNow = DateTime.Now;
                string contractType = trialMemberships.contractType;
                //salesContract.clientID = Convert.ToInt32(Request["clientID"]);
                String NumbersContracts = "";
                try
                {
                    NumbersContracts = ((db.trialMemberships.OrderByDescending(p => p.trialMembershipID).Select(r => r.trialMembershipID).Count()) + 1).ToString();
                    //int ClientID = int.Parse(db.Clients.OrderByDescending(p => p.clientID).Select(r => r.clientID).First().ToString())+1;
                    trialMemberships.contractNumberTM = contractType + NumbersContracts + dateNow.ToString("dd/MM/yyyy").Replace("/", "");
                }
                catch
                {
                    if (NumbersContracts != "") { }
                    trialMemberships.contractNumberTM = contractType + 1 + dateNow.ToString("dd/MM/yyyy").Replace("/", "");
                }

                db.trialMemberships.Add(trialMemberships);
                int numRegistersCreated = db.SaveChanges();
                if (numRegistersCreated > 0)
                { //If SalesContract have been successfully saved
                    List<TrialSalesMembers> csms = linkSalesMemberForContract(trialMemberships.trialMembershipID, SalesMemberTypes, SalesMembers);
                    db.TrialSalesMembers.AddRange(csms);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            else if (!validMemberSelection)
                ModelState.AddModelError("INVALID_SALES_MEMBERS_SELECTION", "Pair every Sales Member with his role, go to Sales Member selection and verify fields.");

            int id = trialMemberships.clientID;
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Se obtiene los datos del cliente nesesarios para realizar el contrato//
            var cliente = db.Clients.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }

            //Send clients names
            ViewBag.Client = cliente;

            //Roles Selection
            ViewBag.SalesMemberTypes = new SelectList(db.SalesMemberTypes, "rolID", "type");
            //Sales Members Selections
            ViewBag.SalesMembers = new SelectList(
                from member in db.SalesMembers
                select new { salesMemberID = member.salesMemberID, legalName = member.firtName + " " + member.lastName },
                "salesMemberID", "legalName");
            return View(trialMemberships);
        }

        /// <summary>
        /// Valiadates if the selections of sales members for default and aditional are correct.
        /// </summary>
        /// <param name="SalesMemberTypes"></param>
        /// <param name="SalesMembers"></param>
        /// <returns></returns>
        private bool validateSalesMemberSelector(int[] SalesMemberTypes, int[] SalesMembers)
        {
            bool result = true;
            for (int c = 0; c < SalesMemberTypes.Length; c++)
                //Invalid when one of the selections are cero (not done)
                if (SalesMemberTypes[c] == 0 ^ SalesMembers[c] == 0)//!XOR
                {
                    result = false;
                    break;
                }
            return result;
        }
        private bool validateSalesMemberSelector(ICollection<TrialSalesMembers> csms)
        {
            bool result = true;
            foreach (TrialSalesMembers csm in csms)
                //Invalid when one of the selections are cero (not done)
                if (csm.rolID == 0 ^ csm.salesMemberID == 0)//!XOR
                {
                    result = false;
                    break;
                }
            return result;
        }

        private List<TrialSalesMembers> linkSalesMemberForContract(int trialMembershipID, int[] SalesMemberTypes, int[] SalesMembers)
        {
            //Every sales member is registered to this contract
            //for (int c = 0; c < SalesMemberTypes.Count(); c++)
            //    if (SalesMembers[c] > 0 && SalesMemberTypes[c] > 0)
            //    {
            //        TrialSalesMembers csm = new TrialSalesMembers(trialMembershipID, SalesMembers[c], SalesMemberTypes[c]);
            //        db.TrialSalesMembers.Add(csm);
            //    }
            //Every sales member is registered to this contract
            List<TrialSalesMembers> csms = new List<TrialSalesMembers>();
            for (int c = 0; c < SalesMemberTypes.Count(); c++)
                if (SalesMembers[c] > 0 && SalesMemberTypes[c] > 0)
                {
                    csms.Add(new TrialSalesMembers(trialMembershipID, SalesMembers[c], SalesMemberTypes[c]));
                }

            return csms;
        }
        // GET: TrialMemberships/Edit/5
        [Authorize(Roles = "Administrador,ContractOfficer,ContractManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrialMemberships trialMemberships = db.trialMemberships.Find(id);
            if (trialMemberships == null)
            {
                return HttpNotFound();
            }
            ViewBag.clientID = new SelectList(db.Clients, "clientID", "firstName", trialMemberships.clientID);
            ViewBag.modoPartialView = "modificar";
            return View(trialMemberships);
        }

        // POST: TrialMemberships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,ContractOfficer,ContractManager")]
        public ActionResult Edit(TrialMemberships trialMemberships, int[] SalesMemberTypes, int[] SalesMembers)
        {

            Boolean validMemberSelection = validateSalesMemberSelector(trialMemberships.trialSalesMembers);
            if (validMemberSelection && ModelState.IsValid)
            {
                //si el contrato fue verificado o cancelado se se agrega la fecha actualcorrespondientemente
                if (trialMemberships.tmVerifiedByAdmin == true && trialMemberships.tmVerificationDate == null) { trialMemberships.tmVerificationDate = DateTime.Now; }
                if (trialMemberships.tmCanceledContract == true && trialMemberships.tmCanceledContractDate == null) { trialMemberships.tmCanceledContractDate = DateTime.Now; }
                if (trialMemberships.csToConcord == true && trialMemberships.csToConcordDate == null) { trialMemberships.csToConcordDate = DateTime.Now; }

                try
                {
                    var trialSalesMember = db.TrialSalesMembers.
                     Where(csm => csm.trialMemberships.trialMembershipID == trialMemberships.trialMembershipID);
                    db.TrialSalesMembers.RemoveRange(trialSalesMember);
                    db.SaveChanges();
                }
                catch { }
                //Despues de haber sido borrados, se agrega el contratos con las nuevas relaciones
                //de los miembros de venta y se modifica
                db.Entry(trialMemberships).State = EntityState.Added;
                db.Entry(trialMemberships).State = EntityState.Modified;
                int registeredModified = db.SaveChanges();
                return RedirectToAction("Index");
            }
            else if (!validMemberSelection)
                ModelState.AddModelError("INVALID_SALES_MEMBERS_SELECTION", "Pair every Sales Member with his role, go to Sales Member selection and verify fields.");

            
            trialMemberships = db.trialMemberships.Find(trialMemberships.trialMembershipID);

            //Roles Selections
            ViewBag.SalesMemberTypes = new SelectList(db.SalesMemberTypes, "rolID", "type");
            //Sales Members Selections
            ViewBag.SalesMembers = new SelectList(
                from member in db.SalesMembers
                select new { salesMemberID = member.salesMemberID, legalName = member.firtName + " " + member.lastName },
                "salesMemberID", "legalName");

            return View(trialMemberships);
        }

        // GET: TrialMemberships/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrialMemberships trialMemberships = db.trialMemberships.Find(id);
            if (trialMemberships == null)
            {
                return HttpNotFound();
            }
            return View(trialMemberships);
        }

        // POST: TrialMemberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            TrialMemberships trialMemberships = db.trialMemberships.Find(id);
            db.trialMemberships.Remove(trialMemberships);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: TrialMemberships Report
        [HttpGet]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CEO +
            "," + AccountRolesNames.CONTRACT_MANAGER + "," + AccountRolesNames.ACCOUNTANT +
            "," + AccountRolesNames.OWNER_SERVICES +
            "," + AccountRolesNames.SALES_MANAGER + "," + AccountRolesNames.CONCIERGE + 
            "," + AccountRolesNames.VLO + "," + AccountRolesNames.PAYMENTS_RESERVATIONS)]
        public ActionResult Report(VMFilterContractReport vmFilter)
        {
            vmFilter.contractTypeName = SalesContract.ContractTypeName.TRIAL_MEMBERSHIPS; //Controller Name
            /*Filter by salesContracts and is in time range filter*/
            Boolean isNotFiltering = Request.Url.Query.Count() == 0;

            //If filters were not received
            if (isNotFiltering || !securityRequiermentsMet(vmFilter))
            {
                ViewBag.vmFilter = vmFilter;
                return View();
            }
            else {
                //Get contracts in range time filter
                var query = from c in db.trialMemberships.Where(pay => pay.tmContractDate >= vmFilter.start &&
                    pay.tmContractDate <= vmFilter.end).ToList()
                            select new VMTrialMemberships(c);

                //If more filtering is needed by comission payment status
                if (vmFilter.commissionPaymentStatus != "All")
                {
                    //bool isDownPaymentsPaid = true, isRequested = true, isCommissionPaid = true;

                    //If downpayment is completed but commission was not requested neither paid
                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.DOWNPAYMENT_PAID)
                        query = query.Where(pay => pay.downPaymentPaid == true);//Filter

                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.DOWNPAYMENT_NOT_PAID)
                        query = query.Where(pay => pay.downPaymentPaid == false);//Filter

                    //Salescontracst where commissions needs to be paid
                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.COMMISSION_PAID)
                        query = query.Where(pay => pay.commissionPaid == true);//Filter

                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.COMMISSION_REQUESTED)
                        query = query.Where(pay => pay.requestToAccountant == true);//Filter
                }

                if (query.Count() > 0)
                { //Data to generate summary is calculated
                    decimal b = 0, c = 0, e = 0, f = 0, g = 0;
                    Double a = 0,d=0;
                    foreach (VMTrialMemberships item in query)
                    {
                        if (item.canceledContract == true)
                        {
                            if (item.currency == "MXN")
                            {
                                if (item.contractDate.Year == 2013)
                                {
                                    a += item.saleAmount / 12.76;
                                }
                                else if (item.contractDate.Year == 2014)
                                {
                                    a += item.saleAmount / 13.31;
                                }
                                else if (item.contractDate.Year == 2015)
                                {
                                    a += item.saleAmount / 15.87;
                                }
                            }
                            else if (item.currency == "USD")
                            {
                                a += item.saleAmount;
                            }
                            //a += item.saleAmount;
                            b += item.balance;
                            c += item.totalPaid;
                            //g += item.contractNumber.Count();  
                        }
                        else
                        {
                            if (item.currency == "MXN")
                            {
                                if (item.contractDate.Year == 2013)
                                {
                                    d += item.saleAmount / 12.76;
                                }
                                else if (item.contractDate.Year == 2014)
                                {
                                    d += item.saleAmount / 13.31;
                                }
                                else if (item.contractDate.Year == 2015)
                                {
                                    d += item.saleAmount / 15.87;
                                }
                            }
                            else if (item.currency == "USD")
                            {
                                d += item.saleAmount;
                            }
                           // d += item.saleAmount;
                            e += item.balance;
                            f += item.totalPaid;
                        }
                    }
                    ViewBag.totalSalesAmountCanceled = a;
                    ViewBag.totalOwedAmountCanceled = b;
                    ViewBag.totalPaidCanceled = c;
                    ViewBag.totalSalesAmount = d;
                    ViewBag.totalOwedAmount = e;
                    ViewBag.totalPaid = f;
                    ViewBag.totalCanceledCount = g; 
                }

                ViewBag.vmFilter = vmFilter;
                return View(query);
            }
        }
        //Validate if User can use specific filters
        private bool securityRequiermentsMet(VMFilterContractReport vmFilter)
        {
            return User.IsInRole(AccountRolesNames.ADMINISTRATOR) || User.IsInRole(AccountRolesNames.ACCOUNTANT) || User.IsInRole(AccountRolesNames.CEO)
            || User.IsInRole(AccountRolesNames.CONTRACT_MANAGER)
            || User.IsInRole(AccountRolesNames.OWNER_SERVICES) || User.IsInRole(AccountRolesNames.PAYMENTS_RESERVATIONS)
            || User.IsInRole(AccountRolesNames.SALES_MANAGER) || User.IsInRole(AccountRolesNames.CONCIERGE) || User.IsInRole(AccountRolesNames.VLO);
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
