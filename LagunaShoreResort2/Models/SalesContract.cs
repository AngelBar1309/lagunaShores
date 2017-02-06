using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LagunaShoreResort2.Models
{
    public class SalesContract
    {
        [DisplayName("Sales Contract ID")]
        public int salesContractID { get; set; }//Primary Key

        [DisplayName("Contract Number")]
        public string contractNumber { get; set; }
        [DisplayName("Contract Type")]
        public string contractType { get; set; }
        [DisplayName("Type Of Fraction")]
        public string typeOfFraction { get; set; }

        [DisplayName("Contract Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime contractDate { get; set; }

        [DisplayName("Sale Amount")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal saleAmount { get; set; }

        [DisplayName("Closing Cost")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal closingCost { get; set; }

        [DisplayName("List Price")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public double listPrice { get; set; }
        //***Start
        //Added to Production
        // This is the new DP for all contracts this is not yet done
        [DisplayName("Deposit")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal? deposit { get; set; }
        //***END

        //uncomment when changes are ready to be make for inventory, and upgrades
        [DisplayName("Trade in Value")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal? tradeInValue { get; set; }


        [DisplayName("Previous Balance")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal? previousBalance { get; set; }

        //Need to add, uncomment to continue with upgrade process.
        [DisplayName("New Unit Price")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal? newUnitPrice { get; set; }

        [DisplayName("Upgrade")]
        public bool upgrade { get; set; }

        [DisplayName("Upgraded Contract")]
        public bool upgraded { get; set; }

        [DisplayName("Previous TM Contract")]
        public int? previousTMID { get; set; }

        [DisplayName("Previous Fractional Contract")]
        public int? previousFCID { get; set; }

        //noraml contracts
        [DisplayName("Interest Rate")]
        [Range(0, 100)]
        public double interestRate { get; set; }

        [DisplayName("Verified By Contract Manager")]
        public bool verifiedByAdmin { get; set; } //Modificable

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Verification Date")]
        public DateTime? verificationDate { get; set; }//Modificable


        [DisplayName("Request to Accountant")]
        public Boolean requestToAccountant { get; set; }//Modificable

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Request to Accountant Date")]
        public DateTime? requestToAccountantDate { get; set; }//Modificable

        [DisplayName("Comments")]
        public string comments { get; set; }//Modificable

        [DisplayName("Canceled Contract")]
        public bool canceledContract { get; set; }//Modificable

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Canceled Date")]
        [DataType(DataType.Date)]
        public DateTime? canceledDate { get; set; }//Modificable

        [DisplayName("Number of DownPayments")]
        public int NumberofDownPayments { get; set; }

        [DisplayName("Qualification ")]
        public String qualification { get; set; }

        [DisplayName("Ofertas Adicionales (Español)")]
        public string attachmentsSpanish { get; set; }//Modificable

        [DisplayName("Attachments (English)")]
        public string attachmentsEnglish { get; set; }//Modificable

        [DisplayName("Currency")]
        public String currency { get; set; }

        [DisplayName("Deed Weeks")]
        public int deedWeeks { get; set; }//Modificable

        [DisplayName("Bonus Weeks")]
        public double bonusWeeks { get; set; }//Modificable

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Bonus Weeks Expiration")]
        public DateTime bonusWeeksExp { get; set; }//Modificable

        [DisplayName("Advantage Weeks")]
        public double advantageWeeks { get; set; }//Modificable

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Advantage Weeks Expiration")]
        [DataType(DataType.Date)]
        public DateTime advantageWeeksExp { get; set; }//Modificable

        [DisplayName("Commission Paid")]
        public Boolean commissionPaid { get; set; }//Modificable

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Commission Paid Date")]
        [DataType(DataType.Date)]
        public DateTime? commissionPaidDate { get; set; }//Modificable

        //ID del Usuario que dio de alta el Contrato
        public String userCreateContract { get; set; }
        //Precio del contratoe en letras Español
        public String priceInWordSpanish { get; set; }
        //Precio del contratoe en letras Ingles
        public String priceInWordInglish { get; set; }
        [DisplayName("Concord")]
        //El concord del contrato
        public String Concord { get; set; }

        [DisplayName("Even Year")]
        public bool yearEven { get; set; }

        [DisplayName("Sent to Owner")]
        public bool csToOwner { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Sent to Owner Date")]
        public DateTime? csToOwnerDate { get; set; }//Modificable

        [DisplayName("Sent to Concord")]
        public bool csToConcord { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Sent to Concord Date")]
        public DateTime? csToConcordDate { get; set; }//Modificable

        //Payments Plan Data
        [DisplayName("Number of Payments")]
        public int paymentsQuantity { get; set; }

        public Boolean downPaymentCompleted { get; set; }

        //Date when HOA begins to be charged
        private DateTime initialHOAMonth { get; set; }
        [DisplayName("First HOA Month")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime InitialHOAMonth { get {
                return this.initialHOAMonth.Year == 1900 ? this.contractDate
                        : this.initialHOAMonth;
            }
            set { this.initialHOAMonth = value; }}

        [Required]
        [DisplayName("HOA Yearly Payment")]
        public decimal HOAYearlyPayment { get; set; }

        //LLave foranea de Condominio
        [DisplayName("Condominium")]
        public int condoID { get; set; }
        public virtual Condo condo { get; set; }
       
        //llave foranea de  cliente
        public int clientID { get; set; }
        public virtual Client client { get; set; }


        //Un contrato le pertenesen mucho pagos y muchos contractSalesMember
        public virtual ICollection<ContractSalesMember> contractSalesMembers { get; set; }
        
        //Finnace deposits and HOA
        [InverseProperty("salesContract")]
        public virtual ICollection<Deposit> deposits { get; set; }
        [InverseProperty("salesContractHOA")]
        public virtual ICollection<Deposit> HOA_deposits { get; set; }

        /// <summary>
        /// This method will generate on the fly the payment plan for downpayment and finance given a SalesContract instance.
        /// </summary>
        //public List<Payment> getPaymentPlan()
        //{
        //    SalesContract salesContract = this;

        //    double totalAmount = (double)salesContract.saleAmount;//Total amount to pay
        //    double closingCost = (double)this.closingCost;
        //    double downPayment = 0;/*(double)this.deposit;/((double)this.saleAmount + closingCost) * .3;//Total downpayment is 30% of total amount to pay*/
        //    double loanToPay = ((double)this.saleAmount + closingCost) - downPayment; //Balance is initially the total loan to pay
        //    DateTime dateToPay = salesContract.contractDate; //First date to pay
        //    if (deposit != null)
        //    {
        //        downPayment = (double)this.deposit;
        //    }
        //    else
        //    {
        //        downPayment = ((double)this.saleAmount + closingCost) * .3;//Total downpayment is 30% of total amount to pay
        //    }
        //    Payment payment;
        //    List<Payment> payments = new List<Payment>();

        //    //Generating payment plan for downpayment, 10 payments by default
        //    int paymentsQtyForDownPayment = this.NumberofDownPayments;
        //    int paymentCounter = 1;
        //    //decimal initialDeposit = getInitialDeposit().Amount;
        //    //double iniDepo = Convert.ToDouble(initialDeposit);
        //    //downPayment = downPayment - iniDepo;
        //    for (int c = 0; c < paymentsQtyForDownPayment; c++)
        //    {
        //        //Re-using finance payment constructor code for downpayment, taking interest as 0 and monthly pay as amortization.
        //        payment = new Payment(paymentCounter,dateToPay, (decimal)downPayment / paymentsQtyForDownPayment, 0, Deposit.PaymentTypes.DOWNPAYMENT);
        //        payments.Add(payment);
        //        dateToPay = dateToPay.AddMonths(1);//Next month
        //        paymentCounter++;
        //    }

        //    //Generating payment plan to pay loan of financing, remaining months after downpayment is paid

        //    for (int c = 0; c < salesContract.paymentsQuantity; c++)
        //    {
        //        payment = new Payment(paymentCounter,dateToPay, 0, 0, Deposit.PaymentTypes.FINNANCE);
        //        payments.Add(payment);
        //        dateToPay = dateToPay.AddMonths(1);//Next month
        //        paymentCounter++;
        //    }

        //    return payments.OrderBy(pay=>pay.dateToPay).ToList();
        //}

        ///// <summary>
        ///// Takes all deposits associated with this instance and generates on the fly the monitoring payments list.
        ///// </summary>
        ///// <returns>A List<PaymentDeposits> wich pairs the monthly payment with the deposit made for it.</returns>
        //public List<PaymentDeposits> getDepositsInPayments()
        //{

        //    List<Payment> payments = getPaymentPlan();

        //    ICollection<Deposit> deposits = this.deposits.Where(dep=>!dep.Refund && dep.verification == true).OrderBy(dep => dep.DepositDate).ToList();
        //    //deposits.OrderBy(dep => dep.DepositDate);
        //    List<PaymentDeposits> listPayDeposit = new List<PaymentDeposits>();

        //    //Calculate balance
        //    double monthInterestPercent = (this.interestRate / 100) / 12; //In LSR, interest percentage in 12 months
        //    double closingCost = (double)this.closingCost;
        //    double balance = (double)this.saleAmount+closingCost; //Balance is initially the loan to pay.
        //    double downPayment = 0;
        //    if(deposit != null)
        //    {
        //        downPayment = (double)this.deposit; 
        //    }
        //    else
        //    {
        //     downPayment = ((double)this.saleAmount + closingCost) * .3;//Total downpayment is 30% of total amount to pay
        //    }
        //    //decimal initialDeposit = getInitialDeposit().Amount;
        //    //double iniDepo = Convert.ToDouble(initialDeposit);
        //    //downPayment = downPayment - iniDepo;
        //    decimal pmt = getPMT(); //Return the financial calculation of finance monthly payment for principal or amortization 

        //    PaymentDeposits pd = new PaymentDeposits();

        //    int i = 0, p = 0, monthsPaidAtOnce = 0;
        //    decimal montoDeposito = 0;
        //    for (; p < payments.Count; p++)
        //    {

        //        Payment pay = payments.ElementAt(p);

        //        //CALCULAR INTERES Y AMORTIZACION
        //        if (pay.paymentType == Deposit.PaymentTypes.FINNANCE)
        //        {
        //            pay.Interest = (decimal)(balance * monthInterestPercent); //Calculate interest per month taking current balance
        //            pay.Amortization = pmt - pay.Interest; //Calculuate amortization which is PMT after interest
        //        }

        //        bool incompletePayment = false;
        //        for (; i < deposits.Count; i++)
        //        {
        //            Deposit dep = deposits.ElementAt(i);

        //            //and previous deposit was not downpayment or is DownPaymentToFinnaceTransition
        //            if ((p!=this.NumberofDownPayments && pay.paymentType != Deposit.PaymentTypes.DOWNPAYMENT) ||
        //                montoDeposito <= 0||incompletePayment)
        //            montoDeposito = dep.getAmountInSalesContractCurrency(this); //Deposited amount

        //            //Creates a pair payment-deposit for view
        //            //If a deposit did not was enough to pay a complete month
        //            if (incompletePayment)
        //            {
        //                pd = new PaymentDeposits(pay, dep, pd.remainingAmortization, pd.remainingInterest, (decimal)balance);
        //                incompletePayment = false;
        //            }
        //            else
        //                pd = new PaymentDeposits(pay, dep, (decimal)balance);

        //            //Deposit cases
        //            if (montoDeposito < pd.remainingInterest) //No apply in downpayment
        //            {
        //                //Interest is paid first
        //                pd.remainingInterest -= montoDeposito;
        //                //deposit is done
        //                incompletePayment = true;
        //            }
        //            else if (montoDeposito >= pd.remainingInterest)
        //            {
        //                //All the interest is paid
        //                montoDeposito -= pd.remainingInterest;
        //                 pd.remainingInterest = 0;

        //                //Amortization is paid with remaining deposit
        //                //if (montoDeposito < pd.remainingAmortization)
        //                if (montoDeposito <= pd.remainingAmortization)
        //                {
        //                    //And so is balance with remaining deposit
        //                    balance -= (double)montoDeposito;
        //                    pd.balance = (decimal)balance;

        //                    pd.remainingAmortization -= montoDeposito;//Principal is paid
        //                    //deposit is done
        //                    incompletePayment = true;

        //                    //reset payments counter
        //                    monthsPaidAtOnce = 0;
        //                }
        //                else
        //                {
        //                    if (pay.paymentType == Deposit.PaymentTypes.FINNANCE)
        //                    {
        //                        //And so is balance with remaining deposit
        //                        balance -= (double)montoDeposito;
        //                        pd.balance = (decimal)balance;

        //                        pd.remainingAmortization = 0;//Principal is paid
        //                        //Financial movement is saved
        //                        listPayDeposit.Add(pd);
        //                        i++;//deposit is done
        //                        break;//monthly payment is done
        //                    }
        //                    else if(pay.paymentType == Deposit.PaymentTypes.DOWNPAYMENT) {//And so is balance with remaining deposit
        //                        //Remaining principal pay balance and is done
        //                        balance -= (double)pd.remainingAmortization;
        //                        pd.balance = (decimal)balance;

        //                        //Deposit amount still remain for next monthly payment
        //                        montoDeposito -= pd.remainingAmortization;
        //                        pd.remainingAmortization = 0;//Principal is paid
        //                        listPayDeposit.Add(pd);

        //                        //If a DOWNPAYMENT month was completed, dates to pay for nextmonths are readjusted
        //                        if(monthsPaidAtOnce>0)
        //                            for (int c = p + 1; c < payments.Count(); c++)
        //                                payments.ElementAt(c).dateToPay = payments.ElementAt(c).dateToPay.AddMonths(-1);

        //                        monthsPaidAtOnce++;
        //                        break;//monthly payment is done
        //                    }
        //                }
        //            }

        //            //Financial movement is saved when month is incomplete
        //            listPayDeposit.Add(pd);
        //        }//Ends foreach deposits

        //        //Count equals to array size to indicate deposits are done with a incomplete payment pending
        //        if (i == deposits.Count && Math.Round(balance) > 0 && pd.payment != pay)
        //            listPayDeposit.Add(new PaymentDeposits(pay, (decimal)balance));//Add PaymentDeposit without deposit.
        //    }//Ends foreach payments

        //    return listPayDeposit;
        //}//ends getDepositsInPayments()

        ///// <summary>
        ///// Return the financial calculation of finance monthly payment for principal or amortization 
        ///// </summary>
        ///// <returns>The calculated PMT for the current attributes of sales contract instance.</returns>
        //public decimal getPMT()
        //{
        //    double monthInterestPercent = (this.interestRate / 100) / 12; //In LSR, interest percentage in 12 months
        //    double closingCost = (double)this.closingCost;
        //    double downPayment = 0;
        //        if (deposit != null)
        //    {
        //        downPayment = (double)this.deposit;
        //    }
        //    else
        //    {
        //        downPayment = ((double)this.saleAmount + closingCost) * .3;//Total downpayment is 30% of total amount to pay
        //    }
        //        //((double)this.saleAmount +closingCost)* .3;//Total downpayment is 30% of total amount to pay
        //    double loanToPay =((double)this.saleAmount+closingCost) - downPayment; //Balance is initially the total loan to pay
        //    if (this.paymentsQuantity == 0)
        //    {
        //        this.paymentsQuantity = 1;
        //    }
        //    return (decimal) -Financial.Pmt(monthInterestPercent, this.paymentsQuantity, loanToPay); //Calculate the PMT to generate payment plan
        //}

        //public decimal getTotalPaid()
        //{
        //    ///List<PaymentDeposits> payments = getDepositsInPayments();
        //    var payments = this.deposits;
        //    List<Payment> payment = Deposit.getPaymentPlan();
        //    decimal result = 0;
        //    ICollection<Deposit> deposits = this.deposits.Where(dep => !dep.Refund && dep.verification == true).OrderBy(dep => dep.DepositDate).ToList();
        //    int i = 0, p = 0;
        //    for (; p < payments.Count; p++)
        //    {
        //        for (; i < deposits.Count; i++)
        //        {
        //            Deposit deposit = deposits.ElementAt(i);
        //            decimal paymentCurrency = deposit.getAmountInSalesContractCurrency(this);
        //            result += paymentCurrency;
        //        }

        //    }

        //    return result;
        //}

        //public decimal getCurrentBalance()
        //{
        //    List<PaymentDeposits> payments = getDepositsInPayments();
        //    return payments.Last().balance;

        //}

        //public Deposit getInitialDeposit()
        //{
        //    Deposit initial = this.deposits.OrderBy(pay => pay.DepositDate).FirstOrDefault();
        //    //if(initial != null)
        //    //{
        //    //    initial = this.deposits.OrderBy(pay => pay.DepositDate).FirstOrDefault();
        //    //}
        //    //else
        //    //{

        //    //}
        //    return initial; 
        //}

        ///// <summary>
        ///// Determines DownPayment of current instace if paid.
        ///// </summary>
        ///// <returns>Boolean indicating if Downpayment is completely paid.</returns>
        //public Boolean isDownPaymentCompleted()
        //{
        //    double currentBalance = (double)this.getCurrentBalance();
        //    double totalDownPayment = ((double)this.saleAmount+ (double)this.closingCost )* .3;



        //    return (((double)this.saleAmount +(double)this.closingCost) - currentBalance)>=totalDownPayment;
        //}

        public static Object[] getPaymentsQuantitiesArray()
        {

            Object[] paymentsQuantitiesArray = new Object[]{
                new {Value=12, Text="12 Months"},
                new {Value=18, Text="18 Months"},
                new {Value=24, Text="24 Months"},
                new {Value=36, Text="36 Months"},
                new {Value=48, Text="48 Months"},
                new {Value=60, Text="60 Months"},
                new {Value=72, Text="72 Months"},
                new {Value=84, Text="84 Months"},   
                new {Value=90, Text="90 Months"},
                new {Value=94, Text="94 Months"},
                new {Value=96, Text="96 Months"},
                new {Value=120, Text="120 Months"}
            };

            return paymentsQuantitiesArray;
        }

        public static Object[] getQualificationsArray()
        {

            Object[] qualificationsArray = new Object[]{
                new {Value="QA", Text="Qualified International, CC"},
                new {Value="QB", Text="Qualified International, DC"},
                new {Value="NA", Text="Qualified National, CC"},
                new {Value="CT", Text="Courtesy Tour"}
            };

            return qualificationsArray;
        }

        public static Object[] getCurrencyArray()
        {

            Object[] currencyArray = new Object[]{
                new { Text="Mexican Pesos", Value = "MXN" },
                new { Text="US Dollar", Value = "USD", Selected=true }
            };
            return currencyArray;
        }

        public static Object[] getContractTypeArray()
        {

            Object[] contractTypeArray = new Object[]{
                new {Value="FA", Text="Annual Fraction"},
                new {Value="BA", Text="Biannual Fraction"}
            };

            return contractTypeArray;
        }

        public static Object[] getTypeOfFractionArray()
        {

            Object[] contractTypeArray = new Object[]{
                new {Value="1/10", Text="1/10"},
                new {Value="1/25", Text="1/25"},
                new {Value="1/50", Text="1/50"},
                new {Value="1/100", Text="1/100"},
                new {Value="Other", Text="Other"}
            };

            return contractTypeArray;
        }

        public static Object[] getSpanishAttachmentsArray()
        {
            Object[] spanishAttachmentsArray = new Object[]{
                new {Value="BONUS NIGHTS EACH YEAR FOR  YEARS, DEPENDING ON AVAILABILITY,"+
                                    "NO WEEKENDS, NO HOLIDAYS, ONLY MONDAY THRU THURSDAY."+
                                    "- 1 BDR OCOTILLO $50.00 USD PER NIGHT."+
                                   " - 2 BDR VENTANAS $75.00 USD PER NIGHT."+
                                    "- 2 BDR VILLA PLAYA $100.00 USD PER NIGHT."+
                                    "- 3 BDR VILLA PLAYA $120.00 USD PER NIGHT.|"+
                                    "NOCHES BONO CADA AÑO POR  AÑOS, SUJETO A"+
                                    "DISPONIBILIDAD, NO FINES DE SEMANA, NO DIAS FESTIVOS, SOLO DE"+
                                    "LUNES A JUEVES."+
                                    "- 1 REC OCOTILLO $50.00 USD POR NOCHE."+
                                    "- 2 REC VENTANAS $75.00 USD POR NOCHE."+
                                    "- 2 REC VILLA PLAYA $100.00 USD POR NOCHE."+
                                    "- 3 REC VILLA PLAYA $120.00 USD POR NOCHE.", Text="Bonus Nights"},
                new {Value="MEDICAL PET WITH MEDICAL CERTIFICATE |MASCOTA DE SERVICIO, CON CERTIFICADO MEDICO", Text="Pets"},
                new {Value="FROZEN PRICE|PRECIO CONGEGALDO", Text="Frozen Prices/Precios Congelados"},
                new {Value="LLOYDSHARE|LLOYDSHARE   ", Text="LLOYDSHARE"},
                new {Value = "ADVANTAGE WEEKS TO BE USED OVER  YEARS.|SEMANAS ADVANTAGE PARA SER USADAS DURANTE  AÑOS.", Text = "Advantage"},
                new {Value = "DISCOUNT CERTIFICATE FOR TWO PERSONS IN A CRUISER WITH ADVANTAGE. |CERTIFICADO DE DESCUENTO PARA DOS PERSONAS EN CRUCERO CON ADVANTAGE.", Text = "Cruise Certificate"},
                new {Value = "AMBASSADOR PROGRAM.|PROGRAMA DE EMBAJADORES.", Text = "Ambassador Program"},
                  new {Value = "HOA PAYMENTS START YEAR 0000.|PAGOS DE HOA COMIENZAN EL AÑO 0000.", Text = "HOA"}
            };

            return spanishAttachmentsArray;
        }

        public static Object[] getEnglishAttachmentsArray()
        {
            Object[] englishAttachmentsArray = new Object[]{
                new {Value="BONUS NIGHTS EACH YEAR FOR  YEARS, DEPENDING ON AVAILABILITY,"+
                                    "NO WEEKENDS, NO HOLIDAYS, ONLY MONDAY THRU THURSDAY."+
                                    "\n 1 BDR OCOTILLO $50.00 USD PER NIGHT."+
                                   " \n 2 BDR VENTANAS $75.00 USD PER NIGHT."+
                                    "\n 2 BDR VILLA PLAYA $100.00 USD PER NIGHT."+
                                    "\n 3 BDR VILLA PLAYA $120.00 USD PER NIGHT.|"+
                                    "NOCHES BONO CADA AÑO POR  AÑOS, SUJETO A"+
                                    "DISPONIBILIDAD, NO FINES DE SEMANA, NO DIAS FESTIVOS, SOLO DE"+
                                    "LUNES A JUEVES."+
                                    "\n 1 REC OCOTILLO $50.00 USD POR NOCHE."+
                                    "\n 2 REC VENTANAS $75.00 USD POR NOCHE."+
                                    "\n 2 REC VILLA PLAYA $100.00 USD POR NOCHE."+
                                    "\n 3 REC VILLA PLAYA $120.00 USD POR NOCHE.", Text="Bonus Nights"},
                new {Value="MEDICAL PET WITH MEDICAL CERTIFICATE|MASCOTA DE SERVICIO, CON CERTIFICADO MEDICO", Text="Pets"},
                 new {Value="FROZEN PRICE|PRECIO CONGEGALDO", Text="Frozen Prices/Precios Congelados"},
                new {Value="LLOYDSHARE|LLOYDSHARE   ", Text="LLOYDSHARE"},
                 new {Value = "ADVANTAGE WEEKS TO BE USED OVER  YEARS.|SEMANAS ADVANTAGE PARA SER USADAS DURANTE  AÑOS.", Text = "Advantage"},
                new {Value = "DISCOUNT CERTIFICATE FOR TWO PERSONS IN A CRUISER WITH ADVANTAGE. |CERTIFICADO DE DESCUENTO PARA DOS PERSONAS EN CRUCERO CON ADVANTAGE.", Text = "Cruise Certificate"},
                new {Value = "AMBASSADOR PROGRAM.|PROGRAMA DE EMBAJADORES.", Text = "Ambassador Program"},
                  new {Value = "HOA PAYMENTS START YEAR 0000.|PAGOS DE HOA COMIENZAN EL AÑO 0000.", Text = "HOA"}
            };
            return englishAttachmentsArray;
        }

        /// <summary>
        /// Class which has defined the contracty type names and controller names to be used
        /// in the whole project
        /// </summary>
        public static class ContractTypeName
        {
            public static string SALES_CONTRACT = "SalesContract";
            public static string TRIAL_MEMBERSHIPS = "TrialMemberships";
            public static string REAL_STATE_CONTRACTS = "RealStateContracts";
        }

    }
}