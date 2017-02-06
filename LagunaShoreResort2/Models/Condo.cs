using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class Condo
    {
        public int condoID {get; set;}//llave primaria 

        [DisplayName("Condo Name")]
        public string name { get; set; }
        [DisplayName("Phase")]
        public double phase { get; set; }
        [DisplayName("Block")]
        public string block {get; set;}
        [DisplayName("Lot")]
        public string lot { get; set; }
        [DisplayName("Bredrooms")]
        public int bedrooms {get; set;}
        [DisplayName("Description")]
        public string description { get; set; }
        [DisplayName("Building")]
        public string building { get; set; }
        [DisplayName("sqft")]
        public string sqft { get; set; }
        [DisplayName("sqmt")]
        public string sqmt { get; set; }
        /*
        [DisplayName("HOA Yearly Payment")]
        public decimal HOAYearlyPayment { get; set; }
        */
        //uncomment when changes are ready to be make for inventory, and upgrades
        [DisplayName("Annual Fractions Sold")]
        public int usedFA { get; set; }
        [DisplayName("Even Biannual Fractions Sold")]
        public int evenFA { get; set; }
        [DisplayName("Odd Biannual Fractions Sold")]
        public int oddFA { get; set; }
        [DisplayName("List Price Minimum")]
        public double listpriceMin { get; set; }
        [DisplayName("List Price Maximum")]
        public double listpriceMax { get; set; }

        //A condo could be included in many fractional contracts
        public virtual ICollection<SalesContract> salesContracts { get; set; }
        //A condo could be included in many Real State Contracts
        public virtual ICollection<RealStateContract> realStateContracts { get; set; }
    }
}