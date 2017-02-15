using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMSalesContract
    {
        [DisplayName("ID")]
        public int salesContractID { get; set; }

        [DisplayName("Contract Number")]
        public String contractNumber{ get; set; }

        [DisplayName("Type Of Fraction")]
        public String typeOfFraction{ get; set; }

        [DisplayName("Contract Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime contractDate{ get; set; }

        [DisplayName("Verified by Admin")]
        public bool verifiedByAdmin { get; set; }

        [DisplayName("Request to Accountant")]
        public bool requestToAccountant { get; set; }

        [DisplayName("Canceled Contract")]
        public bool canceledContract { get; set; }

        [DisplayName("Commission Paid")]
        public bool commissionPaid { get; set; }

        [DisplayName("Downpayment Paid")]
        public Boolean downPaymentPaid { get; set; }

        /*For general data*/
        [DisplayName("Sale Amount")]
        public decimal saleAmount { get; set; }

        [DisplayName("Balance")]
        public decimal balance { get; set; }

        [DisplayName("Total Paid")]
        public decimal totalPaid { get; set; }

        [DisplayName("PMT")]
        public decimal PMT { get; set; }

        [DisplayName("Interest Rate")]
        public double APR { get; set; }

        [DisplayName("Currency")]
        public String currency { get; set; }


        //llave foranea de  cliente
        public int clientID { get; set; }
        public virtual Client client { get; set; }

        /*For Sales Member Reports*/
        [DisplayName("Commission %")]
        public double commissionPercentage { get; set; }

        [DisplayName("Rol")]
        public String rolName { get; set; }

        private void initializeSalesContractAttributes(SalesContract c)
        {
            // TODO: Complete member initialization
            salesContractID = c.contractID;
            this.contractNumber = c.contractNumber;
            this.typeOfFraction = c.typeOfFraction;
            this.contractDate = c.contractDate;
            verifiedByAdmin = c.verifiedByAdmin;
            this.requestToAccountant = c.requestToAccountant;
            this.canceledContract = c.canceledContract;
            this.commissionPaid = c.commissionPaid;
            this.saleAmount = c.saleAmount;
            this.currency = c.currency; 
            this.downPaymentPaid = Deposit.isDownPaymentCompleted(c,null, null);
            this.balance = Deposit.getCurrentBalance(c, null, null);
            this.totalPaid = Deposit.getTotalPaid(c, null, null);
            this.client = c.client;
            this.clientID = c.clientID;
            this.PMT = Deposit.getPMT(c, null, null);
            this.APR = c.interestRate; 
        
            

        }

        public VMSalesContract()
        { }

        public VMSalesContract(SalesContract c)
        {
            initializeSalesContractAttributes(c);
        }

        public VMSalesContract(ContractSalesMember csm)
        {
            initializeSalesContractAttributes(csm.salesContract);
            this.commissionPercentage = (Double)csm.rol.comssion;
            this.rolName = csm.rol.type;
        }
    }
}