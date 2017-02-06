using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class TrialMemberships
    {
        [Key]
        public int trialMembershipID { get; set; }//primary key

        [DisplayName("Contract Number")]
        public string contractNumberTM { get; set; }
        [DisplayName("Contract Type")]
        public string contractType { get; set; }
        
        [DisplayName("Contract Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime tmContractDate { get; set; }

        [DisplayName("Sale Amount")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public double tmSaleAmount { get; set; }
        //***Start
        //Added to Production
        // This is the new DP for all contracts this is not yet done
        [DisplayName("Deposit")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal? deposit { get; set; }

        [DisplayName("Upgraded")]
        public bool upgraded { get; set; }
        //***END

        [DisplayName("Interest Rate")]
        [Range(0, 100)]
        public double tmInterestRate { get; set; }
        [DisplayName("Verified by Admin")]
        public bool tmVerifiedByAdmin { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Verification Date")]
        public DateTime? tmVerificationDate { get; set; }

        [DisplayName("Request to Accountant")]
        public bool tmRequestToAccountat { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Request to Accountant Date")]
        public DateTime? tmRequestToAccountantDate { get; set; }

        [DisplayName("Trial Nights")]
        public double? trialNights { get; set; }

        //Start new type of contracts
        [DisplayName("Weekend Nights")]
        public double? weekendNights { get; set; }

        [DisplayName("Weekday Nights")]
        public double? weekdayNights { get; set; }

        [DisplayName("Referral Nights")]
        public double? referralNights { get; set; }
        //end

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Trial Expiration Date")]
        public DateTime? trialExpDate { get; set; }

        [DisplayName("Advantage Weeks")]
        public double? advantageWeeks { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Advantage Expiration Date")]
        public DateTime? advantageExpDate { get; set; }

        //[DisplayName("Quick Weeks")]
        //public double? quickWeeks { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Quick Weeks Expiration Date")]
        public DateTime? quickWeeksExpDate { get; set; }

        [DisplayName("Canceled Contract")]
        public bool tmCanceledContract { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Canceled Date")]
        public DateTime? tmCanceledContractDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Payments Start")]
        public DateTime tmPaymentsStartDate { get; set; }

        [DisplayName("Number of DownPayments")]
        public int tmNumberofDownPayments { get; set; }
        [DisplayName("Number of Payments ")]
        public int tmNumberPayments { get; set; }
        [DisplayName("Qualification ")]
        public String tmQualification { get; set; }
        [DisplayName("Currency")]
        public String tmCurrency { get; set; }
        [DisplayName("List Price")]
        public double tmListPrice { get; set; } 

        [DisplayName("Commission Paid")]
        public Boolean tmCommissionPaid { get; set; }//Modificable

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Commission Paid Date")]
        //[DataType(DataType.Date)]
        public DateTime? tmCommissionPaidDate { get; set; }//Modificable
        //Payments Plan Data
        //[DisplayName("Number of Payments")]
        //public int tmPaymentsQuantity { get; set; }
        [DisplayName("Room Type")]
        public string tmRoomType { get; set; }
        [DisplayName("Comments")]
        public string tmComments { get; set; }
        //ID del Usuario que dio de alta el Contrato
        public String userCreateContract { get; set; }

        [DisplayName("Concord")]
        //El concord del contrato
        public String Concord { get; set; }

        [DisplayName("Sent to Concord")]
        public bool csToConcord { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Sent to Concord Date")]
        public DateTime? csToConcordDate { get; set; }//Modificable

        //llave foranea de  cliente
        public int clientID { get; set; }
        public virtual Client client { get; set; }
        //Un contrato le pertenesen mucho pagos y muchos trialSalesMember
        public virtual ICollection<TrialSalesMembers> trialSalesMembers { get; set; }
        public virtual ICollection<Deposit> deposits { get; set; }

        //public List<Payment> getPaymentPlan()
        //{
        //    TrialMemberships trialMembership = this;

        //    double totalAmount = (double)trialMembership.tmSaleAmount;//Total amount to pay
        //    double downPayment = totalAmount * .3;//Total downpayment is 30% of total amount to pay
        //    if (deposit != null)
        //    {
        //        downPayment = (double)deposit;

        //    }
        //    else
        //    {
        //        downPayment = (double)this.tmSaleAmount * .3;//Total downpayment is 30% of total amount to pay

        //    }
        //    double loanToPay = totalAmount - downPayment; //Loan remaining after downpayment

        //    DateTime dateToPay = trialMembership.tmContractDate; //First date to pay

        //    Payment payment;
        //    List<Payment> payments = new List<Payment>();

        //    //Generating payment plan for downpayment, 10 payments by default
        //    int paymentsQtyForDownPayment = this.tmNumberofDownPayments;

        //    int paymentCounter = 1;
        //    for (int c = 0; c < paymentsQtyForDownPayment; c++)
        //    {
        //        //Re-using finance payment constructor code for downpayment, taking interest as 0 and monthly pay as amortization.
        //        payment = new Payment(paymentCounter,dateToPay, (decimal)downPayment / paymentsQtyForDownPayment, 0, Deposit.PaymentTypes.DOWNPAYMENT);
        //        payments.Add(payment);
        //        dateToPay = dateToPay.AddMonths(1);//Next month
        //        paymentCounter++;
        //    }

        //    //Generating payment plan to pay loan of financing, remaining months after downpayment is paid

        //    for (int c = 0; c < trialMembership.tmNumberPayments; c++)
        //    {
        //        payment = new Payment(paymentCounter,dateToPay, 0, 0, Deposit.PaymentTypes.FINNANCE);
        //        payments.Add(payment);
        //        dateToPay = dateToPay.AddMonths(1);//Next month
        //        paymentCounter++;
        //    }

        //    return payments.OrderBy(pay => pay.dateToPay).ToList();
        //} 
        //  /// <summary>
        //  /// Return the financial calculation of finance monthly payment for principal or amortization 
        //  /// </summary>
        //  /// <returns>The calculated PMT for the current attributes of sales contract instance.</returns>

        //public List<PaymentDeposits> getDepositsInPayments()
        //{
        //    List<Payment> payments = getPaymentPlan();
        //    ICollection<Deposit> deposits = this.deposits.Where(dep => !dep.Refund && dep.verification == true).OrderBy(dep => dep.DepositDate).ToList();

        //    List<PaymentDeposits> listPayDeposit = new List<PaymentDeposits>();

        //    //Calculate balance
        //    double monthInterestPercent = (this.tmInterestRate / 100) / 12; //In LSR, interest percentage in 12 months
        //    double balance = (double)this.tmSaleAmount; //Balance is initially the loan to pay.
        //    double downPayment = 0;
        //    if (deposit != null)
        //    {
        //        downPayment = (double)deposit;

        //    }
        //    else
        //    {
        //        downPayment = (double)this.tmSaleAmount * .3;//Total downpayment is 30% of total amount to pay

        //    }

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
        //            if ((p != this.tmNumberofDownPayments && pay.paymentType != Deposit.PaymentTypes.DOWNPAYMENT) ||
        //                montoDeposito <= 0 || incompletePayment)
        //                montoDeposito = dep.getAmountInTrialMembershipsCurrency(this); //Deposited amount

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
        //                pd.remainingInterest = 0;

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
        //                    else if (pay.paymentType == Deposit.PaymentTypes.DOWNPAYMENT)
        //                    {//And so is balance with remaining deposit
        //                        //Remaining principal pay balance and is done
        //                        balance -= (double)pd.remainingAmortization;
        //                        pd.balance = (decimal)balance;

        //                        //Deposit amount still remain for next monthly payment
        //                        montoDeposito -= pd.remainingAmortization;
        //                        pd.remainingAmortization = 0;//Principal is paid
        //                        listPayDeposit.Add(pd);

        //                        //If a DOWNPAYMENT month was completed, dates to pay for nextmonths are readjusted
        //                        if (monthsPaidAtOnce > 0)
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
        //public decimal getPMT()
        //{
        //    double monthInterestPercent = (this.tmInterestRate / 100) / 12; //In LSR, interest percentage in 12 months
        //    double downPayment = 0;
        //    if (deposit != null)
        //    {
        //        downPayment = (double)deposit;

        //    }
        //    else
        //    {
        //        downPayment = (double)this.tmSaleAmount * .3;//Total downpayment is 30% of total amount to pay

        //    }
        //    double loanToPay = ((double)this.tmSaleAmount) - downPayment; //Balance is initially the total loan to pay
        //    if (this.tmNumberPayments == 0)
        //    {
        //        this.tmNumberPayments = 1;
        //    }
        //    return (decimal)-Financial.Pmt(monthInterestPercent, this.tmNumberPayments, loanToPay); //Calculate the PMT to generate payment plan


        //}
        
        //public decimal getTotalPaid()
        //{
        //    ///List<PaymentDeposits> payments = getDepositsInPayments();
        //    var payments = this.deposits;
        //    List<Payment> payment = getPaymentPlan();
        //    decimal result = 0;
        //    ICollection<Deposit> deposits = this.deposits.Where(dep => !dep.Refund && dep.verification == true).OrderBy(dep => dep.DepositDate).ToList();
        //    int i = 0, p = 0;
        //    for (; p < payments.Count; p++)
        //    {
        //        for (; i < deposits.Count; i++)
        //        {
        //            Deposit deposit = deposits.ElementAt(i);
        //            decimal paymentCurrency = deposit.getAmountInTrialMembershipCurrency(this);
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
        //    double totalDownPayment = (double)this.tmSaleAmount * .3;



        //    return ((double)this.tmSaleAmount - currentBalance) >= totalDownPayment;
        //}

        public static Object[] getPaymentsQuantitiesArray()
        {

            Object[] paymentsQuantitiesArray = new Object[]{
                new {Value=12, Text="12 Months"},
                new {Value=18, Text="18 Months"},
                new {Value=24, Text="24 Months"},
                new {Value=36, Text="36 Months"},
                new {Value=48, Text="48 Months"},
                new {Value=60, Text="60 Months"}
            };

            return paymentsQuantitiesArray;
        }

        public static Object[] getQualificationsArray()
        {

            Object[] qualificationsArray = new Object[]{
                new {Value="QA", Text="Qualified International, Credit Card"},
                new {Value="QB", Text="Qualified International, Debit Card"},
                new {Value="NA", Text="Qualified National, Credit Card"},
                new {Value="CT", Text="Courtesy Tour"}
            };

            return qualificationsArray;
        }

        public static Object[] getCurrencyArray()
        {

            Object[] currencyArray = new Object[]{
                new { Text="Mexican Pesos", Value = "MXN" },
                new { Text="US Dollar", Value = "USD" }
            };
            return currencyArray;
        }
        public static Object[] getContractType()
        {

            Object[] ContractTypeArray = new Object[]{
                new { Text="Trial Membership", Value = "TM" },
                new { Text="Stand Alone", Value = "SA" },
                 new { Text="Trial Membership w/ Stand Alone", Value = "TSA" }
            };
            return ContractTypeArray;
        }
        public static Object[] getRoomType()
        {

            Object[] roomTypeArray = new Object[]{
                new { Text="1 bedroom", Value = "1 bdrm" },
                new { Text="2 bedroom", Value = "2 bdrm" },
                new { Text="3 bedroom", Value = "3 bdrm" }
            };
            return roomTypeArray;
        }
    }
}