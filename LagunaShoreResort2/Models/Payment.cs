using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class Payment
    {
        public int paymentNumber { get; set; }

        //Fecha limite para pagar
        [DisplayName("Data To Payment")]
        public DateTime dateToPay { get; set; }

        [DisplayName("Payment Type")]
        public String paymentType { get; set; }

        public decimal Amortization { get; set; }

        public decimal Interest { get; set; }

        //Constructor used for payments plan.
        public Payment(int number, DateTime dateToPay, decimal Amortization, decimal Interest, String paymentType )
        {
            this.paymentNumber = number;
            // TODO: Complete member initialization
            this.dateToPay = dateToPay;
            this.paymentType = paymentType;
            this.Amortization = Amortization;
            this.Interest = Interest;
        }
    }

    public class PaymentDeposits
    {
        public Payment payment { get; set; }
        public Deposit deposit{ get; set; }
        public decimal remainingAmortization { get; set; }
        public decimal remainingInterest { get; set; }
        public decimal balance { get; set; }

        public PaymentDeposits(Payment payment, Deposit deposit, decimal remainingAmortization, decimal remainingInterest, decimal balance)
        {
            this.payment = payment;
            this.deposit = deposit;
            this.balance = balance;
            this.remainingAmortization = remainingAmortization;
            this.remainingInterest = remainingInterest;
        }

        public PaymentDeposits(Payment payment, Deposit deposit, decimal balance)
        {
            this.payment = payment;
            this.deposit = deposit;
            this.balance = balance;
            this.remainingAmortization = payment.Amortization;
            this.remainingInterest = payment.Interest;
        }

        public PaymentDeposits(Payment payment,decimal balance)
        {
            this.payment = payment;
            this.balance = balance;
            this.deposit = new Deposit();
            this.remainingAmortization = payment.Amortization;
            this.remainingInterest = payment.Interest;
        }

        public PaymentDeposits() { }
    }
}