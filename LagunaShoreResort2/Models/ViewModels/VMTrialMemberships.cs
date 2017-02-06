using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMTrialMemberships
    {
        [DisplayName("ID")]
        public int trialMembershipID { get; set; }
        [DisplayName("Contract Number")]
        public String contractNumber { get; set; }
        [DisplayName("Type of Contract")]
        public String typeOfContract { get; set; }
        [DisplayName("Contract Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime contractDate { get; set; }
        [DisplayName("Verified by Admin")]
        public bool tmVerifiedByAdmin { get; set; }

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
        private void initializeTrialMembershipAttributes(TrialMemberships c)
        {
            // TODO: Complete member initialization
            this.trialMembershipID = c.trialMembershipID;
            this.contractNumber = c.contractNumberTM;
            this.typeOfContract = c.contractType;
            this.contractDate = c.tmContractDate;
            this.tmVerifiedByAdmin = c.tmVerifiedByAdmin;
            this.requestToAccountant = c.tmRequestToAccountat;
            this.canceledContract = c.tmCanceledContract;
            this.commissionPaid = c.tmCommissionPaid;
            this.saleAmount = c.tmSaleAmount;
            this.currency = c.tmCurrency; 
            this.downPaymentPaid = Deposit.isDownPaymentCompleted(null, c,null);
            this.balance = Deposit.getCurrentBalance(null, c,null);
            this.totalPaid = Deposit.getTotalPaid(null, c, null);
            this.client = c.client;
            this.clientID = c.clientID;
            this.APR = c.tmInterestRate;
            this.PMT = Deposit.getPMT(null, c, null); 
        }

        public VMTrialMemberships()
        { }

        public VMTrialMemberships(TrialMemberships c)
        {
            initializeTrialMembershipAttributes(c);
        }

        public VMTrialMemberships(TrialSalesMembers csm)
        {
            initializeTrialMembershipAttributes(csm.trialMemberships);
            this.commissionPercentage = csm.rol.comssion;
            this.rolName = csm.rol.type;
        }
    }
}