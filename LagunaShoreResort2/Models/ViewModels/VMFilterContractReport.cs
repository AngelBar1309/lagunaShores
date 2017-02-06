using jsreport.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMFilterContractReport
    {
        [DisplayName("Start")]
        [DataType(DataType.Date)]
        public DateTime start { get; set; }

        [DisplayName("End")]
        [DataType(DataType.Date)]
        public DateTime end { get; set; }

        [DisplayName("Paid Downpayment")]
        public String commissionPaymentStatus { get; set; }

        public String contractTypeName { get; set; }

        public VMFilterContractReport()
        {
            this.start = DateTime.MinValue;
            this.end = DateTime.MaxValue;
            this.commissionPaymentStatus = "All";
        }

        public static Object[] getCommissionPaymentStatusOptions()
        {

            Object[] salesContractFilterOptions = new Object[]{
                new {Value="All", Text="All"},
                new {Value=CommissionPaymentStatus.DOWNPAYMENT_NOT_PAID, Text="Not Paid"},
                new {Value=CommissionPaymentStatus.DOWNPAYMENT_PAID, Text="Downpayment Paid"},
                new {Value=CommissionPaymentStatus.COMMISSION_REQUESTED, Text="Commision Requested"},
                new {Value=CommissionPaymentStatus.COMMISSION_PAID, Text="Commision Paid"}
            };

            return salesContractFilterOptions;
        }

        public static Object[] getCommissionPaymentStatusOptionsAccountant()
        {
            Object[] salesContractFilterOptions = new Object[]{
                new {Value=CommissionPaymentStatus.COMMISSION_PAID, Text="Commission Paid"},
                new {Value=CommissionPaymentStatus.COMMISSION_REQUESTED, Text="Commision Requested"}
            };

            return salesContractFilterOptions;
        }

        public static class CommissionPaymentStatus
        {
            public const string DOWNPAYMENT_NOT_PAID = "DownpaymentNotPaid";
            public const string DOWNPAYMENT_PAID = "DownpaymentPaid";
            public const string COMMISSION_REQUESTED = "CommissionRequested";
            public const string COMMISSION_PAID = "CommissionPaid";
        }
    }

}