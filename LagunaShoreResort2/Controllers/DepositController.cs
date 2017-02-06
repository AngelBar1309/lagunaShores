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
    public class DepositController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Deposit/
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER + "," + AccountRolesNames.CEO + "," + AccountRolesNames.PAYMENTS_RESERVATIONS + "," + AccountRolesNames.VLO + "," + AccountRolesNames.ACCOUNTANT + "," + AccountRolesNames.OWNER_SERVICES + "," + AccountRolesNames.CONTRACT_OFFICER)]
        public ActionResult Index(string buscadorContratos, decimal? amountPaid, DateTime? paymentDate)
        {
            //var deposits = db.deposits.Include(d => d.);
            String DepositType = Request["DepositType"];
            var dbDeposits = db.Deposits.Include("salesContract").Include("trialMemberships")
                .Include("realStateContract")
                .Include("salesContractHOA")
                .Include("realStateContractHOA");

            if (DepositType == "No verificados")
            {
                var result = from c in dbDeposits
                             where c.verification == false
                             select c;
                result = result.OrderByDescending(c => c.DepositDate);
                return View(result.ToList().Take(150));
            }
            else if (DepositType == "Verificados") {
                var result = from c in dbDeposits
                             where c.verification == true
                             select c;
                result = result.OrderByDescending(c => c.DepositDate);
                return View(result.ToList().Take(150));
            }
            if (paymentDate != null)
            {
                var result = from c in dbDeposits
                             where c.DepositDate==paymentDate
                             select c;
                return View(result.ToList().Take(150));
            }
            if (amountPaid != null)
            {
                var result = from c in dbDeposits
                             where c.Amount == amountPaid
                             select c;
                return View(result.ToList().Take(150));
            }
            if (!String.IsNullOrEmpty(buscadorContratos))
            {
                //    clients = db.Clients.Where(client =>  client.firstName == buscadorCliente);

                var result = from c in dbDeposits
                             where
                                 c.trialMemberships.contractNumberTM.Contains(buscadorContratos) ||
                                 c.salesContract.contractNumber.Contains(buscadorContratos)||
                                  c.salesContract.client.legalName.Contains(buscadorContratos) ||
                                 c.trialMemberships.client.legalName.Contains(buscadorContratos) ||
                                 c.RefNumber.Contains(buscadorContratos)

                             select c;
                result = result.OrderByDescending(c => c.DepositDate);
                try
                {

                    return View(result.ToList().Take(150));
                }
                catch { return View(dbDeposits.ToList().Take(150)); }
            }
            else
            {
                return View(dbDeposits.ToList().Take(150));
            }
            
        }

        // GET: 
        //Personas Autorizadas
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER + "," + AccountRolesNames.CEO + "," + AccountRolesNames.PAYMENTS_RESERVATIONS + "," + AccountRolesNames.VLO + "," + AccountRolesNames.ACCOUNTANT + "," + AccountRolesNames.OWNER_SERVICES)]
        public ActionResult Details(int? id, String contractType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            if (contractType == "FA" || contractType == "BA")
            {
                ViewBag.contractID = deposit.getContractID();
                ViewBag.contractNumber = deposit.salesContract.contractNumber;
                ViewBag.contractType = deposit.getContractType();
            }
            else
            {
                if (contractType == "TM" || contractType == "SA" || contractType == "TSA")
                {
                    ViewBag.tmID = deposit.getContractID();
                    ViewBag.tmContractNumber = deposit.trialMemberships.contractNumberTM;
                    ViewBag.tmContractType = deposit.getContractType();

                }

            }
            return View(deposit);
        }

        // GET: /Deposit/Create
        [HttpGet]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER + "," + AccountRolesNames.PAYMENTS_RESERVATIONS + "," + AccountRolesNames.OWNER_SERVICES)]
        public ActionResult Create(int? id, string contractType, Boolean isHOA = false, int year = 0)
        {
            Deposit deposit = new Deposit();
            ApplicationDbContext db = new ApplicationDbContext();
            int clientID = 0;
            //For fractional contracts
            if (contractType == "FA" || contractType == "BA") { 
                deposit.salesContractID = id;
                clientID = db.SalesContracts.Find(id).clientID;
            }
            //For trial memberships contracts
            else if(contractType == "TM" || contractType =="SA" || contractType == "TSA") { 
                deposit.trialMembershipID = id;
                clientID = db.trialMemberships.Find(id).clientID;
            }
            //For real state contracts
            else if (contractType == "RS"){
                deposit.realStateContractID = id;
                clientID = db.RealStateContracts.Find(id).clientAssignedID;
            }
            //Current year by default
            year = year == 0 ? DateTime.Today.Year : year;

            //**Check default deposit date for new deposits**

            deposit.DepositDate = DateTime.Today
                .AddYears(year - DateTime.Today.Year);

            //Passing contract data to view
            ViewBag.contractID = id;
            ViewBag.contractType = contractType;
            ViewBag.controllerName = deposit.getControllerName();
            ViewBag.clientID = clientID;
            ViewBag.contractNumber = deposit.getContractNumber();
            ViewBag.isHOA = isHOA;

            return View(deposit);
        }

        // POST: /Deposit/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER + "," + AccountRolesNames.PAYMENTS_RESERVATIONS + "," + AccountRolesNames.OWNER_SERVICES)]
        public ActionResult Create(Deposit deposit, string contractType, int clientID = 0, Boolean isHOA = false)
        {
            //Got information from submited deposit
            String controllerName = deposit.getControllerName();
            int contractID = deposit.getContractID(isHOA);

            if (ModelState.IsValid)
            {
                String validation = deposit.validateNewDeposit();//Payment amount is validated
                if (!isHOA) { 
                    if (validation == Deposit.ErrorMessages.OK)//If it's OK
                    {
                        db.Deposits.Add(deposit);
                        db.SaveChanges();
                        return RedirectToAction("Details", controllerName, new { id = contractID });
                    }
                    else if (validation == Deposit.ErrorMessages.AMOUNT_MORE_THAN_BALANCE)//IF NOT OK
                        ModelState.AddModelError("AMOUNT_MORE_THAN_BALANCE", validation);
                    else if (validation == Deposit.ErrorMessages.AMOUNT_IS_ZERO)//IF AMOUNT IS ZERO
                        ModelState.AddModelError("AMOUNT_IS_ZERO", validation);
                }
                else if (validation == Deposit.ErrorMessages.OK)
                {//If HOA payment it's OK
                    db.Deposits.Add(deposit);
                    db.SaveChanges();
                    return RedirectToAction("listHOADeposits", new { id = contractID, contractType = contractType, year = deposit.DepositDate.Year });
                }
                else if (validation == Deposit.ErrorMessages.INVALID_DEPOSIT_DATE) //Deposit date is less than initialHOAMonth date
                    ModelState.AddModelError("INVALID_DEPOSIT_DATE", validation);
                else if (validation == Deposit.ErrorMessages.AMOUNT_IS_ZERO) //Deposit is zero
                    ModelState.AddModelError("AMOUNT_IS_ZERO", validation);
            }

            //Not valid, redirected to submit form
            ViewBag.contractID = contractID;
            ViewBag.contractType = contractType;
            ViewBag.controllerName = controllerName;
            ViewBag.clientID = clientID;
            ViewBag.contractNumber = deposit.getContractNumber();
            ViewBag.isHOA = isHOA;

            return View(deposit);
        }


        // GET: /Deposit/Edit/5
        [HttpGet]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER + "," + AccountRolesNames.PAYMENTS_RESERVATIONS + "," + AccountRolesNames.OWNER_SERVICES + "," + AccountRolesNames.CONTRACT_MANAGER)]
        public ActionResult Edit(int? id, Boolean isHOA = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.isHOA = isHOA;

            return View(deposit);
        }

        // POST: /Deposit/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER + "," + AccountRolesNames.PAYMENTS_RESERVATIONS + "," + AccountRolesNames.OWNER_SERVICES + "," + AccountRolesNames.CONTRACT_MANAGER)]
        public ActionResult Edit(Deposit deposit, Boolean isHOA = false)
        {
            String contractType = deposit.getContractType(isHOA);
            int contractID = deposit.getContractID(isHOA);
            if (ModelState.IsValid)
            {
                String validation = deposit.validateNewDeposit();//Payment amount is validated
                if (!isHOA)
                {
                    if (validation == Deposit.ErrorMessages.OK)//If it's OK
                    {
                        db.Entry(deposit).State = EntityState.Modified;
                        db.SaveChanges();
                        //Redirects to details the details of its contract
                        return RedirectToAction("Details", deposit.getControllerName(), new { id = contractID });
                    }
                    else if (validation == Deposit.ErrorMessages.AMOUNT_MORE_THAN_BALANCE)//IF NOT OK
                        ModelState.AddModelError("AMOUNT_MORE_THAN_BALANCE", validation);
                    else if (validation == Deposit.ErrorMessages.AMOUNT_IS_ZERO)//IF AMOUNT IS ZERO
                        ModelState.AddModelError("AMOUNT_IS_ZERO", validation);
                }
                else if (validation == Deposit.ErrorMessages.OK) {//If HOA payment it's OK
                    db.Entry(deposit).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("listHOADeposits", new { id = contractID, contractType = contractType, year = deposit.DepositDate.Year });
                }else if(validation == Deposit.ErrorMessages.INVALID_DEPOSIT_DATE) //Deposit date is less than initialHOAMonth date
                    ModelState.AddModelError("INVALID_DEPOSIT_DATE", validation);
                else if (validation == Deposit.ErrorMessages.AMOUNT_IS_ZERO) //Deposit is zero
                    ModelState.AddModelError("AMOUNT_IS_ZERO", validation);
            }
            /*WHY NOT?*/
            //deposit.salesContract.updateDownPaymentStatus();
            
            ViewBag.isHOA = isHOA;
            return View(deposit);
        }

        // GET: /Deposit/Delete/5
         [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            return View(deposit);
        }

        // POST: /Deposit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER)]
         public ActionResult DeleteConfirmed(int id)
        {
            Deposit deposit = db.Deposits.Find(id);
            db.Deposits.Remove(deposit);
            db.SaveChanges();

            //deposit.salesContract.updateDownPaymentStatus();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER
            + "," + AccountRolesNames.PAYMENTS_RESERVATIONS + "," + AccountRolesNames.OWNER_SERVICES)]
        public ActionResult listHOADeposits(int id = 0, String contractType = "",int year=0)
        {
            if (id == 0 || String.IsNullOrEmpty(contractType))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //If year was not indicated, it takes present year by default
            year = year == 0 ? DateTime.Now.Year : year;
            DateTime initialHOAMonth = new DateTime();
            String contractNumber = "";
            String condoName = "";
            List<Deposit> deposits = new List<Deposit>();
            decimal HOAYearlyPayment = 0;
            //Get contracts and HOA payments for requested year
            //For SalesContracts Fractionals
            if (contractType == Deposit.ContractTypes.FA || contractType == Deposit.ContractTypes.BA)
            {
                var contract = db.SalesContracts.Find(id);
                if (contract == null)
                    return HttpNotFound();
                else{
                    initialHOAMonth = contract.InitialHOAMonth;
                    contractNumber = contract.contractNumber;
                    HOAYearlyPayment = contract.HOAYearlyPayment;
                    condoName = contract.condo.name;
                    deposits = contract.HOA_deposits.Where(d => d.DepositDate.Year == year).ToList();
                }
                    
            }
                //For RealState Contracts
            else if (contractType == Deposit.ContractTypes.RS)
            {
                var contract = db.RealStateContracts.Find(id);
                if (contract == null)
                    return HttpNotFound();
                else { 
                    initialHOAMonth = contract.InitialHOAMonth;
                    contractNumber = contract.realStateContractID.ToString();
                    HOAYearlyPayment = contract.HOAYearlyPayment;
                    condoName = contract.condo.name;
                    deposits = contract.HOA_deposits.Where(d => d.DepositDate.Year == year).ToList();
                }
            }
            ViewBag.contractID = id;
            ViewBag.contractType = contractType;
            ViewBag.contractNumber = contractNumber;
            ViewBag.initialHOAMonth = initialHOAMonth;
            ViewBag.condoName = condoName;

            LinkedList<VMHOAMontlhyFeeRow> HOAFees = generateHOAList(id, contractType,year, initialHOAMonth, HOAYearlyPayment);

            ViewBag.HOAFees = HOAFees.ToList();
            ViewBag.year = year;
            return View(deposits);
        }

        /// <summary>
        /// Given contract, year and deposits, this method calculates the HOA Fees Report
        /// returning a linked list
        /// </summary>
        /// <param name="contractID"></param>
        /// <param name="contractType"></param>
        /// <param name="year"></param>
        /// <param name="initialHOAMonth"></param>
        /// <param name="HOAYearlyPayment"></param>
        /// <param name="deposits"></param>
        /// <returns></returns>
        private LinkedList<VMHOAMontlhyFeeRow> generateHOAList(int contractID, String contractType, int year, 
            DateTime initialHOAMonth, decimal HOAYearlyPayment)
        {
            //Get contracts and HOA payments for requested year
            //For SalesContracts Fractionals
            List<Deposit> deposits = new List<Deposit>();
            if (contractType == Deposit.ContractTypes.FA || contractType == Deposit.ContractTypes.BA)
            {
                var contract = db.SalesContracts.Find(contractID);
                deposits = contract.HOA_deposits.Where(d => d.DepositDate.Year == year).ToList();
            }//For RealState Contracts
            else if (contractType == Deposit.ContractTypes.RS)
            {
                var contract = db.RealStateContracts.Find(contractID);
                deposits = contract.HOA_deposits.Where(d => d.DepositDate.Year == year).ToList();
            }
            //Creates HOA Fees list where every deposit will be distribuited
            LinkedList<VMHOAMontlhyFeeRow> HOAFees = new LinkedList<VMHOAMontlhyFeeRow>();
            DateTime month;
            for (int c = 1; c <= 12; c++)
            {
                month = new DateTime(year, c, 1);
                HOAFees.AddLast(new VMHOAMontlhyFeeRow(HOAYearlyPayment / 12, month, initialHOAMonth));
            }

            /***********HERE IS THE REAL DEAL **********/
            //For each generated month, deposits distribution is done
            int monthNumber = 1;
            decimal paidInMonths = 0;
            decimal interestPaidIn3Months = 0;
            foreach (VMHOAMontlhyFeeRow row in HOAFees)
                if (row.active)
                {
                    //Get the sum of every deposit in this month verified and not refund
                    row.paidFee = deposits.Where(d => d.verification && !d.Refund
                    && d.DepositDate.Month == row.month.Month).Sum(d => d.Amount);

                    row.balance = row.fee + row.paidFee;
                    row.totalOwing = row.fee + row.paidFee+row.penalties;
                    //Get previous month in the same year
                    if (HOAFees.FindLast(row).Previous != null)
                    {
                        var lastIndex = HOAFees.FindLast(row).Previous;
                        VMHOAMontlhyFeeRow prev = lastIndex.Value;
                        row.balance += prev.balance;

                        //2% interest over previous owing
                        row.interest = prev.totalOwing < 0 ? prev.totalOwing * ((decimal).02) : 0;
                        row.totalOwing += row.interest + prev.totalOwing;

                        //Each 3 months
                        if (monthNumber % 3 == 0)
                        {
                            //Calculates total paid fee in the last 3 months
                            paidInMonths = row.paidFee + prev.paidFee + lastIndex.Previous.Value.paidFee;
                            interestPaidIn3Months = row.interest + prev.interest + lastIndex.Previous.Value.interest;
                            //Penalties apply monthly fees and interest were not covered
                            if(paidInMonths < HOAYearlyPayment/12*3-interestPaidIn3Months)
                            {
                                paidInMonths = row.paidFee;
                                while (lastIndex != null)
                                {
                                    prev = lastIndex.Value;
                                    paidInMonths += prev.paidFee;
                                    lastIndex = lastIndex.Previous;
                                }

                                if (paidInMonths < HOAYearlyPayment) { 
                                    row.penalties = -125;
                                    row.totalOwing += row.penalties;
                                }
                            }
                            
                        }
                    }
                    //Get previous month in the last year
                    else if ( year-1 >= initialHOAMonth.Year )
                    {
                        //TODO: Method that returns the last month of previous year
                        LinkedList<VMHOAMontlhyFeeRow> HOAFeesPrevYear = generateHOAList(contractID, contractType,
                            year - 1, initialHOAMonth, HOAYearlyPayment); //Recursive invocation

                        VMHOAMontlhyFeeRow prev = HOAFeesPrevYear.Last.Value;
                        row.balance += prev.balance;

                        //2% interest over previous owing
                        row.interest = prev.totalOwing < 0 ? prev.totalOwing * ((decimal).02) : 0;
                        row.totalOwing += row.interest + prev.totalOwing;
                    }

                    monthNumber++;
                }

            return HOAFees;
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
