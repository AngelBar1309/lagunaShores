using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class Deposit_Payment
    {
        public decimal currentAmortization { get; set; }
        public decimal currentInterest { get; set; }

        //Many to Many relationship beetween Deposits and Payments
        [Column(Order = 0), Key]
        public int noFolio { get; set; }
        public virtual Deposit deposit { get; set; }

        [Column(Order = 1), Key]
        public int paymentID { get; set; }
        public virtual Payment payment { get; set; }
    }

    public class VMDeposit_Payment
    {
        public int noFolio { get; set; }
        public decimal depositAmount { get; set; }

        public int paymentID { get; set; }
        public decimal amortization { get; set; }
        public decimal interest { get; set; }

        public decimal remAmortization { get; set; }
        public decimal remInterest { get; set; }
    }
}