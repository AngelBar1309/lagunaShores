using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMNameRolComission
    {
        [DisplayName("Member Name")]
        public String memberName { get; set; }
        [DisplayName("Type")]
        public String memberType { get; set; }
        [DisplayName("Commission")]
        public double comission { get; set; }

        [DisplayName("Earnings")]
        public decimal earning { get; set; }

        public VMNameRolComission() { }

        /// <summary>
        /// Creates a view model based from a given Contract-SalesMember pair.
        /// </summary>
        /// <param name="csm">Instance where is immplied the participation of specific sales member in a contract with a specific ro]/.le.</param>
        public VMNameRolComission(ContractSalesMember csm)
        {
            Double closingCost = (Double)csm.salesContract.closingCost;
            this.memberName = csm.salesMember.firtName + " " + csm.salesMember.lastName;
            this.memberType = csm.rol.type;
            this.comission = (Double)csm.rol.comssion;
            if (closingCost > 0)
            {
                Double closingComission = closingCost * 0.10;
                if (this.memberType == "CLOSER 1" || this.memberType == "CLOSER 2")
                {
                    this.earning = csm.salesContract.saleAmount * (decimal)(comission / 100) + (decimal)closingComission;
                }
                else
                {
                    earning = csm.salesContract.saleAmount * (decimal)(comission / 100);
                }
            }
            else
            {
                this.earning = csm.salesContract.saleAmount * (decimal)(comission / 100);
            }
            
            
            
        }

        /// <param name="csm"></param>
        /// <summary>
        /// Creates a view model given the relationship beetween a salesmember in a contract with certain role.
        /// </summary>
        /// <param name="salesContract">The contract where the member worked.</param>
        /// <param name="salesMember">The instance of the member itslef.</param>
        /// <param name="rol">The role done by the member in the given contract.</param>
        public VMNameRolComission(SalesContract salesContract, SalesMember salesMember, Rol rol)
        {
            Double closingCost = (Double)salesContract.closingCost;
            this.memberName = salesMember.firtName + " " + salesMember.lastName;
            this.memberType = rol.type;
            this.comission = (Double)rol.comssion;
            if (closingCost > 0)
            {
                Double closingComission = closingCost * 0.10;
                if (this.memberType == "CLOSER 1" || this.memberType == "CLOSER 2")
                {
                    this.earning = salesContract.saleAmount * (decimal)(this.comission / 100) + (decimal)closingComission;
                }
                else
                {
                    this.earning = salesContract.saleAmount * (decimal)(this.comission / 100);
                }
            }
            else
            {
                this.earning = salesContract.saleAmount * (decimal)(this.comission / 100);
            }
        }

        /// <param name="csm"></param>
        /// <summary>
        /// Creates a view model given the relationship beetween a salesmember in a contract with certain role.
        /// </summary>
        /// <param name="salesContract">The contract where the member worked.</param>
        /// <param name="salesMember">The instance of the member itslef.</param>
        /// <param name="rol">The role done by the member in the given contract.</param>
        public VMNameRolComission(SalesContract salesContract, Rol rol)
        {
            Double closingCost = (Double)salesContract.closingCost;
            this.memberName = "- -";
            this.memberType = rol.type;
            this.comission = (Double)rol.comssion;
            if (closingCost > 0)
            {
                Double closingComission = closingCost * 0.10;
                if (this.memberType == "CLOSER 1" || this.memberType == "CLOSER 2")
                {
                    this.earning = salesContract.saleAmount * (decimal)(this.comission / 100) + (decimal)closingComission;
                }
                else
                {
                    this.earning = salesContract.saleAmount * (decimal)(this.comission / 100);
                }
            }
            else
            {
                this.earning = salesContract.saleAmount * (decimal)(this.comission / 100);
            }
        }
    }
}