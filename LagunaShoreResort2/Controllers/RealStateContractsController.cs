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
using CommissionPaymentStatus = LagunaShoreResort2.Models.ViewModels.VMFilterContractReport.CommissionPaymentStatus;
using jsreport.Client;
using jsreport.Client.Entities;
using Novacode;

namespace LagunaShoreResort2.Controllers
{
    [Authorize]
    public class RealStateContractsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RealStateContracts
        public ActionResult Index()
        {
            var realStateContracts = db.RealStateContracts.Include(r => r.clientAssigned).Include(r => r.clientAssignor).Include(r => r.salesMember);
            return View(realStateContracts.ToList());
        }

        // GET: RealStateContracts/Details/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR /*+ "," + AccountRolesNames.CONTRACT_MANAGER*/)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealStateContract realStateContract = db.RealStateContracts.Include("deposits").Single(rs => rs.realStateContractID == id);
            if (realStateContract == null)
            {
                return HttpNotFound();
            }
            //Calculate amortization schedules
            ViewBag.payments = Deposit.getDepositsInPayments(null, null, realStateContract);  

            return View(realStateContract);
        }

        // GET: RealStateContracts/Create
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR /*+ "," + AccountRolesNames.CONTRACT_OFFICER*/)]
        public ActionResult Create(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Se obtiene los datos del cliente nesesarios para realizar el contrato//
            var cliente = db.Clients.Find(id);
            if (cliente == null) {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            ViewBag.clientAssigned = cliente;
            ViewBag.clientAssignorID = new SelectList(db.Clients, "clientID", "firstName");
            ViewBag.salesMemberID = new SelectList(db.SalesMembers, "salesMemberID", "firtName");
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name");

            return View();
        }

        // POST: RealStateContracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR /*+ "," + AccountRolesNames.CONTRACT_OFFICER*/)]
        public ActionResult Create([Bind(Include = "realStateContractID,type,MLS,ownershipHeld,saleAmount,saleDate,closingDate,"+
            "transfererName,salesMemberID,clientAssignedID,clientAssignorID,condoID,closingCost,paymentsQuantity,NumberofDownPayments,"+
            "interestRate,currency,deposit,InitialHOAMonth,HOAYearlyPayment")]
            RealStateContract realStateContract)
        {
            if (ModelState.IsValid)
            {
                //If clientAssignor 
                realStateContract.clientAssignorID = realStateContract.clientAssignorID == 0 ? null : realStateContract.clientAssignorID;
                db.RealStateContracts.Add(realStateContract);
                db.SaveChanges();
                return RedirectToAction("details", "client", new { id = realStateContract.clientAssignedID });
            }

            var cliente = db.Clients.Find(realStateContract.clientAssignedID);
            ViewBag.clientAssigned = cliente;
            ViewBag.clientAssignedID = new SelectList(db.Clients, "clientID", "firstName", realStateContract.clientAssignedID);
            ViewBag.salesMemberID = new SelectList(db.SalesMembers, "salesMemberID", "firtName", realStateContract.salesMemberID);
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name");

            return View(realStateContract);
        }

        // GET: RealStateContracts/Edit/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR /*+ "," + AccountRolesNames.CONTRACT_OFFICER +*/
           /* "," + AccountRolesNames.CONTRACT_MANAGER*/)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealStateContract realStateContract = db.RealStateContracts.Find(id);
            if (realStateContract == null)
            {
                return HttpNotFound();
            }
            //FALTA TERMINAR EDICION DE CONTRATO DE REAL STATE
            SelectList clients = new SelectList(db.Clients, "clientID", "firstName", realStateContract.clientAssignedID);
            ViewBag.clientAssigned = realStateContract.clientAssigned;
            ViewBag.clientAssignorID = clients;
            ViewBag.salesMemberID = new SelectList(db.SalesMembers, "salesMemberID", "firtName", realStateContract.salesMemberID);
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", realStateContract.condoID);
            return View(realStateContract);
        }

        // POST: RealStateContracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR /*+ "," + AccountRolesNames.CONTRACT_OFFICER + "," + AccountRolesNames.CONTRACT_MANAGER*/)]
        public ActionResult Edit([Bind(Include = "realStateContractID,type,MLS,ownershipHeld,saleAmount,saleDate,closingDate,"+
            "transfererName,salesMemberID,clientAssignedID,clientAssignorID,condoID,closingCost,paymentsQuantity,NumberofDownPayments,"+
            "interestRate,currency,deposit,InitialHOAMonth,HOAYearlyPayment")]
            RealStateContract realStateContract)
        {
            if (ModelState.IsValid)
            {
                db.Entry(realStateContract).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("details", new { id = realStateContract.realStateContractID});
            }
            SelectList clients = new SelectList(db.Clients, "clientID", "firstName", realStateContract.clientAssignedID);
            ViewBag.clientAssignedID = clients;
            ViewBag.clientAssignorID = clients;
            ViewBag.salesMemberID = new SelectList(db.SalesMembers, "salesMemberID", "firtName", realStateContract.salesMemberID);
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", realStateContract.condoID);
            return View(realStateContract);
        }

        // GET: RealStateContracts/Delete/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealStateContract realStateContract = db.RealStateContracts.Find(id);
            if (realStateContract == null)
            {
                return HttpNotFound();
            }
            return View(realStateContract);
        }

        // POST: RealStateContracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult DeleteConfirmed(int id)
        {
            RealStateContract realStateContract = db.RealStateContracts.Find(id);
            db.RealStateContracts.Remove(realStateContract);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: SalesContract/Report
        [HttpGet]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CEO +
            "," + AccountRolesNames.CONTRACT_MANAGER + "," + AccountRolesNames.ACCOUNTANT +
            "," + AccountRolesNames.OWNER_SERVICES +
            "," + AccountRolesNames.SALES_MANAGER + "," + AccountRolesNames.CONCIERGE +
            "," + AccountRolesNames.VLO + "," + AccountRolesNames.PAYMENTS_RESERVATIONS)]
        public ActionResult Report(VMFilterContractReport vmFilter)
        {

            vmFilter.contractTypeName = SalesContract.ContractTypeName.REAL_STATE_CONTRACTS; //Controller Name
            /*Filter by salesContracts and is in time range filter*/
            Boolean isNotFiltering = Request.Url.Query.Count() == 0;

            //If filters were not received
            if (isNotFiltering || !securityRequiermentsMet(vmFilter))
            {
                ViewBag.vmFilter = vmFilter;
                return View();
            }
            else
            {
                //Get contracts in range time filter
                var filteredContractsRS = from c in db.RealStateContracts.Include("deposits").Where(pay => pay.closingDate >= vmFilter.start &&
                    pay.closingDate <= vmFilter.end).ToList()
                            select new VMRealState(c);

                //If more filtering is needed by comission payment status
                if (vmFilter.commissionPaymentStatus != "All")
                {
                    //bool isDownPaymentsPaid = true, isRequested = true, isCommissionPaid = true;

                    //If downpayment is completed but commission was not requested neither paid
                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.DOWNPAYMENT_PAID)
                        filteredContractsRS = filteredContractsRS.Where(pay => pay.downPaymentPaid == true);//Filter

                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.DOWNPAYMENT_NOT_PAID)
                        filteredContractsRS = filteredContractsRS.Where(pay => pay.downPaymentPaid == false);//Filter

                    //Salescontracst where commissions needs to be paid
                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.COMMISSION_PAID)
                        filteredContractsRS = filteredContractsRS.Where(pay => pay.commissionPaid == true);//Filter

                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.COMMISSION_REQUESTED)
                        filteredContractsRS = filteredContractsRS.Where(pay => pay.requestToAccountant == true);//Filter
                }

                if (filteredContractsRS.Count() > 0)
                { //Data to generate summary is calculated
                    decimal b = 0, c = 0, e = 0, f = 0, g = 0;
                    Double a = 0, d = 0;
                    foreach (VMRealState item in filteredContractsRS)
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
                return View(filteredContractsRS);
            }
        }

        public FileResult DownloadContractFile(int? id)
        {
            //intancias del cliente con el contrato y el condominio que estan relacionados
            RealStateContract rsContract = db.RealStateContracts.Find(id);
            Client client = rsContract.clientAssigned;
            Client clientAssignor = rsContract.clientAssignor;
            Condo condo = rsContract.condo;

            String DownPaymentsIngles = "";
            String DownPaymentsEspañol = "";
            String PaymentMonths = "";
            String PaymentMonthInitialDate = "";
            String PaymentMonthEnd = "";
            int contadorDePagos = 0;

            //Se obtiene el plan de pago del DownPayment en español y en ingles
            List<PaymentDeposits> payments = Deposit.getDepositsInPayments(null, null, rsContract);
            foreach (PaymentDeposits p in payments)
            {
                if (p.payment.paymentType == Deposit.PaymentTypes.DOWNPAYMENT && p.deposit.getAmountInContractCurrency(rsContract.currency) == 0)
                {

                    DownPaymentsIngles = DownPaymentsIngles + ("$" + String.Format("{0:n}", p.payment.Amortization) + " USD to be paid on " + p.payment.dateToPay.ToString("MMM/dd/yyy") /*ToShortDateString()*/ + "\r\n");
                    DownPaymentsEspañol = DownPaymentsEspañol + ("$" + String.Format("{0:n}", p.payment.Amortization) + " USD para ser pagado en " + p.payment.dateToPay.ToString("MMM/dd/yyy") /*ToShortDateString() */+ "\r\n");
                }
                else if (p.payment.paymentType != Deposit.PaymentTypes.DOWNPAYMENT && contadorDePagos < 1)
                {
                    PaymentMonths = String.Format("{0:n}", Deposit.getPMT(null, null, rsContract));
                    PaymentMonthInitialDate = p.payment.dateToPay.ToString("MMM/dd/yyyy");
                    PaymentMonthEnd = (p.payment.dateToPay.AddMonths(rsContract.paymentsQuantity - 1)).ToString("MMM/dd/yyyy");
                    contadorDePagos++;
                }
            }

            //Se crea Instancias de clases convertidores de moneda a letras
            CurrencyTranslator currencyTraslateToInglish = new CurrencyTranslator();
            VMConverterCurrencyInSpanish currencyTraslateToSpanish = new VMConverterCurrencyInSpanish();

            String CantidadEnIngles = currencyTraslateToInglish.TranslateCurrency(rsContract.saleAmount).ToUpper();
            String CantidadEnEspañol = currencyTraslateToSpanish.enletras(rsContract.saleAmount.ToString().ToUpper());
            //Se toma la ubicacion del documento en blanco
            String NewContract = Server.MapPath("~/App_Data/RSContratoEnBlanco.docx");
            //Se toma el contrato Original y se convierte en arreglo de bytes
            byte[] fileBytesContrato = System.IO.File.ReadAllBytes(Server.MapPath("~/App_Data/RSContrato.docx"));
            //Se sobreescribe el documento en blanco con los datos del contrato Original
            System.IO.File.WriteAllBytes(NewContract, fileBytesContrato);
            String phone2 = client.secondPhoneNumber;
            String email2 = client.secondEmailAddress;
            if (phone2 == null)
            {
                phone2 = "";
            }
            if (email2 == null)
            {
                email2 = "";
            }


            String nombrePrimerCliente = (client.firstName + " " + client.middleName + " " + client.lastName).ToUpper();
            String nombreSegundoCliente = (client.secondFirstName + " " + client.secondMiddleName + " " + client.secondLastName).ToUpper();
            //Se abre el documento modificado
            var doc = DocX.Load(NewContract);
            //Se remplazan informacion del documento
            doc.ReplaceText("_name1", nombrePrimerCliente);
            doc.ReplaceText("_name2", nombreSegundoCliente);
            doc.ReplaceText("_nationality", client.nationality);
            doc.ReplaceText("_address", client.primaryResidenceAddress);
            doc.ReplaceText("_city", client.city);
            doc.ReplaceText("_state", client.state);
            doc.ReplaceText("_zipCode", client.zipCode);
            doc.ReplaceText("_phone1", client.primaryPhoneNumber);
            doc.ReplaceText("_phone2", phone2);
            doc.ReplaceText("_email1", client.emailAddress);
            doc.ReplaceText("_phase", (condo.phase).ToString());
            /* doc.ReplaceText("_shareIngles", shareIngles);
             doc.ReplaceText("_shareEspanol", shareEspanol);*/
            doc.ReplaceText("_propertyName", condo.name);
            doc.ReplaceText("_block", condo.block);
            doc.ReplaceText("_lot", condo.lot);
            doc.ReplaceText("_sqmts", condo.sqmt);
            try
            {

                doc.ReplaceText("_typeOfID", client.typeOfID);
                doc.ReplaceText("_nationality", client.nationality);
            }
            catch { }
            decimal totals = (rsContract.saleAmount + (decimal)rsContract.closingCost);
            String total = totals.ToString("#,##0.00");
            decimal downP = 0;
            if (rsContract.deposit > 0 || rsContract.deposit != null)
            {
                downP = (decimal)rsContract.deposit;

            }
            else
            {
                downP = totals * (decimal).3;
            }
            String dp = downP.ToString("#,##0.00");
            Deposit initialDeposit = Deposit.getInitialDeposit(null, null, rsContract);
            doc.ReplaceText("_saleAmount", (rsContract.saleAmount).ToString("#,##0.00"));
            doc.ReplaceText("_closingCost", (rsContract.closingCost).ToString("#,##0.00"));
            doc.ReplaceText("_totalPrice", total);
            doc.ReplaceText("_downPayment", dp);
            doc.ReplaceText("_precioPalabrasENG", CantidadEnIngles);
            doc.ReplaceText("_precioPalabrasEsp", CantidadEnEspañol);
            doc.ReplaceText("_monthsFinance", rsContract.paymentsQuantity.ToString());
            doc.ReplaceText("_DownPaymentIngles", DownPaymentsIngles);
            doc.ReplaceText("_DownPaymentEspañol", DownPaymentsEspañol);
            doc.ReplaceText("_initialPayment", initialDeposit.Amount.ToString("#,##0.00"));
            doc.ReplaceText("_initialPayDate", initialDeposit.DepositDate.ToString("MMM/dd/yyyy"));
            doc.ReplaceText("_contractDate", rsContract.closingDate.ToString("MMM/dd/yyyy"));
            /*doc.ReplaceText("_liner1", LinerName);
            doc.ReplaceText("_closer1", CloserName);*/
            //doc.ReplaceText("_vlo", VloName);
            String balance = (totals - downP).ToString("#,##0.00");
            doc.ReplaceText("_balance", balance);
            doc.ReplaceText("_interestRate", (rsContract.interestRate).ToString());
            doc.ReplaceText("_monthlyPayment", PaymentMonths);
            doc.ReplaceText("_monthPaymentDate", PaymentMonthInitialDate);
            doc.ReplaceText("_paymentsEnd", PaymentMonthEnd);
            doc.ReplaceText("_email1", client.emailAddress);
            doc.ReplaceText("_salesRepresentative", rsContract.salesMember.firtName + " " + rsContract.salesMember.lastName);
            doc.ReplaceText("_currency", rsContract.currency);
            doc.ReplaceText("_propertyListPrice", condo.listpriceMax.ToString("#,##0.00"));
            /*doc.ReplaceText("_attachmentsIng", salesContract.attachmentsEnglish == null ? "" : salesContract.attachmentsEnglish);
            doc.ReplaceText("_attachmentsSpan", salesContract.attachmentsSpanish == null ? "" : salesContract.attachmentsSpanish);*/
            doc.Save();
            //Se convierte el nuevo documento en un arreglo de bytes
            byte[] fileBytesNuevoContrato = System.IO.File.ReadAllBytes(NewContract);
            //Nombre del docuemnto
            string fileDownloadName = Deposit.ContractTypes.RS+"_"+rsContract.realStateContractID + nombrePrimerCliente + "&" + nombreSegundoCliente + ".docx";
            //Se regresa el nuevo contrato
            return File(fileBytesNuevoContrato, System.Net.Mime.MediaTypeNames.Application.Octet, fileDownloadName);
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
