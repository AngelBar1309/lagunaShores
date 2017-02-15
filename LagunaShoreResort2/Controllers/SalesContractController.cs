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
using jsreport.Client;
using System.IO;
using jsreport.Client.Entities;

namespace LagunaShoreResort2.Controllers
{
    [Authorize]
    public class SalesContractController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SalesContract

        public ActionResult Index(string buscadorContratos, int clientID = 0)
        {
            var salesContracts = db.SalesContracts;
            String contractType = Request["contractType"];

            if (!String.IsNullOrEmpty(buscadorContratos))
            {

                var result = from c in salesContracts
                             where
                                 c.contractNumber.Contains(buscadorContratos) ||
                                 c.client.legalName.Contains(buscadorContratos) ||
                                 c.client.secondLegalName.Contains(buscadorContratos)
                             select c;
                result = result.OrderByDescending(c => c.contractDate);
                if (contractType == "ContratosCancelados")
                {
                    result = result.Where(c => c.canceledContract == true);
                }
                else if (contractType == "ContratosNoVerificados")
                { result = result.Where(c => c.verifiedByAdmin == false); }

                if (User.IsInRole(AccountRolesNames.ACCOUNTANT))
                    result = result.Where(c => c.requestToAccountant);

                return View(result.ToList().Take(30));
            }
            else
            {
                List<SalesContract> defaultResult = salesContracts.OrderBy(c => c.contractDate).ToList();
                if (User.IsInRole(AccountRolesNames.ACCOUNTANT))
                {
                    defaultResult = salesContracts.Where(c => c.requestToAccountant).Take(30).ToList();
                }
                else
                {
                    if (contractType == "ContratosCancelados")
                    {
                        defaultResult = salesContracts.Where(c => c.canceledContract == true).ToList();
                    }
                    else if (contractType == "ContratosNoVerificados")
                    {
                        defaultResult = salesContracts.Where(c => c.verifiedByAdmin == false).ToList();
                    }
                    //SdefaultResult = salesContracts.OrderBy(c => c.contractDate).Take(30).ToList();
                }

                return View(defaultResult.Take(30));
            }
        }
        ////jsReportsTesting
        //[Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CEO +
        //   "," + AccountRolesNames.CONTRACT_MANAGER + "," + AccountRolesNames.ACCOUNTANT +
        //   "," + AccountRolesNames.OWNER_SERVICES +
        //   "," + AccountRolesNames.SALES_MANAGER + "," + AccountRolesNames.CONCIERGE +
        //   "," + AccountRolesNames.VLO + "," + AccountRolesNames.PAYMENTS_RESERVATIONS)]
        //public async System.Threading.Tasks.Task<ActionResult> ReportingService(VMFilterSalesContractReport vmFilter)
        //{
        //    //connecting to jsReportOnline
        //    var _reportingService = new ReportingService("https://lagunashoresresort.jsreportonline.net",
        //                                      "deisy@lagunashoresresort.com", "reports0217");

        //    var report = await _reportingService.RenderAsync("g1xcKBanJc", new
        //    {
        //        someData = "foo"
        //    });
        //    new StreamReader(report.Content).ReadToEnd();
        //    return 0;
        //}

