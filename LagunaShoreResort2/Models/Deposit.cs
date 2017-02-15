using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml;
using System.Web;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LagunaShoreResort2.Models
{
    public class Deposit
    {
        [Key]
        [DisplayName("Folio No.")]
        public Int32 noFolio { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
        [DisplayName("Deposit Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime DepositDate { get; set; }

        [Range(0,Double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [DisplayName("Currency")]
        public String CurrencyType { get; set; } //MXN, USD

        [DisplayName("Exchange Rate (MXN)")]
        public decimal ExchangeRate { get; set; } //At deposit day

        [DisplayName("Ref. Number")]
        [Required]
        public String RefNumber { get; set; }

        [DisplayName("Payment Method")]
        [Required]
        public String PayMethod { get; set; } //"Cash", "CreditCard", "DebitCard", "Transfer", "Check", "Others"
        
        public bool Refund { get; set; }

        [DisplayName("Comments")]
        public String depositComments { get; set; }

        [DisplayName("Verified By Admin")]
        public bool verification { get; set; }

        //llave foranea de contrato
        public int? salesContractID { get; set; }
        [ForeignKey("salesContractID")]
        public virtual SalesContract salesContract { get; set; }

        public int? trialMembershipID { get; set; }
        public virtual TrialMemberships trialMemberships { get; set; }

        public int? realStateContractID { get; set; }
        [ForeignKey("realStateContractID")]
        public virtual RealStateContract realStateContract { get; set; }

        //Contracts relations for HOA Payments
        public int? salesContractID_HOA { get; set; }
        [ForeignKey("salesContractID_HOA")]
        public virtual SalesContract salesContractHOA { get; set; }

        public int? realStateContractID_HOA { get; set; }
        [ForeignKey("realStateContractID_HOA")]
        public virtual RealStateContract realStateContractHOA { get; set; }
        
        //Default validation error messages
        public sealed class ErrorMessages
        {
            public static readonly String OK="OK";
            public static readonly String AMOUNT_MORE_THAN_BALANCE = "The deposited amount exceeds current balance.";
            public static readonly String AMOUNT_IS_ZERO = "The deposited amount cannot be zero.";
            public static readonly String INVALID_DEPOSIT_DATE = "The selected date for this payment is invalid in this contract.";
        }

        //Definig the constante names of payment types
        public sealed class PaymentTypes
        {
            public static readonly String DOWNPAYMENT = "DownPayment";
            public static readonly String FINNANCE = "Finance";
        }

        /// <summary>
        /// Validates a deposit considering its balance based on received sales contract.
        /// </summary>
        /// <param name="rs">Currency Code of the contract</param>
        /// <param name="saleAmount">Total amount to pay for the contract</param>
        /// <param name="totalPaid">Total amount currently paid in the contract</param>
        /// <returns>A error message string taken from Deposit.ErrorMessages constants. OK is return where
        /// the deposit is valid in the contract</returns>
        public String validateNewDeposit(String currency, decimal saleAmount, decimal totalPaid)
        {
            decimal depositAmount = getAmountInContractCurrency(currency);

            if (depositAmount > (saleAmount - totalPaid))
                return ErrorMessages.OK;
            //return ErrorMessages.AMOUNT_MORE_THAN_BALANCE;
            else if (depositAmount == 0)
                return ErrorMessages.OK;
            //return ErrorMessages.AMOUNT_IS_ZERO;
            else
                return ErrorMessages.OK;
        }

        /// <summary>
        /// Detects automatically what type of contract is related to deposit instance throwing the inital
        /// HOA month when client begins to pay.
        /// </summary>
        /// <returns>A DateTime data that indicates the beginning of payments for HOA</returns>
        public DateTime getInitialHOAMonth()
        {
            String contractType = this.getContractType(this.isHOA());
            int contractID = this.getContractID(this.isHOA());
            DateTime initialHOAMonth= new DateTime();
            ApplicationDbContext db = new ApplicationDbContext();
            if (contractType == "FA" || contractType == "BA")
                initialHOAMonth = db.SalesContracts.Find(contractID).InitialHOAMonth;
            else if(contractType == "RS")
                initialHOAMonth = db.RealStateContracts.Find(contractID).InitialHOAMonth;
            return initialHOAMonth;
        }

        /// <summary>
        /// Validates a deposit considering its balance based on balance taken from SalesContactID attribute in this instance of deposit.
        /// </summary>
        /// <returns>A error message string taken from Deposit.ErrorMessages constants.</returns>
        public String validateNewDeposit()
        {
            bool isHOA = this.isHOA();

            String contractType = this.getContractType(isHOA);
            if (!isHOA) { 
                String currency = "";
                decimal saleAmount = 0, totalPaid=0;
                //For fractional contracts
                if (contractType == "FA" || contractType == "BA")
                {
                    SalesContract sc = getExistingSalesContractFromDB();
                    currency = sc.currency;
                    saleAmount = sc.saleAmount;
                    totalPaid =  getTotalPaid(sc, null, null);
                }
                //For trial memberships contracts
                else if (contractType == "TM" || contractType == "SA" || contractType == "TSA")
                {
                    TrialMemberships tm = getExistingTrialMembershipsFromDB();
                    currency = tm.currency;
                    saleAmount = (decimal)tm.saleAmount;
                    totalPaid = getTotalPaid(null, tm, null);
                }
                //For real state contracts
                else if (contractType == "RS")
                {
                    RealStateContract rs = getExistingRSContractFromDB();
                    currency = rs.currency;
                    saleAmount = rs.saleAmount;
                    totalPaid = getTotalPaid(null, null, rs);
                }
            
                return validateNewDeposit(currency, saleAmount, totalPaid);
            }else { 
                return validateNewHOADeposit();
            }
        }

        private String validateNewHOADeposit()
        {
            DateTime initialHOAMonth = this.getInitialHOAMonth();
            if (this.DepositDate < initialHOAMonth)
                return ErrorMessages.INVALID_DEPOSIT_DATE;
            else
                return ErrorMessages.OK;
        }

        /// <summary>
        /// Given the contract currency, the deposit amount is calculated taking the exchange rate registed
        /// when the contract was created
        /// </summary>
        /// <param name="contractCurrency"></param>
        /// <returns>Returns a decimal data with the total amount of the 
        /// deposit translated to the indicated currency</returns>
        public decimal getAmountInContractCurrency(String contractCurrency)
        {
            decimal result = this.Amount;
            if (this.CurrencyType != contractCurrency)
            {
                if (this.CurrencyType == Currencies.MXN)
                    result = result / this.ExchangeRate;

                if (this.CurrencyType == Currencies.USD)
                    result = result * this.ExchangeRate;
            }

            return result;
        }

        private SalesContract getExistingSalesContractFromDB()
        {
            if (salesContract == null)
            {
                int salesContractID = this.salesContractID.Value;
                ApplicationDbContext db = new ApplicationDbContext();
                return db.SalesContracts.Find(salesContractID);
            }
            else
            {
                return salesContract;
            }
        }
        
        private RealStateContract getExistingRSContractFromDB()
        {
            if (salesContract == null)
            {
                int realStateContractID = this.realStateContractID.Value;
                ApplicationDbContext db = new ApplicationDbContext();
                return db.RealStateContracts.Include("deposits").FirstOrDefault(rsc => rsc.contractID == realStateContractID);
            }
            else
                return realStateContract;
        }

        /// <summary>
        /// Determines de type of contract with an abreviature.
        /// </summary>
        /// <param name="isHOA">Boolean value that indicates the type of deposit
        /// to determine properly the type of contract</param>
        /// <returns>A String "FA" or "BA" for Fractional Sales Contracts,
        /// "TM" for Trial Memberships and "RS" for Real States Contracts</returns>
        public string getContractType(bool isHOA = false)
        {
            String contractType = "";
            if (!isHOA) { 
                if(this.salesContractID != null)
                {
                    contractType = "FA";
                }
                else if (this.trialMembershipID != null)
                {
                    contractType = "TM";
                }
                else if (this.realStateContractID != null)
                {
                    contractType = "RS";
                }
            }
            else
            {
                if (this.salesContractID_HOA != null)
                {
                    contractType = "FA";
                }
                else if (this.realStateContractID_HOA != null)
                {
                    contractType = "RS";
                }
            }
            return contractType;
        }

        /// <summary>
        /// Determines de controller name that manage HTTP Requests for related contract to this deposit.
        /// </summary>
        /// <returns>Returns the controller name of the contract related to this deposit</returns>
        public string getControllerName()
        {
            String controllerName = "";
            if (this.salesContractID != null || this.salesContractID_HOA != null)
            {
                controllerName = "SalesContract";
            }
            else if (this.realStateContractID != null || this.realStateContractID_HOA != null)
            {
                controllerName = "RealStateContracts";
            }
            else if (this.trialMembershipID != null)
            {
                controllerName = "TrialMemberships";
            }
            return controllerName;
        }

        /// <summary>
        /// Returns controller name that corresponds with the type of contract paid.
        /// </summary>
        /// <param name="isHOA">Boolean value to indicate if the deposit its and HOA payment.</param>
        /// <returns>Contract number corresponding to the contract related to this deposit.</returns>
        public String getContractNumber(Boolean isHOA = false)
        {
            String contractNumber = "";
            ApplicationDbContext db = new ApplicationDbContext();
            if (!isHOA) { 
                if (this.salesContract != null)
                {
                    contractNumber = this.salesContract.contractNumber;
                }
                else if (this.trialMemberships != null)
                {
                    contractNumber = this.trialMemberships.contractNumberTM;
                }
                if (String.IsNullOrEmpty(contractNumber))
                {
                    if (this.salesContractID != null || this.salesContractID > 0)
                    {
                       try
                        {
                            contractNumber = db.SalesContracts.Find(this.salesContractID).contractNumber;
                        }
                        catch
                        {
                            contractNumber = "No Contract";
                        }
                    }
                    else if (this.trialMembershipID != null || this.trialMembershipID > 0)
                    {
                        try
                        {
                            contractNumber = db.trialMemberships.Find(this.trialMembershipID).contractNumberTM;
                        }
                        catch
                        {
                            contractNumber = "No Contract";
                        }
                    }
                }
            }else
            {
                if (this.salesContractID_HOA != null)
                {
                    contractNumber = contractNumber = db.SalesContracts.Find(this.salesContractID_HOA).contractNumber;
                }
            }

            return contractNumber;
        }

        /// <summary>
        /// Gets automatically the contract related with the deposit
        /// </summary>
        /// <param name="isHOA">Boolean value to indicate if the deposit its and HOA payment.</param>
        /// <returns></returns>
        public int getContractID(bool isHOA = false)
        {
            int contractID = 0;
            if (!isHOA)
            { //Finnace Deposit 
                if (this.salesContractID != null)
                {
                    contractID = this.salesContractID.Value;
                }
                else if (this.trialMembershipID != null)
                {
                    contractID = this.trialMembershipID.Value;
                }
                else if (this.realStateContractID != null)
                {
                    contractID = this.realStateContractID.Value;
                }
            }
            else //HOA Deposit
            {
                if (this.salesContractID_HOA != null)
                {
                    contractID = this.salesContractID_HOA.Value;
                }
                else if (this.realStateContractID_HOA != null)
                {
                    contractID = this.realStateContractID_HOA.Value;
                }
            }
            return contractID;
        }

        /// <summary>
        /// Gets automatically the contract related with the deposit
        /// </summary>
        /// <param name="isHOA">Boolean value to indicate if the deposit its and HOA payment.</param>
        /// <returns></returns>
        public String getContractLegalNameOwner(bool isHOA = false)
        {
            String legalName = "";
            if (!isHOA)
            { //Finnace Deposit 
                if (this.salesContractID != null)
                {
                    try
                    {
                        legalName = this.salesContract.client.legalName;
                    }
                    catch
                    {
                        legalName = "No name";
                    }
                }
                else if (this.trialMembershipID != null)
                {
                    try
                    {
                        legalName = this.trialMemberships.client.legalName;
                    }
                    catch
                    {
                        legalName = "No name";
                    }
                }
                else if (this.realStateContractID != null)
                {
                    try
                    {
                        legalName = this.realStateContract.clientAssigned.legalName;
                    }
                    catch
                    {
                        legalName = "No name";
                    }
                }
            }
            else //HOA Deposit
            {
                if (this.salesContractID_HOA != null)
                {
                    legalName = this.salesContract.client.legalName;
                }
                else if (this.realStateContractID_HOA != null)
                {
                    legalName = this.realStateContract.clientAssigned.legalName;
                }
            }
            return legalName;
        }

        /// <summary>
        /// Gets automatically the client ID related with the deposit
        /// </summary>
        /// <param name="isHOA">Boolean value to indicate if the deposit its and HOA payment.</param>
        /// <returns></returns>
        public int getClientID(bool isHOA = false)
        {
            int clientID = 0;
            ApplicationDbContext db = new ApplicationDbContext();
            if (!isHOA)
            { //Finnace Deposit 
                if (this.salesContractID != null)
                {
                    clientID = db.SalesContracts.Find(this.salesContractID).client.clientID;
                }
                else if (this.trialMembershipID != null)
                {
                    clientID = db.trialMemberships.Find(this.trialMembershipID).client.clientID;
                }
                else if (this.realStateContractID != null)
                {
                    clientID = db.RealStateContracts.Find(this.realStateContractID).clientAssigned.clientID;
                }
            }
            else //HOA Deposit
            {
                if (this.salesContractID_HOA != null)
                {
                    clientID = db.SalesContracts.Find(this.salesContractID_HOA).client.clientID;
                }
                else if (this.realStateContractID_HOA != null)
                {
                    clientID = db.RealStateContracts.Find(this.realStateContractID_HOA).clientAssigned.clientID;
                }
            }
            return clientID;
        }

        private TrialMemberships getExistingTrialMembershipsFromDB()
        {
            if(trialMemberships  == null)
            {
                int trialMebershipID = this.trialMembershipID.Value;
                ApplicationDbContext db = new ApplicationDbContext();
                return db.trialMemberships.Find(trialMebershipID);
            }
            else
            {
                return trialMemberships;
            }
        }
        public static Object[] getPaymentMethodsArray()
        {
            Object[] paymentMethods = new Object[]{
                    new { Text="Cash", Value = "Cash" },
                    new { Text="Credit Card", Value = "Credit Card" },
                    new { Text="Debit Card", Value = "Debit Card" },
                    new { Text="Wire Transfer", Value = "Wire Transfer" },
                    new { Text="Check", Value = "Check" },
                    new {Text = "Concord", Value = "Concord"},
                    new { Text="Other...", Value = "" }
                };

            return paymentMethods;
        }

        public static Object[] getCurrencyArray()
        {
            Object[] paymentMethods = new Object[]{
                    new { Text="US Dollars", Value = Currencies.USD, Selected=true },
                    new { Text="Mexican Pesos", Value = Currencies.MXN }
                };

            return paymentMethods;
        }

        //Defining constants for currency names.
        public static class Currencies
        {
            public const string MXN = "MXN";
            public const string USD = "USD";
        }

        //Amortization for all types of contracts
        public static List<Payment> getPaymentPlan(SalesContract sc, TrialMemberships tm, RealStateContract rc)
        {
            double closingCost =0, totalAmount = 0, downPayment = 0, loanToPay = 0, saleAmount =0, tradeInValue =0, balance1=0;
            int numberOfDownPayments =0, pmtsQuantity =0; 
            DateTime dateToPay = DateTime.Now; 
            if(sc != null)
            {
                if (sc.upgrade == true)
                {
                    saleAmount = (double)sc.saleAmount;
                    tradeInValue = (double)sc.tradeInValue;
                    numberOfDownPayments = sc.NumberofDownPayments;
                    pmtsQuantity = sc.paymentsQuantity;
                    totalAmount = (double)sc.newUnitPrice;
                    closingCost = (double)sc.closingCost;
                    balance1 = totalAmount + closingCost;
                    dateToPay = sc.contractDate;
                    if (sc.deposit != null)
                    {
                        downPayment = (double)sc.deposit;
                        loanToPay = (balance1 - downPayment) + (double)sc.previousBalance;
                    }
                    else
                    {
                        downPayment = balance1 * .3;
                        loanToPay = (balance1 - downPayment) + (double)sc.previousBalance;
                    }

                }
                else
                {
                    closingCost = (double)sc.closingCost;
                    totalAmount = (double)sc.saleAmount;
                    numberOfDownPayments = sc.NumberofDownPayments;
                    pmtsQuantity = sc.paymentsQuantity;
                    dateToPay = sc.contractDate;
                    if (sc.deposit != null)
                    {
                        downPayment = (double)sc.deposit;
                        loanToPay = (totalAmount + closingCost) - downPayment;
                    }
                    else
                    {
                        downPayment = (totalAmount + closingCost) * .3;
                        loanToPay = (totalAmount + closingCost) - downPayment;
                    }
                }
            }
            else if (tm != null)
            {
                
                totalAmount = (double)tm.saleAmount;
                numberOfDownPayments = tm.NumberofDownPayments;
                pmtsQuantity = tm.tmNumberPayments;
                dateToPay = tm.tmContractDate;
                if (tm.deposit != null)
                {
                    downPayment = (double)tm.deposit;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
                else
                {
                    downPayment = (totalAmount + closingCost) * .3;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
            }
            else if (rc != null)
            {
                closingCost = (double)rc.closingCost;
                pmtsQuantity = rc.paymentsQuantity;
                totalAmount = (double)rc.saleAmount;
                numberOfDownPayments = rc.NumberofDownPayments;
                dateToPay = rc.saleDate;
                if (rc.deposits != null)
                {
                    downPayment = (double)rc.deposit;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
                else
                {
                    downPayment = (totalAmount + closingCost) * .3;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
            }
            Payment payment;
            List<Payment> payments = new List<Payment>();

            //Generating payment plan for downpayment, 10 payments by default
            int paymentsQtyForDownPayment = numberOfDownPayments;//this.NumberofDownPayments;
            int paymentCounter = 1;
            //decimal initialDeposit = getInitialDeposit().Amount;
            //double iniDepo = Convert.ToDouble(initialDeposit);
            //downPayment = downPayment - iniDepo;
            //for (int c = 0; c < paymentsQtyForDownPayment; c++)
            //{
            //    //Re-using finance payment constructor code for downpayment, taking interest as 0 and monthly pay as amortization.
            //    payment = new Payment(paymentCounter, dateToPay, (decimal)downPayment / paymentsQtyForDownPayment, 0, Deposit.PaymentTypes.DOWNPAYMENT);
            //    payments.Add(payment);
            //    dateToPay = dateToPay.AddMonths(1);//Next month
            //    paymentCounter++;
            //}
            //TESTING DP to calculate as a full
            for (int c = 0; c < paymentsQtyForDownPayment; c++)
            {
                //Re-using finance payment constructor code for downpayment, taking interest as 0 and monthly pay as amortization.
                payment = new Payment(paymentCounter, dateToPay, (decimal)downPayment /*/ paymentsQtyForDownPayment*/, 0, Deposit.PaymentTypes.DOWNPAYMENT);
                payments.Add(payment);
                dateToPay = dateToPay.AddMonths(1);//Next month
                paymentCounter++;
            }
            //Generating payment plan to pay loan of financing, remaining months after downpayment is paid

            for (int c = 0; c < pmtsQuantity; c++)
            {
                payment = new Payment(paymentCounter, dateToPay, 0, 0, Deposit.PaymentTypes.FINNANCE);
                payments.Add(payment);
                dateToPay = dateToPay.AddMonths(1);//Next month
                paymentCounter++;
            }

            return payments.OrderBy(pay => pay.dateToPay).ToList();
        }

        /// <summary>
        /// Takes all deposits associated with this instance and generates on the fly the monitoring payments list.
        /// </summary>
        /// <returns>A List<PaymentDeposits> wich pairs the monthly payment with the deposit made for it.</returns>
        public static List<PaymentDeposits> getDepositsInPayments(SalesContract sc, TrialMemberships tm, RealStateContract rc)
        {

            List<Payment> payments = getPaymentPlan(sc, tm, rc);
            double closingCost = 0, totalAmount = 0, downPayment = 0, loanToPay = 0,
                monthInterestPercent = 0, balance = 0, tradeInValue = 0, saleAmount = 0;
            int numberOfDownPayments = 0;
            DateTime dateToPay = DateTime.Now;
            List<Deposit> deposits = new List<Deposit>(); 
            if (sc != null)
            {
                if(sc.upgrade == true)
                {
                    saleAmount = (double)sc.saleAmount;
                    tradeInValue = (double)sc.tradeInValue;
                    totalAmount = (double)sc.newUnitPrice;
                    numberOfDownPayments = sc.NumberofDownPayments;
                    dateToPay = sc.contractDate;
                    deposits = sc.deposits.ToList();
                    monthInterestPercent = (sc.interestRate / 100) / 12;
                    closingCost = (double)sc.closingCost;
                    balance = totalAmount + closingCost + (double)sc.previousBalance; ; 
                    if(sc.deposit != null)
                    {
                        downPayment = (double)sc.deposit;
                        loanToPay = (balance - downPayment);
                    }
                    else
                    {
                        downPayment = balance * .3;
                        loanToPay = (balance - downPayment); 
                    }
                    
                }
                else
                {
                    closingCost = (double)sc.closingCost;
                    totalAmount = (double)sc.saleAmount;
                    numberOfDownPayments = sc.NumberofDownPayments;
                    dateToPay = sc.contractDate;
                    deposits = sc.deposits.ToList();
                    monthInterestPercent = (sc.interestRate / 100) / 12;
                    balance = totalAmount + closingCost;
                    if (sc.deposit != null)
                    {
                        downPayment = (double)sc.deposit;
                        loanToPay = (totalAmount + closingCost) - downPayment;
                    }
                    else
                    {
                        downPayment = (totalAmount + closingCost) * .3;
                        loanToPay = (totalAmount + closingCost) - downPayment;
                    }
                }
            }
            else if (tm != null)
            {
                monthInterestPercent = (tm.interestRate / 100) / 12;
                totalAmount = (double)tm.saleAmount;
                numberOfDownPayments = tm.NumberofDownPayments;
                dateToPay = tm.tmContractDate;
                balance = totalAmount + closingCost;
                deposits = tm.deposits.ToList(); 
                if (tm.deposit != null)
                {
                    downPayment = (double)tm.deposit;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
                else
                {
                    downPayment = (totalAmount + closingCost) * .3;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
            }
            else if (rc != null)
            {
                closingCost = (double)rc.closingCost;
                monthInterestPercent = (rc.interestRate / 100) / 12;
                totalAmount = (double)rc.saleAmount;
                numberOfDownPayments = rc.NumberofDownPayments;
                dateToPay = rc.saleDate;
                deposits = rc.deposits.ToList();
                balance = totalAmount + closingCost;
                if (rc.deposits != null)
                {
                    downPayment = (double)rc.deposit;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
                else
                {
                    downPayment = (totalAmount + closingCost) * .3;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
            }
            deposits = deposits.Where(dep => !dep.Refund && dep.verification == true).OrderBy(dep => dep.DepositDate).ToList();
            //deposits.OrderBy(dep => dep.DepositDate);
            List<PaymentDeposits> listPayDeposit = new List<PaymentDeposits>();
            //downPayment = downPayment - iniDepo;
            decimal pmt = Math.Round(getPMT(sc, tm, rc),2); //Return the financial calculation of finance monthly payment for principal or amortization 

            PaymentDeposits pd = new PaymentDeposits();

            int i = 0, p = 0, monthsPaidAtOnce = 0;
            decimal montoDeposito = 0, sumaDeposit=0;
            bool incompletePayment;
            for (; p < payments.Count; p++)
            {

                Payment pay = payments.ElementAt(p);

                //CALCULAR INTERES Y AMORTIZACION
                if (pay.paymentType == Deposit.PaymentTypes.FINNANCE)
                {
                    pay.Interest = Math.Round((decimal)(balance * monthInterestPercent),2); //Calculate interest per month taking current balance
                    pay.Amortization = Math.Round(pmt - pay.Interest,2); //Calculuate amortization which is PMT after interest
                }
                if(sumaDeposit == (decimal)downPayment)
                {
                    //if (numberOfDownPayments > 1)
                    //{
                    //    i++;
                    //}
                    incompletePayment = true;
                }
                else
                {
                    incompletePayment = false;
                }
                
                for (; i < deposits.Count; i++)
                {
                    if(i>=1)
                    {

                        if(pay.paymentType == PaymentTypes.DOWNPAYMENT)
                        {
                            if(balance == loanToPay)
                            {
                                p++;
                                break;
                            }
                        }
                    }
                    Deposit deposit = deposits.ElementAt(i);
                    
                    //and previous deposit was not downpayment or is DownPaymentToFinnaceTransition
                    if ((p != numberOfDownPayments && pay.paymentType != Deposit.PaymentTypes.DOWNPAYMENT) ||
                        montoDeposito <= 0 || incompletePayment)
                        if (sc != null)
                        {
                            montoDeposito = Math.Round(deposit.getAmountInContractCurrency(sc.currency), 2); //Deposited amount
                            incompletePayment = false; 
                        }
                        else if (tm != null)
                        {
                            montoDeposito = Math.Round(deposit.getAmountInContractCurrency(tm.currency), 2); //Deposited amount
                            incompletePayment = false;
                        }
                        else if (rc != null)
                        {
                            montoDeposito = Math.Round(deposit.getAmountInContractCurrency(rc.currency), 2); //Deposited amount
                            incompletePayment = false;
                        }  
                    //Creates a pair payment-deposit for view
                    //If a deposit did not was enough to pay a complete month
                    if (incompletePayment)
                    {
                        if (deposit.Amount == pmt)
                        {
                            break;
                        }
                        else
                        {
                            pd = new PaymentDeposits(pay, deposit, pd.remainingAmortization, pd.remainingInterest, (decimal)balance);
                            incompletePayment = false;
                        }
                    }
                    else
                        pd = new PaymentDeposits(pay, deposit, (decimal)balance);

                    //Deposit cases
                    if (Math.Round(montoDeposito,2) < Math.Round(pd.remainingInterest,2)) //No apply in downpayment
                    {
                        //Interest is paid first
                        pd.remainingInterest -= montoDeposito;
                        //deposit is done
                        incompletePayment = true;
                    }
                    else if (Math.Round(montoDeposito,2) >= Math.Round(pd.remainingInterest,2))
                    {
                        
                        //All the interest is paid
                        montoDeposito -= pd.remainingInterest;
                        pd.remainingInterest = 0;

                        //Amortization is paid with remaining deposit
                        //if (montoDeposito <= pd.remainingAmortization)
                        if (sumaDeposit < (decimal)downPayment)
                        {
                            if (Math.Round(montoDeposito, 2) <= Math.Round(pd.remainingAmortization, 2))
                            {

                                //And so is balance with remaining deposit
                                balance -= (double)montoDeposito;
                                pd.balance = (decimal)balance;

                                pd.remainingAmortization -= montoDeposito;//Principal is paid

                                //if (Math.Round(montoDeposito, 2) < Math.Round(pd.remainingAmortization, 2))
                                //{
                                //    //deposit is done
                                incompletePayment = true;
                                //}
                                //reset payments counter
                                monthsPaidAtOnce = 0;
                                sumaDeposit += deposit.Amount;

                                //break;
                            }
                            else
                            {
                                if (pay.paymentType == Deposit.PaymentTypes.FINNANCE)
                                {
                                    //And so is balance with remaining deposit
                                    balance -= Math.Round((double)montoDeposito, 2);
                                    pd.balance = (decimal)balance;

                                    pd.remainingAmortization = 0;//Principal is paid
                                                                 //Financial movement is saved
                                    listPayDeposit.Add(pd);
                                    i++;//deposit is done
                                    sumaDeposit += deposit.Amount;
                                    break;//monthly payment is done
                                }
                                else if (pay.paymentType == Deposit.PaymentTypes.DOWNPAYMENT)
                                {//And so is balance with remaining deposit
                                 //Remaining principal pay balance and is done

                                    balance -= Math.Round((double)pd.remainingAmortization, 2);
                                    pd.balance = (decimal)balance;

                                    //Deposit amount still remain for next monthly payment
                                    montoDeposito -= Math.Round(pd.remainingAmortization, 2);
                                    pd.remainingAmortization = 0;//Principal is paid
                                    listPayDeposit.Add(pd);

                                    //If a DOWNPAYMENT month was completed, dates to pay for nextmonths are readjusted
                                    if (monthsPaidAtOnce > 0)
                                        for (int c = p + 1; c < payments.Count(); c++)
                                            payments.ElementAt(c).dateToPay = payments.ElementAt(c).dateToPay.AddMonths(-1);

                                    monthsPaidAtOnce++;
                                    sumaDeposit += deposit.Amount;
                                    if (balance > loanToPay)
                                        break;//monthly payment is done
                                }
                            }
                        }
                        else
                        {
                            if (pay.paymentType == Deposit.PaymentTypes.FINNANCE)
                            {
                                //And so is balance with remaining deposit
                                balance -= Math.Round((double)montoDeposito,2); 
                                pd.balance = (decimal)balance;

                                pd.remainingAmortization = 0;//Principal is paid
                                //Financial movement is saved
                                listPayDeposit.Add(pd);
                                i++;//deposit is done
                                sumaDeposit += deposit.Amount;
                                break;//monthly payment is done
                            }
                            else if (pay.paymentType == Deposit.PaymentTypes.DOWNPAYMENT)
                            {//And so is balance with remaining deposit
                                //Remaining principal pay balance and is done
                                
                                balance -= Math.Round((double)pd.remainingAmortization,2);
                                pd.balance = (decimal)balance;

                                //Deposit amount still remain for next monthly payment
                                montoDeposito -= Math.Round(pd.remainingAmortization,2);
                                pd.remainingAmortization = 0;//Principal is paid
                                listPayDeposit.Add(pd);

                                //If a DOWNPAYMENT month was completed, dates to pay for nextmonths are readjusted
                                if (monthsPaidAtOnce > 0)
                                    for (int c = p + 1; c < payments.Count(); c++)
                                        payments.ElementAt(c).dateToPay = payments.ElementAt(c).dateToPay.AddMonths(-1);

                                monthsPaidAtOnce++;
                                sumaDeposit += deposit.Amount;
                            if (balance > loanToPay)
                                break;//monthly payment is done
                            }
                        }
                    }

                    //Financial movement is saved when month is incomplete
                    listPayDeposit.Add(pd);
                }//Ends foreach deposits

                //Count equals to array size to indicate deposits are done with a incomplete payment pending
                if (i == deposits.Count && Math.Round(balance) > 0 && pd.payment != pay)
                    listPayDeposit.Add(new PaymentDeposits(pay, (decimal)balance));//Add PaymentDeposit without deposit.
            }//Ends foreach payments

            return listPayDeposit;
        }//ends getDepositsInPayments()

        /// <summary>
        /// Return the financial calculation of finance monthly payment for principal or amortization 
        /// </summary>
        /// <returns>The calculated PMT for the current attributes of sales contract instance.</returns>
        public static decimal getPMT(SalesContract sc, TrialMemberships tm, RealStateContract rc)
        {
            double closingCost = 0, totalAmount = 0, downPayment = 0, loanToPay = 0,
                monthInterestPercent = 0, balance = 0, tradeInValue = 0, saleAmount = 0;
            int numberOfDownPayments = 0, paymentsQuantity=0;
            DateTime dateToPay = DateTime.Now;
            ICollection<Deposit> depositss = null;
            if (sc != null)
            {
                List<Deposit> deposits = new List<Deposit>();
                if (sc != null)
                {
                    if (sc.upgrade == true)
                    {
                        saleAmount = (double)sc.saleAmount;
                        tradeInValue = (double)sc.tradeInValue;
                        totalAmount = (double)sc.newUnitPrice;
                        numberOfDownPayments = sc.NumberofDownPayments;
                        dateToPay = sc.contractDate;
                        deposits = sc.deposits.ToList();
                        monthInterestPercent = (sc.interestRate / 100) / 12;
                        closingCost = (double)sc.closingCost;
                        balance = totalAmount + closingCost;
                        paymentsQuantity = sc.paymentsQuantity;
                        if (sc.deposit != null)
                        {
                            downPayment = (double)sc.deposit;
                            loanToPay = (balance - downPayment) + (double)sc.previousBalance;
                        }
                        else
                        {
                            downPayment = balance * .3;
                            loanToPay = (balance - downPayment) + (double)sc.previousBalance;
                        }

                    }
                    else
                    {
                        closingCost = (double)sc.closingCost;
                        totalAmount = (double)sc.saleAmount;
                        numberOfDownPayments = sc.NumberofDownPayments;
                        dateToPay = sc.contractDate;
                        depositss = sc.deposits;
                        monthInterestPercent = (sc.interestRate / 100) / 12;
                        balance = totalAmount + closingCost;
                        paymentsQuantity = sc.paymentsQuantity;
                        if (sc.deposit != null)
                        {
                            downPayment = (double)sc.deposit;
                            loanToPay = (totalAmount + closingCost) - downPayment;
                        }
                        else
                        {
                            downPayment = (totalAmount + closingCost) * .3;
                            loanToPay = (totalAmount + closingCost) - downPayment;
                        }

                    }
                }
            }
            else if (tm != null)
            {
                monthInterestPercent = (tm.interestRate / 100) / 12;
                totalAmount = (double)tm.saleAmount;
                numberOfDownPayments = tm.NumberofDownPayments;
                dateToPay = tm.tmContractDate;
                balance = totalAmount + closingCost;
                depositss = tm.deposits;
                paymentsQuantity = tm.tmNumberPayments;
                if (tm.deposit != null)
                {
                    downPayment = (double)tm.deposit;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
                else
                {
                    downPayment = (totalAmount + closingCost) * .3;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
            }
            else if (rc != null)
            {
                closingCost = (double)rc.closingCost;
                monthInterestPercent = (rc.interestRate / 100) / 12;
                paymentsQuantity = rc.paymentsQuantity;
                totalAmount = (double)rc.saleAmount;
                numberOfDownPayments = rc.NumberofDownPayments;
                dateToPay = rc.saleDate;
                depositss = rc.deposits;
                balance = totalAmount + closingCost;
                if (rc.deposits != null)
                {
                    downPayment = (double)rc.deposit;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
                else
                {
                    downPayment = (totalAmount + closingCost) * .3;
                    loanToPay = (totalAmount + closingCost) - downPayment;
                }
            }
            //double monthInterestPercent = (this.interestRate / 100) / 12; //In LSR, interest percentage in 12 months
            //double closingCost = (double)this.closingCost;
            //double downPayment = 0;
            //if (deposit != null)
            //{
            //    downPayment = (double)this.deposit;
            //}
            //else
            //{
            //    downPayment = ((double)this.saleAmount + closingCost) * .3;//Total downpayment is 30% of total amount to pay
            //}
            ////((double)this.saleAmount +closingCost)* .3;//Total downpayment is 30% of total amount to pay
            //double loanToPay = ((double)this.saleAmount + closingCost) - downPayment; //Balance is initially the total loan to pay
            if (paymentsQuantity == 0)
            {
                paymentsQuantity = 1;
            }
            return (decimal)-Financial.Pmt(monthInterestPercent, paymentsQuantity, loanToPay); //Calculate the PMT to generate payment plan
        }
        public static decimal getTotalPaid(SalesContract sc, TrialMemberships tm, RealStateContract rc)
        {
           
            List<Payment> payment;
            ICollection<Deposit> deposits;
            decimal result = 0;
            if (sc != null)
            {
                var paymentssc = sc.deposits;
                paymentssc = sc.deposits;
                payment = getPaymentPlan(sc, null, null);
                deposits = sc.deposits.Where(dep => !dep.Refund && dep.verification == true).OrderBy(dep => dep.DepositDate).ToList();
                int i = 0, p = 0;
                for (; p<paymentssc.Count; p++)
                {
                    for (; i<deposits.Count; i++)
                    {
                        Deposit deposit = deposits.ElementAt(i);
                      decimal paymentCurrency = deposit.getAmountInContractCurrency(sc.currency);
                       result += paymentCurrency;
                    }

                }
            }else if (tm != null)
            {
                var paymentstm = tm.deposits;
                paymentstm = tm.deposits;
                payment = getPaymentPlan(null, tm, null);
                deposits = tm.deposits.Where(dep => !dep.Refund && dep.verification == true).OrderBy(dep => dep.DepositDate).ToList();
                int i = 0, p = 0;
                for (; p<paymentstm.Count; p++)
                {
                    for (; i<deposits.Count; i++)
                    {
                        Deposit deposit = deposits.ElementAt(i);
                        decimal paymentCurrency = deposit.getAmountInContractCurrency(tm.currency);
                        result += paymentCurrency;
                    }

                }
            }
            else if (rc != null)
            {
                var paymentsrc = rc.deposits;
                paymentsrc = rc.deposits;
                payment = getPaymentPlan(null, null, rc);
                deposits = rc.deposits.Where(dep => !dep.Refund && dep.verification == true).OrderBy(dep => dep.DepositDate).ToList();
                int i = 0, p = 0;
                for (; p<paymentsrc.Count; p++)
                {
                    for (; i<deposits.Count; i++)
                    {
                        Deposit deposit = deposits.ElementAt(i);
                        decimal paymentCurrency = deposit.getAmountInContractCurrency(rc.currency);
                        result += paymentCurrency;
                    }

                }
            }
            return result;
        }

        public static decimal getCurrentBalance(SalesContract sc, TrialMemberships tm, RealStateContract rc)
        {
            List<PaymentDeposits> payments = null;
            decimal result = 0; 
            if (sc != null)
            {
                payments = getDepositsInPayments(sc, null, null);
                //result = payments.Last().balance;
                if (payments.Count() > 0)
                {
                    result = payments.Last().balance;
                }
                else
                {
                    result = sc.saleAmount;
                }
            }
            else if (tm != null)
            {
                payments = getDepositsInPayments(null, tm, null);
                //result = payments.Last().balance;
                if (payments.Count() > 0)
                {
                    result = payments.Last().balance;
                }
                else
                {
                    result = (decimal)tm.saleAmount;
                }
            }
            else if (rc != null)
            {
                payments = getDepositsInPayments(null, null, rc);
                //result = payments.Last().balance;
                if (payments.Count() > 0)
                {
                    result = payments.Last().balance;
                }
                else
                {
                    result = rc.saleAmount;
                }
            }
 
           return result; 
       }

        public static Deposit getInitialDeposit(SalesContract sc, TrialMemberships tm, RealStateContract rc)
        {
            Deposit initial = new Deposit();
            if (sc != null)
            {
                 initial = sc.deposits.OrderBy(pay => pay.DepositDate).FirstOrDefault();
            }
            else if (tm != null)
             {
                 initial = tm.deposits.OrderBy(pay => pay.DepositDate).FirstOrDefault();
             }
             else if (rc != null)
            {
                initial = rc.deposits.OrderBy(pay => pay.DepositDate).FirstOrDefault();
            }
             return initial;
         }
 
         /// <summary>
         /// Determines DownPayment of current instace if paid.
         /// </summary>
         /// <returns>Boolean indicating if Downpayment is completely paid.</returns>
         public static Boolean isDownPaymentCompleted(SalesContract sc, TrialMemberships tm, RealStateContract rc)
         {
             double currentBalance = 0, totalDownPayment = 0;
             Boolean result = false; 
             if (sc != null)
             {
                 currentBalance = (double)getCurrentBalance(sc, null, null);
                 totalDownPayment = ((double)sc.saleAmount + (double)sc.closingCost) * .3;
                 result = (((double)sc.saleAmount + (double)sc.closingCost) - currentBalance) >= totalDownPayment;
             }
             else if (tm != null)
             {
                 currentBalance = (double)getCurrentBalance(null, tm, null);
                 totalDownPayment = ((double)tm.saleAmount) * .3;
                 result = (((double)tm.saleAmount) - currentBalance) >= totalDownPayment;
             }
             else if (rc != null)
             {
                 currentBalance = (double)getCurrentBalance(null, null, rc);
                 totalDownPayment = ((double)rc.saleAmount + (double)rc.closingCost) * .3;
                 result = (((double)rc.saleAmount + (double)rc.closingCost) - currentBalance) >= totalDownPayment;
             }
 
             return result;
        }

        /// <summary>
        /// Checks foreing keys for HOA payments collections in Sales Contract or
        /// Real States Contract entities.
        /// </summary>
        /// <returns>Returns a boolean value indicating if deposit is for HOA payments.</returns>
        public Boolean isHOA()
        {
            return salesContractID_HOA != null || realStateContractID_HOA != null;
        }

        public static class ContractTypes
        {
            public static readonly String FA = "FA";
            public static readonly String BA = "BA";
            public static readonly String TM = "TM";
            public static readonly String RS = "RS";
        }
    }
}