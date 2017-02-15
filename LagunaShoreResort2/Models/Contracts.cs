using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class Contracts
    {
        [DisplayName("Contract ID")]
        public int contractID { get; set; }

        [DisplayName("Verified By Contract Manager")]
        public bool verifiedByAdmin { get; set; }

        /*el tipo se definira despues de que corra la app
         ya que la migracion hace un espacio nuevo en la tabla donde 
        verifica de que tabla es su prcedencia*/
        [DisplayName("Currency")]
        public string currency { get; set; }

        [DisplayName("Sale Amount")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal saleAmount { get; set; }

        //***Start
        //Added to Production
        // This is the new DP for all contracts this is not yet done
        [DisplayName("Deposit")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal? deposit { get; set; }

        //noraml contracts
        [DisplayName("Interest Rate")]
        [Range(0, 100)]
        public double interestRate { get; set; }

        [Required]
        [DisplayName("Number of DownPayments")]
        public int NumberofDownPayments { get; set; }

        [DisplayName("Commission Paid")]
        public Boolean commissionPaid { get; set; }

        [DisplayName("Canceled Contract")]
        public bool canceledContract { get; set; }
        //las fechas del contrato son totalmente diferentes en las 3

        [DisplayName("Request to Accountant")]
        public bool requestToAccountant { get; set; }

        //llave foranea de cliente
        public int clientID { get; set; }
        public virtual Client client { get; set; }

        [InverseProperty("Contract")]
        public virtual ICollection<Deposit> deposits { get; set; }
    }
}