        // GET: SalesContract/Report
        [HttpGet]
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CEO +
            "," + AccountRolesNames.CONTRACT_MANAGER + "," + AccountRolesNames.ACCOUNTANT +
            "," + AccountRolesNames.OWNER_SERVICES +
            "," + AccountRolesNames.SALES_MANAGER + "," + AccountRolesNames.CONCIERGE +
            "," + AccountRolesNames.VLO + "," + AccountRolesNames.PAYMENTS_RESERVATIONS)]
        public ActionResult Report(VMFilterContractReport vmFilter)
        {
            vmFilter.contractTypeName = SalesContract.ContractTypeName.SALES_CONTRACT; //Controller Name
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
                ////connecting to jsReportOnline //commented for later
                //var _reportingService = new ReportingService("https://lagunashoresresort.jsreportonline.net",
                //                                  "deisy@lagunashoresresort.com", "reports0217");
                //var report = await _reportingService.RenderAsync(new RenderRequest()
                //{
                //    template = new Template()
                //    {
                //        recipe = "html",
                //        engine = "jsrender",
                //        content = "some dynamic template content"
                //    },
                //    data = new { firstName = "Jan", surname = "Blaha" }
                //});

                //Get Fractional contracts in range time filter
                var query = from fc in db.SalesContracts.Where(pay => pay.contractDate >= vmFilter.start &&
                    pay.contractDate <= vmFilter.end).ToList()
                            select new VMSalesContract(fc);


                //Get Fractional contracts in range time filter
                var query2 = from tm in db.trialMemberships.Where(pay => pay.tmContractDate >= vmFilter.start &&
                    pay.tmContractDate <= vmFilter.end).ToList()
                             select new VMTrialMemberships(tm);

                //If more filtering is needed by comission payment status
                if (vmFilter.commissionPaymentStatus != "All")
                {
                    //bool isDownPaymentsPaid = true, isRequested = true, isCommissionPaid = true;

                    //If downpayment is completed but commission was not requested neither paid
                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.DOWNPAYMENT_PAID)
                    {
                        query = query.Where(pay => pay.downPaymentPaid == true);//Filter
                        query2 = query2.Where(pay => pay.downPaymentPaid == true);//Filter

                    }

                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.DOWNPAYMENT_NOT_PAID)
                    {
                        query = query.Where(pay => pay.downPaymentPaid == false);//Filter
                        query2 = query2.Where(pay => pay.downPaymentPaid == false);//Filter
                    }
                    //Salescontracst where commissions needs to be paid
                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.COMMISSION_PAID)
                    {
                        query = query.Where(pay => pay.commissionPaid == true);//Filter
                        query2 = query2.Where(pay => pay.commissionPaid == true);//Filter
                    }

                    if (vmFilter.commissionPaymentStatus == CommissionPaymentStatus.COMMISSION_REQUESTED)
                    {
                        query = query.Where(pay => pay.requestToAccountant == true);//Filter
                        query2 = query2.Where(pay => pay.requestToAccountant == true);//Filter
                    }
                }

                if (query.Count() > 0) //Data to generate summary is calculated
                {

                    decimal a1 = 0, b1 = 0, c1 = 0, d1 = 0, e1 = 0, f1 = 0, can1 = 0, pmt = 0, pmtc = 0;
                    //TOTAL PMT PER INTEREST TYPE
                    decimal zero = 0, five = 0, fiveNine = 0, sixNine = 0, seven = 0, eight = 0,
                        nine = 0, nineNine = 0, ten = 0, eleven = 0,
                        twelve = 0, twleveOne = 0, twelveSeven = 0, twelveNine = 0, fourteen = 0,
                        fourteenNine = 0, seventeenEight = 0, seventeenNine = 0,
                        eightTeenNine = 0;
                    //TOTAL SALE AMOUNT PER INTEREST TYPE
                    decimal zeroSA = 0, fiveSA = 0, fiveNineSA = 0, sixNineSA = 0, sevenSA = 0, eightSA = 0,
                       nineSA = 0, nineNineSA = 0, tenSA = 0, elevenSA = 0,
                       twelveSA = 0, twleveOneSA = 0, twelveSevenSA = 0, twelveNineSA = 0, fourteenSA = 0,
                       fourteenNineSA = 0, seventeenEightSA = 0, seventeenNineSA = 0,
                       eightTeenNineSA = 0;
                    //TOTAL BALANCE PER INTEREST TYPE
                    decimal zeroB = 0, fiveB = 0, fiveNineB = 0, sixNineB = 0, sevenB = 0, eightB = 0,
                       nineB = 0, nineNineB = 0, tenB = 0, elevenB = 0,
                       twelveB = 0, twleveOneB = 0, twelveSevenB = 0, twelveNineB = 0, fourteenB = 0,
                       fourteenNineB = 0, seventeenEightB = 0, seventeenNineB = 0,
                       eightTeenNineB = 0;

                    foreach (VMSalesContract item in query)
                    {
                        decimal dp = item.saleAmount * (decimal).30;


                        //decimal downPayment = item.saleAmount * (decimal).30;
                        //if (item.downPaymentPaid == true || item.balance <= downPayment)
                        //{
                        if (item.canceledContract == true)
                        {
                            can1++;
                            if (item.currency == "MXN")
                            {
                                if (item.contractDate.Year == 2013)
                                {
                                    a1 += item.saleAmount / (decimal)12.76;
                                    b1 += item.balance / (decimal)12.76;
                                    c1 += item.totalPaid / (decimal)12.76;
                                    pmtc += item.PMT / (decimal)12.76;
                                }
                                else if (item.contractDate.Year == 2014)
                                {
                                    a1 += item.saleAmount / (decimal)13.31;
                                    b1 += item.balance / (decimal)13.31;
                                    c1 += item.totalPaid / (decimal)13.31;
                                    pmtc += item.PMT / (decimal)13.31;
                                }
                                else if (item.contractDate.Year == 2015)
                                {
                                    a1 += item.saleAmount / (decimal)15.87;
                                    b1 += item.balance / (decimal)15.87;
                                    c1 += item.totalPaid / (decimal)15.87;
                                    pmtc += item.PMT / (decimal)15.87;
                                }


                            }
                            else if (item.currency == "USD")
                            {
                                a1 += item.saleAmount;
                                if (item.balance <= 0)
                                {

                                }
                                else
                                {
                                    pmtc += item.PMT;
                                }
                                double apr = item.APR;
                                b1 += item.balance;
                                c1 += item.totalPaid;
                            }
                        }
                        else
                        {
                            if (item.currency == "MXN")
                            {
                                if (item.contractDate.Year == 2013)
                                {
                                    d1 += item.saleAmount / (decimal)12.76;
                                    e1 += item.balance / (decimal)12.76;
                                    f1 += item.totalPaid / (decimal)12.76;
                                    if (item.balance <= 0)
                                    {

                                    }
                                    else
                                    {
                                        zeroSA += dp / (decimal)13.31; ;
                                        if (item.APR == 0)
                                        {
                                            zero += item.PMT / (decimal)13.31;
                                            zeroSA += item.saleAmount / (decimal)12.76;
                                            zeroB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 5)
                                        {
                                            five += item.PMT / (decimal)12.76;
                                            fiveNineSA += item.saleAmount / (decimal)12.76;
                                            fiveNineB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 5.9)
                                        {
                                            fiveNine += item.PMT / (decimal)12.76;
                                            fiveNineSA += item.saleAmount / (decimal)12.76;
                                            fiveNineB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 6.9 || item.APR == 69)
                                        {
                                            sixNine += item.PMT / (decimal)12.76;
                                            sixNineSA += item.saleAmount / (decimal)12.76;
                                            sixNineB += item.balance / (decimal)12.76; ;
                                        }
                                        if (item.APR == 7)
                                        {
                                            seven += item.PMT / (decimal)12.76;
                                            sevenSA += item.saleAmount / (decimal)12.76; ;
                                            sevenB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 8)
                                        {
                                            eight += item.PMT / (decimal)12.76;
                                            eightSA += item.saleAmount / (decimal)12.76;
                                            eightB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 9)
                                        {
                                            nine += item.PMT / (decimal)12.76;
                                            nineSA += item.saleAmount / (decimal)12.76;
                                            nineB += item.balance / (decimal)12.76;

                                        }
                                        if (item.APR == 9.9)
                                        {
                                            nineNine += item.PMT / (decimal)12.76;
                                            nineNineSA += item.saleAmount / (decimal)12.76;
                                            nineNineB += item.balance / (decimal)12.76;

                                        }
                                        if (item.APR == 10)
                                        {
                                            ten += item.PMT / (decimal)12.76;
                                            tenSA += item.saleAmount / (decimal)12.76;
                                            tenB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 11)
                                        {
                                            eleven += item.PMT / (decimal)12.76;
                                            elevenSA += item.saleAmount / (decimal)12.76;
                                            elevenB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 12)
                                        {
                                            twelve += item.PMT / (decimal)12.76;
                                            twelveSA += item.saleAmount / (decimal)12.76;
                                            twelveB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 12.1)
                                        {
                                            twleveOne += item.PMT / (decimal)12.76;
                                            twleveOneSA += item.saleAmount / (decimal)12.76;
                                            twleveOneB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 12.7)
                                        {
                                            twelveSeven += item.PMT / (decimal)12.76;
                                            twelveSevenSA += item.saleAmount / (decimal)12.76;
                                            twelveSevenB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 12.9)
                                        {
                                            twelveNine += item.PMT / (decimal)12.76;
                                            twelveNineSA += item.saleAmount / (decimal)12.76;
                                            twelveNineB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 14)
                                        {
                                            fourteen += item.PMT / (decimal)12.76;
                                            fourteenSA += item.saleAmount / (decimal)12.76;
                                            fourteenB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 14.9)
                                        {
                                            fourteenNine += item.PMT / (decimal)12.76;
                                            fourteenNineSA += item.saleAmount / (decimal)12.76;
                                            fourteenNineB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 17.8)
                                        {
                                            seventeenEight += item.PMT / (decimal)12.76;
                                            seventeenEightSA += item.saleAmount / (decimal)12.76;
                                            seventeenEightB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 17.9)
                                        {
                                            seventeenNine += item.PMT / (decimal)12.76;
                                            seventeenNineSA += item.saleAmount / (decimal)12.76;
                                            seventeenNineB += item.balance / (decimal)12.76;
                                        }
                                        if (item.APR == 18.9)
                                        {
                                            eightTeenNine += item.PMT / (decimal)12.76;
                                            eightTeenNineSA += item.saleAmount / (decimal)12.76;
                                            eightTeenNineB += item.balance / (decimal)12.76;
                                        }
                                    }
                                }
                                else if (item.contractDate.Year == 2014)
                                {
                                    d1 += item.saleAmount / (decimal)13.31;
                                    e1 += item.balance / (decimal)13.31;
                                    f1 += item.totalPaid / (decimal)13.31;
                                    if (item.balance <= 0)
                                    {

                                    }
                                    else
                                    {
                                        zeroSA += dp / (decimal)13.31; ;
                                        if (item.APR == 0)
                                        {
                                            zero += item.PMT / (decimal)13.31;
                                            zeroSA += item.saleAmount / (decimal)13.31;
                                            zeroB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 5)
                                        {
                                            five += item.PMT / (decimal)13.31;
                                            fiveNineSA += item.saleAmount / (decimal)13.31;
                                            fiveNineB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 5.9)
                                        {
                                            fiveNine += item.PMT / (decimal)13.31;
                                            fiveNineSA += item.saleAmount / (decimal)13.31;
                                            fiveNineB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 6.9 || item.APR == 69)
                                        {
                                            sixNine += item.PMT / (decimal)13.31;
                                            sixNineSA += item.saleAmount / (decimal)13.31;
                                            sixNineB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 7)
                                        {
                                            seven += item.PMT / (decimal)13.31;
                                            sevenSA += item.saleAmount / (decimal)13.31;
                                            sevenB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 8)
                                        {
                                            eight += item.PMT / (decimal)13.31;
                                            eightSA += item.saleAmount / (decimal)13.31;
                                            eightB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 9)
                                        {
                                            nine += item.PMT / (decimal)13.31;
                                            nineSA += item.saleAmount / (decimal)13.31;
                                            nineB += item.balance / (decimal)13.31;

                                        }
                                        if (item.APR == 9.9)
                                        {
                                            nineNine += item.PMT / (decimal)13.31;
                                            nineNineSA += item.saleAmount / (decimal)13.31;
                                            nineNineB += item.balance / (decimal)13.31;

                                        }
                                        if (item.APR == 10)
                                        {
                                            ten += item.PMT / (decimal)13.31;
                                            tenSA += item.saleAmount / (decimal)13.31;
                                            tenB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 11)
                                        {
                                            eleven += item.PMT / (decimal)13.31;
                                            elevenSA += item.saleAmount / (decimal)13.31;
                                            elevenB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 12)
                                        {
                                            twelve += item.PMT / (decimal)13.31;
                                            twelveSA += item.saleAmount / (decimal)13.31;
                                            twelveB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 12.1)
                                        {
                                            twleveOne += item.PMT / (decimal)13.31;
                                            twleveOneSA += item.saleAmount / (decimal)13.31;
                                            twleveOneB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 12.7)
                                        {
                                            twelveSeven += item.PMT / (decimal)13.31; ;
                                            twelveSevenSA += item.saleAmount / (decimal)13.31;
                                            twelveSevenB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 12.9)
                                        {
                                            twelveNine += item.PMT / (decimal)13.31;
                                            twelveNineSA += item.saleAmount / (decimal)13.31;
                                            twelveNineB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 14)
                                        {
                                            fourteen += item.PMT / (decimal)13.31;
                                            fourteenSA += item.saleAmount / (decimal)13.31;
                                            fourteenB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 14.9)
                                        {
                                            fourteenNine += item.PMT / (decimal)13.31;
                                            fourteenNineSA += item.saleAmount / (decimal)13.31;
                                            fourteenNineB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 17.8)
                                        {
                                            seventeenEight += item.PMT / (decimal)13.31;
                                            seventeenEightSA += item.saleAmount / (decimal)13.31;
                                            seventeenEightB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 17.9)
                                        {
                                            seventeenNine += item.PMT / (decimal)13.31;
                                            seventeenNineSA += item.saleAmount / (decimal)13.31;
                                            seventeenNineB += item.balance / (decimal)13.31;
                                        }
                                        if (item.APR == 18.9)
                                        {
                                            eightTeenNine += item.PMT / (decimal)13.31;
                                            eightTeenNineSA += item.saleAmount / (decimal)13.31;
                                            eightTeenNineB += item.balance / (decimal)13.31;
                                        }
                                    }
                                }
                                else if (item.contractDate.Year == 2015)
                                {
                                    d1 += item.saleAmount / (decimal)15.87;
                                    e1 += item.balance / (decimal)15.87;
                                    f1 += item.totalPaid / (decimal)15.87;
                                    if (item.balance <= 0)
                                    {

                                    }
                                    else
                                    {
                                        zeroSA += dp / (decimal)15.87; ;
                                        if (item.APR == 0)
                                        {
                                            zero += item.PMT / (decimal)15.87;
                                            zeroSA += item.saleAmount / (decimal)15.87;
                                            zeroB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 5)
                                        {
                                            five += item.PMT / (decimal)15.87;
                                            fiveNineSA += item.saleAmount / (decimal)15.87;
                                            fiveNineB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 5.9)
                                        {
                                            fiveNine += item.PMT / (decimal)15.87;
                                            fiveNineSA += item.saleAmount / (decimal)15.87;
                                            fiveNineB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 6.9 || item.APR == 69)
                                        {
                                            sixNine += item.PMT / (decimal)15.87;
                                            sixNineSA += item.saleAmount / (decimal)15.87;
                                            sixNineB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 7)
                                        {
                                            seven += item.PMT / (decimal)15.87;
                                            sevenSA += item.saleAmount / (decimal)15.87;
                                            sevenB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 8)
                                        {
                                            eight += item.PMT / (decimal)15.87;
                                            eightSA += item.saleAmount / (decimal)15.87;
                                            eightB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 9)
                                        {
                                            nine += item.PMT / (decimal)15.87;
                                            nineSA += item.saleAmount / (decimal)15.87;
                                            nineB += item.balance / (decimal)15.87;

                                        }
                                        if (item.APR == 9.9)
                                        {
                                            nineNine += item.PMT / (decimal)15.87;
                                            nineNineSA += item.saleAmount / (decimal)15.87;
                                            nineNineB += item.balance / (decimal)15.87;

                                        }
                                        if (item.APR == 10)
                                        {
                                            ten += item.PMT / (decimal)15.87;
                                            tenSA += item.saleAmount / (decimal)15.87;
                                            tenB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 11)
                                        {
                                            eleven += item.PMT / (decimal)15.87;
                                            elevenSA += item.saleAmount / (decimal)15.87;
                                            elevenB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 12)
                                        {
                                            twelve += item.PMT / (decimal)15.87;
                                            twelveSA += item.saleAmount / (decimal)15.87;
                                            twelveB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 12.1)
                                        {
                                            twleveOne += item.PMT / (decimal)15.87;
                                            twleveOneSA += item.saleAmount / (decimal)15.87;
                                            twleveOneB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 12.7)
                                        {
                                            twelveSeven += item.PMT / (decimal)15.87; ;
                                            twelveSevenSA += item.saleAmount / (decimal)15.87;
                                            twelveSevenB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 12.9)
                                        {
                                            twelveNine += item.PMT / (decimal)15.87;
                                            twelveNineSA += item.saleAmount / (decimal)15.87;
                                            twelveNineB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 14)
                                        {
                                            fourteen += item.PMT / (decimal)15.87;
                                            fourteenSA += item.saleAmount / (decimal)15.87;
                                            fourteenB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 14.9)
                                        {
                                            fourteenNine += item.PMT / (decimal)15.87;
                                            fourteenNineSA += item.saleAmount / (decimal)15.87;
                                            fourteenNineB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 17.8)
                                        {
                                            seventeenEight += item.PMT / (decimal)15.87;
                                            seventeenEightSA += item.saleAmount / (decimal)15.87;
                                            seventeenEightB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 17.9)
                                        {
                                            seventeenNine += item.PMT / (decimal)15.87;
                                            seventeenNineSA += item.saleAmount / (decimal)15.87;
                                            seventeenNineB += item.balance / (decimal)15.87;
                                        }
                                        if (item.APR == 18.9)
                                        {
                                            eightTeenNine += item.PMT / (decimal)15.87;
                                            eightTeenNineSA += item.saleAmount / (decimal)15.87;
                                            eightTeenNineB += item.balance / (decimal)15.87;
                                        }
                                    }
                                }

                                double apr = item.APR;
                            }
                            else if (item.currency == "USD")
                            {
                                d1 += item.saleAmount;
                                e1 += item.balance;
                                f1 += item.totalPaid;
                                if (item.balance <= 0)
                                {


                                }
                                else
                                {
                                    zeroSA += dp;
                                    if (item.APR == 0)
                                    {
                                        zero += item.PMT;
                                        zeroSA += item.saleAmount;
                                        zeroB += item.balance;
                                    }
                                    if (item.APR == 5)
                                    {
                                        five += item.PMT;
                                        fiveNineSA += item.saleAmount;
                                        fiveNineB += item.balance;
                                    }
                                    if (item.APR == 5.9)
                                    {
                                        fiveNine += item.PMT;
                                        fiveNineSA += item.saleAmount;
                                        fiveNineB += item.balance;
                                    }
                                    if (item.APR == 6.9 || item.APR == 69)
                                    {
                                        sixNine += item.PMT;
                                        sixNineSA += item.saleAmount;
                                        sixNineB += item.balance;
                                    }
                                    if (item.APR == 7)
                                    {
                                        seven += item.PMT;
                                        sevenSA += item.saleAmount;
                                        sevenB += item.balance;
                                    }
                                    if (item.APR == 8)
                                    {
                                        eight += item.PMT;
                                        eightSA += item.saleAmount;
                                        eightB += item.balance;
                                    }
                                    if (item.APR == 9)
                                    {
                                        nine += item.PMT;
                                        nineSA += item.saleAmount;
                                        nineB += item.balance;

                                    }
                                    if (item.APR == 9.9)
                                    {
                                        nineNine += item.PMT;
                                        nineNineSA += item.saleAmount;
                                        nineNineB += item.balance;

                                    }
                                    if (item.APR == 10)
                                    {
                                        ten += item.PMT;
                                        tenSA += item.saleAmount;
                                        tenB += item.balance;
                                    }
                                    if (item.APR == 11)
                                    {
                                        eleven += item.PMT;
                                        elevenSA += item.saleAmount;
                                        elevenB += item.balance;
                                    }
                                    if (item.APR == 12)
                                    {
                                        twelve += item.PMT;
                                        twelveSA += item.saleAmount;
                                        twelveB += item.balance;
                                    }
                                    if (item.APR == 12.1)
                                    {
                                        twleveOne += item.PMT;
                                        twleveOneSA += item.saleAmount;
                                        twleveOneB += item.balance;
                                    }
                                    if (item.APR == 12.7)
                                    {
                                        twelveSeven += item.PMT; ;
                                        twelveSevenSA += item.saleAmount;
                                        twelveSevenB += item.balance;
                                    }
                                    if (item.APR == 12.9)
                                    {
                                        twelveNine += item.PMT;
                                        twelveNineSA += item.saleAmount;
                                        twelveNineB += item.balance;
                                    }
                                    if (item.APR == 14)
                                    {
                                        fourteen += item.PMT;
                                        fourteenSA += item.saleAmount;
                                        fourteenB += item.balance;
                                    }
                                    if (item.APR == 14.9)
                                    {
                                        fourteenNine += item.PMT;
                                        fourteenNineSA += item.saleAmount;
                                        fourteenNineB += item.balance;
                                    }
                                    if (item.APR == 17.8)
                                    {
                                        seventeenEight += item.PMT;
                                        seventeenEightSA += item.saleAmount;
                                        seventeenEightB += item.balance;
                                    }
                                    if (item.APR == 17.9)
                                    {
                                        seventeenNine += item.PMT;
                                        seventeenNineSA += item.saleAmount;
                                        seventeenNineB += item.balance;
                                    }
                                    if (item.APR == 18.9)
                                    {
                                        eightTeenNine += item.PMT;
                                        eightTeenNineSA += item.saleAmount;
                                        eightTeenNineB += item.balance;
                                    }
                                }
                            }

                        }
                        //}
                    }
                    //Sum Canceled Fractional Contracts 
                    ViewBag.tSalesAmountFcCanceled = a1;
                    ViewBag.tOwedAmountFcCanceled = b1;
                    ViewBag.tPaidFcCanceled = c1;
                    ViewBag.totalCanceled = can1;
                    ViewBag.pmtc = pmtc;
                    //Sum Active Fractional Contracts 
                    ViewBag.tSalesAmountFc = d1;
                    ViewBag.tOwedAmountFc = e1;
                    ViewBag.tPaidFc = f1;
                    ViewBag.pmt = pmt;
                    //Sum Canceled and Active Fractional Contracts
                    ViewBag.tSalesAmountFcTotal = a1 + d1;
                    ViewBag.tOwedAmountFcTotal = b1 + e1;
                    ViewBag.tPaidFcTotal = c1 + f1;
                    ViewBag.tPmt = pmtc + pmt;
                    //Monthly payment by Interest Type 
                    ViewBag.zero = zero;
                    ViewBag.five = five;
                    ViewBag.fiveNine = fiveNine;
                    ViewBag.sixNine = sixNine;
                    ViewBag.seven = seven;
                    ViewBag.eight = eight;
                    ViewBag.nine = nine;
                    ViewBag.nineNine = nineNine;
                    ViewBag.ten = ten;
                    ViewBag.eleven = eleven;
                    ViewBag.twelve = twelve;
                    ViewBag.twleveOne = twleveOne;
                    ViewBag.twelveSeven = twelveSeven;
                    ViewBag.twelveNine = twelveNine;
                    ViewBag.fourteen = fourteen;
                    ViewBag.fourteenNine = fourteenNine;
                    ViewBag.seventeenEight = seventeenEight;
                    ViewBag.seventeenNine = seventeenNine;
                    ViewBag.eightTeenNine = eightTeenNine;
                    //sale amount by interest type
                    ViewBag.zeroSA = zeroSA;
                    ViewBag.fiveSA = fiveSA;
                    ViewBag.fiveNineSA = fiveNineSA;
                    ViewBag.sixNineSA = sixNineSA;
                    ViewBag.sevenSA = sevenSA;
                    ViewBag.eightSA = eightSA;
                    ViewBag.nineSA = nineSA;
                    ViewBag.nineNineSA = nineNineSA;
                    ViewBag.tenSA = tenSA;
                    ViewBag.elevenSA = elevenSA;
                    ViewBag.twelveSA = twelveSA;
                    ViewBag.twleveOneSA = twleveOneSA;
                    ViewBag.twelveSevenSA = twelveSevenSA;
                    ViewBag.twelveNineSA = twelveNineSA;
                    ViewBag.fourteenSA = fourteenSA;
                    ViewBag.fourteenNineSA = fourteenNineSA;
                    ViewBag.seventeenEightSA = seventeenEightSA;
                    ViewBag.seventeenNineSA = seventeenNineSA;
                    ViewBag.eightTeenNineSA = eightTeenNineSA;
                    //BALANCE BY INTEREST TYPE
                    ViewBag.zeroB = zeroB;
                    ViewBag.fiveB = fiveB;
                    ViewBag.fiveNineB = fiveNineB;
                    ViewBag.sixNineB = sixNineB;
                    ViewBag.sevenB = sevenB;
                    ViewBag.eightB = eightB;
                    ViewBag.nineB = nineB;
                    ViewBag.nineNineB = nineNineB;
                    ViewBag.tenB = tenB;
                    ViewBag.elevenB = elevenB;
                    ViewBag.twelveB = twelveB;
                    ViewBag.twleveOneB = twleveOneB;
                    ViewBag.twelveSevenB = twelveSevenB;
                    ViewBag.twelveNineB = twelveNineB;
                    ViewBag.fourteenB = fourteenB;
                    ViewBag.fourteenNineB = fourteenNineB;
                    ViewBag.seventeenEightB = seventeenEightB;
                    ViewBag.seventeenNineB = seventeenNineB;
                    ViewBag.eightTeenNineB = eightTeenNineB;

                }
                decimal a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, can = 0;
                decimal zeroC = 0, fiveC = 0, fiveNineC = 0, sixNineC = 0, sevenC = 0, eightC = 0,
                    nineC = 0, nineNineC = 0, tenC = 0, elevenC = 0,
                    twelveC = 0, twleveOneC = 0, twelveSevenC = 0, twelveNineC = 0, fourteenC = 0,
                    fourteenNineC = 0, seventeenEightC = 0, seventeenNineC = 0,
                    eightTeenNineC = 0;
                decimal zeroCSA = 0, fiveCSA = 0, fiveNineCSA = 0, sixNineCSA = 0, sevenCSA = 0, eightCSA = 0,
                    nineCSA = 0, nineNineCSA = 0, tenCSA = 0, elevenCSA = 0,
                    twelveCSA = 0, twleveOneCSA = 0, twelveSevenCSA = 0, twelveNineCSA = 0, fourteenCSA = 0,
                    fourteenNineCSA = 0, seventeenEightCSA = 0, seventeenNineCSA = 0,
                    eightTeenNineCSA = 0;
                decimal zeroCB = 0, fiveCB = 0, fiveNineCB = 0, sixNineCB = 0, sevenCB = 0, eightCB = 0,
                   nineCB = 0, nineNineCB = 0, tenCB = 0, elevenCB = 0,
                   twelveCB = 0, twleveOneCB = 0, twelveSevenCB = 0, twelveNineCB = 0, fourteenCB = 0,
                   fourteenNineCB = 0, seventeenEightCB = 0, seventeenNineCB = 0,
                   eightTeenNineCB = 0;
                //List<String> negativeBalance = new List<string>();
                if (query2.Count() > 0) //Data to generate summary is calculated
                {

                    foreach (VMTrialMemberships item in query2)
                    {
                        //TODO verificar conn gibran
                        double algo = double.Parse(item.saleAmount.ToString());
                        double dp = algo * .30;
                        //if(item.downPaymentPaid == true || item.balance <= (decimal)dp)
                        //{
                        if (item.canceledContract == true)
                        {
                            can++;
                            if (item.currency == "MXN")
                            {
                                if (item.contractDate.Year == 2013)
                                {
                                    a += Decimal.Parse(item.saleAmount.ToString()) / (decimal)12.76;
                                    b += item.balance / (decimal)12.76;
                                    c += item.totalPaid / (decimal)12.76;
                                }
                                else if (item.contractDate.Year == 2014)
                                {
                                    a += Decimal.Parse(item.saleAmount.ToString()) / (decimal)13.31;
                                    b += item.balance / (decimal)13.31;
                                    c += item.totalPaid / (decimal)13.31;
                                }
                                else if (item.contractDate.Year == 2015)
                                {
                                    a += Decimal.Parse(item.saleAmount.ToString()) / (decimal)15.87;
                                    b += item.balance / (decimal)15.87;
                                    c += item.totalPaid / (decimal)15.87;
                                }
                            }
                            else if (item.currency == "USD")
                            {
                                a += Decimal.Parse(item.saleAmount.ToString());
                                b += item.balance;
                                c += item.totalPaid;
                            }
                            //a += Decimal.Parse(item.saleAmount.ToString());
                        }
                        else
                        {
                            if (item.currency == "MXN")
                            {
                                if (item.contractDate.Year == 2013)
                                {
                                    d += Decimal.Parse(item.saleAmount.ToString()) / (decimal)12.76;
                                    e += item.balance / (decimal)12.76;
                                    f += item.totalPaid / (decimal)12.76;
                                    zeroCSA += (decimal)dp / (decimal)12.76; ;
                                    if (item.APR == 0)
                                    {
                                        zeroC += item.PMT / (decimal)12.76;
                                        zeroCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        zeroCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 5)
                                    {
                                        fiveC += item.PMT / (decimal)12.76;
                                        fiveCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        fiveCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 5.9)
                                    {
                                        fiveNineC += item.PMT / (decimal)12.76;
                                        fiveNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        fiveNineCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 6.9 || item.APR == 69)
                                    {
                                        sixNineC += item.PMT / (decimal)12.76;
                                        sixNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        sixNineCB += item.balance / (decimal)12.76; ;
                                    }
                                    if (item.APR == 7)
                                    {
                                        sevenC += item.PMT / (decimal)12.76;
                                        sevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        sevenCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 8)
                                    {
                                        eightC += item.PMT / (decimal)12.76;
                                        eightCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        eightCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 9)
                                    {
                                        nineC += item.PMT / (decimal)12.76;
                                        nineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        nineCB += item.balance / (decimal)12.76;

                                    }
                                    if (item.APR == 9.9)
                                    {
                                        nineNineC += item.PMT / (decimal)12.76;
                                        nineNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        nineNineCB += item.balance / (decimal)12.76;

                                    }
                                    if (item.APR == 10)
                                    {
                                        tenC += item.PMT / (decimal)12.76;
                                        tenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        tenCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 11)
                                    {
                                        elevenC += item.PMT / (decimal)12.76;
                                        elevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        elevenCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 12)
                                    {
                                        twelveC += item.PMT / (decimal)12.76;
                                        twelveCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        twelveCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 12.1)
                                    {
                                        twleveOneC += item.PMT / (decimal)12.76;
                                        twleveOneCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        twleveOneCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 12.7)
                                    {
                                        twelveSevenC += item.PMT / (decimal)12.76;
                                        twelveSevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        twelveSevenCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 12.9)
                                    {
                                        twelveNineC += item.PMT / (decimal)12.76;
                                        twelveNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        twelveNineCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 14)
                                    {
                                        fourteenC += item.PMT / (decimal)12.76;
                                        fourteenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        fourteenCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 14.9)
                                    {
                                        fourteenNineC += item.PMT / (decimal)12.76;
                                        fourteenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        fourteenNineCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 17.8)
                                    {
                                        seventeenEightC += item.PMT / (decimal)12.76;
                                        seventeenEightCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        seventeenEightCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 17.9)
                                    {
                                        seventeenNineC += item.PMT / (decimal)12.76;
                                        seventeenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        seventeenNineCB += item.balance / (decimal)12.76;
                                    }
                                    if (item.APR == 18.9)
                                    {
                                        eightTeenNineC += item.PMT / (decimal)12.76;
                                        eightTeenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)12.76;
                                        eightTeenNineCB += item.balance / (decimal)12.76;
                                    }
                                }
                                else if (item.contractDate.Year == 2014)
                                {
                                    d += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                    e += item.balance / (decimal)13.31;
                                    f += item.totalPaid / (decimal)13.31;
                                    zeroCSA += (decimal)dp / (decimal)13.31; ;
                                    if (item.APR == 0)
                                    {
                                        zeroC += item.PMT / (decimal)13.31;
                                        zeroCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        zeroCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 5)
                                    {
                                        fiveC += item.PMT / (decimal)13.31;
                                        fiveCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        fiveCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 5.9)
                                    {
                                        fiveNineC += item.PMT / (decimal)13.31;
                                        fiveNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        fiveNineCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 6.9 || item.APR == 69)
                                    {
                                        sixNineC += item.PMT / (decimal)13.31;
                                        sixNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        sixNineCB += item.balance / (decimal)13.31; ;
                                    }
                                    if (item.APR == 7)
                                    {
                                        sevenC += item.PMT / (decimal)13.31;
                                        sevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        sevenCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 8)
                                    {
                                        eightC += item.PMT / (decimal)13.31;
                                        eightCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        eightCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 9)
                                    {
                                        nineC += item.PMT / (decimal)13.31;
                                        nineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        nineCB += item.balance / (decimal)13.31;

                                    }
                                    if (item.APR == 9.9)
                                    {
                                        nineNineC += item.PMT / (decimal)13.31;
                                        nineNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        nineNineCB += item.balance / (decimal)13.31;

                                    }
                                    if (item.APR == 10)
                                    {
                                        tenC += item.PMT / (decimal)13.31;
                                        tenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        tenCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 11)
                                    {
                                        elevenC += item.PMT / (decimal)13.31;
                                        elevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        elevenCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 12)
                                    {
                                        twelveC += item.PMT / (decimal)13.31;
                                        twelveCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        twelveCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 12.1)
                                    {
                                        twleveOneC += item.PMT / (decimal)13.31;
                                        twleveOneCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        twleveOneCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 12.7)
                                    {
                                        twelveSevenC += item.PMT / (decimal)13.31;
                                        twelveSevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        twelveSevenCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 12.9)
                                    {
                                        twelveNineC += item.PMT / (decimal)13.31;
                                        twelveNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        twelveNineCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 14)
                                    {
                                        fourteenC += item.PMT / (decimal)13.31;
                                        fourteenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        fourteenCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 14.9)
                                    {
                                        fourteenNineC += item.PMT / (decimal)13.31;
                                        fourteenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        fourteenNineCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 17.8)
                                    {
                                        seventeenEightC += item.PMT / (decimal)13.31;
                                        seventeenEightCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        seventeenEightCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 17.9)
                                    {
                                        seventeenNineC += item.PMT / (decimal)13.31;
                                        seventeenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        seventeenNineCB += item.balance / (decimal)13.31;
                                    }
                                    if (item.APR == 18.9)
                                    {
                                        eightTeenNineC += item.PMT / (decimal)13.31;
                                        eightTeenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)13.31;
                                        eightTeenNineCB += item.balance / (decimal)13.31;
                                    }
                                }
                                else if (item.contractDate.Year == 2015)
                                {
                                    d += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                    e += item.balance / (decimal)15.87;
                                    f += item.totalPaid / (decimal)15.87;
                                    zeroCSA += (decimal)dp / (decimal)15.87; ;
                                    if (item.APR == 0)
                                    {
                                        zeroC += item.PMT / (decimal)15.87;
                                        zeroCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        zeroCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 5)
                                    {
                                        fiveC += item.PMT / (decimal)15.87;
                                        fiveCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        fiveCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 5.9)
                                    {
                                        fiveNineC += item.PMT / (decimal)15.87;
                                        fiveNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        fiveNineCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 6.9 || item.APR == 69)
                                    {
                                        sixNineC += item.PMT / (decimal)15.87;
                                        sixNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        sixNineCB += item.balance / (decimal)15.87; ;
                                    }
                                    if (item.APR == 7)
                                    {
                                        sevenC += item.PMT / (decimal)15.87;
                                        sevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        sevenCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 8)
                                    {
                                        eightC += item.PMT / (decimal)15.87;
                                        eightCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        eightCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 9)
                                    {
                                        nineC += item.PMT / (decimal)15.87;
                                        nineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        nineCB += item.balance / (decimal)15.87;

                                    }
                                    if (item.APR == 9.9)
                                    {
                                        nineNineC += item.PMT / (decimal)15.87;
                                        nineNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        nineNineCB += item.balance / (decimal)15.87;

                                    }
                                    if (item.APR == 10)
                                    {
                                        tenC += item.PMT / (decimal)15.87;
                                        tenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        tenCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 11)
                                    {
                                        elevenC += item.PMT / (decimal)15.87;
                                        elevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        elevenCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 12)
                                    {
                                        twelveC += item.PMT / (decimal)15.87;
                                        twelveCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        twelveCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 12.1)
                                    {
                                        twleveOneC += item.PMT / (decimal)15.87;
                                        twleveOneCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        twleveOneCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 12.7)
                                    {
                                        twelveSevenC += item.PMT / (decimal)15.87;
                                        twelveSevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        twelveSevenCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 12.9)
                                    {
                                        twelveSevenC += item.PMT / (decimal)15.87;
                                        twelveSevenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        twelveSevenCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 14)
                                    {
                                        fourteenC += item.PMT / (decimal)15.87;
                                        fourteenCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        fourteenCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 14.9)
                                    {
                                        fourteenNineC += item.PMT / (decimal)15.87;
                                        fourteenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        fourteenNineCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 17.8)
                                    {
                                        seventeenEightC += item.PMT / (decimal)15.87;
                                        seventeenEightCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        seventeenEightCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 17.9)
                                    {
                                        seventeenNineC += item.PMT / (decimal)15.87;
                                        seventeenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        seventeenNineCB += item.balance / (decimal)15.87;
                                    }
                                    if (item.APR == 18.9)
                                    {
                                        eightTeenNineC += item.PMT / (decimal)15.87;
                                        eightTeenNineCSA += ((decimal)item.saleAmount - (decimal)dp) / (decimal)15.87;
                                        eightTeenNineCB += item.balance / (decimal)15.87;
                                    }
                                }
                            }
                            else if (item.currency == "USD")
                            {
                                d += Decimal.Parse(item.saleAmount.ToString());
                                e += item.balance;
                                f += item.totalPaid;
                                if (item.balance <= 0)
                                {
                                    //int numberNegative = 0;
                                    //numberNegative++;
                                    //negativeBalance.Add(item.contractNumber);
                                }
                                else
                                {
                                    zeroCSA += (decimal)dp;
                                    if (item.APR == 0)
                                    {
                                        zeroC += item.PMT;
                                        zeroCSA += (decimal)item.saleAmount - (decimal)dp;
                                        zeroCB += item.balance;
                                    }
                                    if (item.APR == 5)
                                    {
                                        fiveC += item.PMT;
                                        fiveCSA += (decimal)item.saleAmount - (decimal)dp;
                                        fiveCB += item.balance;
                                    }
                                    if (item.APR == 5.9)
                                    {
                                        fiveNineC += item.PMT;
                                        fiveNineCSA += (decimal)item.saleAmount - (decimal)dp;
                                        fiveNineCB += item.balance;
                                    }
                                    if (item.APR == 6.9 || item.APR == 69)
                                    {
                                        sixNineC += item.PMT;
                                        sixNineCSA += (decimal)item.saleAmount - (decimal)dp;
                                        sixNineCB += item.balance; ;
                                    }
                                    if (item.APR == 7)
                                    {
                                        sevenC += item.PMT;
                                        sevenCSA += (decimal)item.saleAmount - (decimal)dp;
                                        sevenCB += item.balance;
                                    }
                                    if (item.APR == 8)
                                    {
                                        eightC += item.PMT;
                                        eightCSA += (decimal)item.saleAmount - (decimal)dp;
                                        eightCB += item.balance;
                                    }
                                    if (item.APR == 9)
                                    {
                                        nineC += item.PMT;
                                        nineCSA += (decimal)item.saleAmount - (decimal)dp;
                                        nineCB += item.balance;

                                    }
                                    if (item.APR == 9.9)
                                    {
                                        nineNineC += item.PMT;
                                        nineNineCSA += (decimal)item.saleAmount - (decimal)dp;
                                        nineNineCB += item.balance;

                                    }
                                    if (item.APR == 10)
                                    {
                                        tenC += item.PMT;
                                        tenCSA += (decimal)item.saleAmount - (decimal)dp;
                                        tenCB += item.balance;
                                    }
                                    if (item.APR == 11)
                                    {
                                        elevenC += item.PMT;
                                        elevenCSA += (decimal)item.saleAmount - (decimal)dp;
                                        elevenCB += item.balance;
                                    }
                                    if (item.APR == 12)
                                    {
                                        twelveC += item.PMT;
                                        twelveCSA += (decimal)item.saleAmount - (decimal)dp;
                                        twelveCB += item.balance;
                                    }
                                    if (item.APR == 12.1)
                                    {
                                        twleveOneC += item.PMT;
                                        twleveOneCSA += (decimal)item.saleAmount - (decimal)dp;
                                        twleveOneCB += item.balance;
                                    }
                                    if (item.APR == 12.7)
                                    {
                                        twelveSevenC += item.PMT;
                                        twelveSevenCSA += (decimal)item.saleAmount - (decimal)dp;
                                        twelveSevenCB += item.balance;
                                    }
                                    if (item.APR == 12.9)
                                    {
                                        twelveNineC += item.PMT;
                                        twelveNineCSA += (decimal)item.saleAmount - (decimal)dp;
                                        twelveNineCB += item.balance;
                                    }
                                    if (item.APR == 14)
                                    {
                                        fourteenC += item.PMT;
                                        fourteenCSA += (decimal)item.saleAmount - (decimal)dp;
                                        fourteenCB += item.balance;
                                    }
                                    if (item.APR == 14.9)
                                    {
                                        fourteenNineC += item.PMT;
                                        fourteenNineCSA += (decimal)item.saleAmount - (decimal)dp;
                                        fourteenNineCB += item.balance;
                                    }
                                    if (item.APR == 17.8)
                                    {
                                        seventeenEightC += item.PMT;
                                        seventeenEightCSA += (decimal)item.saleAmount - (decimal)dp;
                                        seventeenEightCB += item.balance;
                                    }
                                    if (item.APR == 17.9)
                                    {
                                        seventeenNineC += item.PMT;
                                        seventeenNineCSA += (decimal)item.saleAmount - (decimal)dp;
                                        seventeenNineCB += item.balance;
                                    }
                                    if (item.APR == 18.9)
                                    {
                                        eightTeenNineC += item.PMT;
                                        eightTeenNineCSA += (decimal)item.saleAmount - (decimal)dp;
                                        eightTeenNineCB += item.balance;
                                    }
                                }
                            }
                            //d += Decimal.Parse(item.saleAmount.ToString());
                        }
                        //}
                    }

                }
                //Sum Canceled TM Contracts 
                ViewBag.tSalesAmountTmCanceled = a;
                ViewBag.tOwedAmountTmCanceled = b;
                ViewBag.tPaidTmCanceled = c;
                ViewBag.tTotalCanceledTM = can;
                //Sum Active TM Contracts 
                ViewBag.tSalesAmountTm = d;
                ViewBag.tOwedAmountTm = e;
                ViewBag.tPaidTm = f;
                //Sum Canceled and Active TM Contracts
                ViewBag.tSalesAmountTmTotal = a + d;
                ViewBag.tOwedAmountTmTotal = b + e;
                ViewBag.tPaidTmTotal = c + f;
                ViewBag.totalTM = query2.Count();
                //ACTIVE BY APR
                //Active by Interest Type 
                ViewBag.zeroC = zeroC;
                ViewBag.fiveC = fiveC;
                ViewBag.fiveNineC = fiveNineC;
                ViewBag.sixNineC = sixNineC;
                ViewBag.sevenC = sevenC;
                ViewBag.eightC = eightC;
                ViewBag.nineC = nineC;
                ViewBag.nineNineC = nineNineC;
                ViewBag.tenC = tenC;
                ViewBag.elevenC = elevenC;
                ViewBag.twelveC = twelveC;
                ViewBag.twleveOneC = twleveOneC;
                ViewBag.twelveSevenC = twelveSevenC;
                ViewBag.twelvenineC = twelveNineC;
                ViewBag.fourteenC = fourteenC;
                ViewBag.fourteenNineC = fourteenNineC;
                ViewBag.seventeenEightC = seventeenEightC;
                ViewBag.seventeenNineC = seventeenNineC;
                ViewBag.eightTeenNineC = eightTeenNineC;
                //Sale amount by Interest Type 
                ViewBag.zeroCSA = zeroCSA;
                ViewBag.fiveCSA = fiveCSA;
                ViewBag.fiveNineCSA = fiveNineCSA;
                ViewBag.sixNineCSA = sixNineCSA;
                ViewBag.sevenCSA = sevenCSA;
                ViewBag.eightCSA = eightCSA;
                ViewBag.nineCSA = nineCSA;
                ViewBag.nineNineCSA = nineNineCSA;
                ViewBag.tenCSA = tenCSA;
                ViewBag.elevenCSA = elevenCSA;
                ViewBag.twelveCSA = twelveCSA;
                ViewBag.twleveOneCSA = twleveOneCSA;
                ViewBag.twelveSevenCSA = twelveSevenCSA;
                ViewBag.twelvenineCSA = twelveNineCSA;
                ViewBag.fourteenCSA = fourteenCSA;
                ViewBag.fourteenNineCSA = fourteenNineCSA;
                ViewBag.seventeenEightCSA = seventeenEightCSA;
                ViewBag.seventeenNineCSA = seventeenNineCSA;
                ViewBag.eightTeenNineCSA = eightTeenNineCSA;
                //BALANCE by Interest Type 
                ViewBag.zeroCB = zeroCB;
                ViewBag.fiveCB = fiveCB;
                ViewBag.fiveNineCB = fiveNineCB;
                ViewBag.sixNineCB = sixNineCB;
                ViewBag.sevenCB = sevenCB;
                ViewBag.eightCB = eightCB;
                ViewBag.nineCB = nineCB;
                ViewBag.nineNineCB = nineNineCB;
                ViewBag.tenCB = tenCB;
                ViewBag.elevenCB = elevenCB;
                ViewBag.twelveCB = twelveCB;
                ViewBag.twleveOneCB = twleveOneCB;
                ViewBag.twelveSevenCB = twelveSevenCB;
                ViewBag.twelvenineCB = twelveNineCB;
                ViewBag.fourteenCB = fourteenCB;
                ViewBag.fourteenNineCB = fourteenNineCB;
                ViewBag.seventeenEightCB = seventeenEightCB;
                ViewBag.seventeenNineCB = seventeenNineCB;
                ViewBag.eightTeenNineCB = eightTeenNineCB;
                //Count TM contracts 
                ViewBag.countContractsTM = query2.Count();
                ViewBag.query2 = query2;
                ViewBag.vmFilter = vmFilter;
                //Show list of negative contracts
                //ViewBag.negativeBalance = negativeBalance;
                return View(query);
            }
        }

        //Validate if User can use specific filters
        private bool securityRequiermentsMet(VMFilterContractReport vmFilter)
        {
            return User.IsInRole(AccountRolesNames.ADMINISTRATOR) || User.IsInRole(AccountRolesNames.ACCOUNTANT) || User.IsInRole(AccountRolesNames.CEO)
                || User.IsInRole(AccountRolesNames.CONTRACT_MANAGER) || User.IsInRole(AccountRolesNames.OWNER_SERVICES)
                || User.IsInRole(AccountRolesNames.SALES_MANAGER) || User.IsInRole(AccountRolesNames.PAYMENTS_RESERVATIONS)
                || User.IsInRole(AccountRolesNames.CONCIERGE) || User.IsInRole(AccountRolesNames.VLO);
        }
        // GET: SalesContract
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult CanceledContracts()
        {
            var query = from c in db.SalesContracts
                        where c.canceledContract == true
                        select c;
            return View(query.ToList());
        }

        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult ContractsUnverified()
        {
            var query = from c in db.SalesContracts
                        where c.verifiedByAdmin == false
                        select c;
            return View(query.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesContract salesContract = db.SalesContracts.Find(id);
            Deposit dp = new Deposit();
            int? previousid = 0;
                if (salesContract.previousFCID > 0)
            {
                previousid = salesContract.previousFCID;
                var sc = db.SalesContracts.Find(previousid);
                ViewBag.condoName = sc.condo.name;
                ViewBag.previoussCN = sc.contractNumber;
                ViewBag.scID = sc.contractID;
                ViewBag.balance = Deposit.getCurrentBalance(sc, null, null);
                ViewBag.tradeInValue = sc.saleAmount;
            }
            else if (salesContract
                .previousTMID > 0)
            {
                previousid = salesContract.previousTMID;
                var tm = db.trialMemberships.Find(previousid);
                ViewBag.previousTmCN = tm.contractNumberTM;
                ViewBag.tmID = tm.contractID;
                ViewBag.balance = Deposit.getCurrentBalance(null, tm, null);
                ViewBag.tradeInValue = tm.saleAmount;
            }
            if (salesContract == null)
            {
                return HttpNotFound();
            }
            //ICollection<PaymentDeposits> pays =salesContract.getDepositsInPayments();
            ViewBag.payments = Deposit.getDepositsInPayments(salesContract, null, null);

            return View(salesContract);
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
            SalesContract salesContract = db.SalesContracts.Find(id);

            if (salesContract == null)//SalesContract not found
            {
                return HttpNotFound();
            }

            if (Deposit.isDownPaymentCompleted(salesContract, null, null))//If commission can be requested
            {
                VMComissionRequest request = new VMComissionRequest(salesContract);
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
        public ActionResult ConfirmRequestComission(int? salesContractID)
        {
            if (salesContractID == null)//Invalid ID
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesContract salesContract = db.SalesContracts.Find(salesContractID);

            if (salesContract == null)//SalesContract not found
            {
                return HttpNotFound();
            }
            if (Deposit.isDownPaymentCompleted(salesContract, null, null) && salesContract.verifiedByAdmin && !salesContract.requestToAccountant)//If commission can be requested
            {
                salesContract.requestToAccountant = true;
                salesContract.requestToAccountantDate = DateTime.Now;
                try
                {
                    db.Entry(salesContract).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Report", new { downPaymentPaid = VMFilterSalesContractReport.CommissionPaymentStatus.COMMISSION_REQUESTED });//See Requested Commissions Report
                    return RedirectToAction("RequestComission", new { id = salesContractID });//See Requested Commissions Report
                }
                catch (Exception exc)
                {
                    VMComissionRequest request = new VMComissionRequest(salesContract);
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
        public ActionResult confirmCommissionPayment(int? salesContractID)
        {
            if (salesContractID == null)//Invalid ID
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesContract salesContract = db.SalesContracts.Find(salesContractID);

            if (salesContract == null)//SalesContract not found
            {
                return HttpNotFound();
            }
            if (Deposit.isDownPaymentCompleted(salesContract, null, null) && salesContract.requestToAccountant && !salesContract.commissionPaid)//If commission can be requested
            {
                salesContract.commissionPaid = true;
                salesContract.commissionPaidDate = DateTime.Now;
                try
                {
                    db.Entry(salesContract).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Report", new { downPaymentPaid = VMFilterSalesContractReport.CommissionPaymentStatus.COMMISSION_PAID });//See Requested Commissions Report
                    return RedirectToAction("RequestComission", new { id = salesContractID });//See Requested Commissions Report
                }
                catch (Exception exc)
                {
                    VMComissionRequest request = new VMComissionRequest(salesContract);
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
        public ActionResult cancelRequestCommission(int? salesContractID)
        {
            if (salesContractID == null)//Invalid ID
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesContract salesContract = db.SalesContracts.Find(salesContractID);

            if (salesContract == null)//SalesContract not found
            {
                return HttpNotFound();
            }

            salesContract.requestToAccountant = false;
            salesContract.requestToAccountantDate = null;

            salesContract.commissionPaid = false;
            salesContract.commissionPaidDate = null;

            try
            {
                db.Entry(salesContract).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Report", new { downPaymentPaid = VMFilterSalesContractReport.CommissionPaymentStatus.COMMISSION_PAID });//See Requested Commissions Report
                return RedirectToAction("RequestComission", new { id = salesContractID });//See Requested Commissions Report
            }
            catch (Exception exc)
            {
                VMComissionRequest request = new VMComissionRequest(salesContract);
                ViewBag.errorMessage = "Contact technical support: " + exc.Message;
                return View(request);
            }

        }

        // GET: SalesContract/Create
        //TODO separar la logica de get de create para los contratos de TM y SC o FA
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER)]
        public ActionResult Create(int? id, int? sCID, int? tMID)
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
            if (tMID > 0)
            {
                var tm = db.trialMemberships.Find(tMID);
                ViewBag.previousTmCN = tm.contractNumberTM;
                ViewBag.tmID = tm.contractID;
                ViewBag.balance = Deposit.getCurrentBalance(null, tm, null);
                ViewBag.tradeInValue = tm.saleAmount;
            }
            if (sCID > 0)
            {
                var sc = db.SalesContracts.Find(sCID);
                ViewBag.condoName = sc.condo.name;
                ViewBag.previoussCN = sc.contractNumber;
                ViewBag.scID = sc.contractID;
                ViewBag.balance = Deposit.getCurrentBalance(sc, null, null);
                ViewBag.tradeInValue = sc.saleAmount;
            }

            if (cliente == null)
            {
                return HttpNotFound();
            }

            ViewBag.Client = cliente;

            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name");

            ////Roles Selections
            //ViewBag.SalesMemberTypes = new SelectList(db.SalesMemberTypes, "rolID", "type");
            ////Sales Members Selections
            //ViewBag.SalesMembers = new SelectList(
            //    from member in db.SalesMembers 
            //    select new { salesMemberID = member.salesMemberID, legalName = member.firtName +" "+member.lastName},
            //    "salesMemberID", "legalName");

            return View();
        }

        // POST: SalesContract/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesContract salesContract, int[] SalesMemberTypes, int[] SalesMembers,
            int? scID, int? tmID)
        {
            //Boolean validMemberSelection = validateSalesMemberSelector(SalesMemberTypes, SalesMembers);
            String otherTypeOfFraction = Request["typeOfFractionOther"];
            if (otherTypeOfFraction != null)
            {
                salesContract.typeOfFraction = otherTypeOfFraction;
            }
            Boolean validMemberSelection = validateSalesMemberSelector(salesContract.contractSalesMembers);
            if (validMemberSelection && ModelState.IsValid)
            {
                //Se guarda el id del usurio que esta creando el contrato//
                salesContract.userCreateContract = User.Identity.GetUserId();
                //Generate ContractNumber//
                DateTime dateNow = DateTime.Now;
                //TODO Esperar para el descriminador de la herencia....
              ///  string contractType = salesContract.contractType;
                if (tmID > 0)
                {
                    var tm = db.trialMemberships.Find(tmID);
                    salesContract.previousTMID = tm.contractID;
                    salesContract.tradeInValue = (decimal?)tm.saleAmount;
                    salesContract.previousBalance = Deposit.getCurrentBalance(null, tm, null);
                    salesContract.newUnitPrice = salesContract.saleAmount - ((decimal)tm.saleAmount); 
                    salesContract.upgrade = true;
                    tm.upgraded = true;
                }
                if (scID > 0)
                {
                    var sc = db.SalesContracts.Find(scID);
                    salesContract.previousFCID = sc.contractID;
                    salesContract.tradeInValue = sc.saleAmount;
                    salesContract.previousBalance = Deposit.getCurrentBalance(sc, null, null);
                    salesContract.newUnitPrice = salesContract.saleAmount - (sc.saleAmount); 
                    salesContract.upgrade = true;
                    sc.upgraded = true;
                }
                //salesContract.clientID = Convert.ToInt32(Request["clientID"]);
                String NumbersContracts = "";
                try
                {
                    NumbersContracts = ((db.SalesContracts.OrderByDescending(p => p.contractID).Select(r => r.contractID).Count()) + 1).ToString();
                    //int ClientID = int.Parse(db.Clients.OrderByDescending(p => p.clientID).Select(r => r.clientID).First().ToString())+1;
                    //TODO verificar el type
                    //salesContract.contractNumber = contractType + NumbersContracts + dateNow.ToString("dd/MM/yyyy").Replace("/", "");
                }
                catch
                {
                    if (NumbersContracts != "") { }
                    //salesContract.contractNumber = contractType + 1 + dateNow.ToString("dd/MM/yyyy").Replace("/", "");
                }

                db.SalesContracts.Add(salesContract);
                int numRegistersCreated = db.SaveChanges();
                if (numRegistersCreated > 0)
                { //If SalesContract have been successfully saved
                    List<ContractSalesMember> csms = linkSalesMemberForContract(salesContract.contractID,
                        SalesMemberTypes, SalesMembers);
                    db.ContractSalesMembers.AddRange(csms);
                    db.SaveChanges();
                }

                //return RedirectToAction("Index");
                return RedirectToAction("details", "client", new { id = salesContract.clientID });
            }
            else if (!validMemberSelection)
            {
                ModelState.AddModelError("INVALID_SALES_MEMBERS_SELECTION", "Pair every Sales Member with his role, go to Sales Member selection and verify fields.");
                ICollection<ContractSalesMember> csms = linkCompleteSalesMemberForContract(salesContract.contractID, salesContract.contractSalesMembers);
                salesContract.contractSalesMembers = csms;
            }

            int id = salesContract.clientID;
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
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", salesContract.condoID);

            return View(salesContract);
        }
        //GET: SalesContract/CreateUpgrade
        //[Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER)]
        // public ActionResult Create(int? id, int? sCID, int? tMID)
        // {
        //     if (id == null)
        //     {
        //         return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //     }
        //     //Se obtiene los datos del cliente nesesarios para realizar el contrato//
        //     var cliente = db.Clients.Find(id);
        //     ViewBag.legalName = cliente.legalName;
        //     ViewBag.slegalName = cliente.secondLegalName;
        //     ViewBag.clientID = cliente.clientID;
        //     if (sCID <= 0)
        //     {
        //         var tm = db.trialMemberships.Find(tMID);
        //         ViewBag.previousTmCN = tm.contractNumberTM;
        //         ViewBag.tmID = tm.trialMembershipID;
        //         ViewBag.balance = tm.getCurrentBalance();
        //         ViewBag.tradeInValue = tm.tmSaleAmount;
        //     }
        //     else
        //     {
        //         var sc = db.SalesContracts.Find(sCID);
        //         ViewBag.condoName = sc.condo.name;
        //         ViewBag.previoussCN = sc.contractNumber;
        //         ViewBag.scID = sc.salesContractID;
        //         ViewBag.balance = sc.getCurrentBalance();
        //         ViewBag.tradeInValue = sc.saleAmount;
        //     }


        //     if (cliente == null)
        //     {
        //         return HttpNotFound();
        //     }

        //     ViewBag.Client = cliente;

        //     ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name");

        //     //Roles Selections
        //     ViewBag.SalesMemberTypes = new SelectList(db.SalesMemberTypes, "rolID", "type");
        //     //Sales Members Selections
        //     ViewBag.SalesMembers = new SelectList(
        //         from member in db.SalesMembers
        //         select new { salesMemberID = member.salesMemberID, legalName = member.firtName + " " + member.lastName },
        //         "salesMemberID", "legalName");

        //     return View();
        // }

        //POST: SalesContract/CreateUpgrade
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER)]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(SalesContract salesContract, int[] SalesMemberTypes, int[] SalesMembers,
        //    int? scID, int? tmID)
        //{
        //    //Boolean validMemberSelection = validateSalesMemberSelector(SalesMemberTypes, SalesMembers);
        //    String otherTypeOfFraction = Request["typeOfFractionOther"];
        //    if (otherTypeOfFraction != null)
        //    {
        //        salesContract.typeOfFraction = otherTypeOfFraction;
        //    }
        //    Boolean validMemberSelection = validateSalesMemberSelector(salesContract.contractSalesMembers);
        //    if (validMemberSelection && ModelState.IsValid)
        //    {
        //        //Se guarda el id del usurio q ue esta creando el contrato//
        //        salesContract.userCreateContract = User.Identity.GetUserId();
        //        //Generate ContractNumber//
        //        DateTime dateNow = DateTime.Now;
        //        string contractType = salesContract.contractType;
        //        //salesContract.clientID = Convert.ToInt32(Request["clientID"]);
        //        if (scID <= 0)
        //        {
        //            var tm = db.trialMemberships.Find(tmID);
        //            salesContract.previousTMID = tm.trialMembershipID;
        //            salesContract.tradeInValue = (decimal?)tm.tmSaleAmount;
        //            salesContract.previousBalance = tm.getCurrentBalance();
        //            salesContract.newUnitPrice = ((decimal?)tm.tmSaleAmount - salesContract.saleAmount);

        //        }
        //        else
        //        {
        //            var sc = db.SalesContracts.Find(scID);
        //            salesContract.previousFCID = sc.salesContractID;
        //            salesContract.tradeInValue = sc.saleAmount;
        //            salesContract.previousBalance = sc.getCurrentBalance();
        //            salesContract.newUnitPrice = sc.saleAmount - salesContract.saleAmount;
        //        }
        //        String NumbersContracts = "";
        //        try
        //        {
        //            NumbersContracts = ((db.SalesContracts.OrderByDescending(p => p.salesContractID).Select(r => r.salesContractID).Count()) + 1).ToString();
        //            //int ClientID = int.Parse(db.Clients.OrderByDescending(p => p.clientID).Select(r => r.clientID).First().ToString())+1;
        //            salesContract.contractNumber = contractType + NumbersContracts + dateNow.ToString("dd/MM/yyyy").Replace("/", "");
        //        }
        //        catch
        //        {
        //            if (NumbersContracts != "") { }
        //            salesContract.contractNumber = contractType + 1 + dateNow.ToString("dd/MM/yyyy").Replace("/", "");
        //        }

        //        db.SalesContracts.Add(salesContract);
        //        int numRegistersCreated = db.SaveChanges();
        //        if (numRegistersCreated > 0)
        //        { //If SalesContract have been successfully saved
        //            List<ContractSalesMember> csms = linkSalesMemberForContract(salesContract.salesContractID, SalesMemberTypes, SalesMembers);
        //            db.ContractSalesMembers.AddRange(csms);
        //            db.SaveChanges();
        //        }

        //        return RedirectToAction("Index");
        //    }
        //    else if (!validMemberSelection)
        //    {
        //        ModelState.AddModelError("INVALID_SALES_MEMBERS_SELECTION", "Pair every Sales Member with his role, go to Sales Member selection and verify fields.");
        //        ICollection<ContractSalesMember> csms = linkCompleteSalesMemberForContract(salesContract.salesContractID, salesContract.contractSalesMembers);
        //        salesContract.contractSalesMembers = csms;
        //    }

        //    int id = salesContract.clientID;
        //    if (id == 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    //Se obtiene los datos del cliente nesesarios para realizar el contrato//
        //    var cliente = db.Clients.Find(id);
        //    if (cliente == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    //Send clients names
        //    ViewBag.Client = cliente;

        //    //Roles Selection
        //    ViewBag.SalesMemberTypes = new SelectList(db.SalesMemberTypes, "rolID", "type");
        //    //Sales Members Selections
        //    ViewBag.SalesMembers = new SelectList(
        //        from member in db.SalesMembers
        //        select new { salesMemberID = member.salesMemberID, legalName = member.firtName + " " + member.lastName },
        //        "salesMemberID", "legalName");
        //    ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", salesContract.condoID);

        //    return View(salesContract);
        //}

        private bool validateSalesMemberSelector(ICollection<ContractSalesMember> csms)
        {
            bool result = true;
            foreach (ContractSalesMember csm in csms)
                //Invalid when one of the selections are cero (not done)
                if (csm.rolID == 0 ^ csm.salesMemberID == 0)//!XOR
                {
                    result = false;
                    break;
                }
            return result;
        }

        private List<ContractSalesMember> completeContractSalesMembersList(List<ContractSalesMember> csms)
        {
            for (int c = 0; c < csms.Count; c++)
                csms.ElementAt(c).completeContractSalesMember();

            return csms;
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

        private List<ContractSalesMember> linkSalesMemberForContract(int salesContractID, int[] SalesMemberTypes, int[] SalesMembers)
        {
            //Every sales member is registered to this contract
            List<ContractSalesMember> csms = new List<ContractSalesMember>();
            for (int c = 0; c < SalesMemberTypes.Count(); c++)
                if (SalesMembers[c] > 0 && SalesMemberTypes[c] > 0)
                {
                    csms.Add(new ContractSalesMember(salesContractID, SalesMembers[c], SalesMemberTypes[c]));
                }

            return csms;
        }

        private List<ContractSalesMember> linkCompleteSalesMemberForContract(int salesContractID, int[] SalesMemberTypes, int[] SalesMembers)
        {
            //Every sales member is registered to this contract
            List<ContractSalesMember> csms = new List<ContractSalesMember>();
            ContractSalesMember csm;
            for (int c = 0; c < SalesMemberTypes.Count(); c++)
                if (SalesMembers[c] > 0 && SalesMemberTypes[c] > 0)
                {
                    csm = new ContractSalesMember(salesContractID, SalesMembers[c], SalesMemberTypes[c]);
                    csm.completeContractSalesMember();
                    csms.Add(csm);
                }

            return csms;
        }

        private ICollection<ContractSalesMember> linkCompleteSalesMemberForContract(int salesContractID, ICollection<ContractSalesMember> csms)
        {
            //Every sales member is registered to this contract
            foreach (ContractSalesMember csm in csms)
                if (csm.salesMemberID > 0 && csm.rolID > 0)
                {
                    csm.salesContractID = salesContractID;
                    csm.completeContractSalesMember();
                }

            return csms;
        }
        public FileResult DownloadUpgradeContractFile(int? id)
        {
            //intancias del cliente con el contrato y el condominio que estan relacionados
            SalesContract salesContract = db.SalesContracts.Find(id);
            Client client = db.Clients.Find(salesContract.clientID);
            Condo condo = db.Condoes.Find(salesContract.condoID);

            String DownPaymentsIngles = "";
            String DownPaymentsEspañol = "";
            String PaymentMonths = "";
            String PaymentMonthInitialDate = "";
            String PaymentMonthEnd = "";
            int contadorDePagos = 0;

            //Se obtiene el plan de pago del DownPayment en español y en ingles
            List<PaymentDeposits> payments = Deposit.getDepositsInPayments(salesContract, null, null);
            foreach (PaymentDeposits p in payments)
            {
                if (p.payment.paymentType == "DownPayment" && p.deposit.getAmountInContractCurrency(salesContract.currency) == 0)
                {

                    DownPaymentsIngles = DownPaymentsIngles + ("$" + String.Format("{0:n}", p.payment.Amortization) + " USD to be paid on " + p.payment.dateToPay.ToString("MMM/dd/yyy") /*ToShortDateString()*/ + "\r\n");
                    DownPaymentsEspañol = DownPaymentsEspañol + ("$" + String.Format("{0:n}", p.payment.Amortization) + " USD para ser pagado en " + p.payment.dateToPay.ToString("MMM/dd/yyy") /*ToShortDateString() */+ "\r\n");
                }
                else if (p.payment.paymentType != "DownPayment" && contadorDePagos < 1)
                {
                    PaymentMonths = String.Format("{0:n}", Deposit.getPMT(salesContract, null, null));
                    PaymentMonthInitialDate = p.payment.dateToPay.ToString("MMM/dd/yyyy");
                    PaymentMonthEnd = (p.payment.dateToPay.AddMonths(salesContract.paymentsQuantity - 1)).ToString("MMM/dd/yyyy");
                    contadorDePagos++;
                }
            }
            //Se obtiene los nombres de las personas que intervinieron en la venta
            string LinerName = "", CloserName = "", VloName = "";
            foreach (ContractSalesMember sm in salesContract.contractSalesMembers)
            {
                if (sm.rol.type == "LINER 1" || sm.rol.type == "LINER 2")
                {
                    LinerName = sm.salesMember.firtName + " " + sm.salesMember.lastName;
                }
                if (sm.rol.type == "CLOSER 1" || sm.rol.type == "CLOSER 2")
                {
                    CloserName = sm.salesMember.firtName + " " + sm.salesMember.lastName;
                }
                if (sm.rol.type == "VLO")
                {
                    VloName = sm.salesMember.firtName + " " + sm.salesMember.lastName;
                }
            }
            //Se identifica cual es el tipo de contrato para poner el tipo de fraccion. 
            string shareIngles = "", shareEspanol = "";
            if (salesContract.typeOfFraction == "1/100" && salesContract.yearEven == false)
            {
                shareIngles = "1/50TH SHARE EVERY ODD YEAR";
                shareEspanol = "1/50VO DE UNIDAD FRACCIONAL CADA AÑO NON";
            }
            else if (salesContract.typeOfFraction == "1/100" && salesContract.yearEven == true)
            {
                shareIngles = "1/50TH SHARE EVERY EVEN YEAR";
                shareEspanol = "1/50VO DE UNIDAD FRACCIONAL CADA AÑO PAR";
            }
            else if (salesContract.typeOfFraction == "1/50")
            {
                shareEspanol = "1/50VO DE UNIDAD FRACCIONAL";
                shareIngles = "1/50TH SHARE";
            }

            //Se crea Instancias de clases convertidores de moneda a letras
            CurrencyTranslator currencyTraslateToInglish = new CurrencyTranslator();
            VMConverterCurrencyInSpanish currencyTraslateToSpanish = new VMConverterCurrencyInSpanish();

            String CantidadEnIngles = currencyTraslateToInglish.TranslateCurrency(salesContract.saleAmount).ToUpper();
            String CantidadEnEspañol = currencyTraslateToSpanish.enletras(salesContract.saleAmount.ToString().ToUpper());
            //Se toma la ubicacion del documento en blanco
            String NewContract = Server.MapPath("~/App_Data/ContratoUpgradeEnBlanco.docx");
            //Se toma el contrato Original y se convierte en arreglo de bytes
            byte[] fileBytesContrato = System.IO.File.ReadAllBytes(Server.MapPath("~/App_Data/ContratoUpgrade.docx"));
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
            doc.ReplaceText("_client1", nombrePrimerCliente);
            doc.ReplaceText("_client2", nombreSegundoCliente);
            doc.ReplaceText("_address", client.primaryResidenceAddress);
            doc.ReplaceText("_city", client.city);
            doc.ReplaceText("_state", client.state);
            doc.ReplaceText("_zipCode", client.zipCode);
            doc.ReplaceText("_phoneNumber", client.primaryPhoneNumber);
            doc.ReplaceText("_phoneTwo", phone2);
            doc.ReplaceText("_mailTwo", email2);
            doc.ReplaceText("_contractNumber", salesContract.contractNumber);
            doc.ReplaceText("_email", client.emailAddress);
            doc.ReplaceText("_phase", (condo.phase).ToString());
            doc.ReplaceText("_shareIngles", shareIngles);
            doc.ReplaceText("_shareEspanol", shareEspanol);
            doc.ReplaceText("_condo", condo.name);
            doc.ReplaceText("_block", condo.block);
            doc.ReplaceText("_lot", condo.lot);
            doc.ReplaceText("_sqmts", condo.sqmt);
            doc.ReplaceText("_building", condo.building);
            try
            {

                doc.ReplaceText("_typeOfID", client.typeOfID);
                doc.ReplaceText("_nationality", client.nationality);
            }
            catch { }
            decimal totals = (salesContract.saleAmount + salesContract.closingCost);
            String total = totals.ToString("#,##0.00");
            decimal downP = 0;
            if (salesContract.deposit > 0 || salesContract.deposit != null)
            {
                downP = (decimal)salesContract.deposit;

            }
            else
            {
                downP = totals * (decimal).3;
            }

            string previousContract = "", previousShareEng = "", previousShareEsp = "", previousBalance = "", tradeInValue = "",
            newUnitPrice = "", newBalance = "", finalBalance = "", previousCondo = "", newTotal = "";
            if (salesContract.previousFCID > 0)
            {
                SalesContract psc = db.SalesContracts.Find(salesContract.previousFCID);
                previousContract = psc.contractNumber;
                if (psc.typeOfFraction == "1/100" && psc.yearEven == false)
                {
                    previousShareEng = "1/50TH SHARE EVERY ODD YEAR";
                    previousShareEsp = "1/50VO DE UNIDAD FRACCIONAL CADA AÑO NON";
                }
                else if (psc.typeOfFraction == "1/100" && psc.yearEven == true)
                {
                    previousShareEng = "1/50TH SHARE EVERY EVEN YEAR";
                    previousShareEsp = "1/50VO DE UNIDAD FRACCIONAL CADA AÑO PAR";
                }
                else if (psc.typeOfFraction == "1/50")
                {
                    previousShareEsp = "1/50VO DE UNIDAD FRACCIONAL";
                    previousShareEng = "1/50TH SHARE";
                }
                newUnitPrice = ((decimal)salesContract.newUnitPrice).ToString("#,##0.00");
                tradeInValue = ((decimal)salesContract.tradeInValue).ToString("#,##0.00");
                previousBalance = ((decimal)salesContract.previousBalance).ToString("#,##0.00");
                newTotal = ((decimal)salesContract.newUnitPrice + salesContract.closingCost).ToString("#,##0.00"); 
                newBalance = ((decimal)(salesContract.newUnitPrice + salesContract.closingCost- downP)).ToString("#,##0.00");
                finalBalance = ((decimal)(salesContract.newUnitPrice + salesContract.closingCost - downP + salesContract.previousBalance)).ToString("#,##0.00");
                previousCondo = psc.condo.name;  
            }else if (salesContract.previousTMID > 0)
            {
                TrialMemberships tm = db.trialMemberships.Find(salesContract.previousTMID);
                previousContract = tm.contractNumberTM;
                previousShareEng = "";
                previousShareEsp = "";
                previousBalance = ((decimal)salesContract.previousBalance).ToString("#,##0.00");
                tradeInValue = ((decimal)salesContract.tradeInValue).ToString("#,##0.00");
                newTotal = ((decimal)salesContract.newUnitPrice + salesContract.closingCost).ToString("#,##0.00");
                newUnitPrice = ((decimal)salesContract.newUnitPrice).ToString("#,##0.00");
                newBalance = ((decimal)(salesContract.newUnitPrice + salesContract.closingCost - downP)).ToString("#,##0.00");
                finalBalance = ((decimal)(salesContract.newUnitPrice + salesContract.closingCost - downP + salesContract.previousBalance)).ToString("#,##0.00");
                previousCondo = "";
            }
            doc.ReplaceText("_tradeInValue", tradeInValue);
            doc.ReplaceText("_newUnitPrice", newUnitPrice);
            doc.ReplaceText("_previousBalance", previousBalance);
            doc.ReplaceText("_newTotal", newTotal);
            doc.ReplaceText("_newBalance", newBalance);
            doc.ReplaceText("_finalBalance", finalBalance);
            doc.ReplaceText("_previousContract", previousContract);
            doc.ReplaceText("_previousShareIngles", previousShareEng);
            doc.ReplaceText("_previousShareEspanol", previousShareEsp);
            doc.ReplaceText("_previousCondo", previousCondo); 
            String dp = downP.ToString("#,##0.00");
            doc.ReplaceText("_saleAmount", (salesContract.saleAmount).ToString("#,##0.00"));
            doc.ReplaceText("_closingCost", (salesContract.closingCost).ToString("#,##0.00"));
            doc.ReplaceText("_totalPrice", total);
            doc.ReplaceText("_downPayment", dp);
            doc.ReplaceText("_precioPalabrasENG", CantidadEnIngles);
            doc.ReplaceText("_precioPalabrasEsp", CantidadEnEspañol);
            doc.ReplaceText("_MonthsFinance", salesContract.paymentsQuantity.ToString());
            doc.ReplaceText("_DownPaymentIngles", DownPaymentsIngles);
            doc.ReplaceText("_DownPaymentEspañol", DownPaymentsEspañol);
            doc.ReplaceText("_initialPayment", Deposit.getInitialDeposit(salesContract, null, null).Amount.ToString("#,##0.00"));
            doc.ReplaceText("_initialPayDate", Deposit.getInitialDeposit(salesContract, null, null).DepositDate.ToString("MMM/dd/yyyy"));
            doc.ReplaceText("_contractDate", salesContract.contractDate.ToString("MMM/dd/yyyy"));
            doc.ReplaceText("_liner1", LinerName);
            doc.ReplaceText("_closer1", CloserName);
            doc.ReplaceText("_vlo", VloName);
            String balance = (totals - downP).ToString("#,##0.00");
            doc.ReplaceText(" _balance", balance);
            doc.ReplaceText("_interestRate", (salesContract.interestRate).ToString());
            doc.ReplaceText("_monthlyPayment", PaymentMonths);
            doc.ReplaceText("_monthPaymentDate", PaymentMonthInitialDate);
            doc.ReplaceText("_paymentsEnd", PaymentMonthEnd);
            doc.ReplaceText("_email1", client.emailAddress);
            doc.ReplaceText("_attachmentsIng", salesContract.attachmentsEnglish == null ? "" : salesContract.attachmentsEnglish);
            doc.ReplaceText("_attachmentsSpan", salesContract.attachmentsSpanish == null ? "" : salesContract.attachmentsSpanish);
            doc.Save();
            //Se convierte el nuevo documento en un arreglo de bytes
            byte[] fileBytesNuevoContrato = System.IO.File.ReadAllBytes(NewContract);
            //Nombre del docuemnto
            string fileDownloadName = salesContract.contractNumber + nombrePrimerCliente + "&" + nombreSegundoCliente + ".docx";
            //Se regresa el nuevo contrato
            return File(fileBytesNuevoContrato, System.Net.Mime.MediaTypeNames.Application.Octet, fileDownloadName);
        }

        public FileResult DownloadContractFile(int? id)
        {
            //intancias del cliente con el contrato y el condominio que estan relacionados
            SalesContract salesContract = db.SalesContracts.Find(id);
            Client client = db.Clients.Find(salesContract.clientID);
            Condo condo = db.Condoes.Find(salesContract.condoID);

            String DownPaymentsIngles = "";
            String DownPaymentsEspañol = "";
            String PaymentMonths = "";
            String PaymentMonthInitialDate = "";
            String PaymentMonthEnd = "";
            int contadorDePagos = 0;

            //Se obtiene el plan de pago del DownPayment en español y en ingles
            List<PaymentDeposits> payments = Deposit.getDepositsInPayments(salesContract, null, null);
            foreach (PaymentDeposits p in payments)
            {
                if (p.payment.paymentType == "DownPayment" && p.deposit.getAmountInContractCurrency(salesContract.currency) == 0)
                {

                    DownPaymentsIngles = DownPaymentsIngles + ("$" + String.Format("{0:n}", p.payment.Amortization) + " USD to be paid on " + p.payment.dateToPay.ToString("MMM/dd/yyy") /*ToShortDateString()*/ + "\r\n");
                    DownPaymentsEspañol = DownPaymentsEspañol + ("$" + String.Format("{0:n}", p.payment.Amortization) + " USD para ser pagado en " + p.payment.dateToPay.ToString("MMM/dd/yyy") /*ToShortDateString() */+ "\r\n");
                }
                else if (p.payment.paymentType != "DownPayment" && contadorDePagos < 1)
                {
                    PaymentMonths = String.Format("{0:n}", Deposit.getPMT(salesContract, null, null));
                    PaymentMonthInitialDate = p.payment.dateToPay.ToString("MMM/dd/yyyy");
                    PaymentMonthEnd = (p.payment.dateToPay.AddMonths(salesContract.paymentsQuantity - 1)).ToString("MMM/dd/yyyy");
                    contadorDePagos++;
                }
            }
            //Se obtiene los nombres de las personas que intervinieron en la venta
            string LinerName = "", CloserName = "", VloName = "";
            foreach (ContractSalesMember sm in salesContract.contractSalesMembers)
            {
                if (sm.rol.type == "LINER 1" || sm.rol.type == "LINER 2")
                {
                    LinerName = sm.salesMember.firtName + " " + sm.salesMember.lastName;
                }
                if (sm.rol.type == "CLOSER 1" || sm.rol.type == "CLOSER 2")
                {
                    CloserName = sm.salesMember.firtName + " " + sm.salesMember.lastName;
                }
                if (sm.rol.type == "VLO")
                {
                    VloName = sm.salesMember.firtName + " " + sm.salesMember.lastName;
                }
            }
            //Se identifica cual es el tipo de contrato para poner el tipo de fraccion. 
            string shareIngles = "", shareEspanol = "";
            if (salesContract.typeOfFraction == "1/100" && salesContract.yearEven == false)
            {
                shareIngles = "1/50TH SHARE EVERY ODD YEAR";
                shareEspanol = "1/50VO DE UNIDAD FRACCIONAL CADA AÑO NON";
            }
            else if (salesContract.typeOfFraction == "1/100" && salesContract.yearEven == true)
            {
                shareIngles = "1/50TH SHARE EVERY EVEN YEAR";
                shareEspanol = "1/50VO DE UNIDAD FRACCIONAL CADA AÑO PAR";
            }
            else if (salesContract.typeOfFraction == "1/50")
            {
                shareEspanol = "1/50VO DE UNIDAD FRACCIONAL";
                shareIngles = "1/50TH SHARE";
            }

            //Se crea Instancias de clases convertidores de moneda a letras
            CurrencyTranslator currencyTraslateToInglish = new CurrencyTranslator();
            VMConverterCurrencyInSpanish currencyTraslateToSpanish = new VMConverterCurrencyInSpanish();

            String CantidadEnIngles = currencyTraslateToInglish.TranslateCurrency(salesContract.saleAmount).ToUpper();
            String CantidadEnEspañol = currencyTraslateToSpanish.enletras(salesContract.saleAmount.ToString().ToUpper());
            //Se toma la ubicacion del documento en blanco
            String NewContract = Server.MapPath("~/App_Data/ContratoEnBlanco.docx");
            //Se toma el contrato Original y se convierte en arreglo de bytes
            byte[] fileBytesContrato = System.IO.File.ReadAllBytes(Server.MapPath("~/App_Data/Contrato.docx"));
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
            doc.ReplaceText("_client1", nombrePrimerCliente);
            doc.ReplaceText("_client2", nombreSegundoCliente);
            doc.ReplaceText("_address", client.primaryResidenceAddress);
            doc.ReplaceText("_city", client.city);
            doc.ReplaceText("_state", client.state);
            doc.ReplaceText("_zipCode", client.zipCode);
            doc.ReplaceText("_phoneNumber", client.primaryPhoneNumber);
            doc.ReplaceText("_phoneTwo", phone2);
            doc.ReplaceText("_mailTwo", email2);
            doc.ReplaceText("_contractNumber", salesContract.contractNumber);
            doc.ReplaceText("_email", client.emailAddress);
            doc.ReplaceText("_phase", (condo.phase).ToString());
            doc.ReplaceText("_shareIngles", shareIngles);
            doc.ReplaceText("_shareEspanol", shareEspanol);
            doc.ReplaceText("_condo", condo.name);
            doc.ReplaceText("_block", condo.block);
            doc.ReplaceText("_lot", condo.lot);
            doc.ReplaceText("_sqmts", condo.sqmt);
            doc.ReplaceText("_building", condo.building);
            try
            {

                doc.ReplaceText("_typeOfID", client.typeOfID);
                doc.ReplaceText("_nationality", client.nationality);
            }
            catch { }
            decimal totals = (salesContract.saleAmount + salesContract.closingCost);
            String total = totals.ToString("#,##0.00");
            decimal downP = 0;
            if (salesContract.deposit > 0 || salesContract.deposit != null)
            {
                downP = (decimal)salesContract.deposit;

            }
            else
            {
                downP = totals * (decimal).3;
            }
            String dp = downP.ToString("#,##0.00");
            doc.ReplaceText("_saleAmount", (salesContract.saleAmount).ToString("#,##0.00"));
            doc.ReplaceText("_closingCost", (salesContract.closingCost).ToString("#,##0.00"));
            doc.ReplaceText("_totalPrice", total);
            doc.ReplaceText("_downPayment", dp);
            doc.ReplaceText("_precioPalabrasENG", CantidadEnIngles);
            doc.ReplaceText("_precioPalabrasEsp", CantidadEnEspañol);
            doc.ReplaceText("_MonthsFinance", salesContract.paymentsQuantity.ToString());
            doc.ReplaceText("_DownPaymentIngles", DownPaymentsIngles);
            doc.ReplaceText("_DownPaymentEspañol", DownPaymentsEspañol);
            doc.ReplaceText("_initialPayment", Deposit.getInitialDeposit(salesContract, null, null).Amount.ToString("#,##0.00"));
            doc.ReplaceText("_initialPayDate", Deposit.getInitialDeposit(salesContract, null, null).DepositDate.ToString("MMM/dd/yyyy"));
            doc.ReplaceText("_contractDate", salesContract.contractDate.ToString("MMM/dd/yyyy"));
            doc.ReplaceText("_liner1", LinerName);
            doc.ReplaceText("_closer1", CloserName);
            doc.ReplaceText("_vlo", VloName);
            String balance = (totals - downP).ToString("#,##0.00");
            doc.ReplaceText(" _balance", balance);
            doc.ReplaceText("_interestRate", (salesContract.interestRate).ToString());
            doc.ReplaceText("_monthlyPayment", PaymentMonths);
            doc.ReplaceText("_monthPaymentDate", PaymentMonthInitialDate);
            doc.ReplaceText("_paymentsEnd", PaymentMonthEnd);
            doc.ReplaceText("_email1", client.emailAddress);
            doc.ReplaceText("_attachmentsIng", salesContract.attachmentsEnglish==null?"":salesContract.attachmentsEnglish);
            doc.ReplaceText("_attachmentsSpan", salesContract.attachmentsSpanish == null ? "" : salesContract.attachmentsSpanish);
            doc.Save();
            //Se convierte el nuevo documento en un arreglo de bytes
            byte[] fileBytesNuevoContrato = System.IO.File.ReadAllBytes(NewContract);
            //Nombre del docuemnto
            string fileDownloadName = salesContract.contractNumber + nombrePrimerCliente + "&" + nombreSegundoCliente + ".docx";
            //Se regresa el nuevo contrato
            return File(fileBytesNuevoContrato, System.Net.Mime.MediaTypeNames.Application.Octet, fileDownloadName);
        }

        // GET: SalesContract/Edit/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_OFFICER + "," + AccountRolesNames.CONTRACT_MANAGER)]
        public ActionResult Edit(int? id, int? upgrade)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesContract salesContract = db.SalesContracts.Find(id);
            if(upgrade != null)
            {
                var psc = db.SalesContracts.Find(salesContract.previousFCID);
                var ptm = db.trialMemberships.Find(salesContract.previousTMID);
                if (ptm != null)
                {
                    ViewBag.previousTmCN = ptm.contractNumberTM;
                    ViewBag.tmID = ptm.contractID;
                    ViewBag.balance = Deposit.getCurrentBalance(null, ptm, null);
                    ViewBag.tradeInValue = ptm.saleAmount;
                }
                if (psc != null)
                {
                    ViewBag.condoName = psc.condo.name;
                    ViewBag.previoussCN = psc.contractNumber;
                    ViewBag.scID = psc.contractID;
                    ViewBag.balance = Deposit.getCurrentBalance(psc, null, null);
                    ViewBag.tradeInValue = psc.saleAmount;
                }
            }
            if (salesContract == null)
            {
                return HttpNotFound();
            }
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", salesContract.condoID);
            ViewBag.modoPartialView = "modificar";
            return View(salesContract);
        }

        // POST: SalesContract/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR + "," + AccountRolesNames.CONTRACT_MANAGER + "," +
            AccountRolesNames.CONTRACT_OFFICER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SalesContract salesContract, int[] SalesMemberTypes, int[] SalesMembers)
        {
            String otherTypeOfFraction = Request["typeOfFractionOther"];
            if (otherTypeOfFraction != null)
            {
                salesContract.typeOfFraction = otherTypeOfFraction;
            }
            Boolean validMemberSelection = validateSalesMemberSelector(salesContract.contractSalesMembers);
            if (validMemberSelection && ModelState.IsValid)
            {
                //si el contrato fue verificado o cancelado se se agrega la fecha actualcorrespondientemente
                if (salesContract.verifiedByAdmin == true && salesContract.verificationDate == null) { salesContract.verificationDate = DateTime.Now; }
                if (salesContract.canceledContract == true && salesContract.verificationDate == null) { salesContract.canceledDate = DateTime.Now; }
                if (salesContract.csToOwner == true && salesContract.csToOwnerDate == null) { salesContract.csToOwnerDate = DateTime.Now; }
                if (salesContract.csToConcord == true && salesContract.csToConcordDate == null) { salesContract.csToConcordDate = DateTime.Now; }

                //Al relation beetween members and sales contracts are deleted to create new ones.
                try
                {
                    var contractSalesMembers = db.ContractSalesMembers.
                     Where(csm => csm.salesContract.contractID == salesContract.contractID);
                    db.ContractSalesMembers.RemoveRange(contractSalesMembers);
                    db.SaveChanges();
                }
                catch { }
                //Despues de haber sido borrados, se agrega el contratos con las nuevas relaciones
                //de los miembros de venta y se modifica
                db.Entry(salesContract).State = EntityState.Added;
                db.Entry(salesContract).State = EntityState.Modified;
                int registeredModified = db.SaveChanges();
                           
                return RedirectToAction("details",new { id = salesContract.contractID });
            }
            else if (!validMemberSelection)
                ModelState.AddModelError("INVALID_SALES_MEMBERS_SELECTION", "Pair every Sales Member with his role, go to Sales Member selection and verify fields.");

            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", salesContract.condoID);
            salesContract = db.SalesContracts.Find(salesContract.contractID);

            //Roles Selections
            ViewBag.SalesMemberTypes = new SelectList(db.SalesMemberTypes, "rolID", "type");
            //Sales Members Selections
            ViewBag.SalesMembers = new SelectList(
                from member in db.SalesMembers
                select new { salesMemberID = member.salesMemberID, legalName = member.firtName + " " + member.lastName },
                "salesMemberID", "legalName");

            return View(salesContract);
        }

        // GET: SalesContract/Delete/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesContract salesContract = db.SalesContracts.Find(id);
            if (salesContract == null)
            {
                return HttpNotFound();
            }
            return View(salesContract);
        }
        
        // POST: SalesContract/Delete/5
        [Authorize(Roles = AccountRolesNames.ADMINISTRATOR)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SalesContract salesContract = db.SalesContracts.Find(id);
            db.SalesContracts.Remove(salesContract);
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
