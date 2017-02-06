using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMHOAMontlhyFeeRow
    {
        public static String CURRENCY = Deposit.Currencies.USD;

        [DisplayName("Month")]
        [DisplayFormat(DataFormatString = "{0:MMMM}")]
        public DateTime month { get; set; }
        [DisplayName("HOA Fee")]
        public decimal fee { get; set; }
        [DisplayName("Paid Fee")]
        public decimal paidFee { get; set; }
        [DisplayName("Balance")]
        public decimal balance { get; set; }
        [DisplayName("Penalties")]
        public decimal penalties { get; set; }
        [DisplayName("Interest")]
        public decimal interest { get; set; } //%2
        [DisplayName("Total Owing")]
        public decimal totalOwing { get; set; }

        public bool active { get; set; }

        public VMHOAMontlhyFeeRow() { }
        public VMHOAMontlhyFeeRow(int month, int year)
        {
            this.fee = -125;
            this.month = new DateTime(year, month, 1);
        }
        public VMHOAMontlhyFeeRow(decimal fee, DateTime month,DateTime initialHOAMonth)
        {
            this.month = month;
            this.active = month.Year > initialHOAMonth.Year || (month.Year == initialHOAMonth.Year 
                && month.Month >= initialHOAMonth.Month);
            if (this.active) 
                this.fee = -fee;
        }
    }
}