using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMRealState
    {
        [DisplayName("ID")]
        public int realStateID { get; set; }
        
        [DisplayName("Contract Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime contractDate { get; set; }

        [DisplayName("Verified by Admin")]
        public bool verifiedByAdmin { get; set; }

        [DisplayName("Request to Accountant")]
        public bool requestToAccountant { get; set; }

        [DisplayName("Canceled Contract")]
        public bool canceledContract { get; set; }

        [DisplayName("PMT")]
        public decimal PMT { get; set; }

        [DisplayName("Interest Rate")]
        public double APR { get; set; }

        [DisplayName("Commission Paid")]
        public bool commissionPaid { get; set; }

        [DisplayName("Downpayment Paid")]
        public Boolean downPaymentPaid { get; set; }

        /*For general data*/
        [DisplayName("Sale Amount")]
        public double saleAmount { get; set; }

        [DisplayName("Currency")]
        public String currency { get; set; }

        [DisplayName("Balance")]
        public decimal balance { get; set; }

        [DisplayName("Total Paid")]
        public decimal totalPaid { get; set; }

        //llave foranea de  cliente
        public int clientID { get; set; }
        public virtual Client client { get; set; }

        /*For Sales Member Reports*/
        [DisplayName("Commission %")]
        public double commissionPercentage { get; set; }

        [DisplayName("Rol")]
        public String rolName { get; set; }
        private void initializeVMRealStateAttributes(RealStateContract c)
        {
            // TODO: Complete member initialization
            this.realStateID = c.contractID;
            this.contractDate = c.closingDate;
            this.verifiedByAdmin = c.verifiedByAdmin;
            this.requestToAccountant = c.requestToAccountant;
            this.canceledContract = c.canceledContract;
            this.commissionPaid = c.commissionPaid;
            this.saleAmount = (double) c.saleAmount;
            this.currency = c.currency;
            this.downPaymentPaid = Deposit.isDownPaymentCompleted(null, null, c);
            this.balance = Deposit.getCurrentBalance(null, null, c);
            this.totalPaid = Deposit.getTotalPaid(null, null, c);
            this.client = c.clientAssigned;
            this.clientID = c.clientID;
            this.APR = c.interestRate;
            this.PMT = Deposit.getPMT(null, null, c);
        }

        public VMRealState()
        { }

        public VMRealState(RealStateContract c)
        {
            initializeVMRealStateAttributes(c);
        }
    }
}