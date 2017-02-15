using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class RealStateContract : Contracts
    {
    //    public int realStateContractID { get; set; } //KEY

    //    [Required]
    //    [Display(Name="Type")]
    //    public String type { get; set; }

        [Required]
        public String MLS { get; set; }

        [Required]
        [Display(Name = "Ownership Held")]
        public String ownershipHeld { get; set; }

        //[Required]
        //[Display(Name = "Sale Amount")]
        //public decimal saleAmount { get; set; }

        [Required]
        [Display(Name = "Sale Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public DateTime saleDate { get; set; }

        [Display(Name = "Closing Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime closingDate { get; set; }

        //[Display(Name = "Verified By Contract Manager")]
        //public bool verifiedByAdmin { get; set; } //Modificable
        
        //[DisplayName("Request to Accountant")]
        //public Boolean requestToAccountant { get; set; }//Modificable

        //[DisplayName("Canceled Contract")]
        //public bool canceledContract { get; set; }//Modificable

        //[DisplayName("Commission Paid")]
        //public Boolean commissionPaid { get; set; }//Modificable

        [Display(Name = "Sales Member")]
        public int salesMemberID { get; set; }
        public virtual SalesMember salesMember { get; set; }
        
        //Condominio en venta
        [Display(Name = "Condo")]
        [Required]
        public int condoID { get; set; }
        public virtual Condo condo { get; set; }

        [Required]
        [DisplayName("Closing Cost")]
        public double closingCost { get; set; }

        [Required]
        [DisplayName("Payments Quantity")]
        public int paymentsQuantity { get; set; }

        //[Required]
        //[DisplayName("Number of DownPayments")]
        //public int NumberofDownPayments { get; set; }

        //noraml contracts
        //[Required]
        //[DisplayName("Interest Rate")]
        //[Range(0, 100)]
        //public double interestRate { get; set; }

        //[DisplayName("Currency")]
        //[Required]
        //public String currency { get; set; }

        private DateTime initialHOAMonth { get; set; }
        //Date when HOA begins to be charged
        [DisplayName("First HOA Month")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime InitialHOAMonth
        {
            get
            {
                return this.initialHOAMonth.Year == 1900 ? this.closingDate
                        : this.initialHOAMonth;
            }
            set { this.initialHOAMonth = value; }
        }

        [DisplayName("HOA Yearly Payment")]
        public decimal HOAYearlyPayment { get; set; }

        //***Start
        //Added to Production
        // This is the new DP for all contracts this is not yet done
        //[Display(Name = "Deposit")]
        //[DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        //public decimal? deposit { get; set; }
        //***END

        //One real state contract is assigned to this one client
        //[Display(Name = "New Owner")]
        //public int clientID { get; set; }
        [ForeignKey("clientAssignedID")]
        public virtual Client clientAssigned { get; set; }

        //One real state contract is given by one client already in the system
        [Display(Name = "Current Owner")]
        public int? clientAssignorID { get; set; }
        [ForeignKey("clientAssignorID")]
        public virtual Client clientAssignor { get; set; }

        //Finnace deposits and HOA
        //[InverseProperty("realStateContract")]
        //public virtual ICollection<Deposit> deposits { get; set; }
        [InverseProperty("realStateContractHOA")]
        public virtual ICollection<Deposit> HOA_deposits { get; set; }

        /*************************/
        public decimal getTotalPaid()
        {
            List<Deposit> deposits = this.deposits.ToList();
            decimal totalPaid = deposits.Sum(d => d.Amount);
            return totalPaid;
        }

        public static Object[] getRealStateContractTypes()
        {
            Object[] list = new Object[]{
                new {Value="LOT", Text="LOT"},
                new {Value="HOUSE", Text="HOUSE"},
                new {Value="BUILD", Text="BUILD"},
            };
            return list;
        }

        public static Object[] getOwnershipHeldTypes()
        {
            Object[] list = new Object[]{
                new {Value="BT", Text="Banktrus"},
                new {Value="BTUSL", Text="Banktrus US LLC"},
                new {Value="MC", Text="Mexican Corporation"},
                new {Value="D", Text="Deed"},
                new {Value="DWM", Text="Deed with mortage"},
            };
            return list;
        }

    }
}