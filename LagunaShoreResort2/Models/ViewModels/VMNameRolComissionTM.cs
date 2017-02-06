using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMNameRolComissionTM
    {
        [DisplayName("Member Name")]
        public String memberName { get; set; }
        [DisplayName("Type")]
        public String memberType { get; set; }
        [DisplayName("Commission")]
        public double comission { get; set; }
        [DisplayName("Earnings")]
        public double earning { get; set; }

        public VMNameRolComissionTM() { }
        /// <summary>
        /// Creates a view model based froma given Contract-SalesMember pair.
        /// </summary>
        /// <param name="csm"></param>
        public VMNameRolComissionTM(TrialSalesMembers csm)
        {
            this.memberName = csm.salesMember.firtName + csm.salesMember.lastName;
            this.memberType = csm.rol.type;
            this.comission = csm.rol.comssion;
            this.earning = csm.trialMemberships.tmSaleAmount * (this.comission / 100);
        }
        /// <param name="csm"></param>
        /// <summary>
        /// Creates a view model given the relationship beetween a salesmember in a contract with certain role.
        /// </summary>
        /// <param name="salesContract">The contract where the member worked.</param>
        /// <param name="salesMember">The instance of the member itslef.</param>
        /// <param name="rol">The role done by the member in the given contract.</param>
        public VMNameRolComissionTM(TrialMemberships trialMemberships, SalesMember salesMember, Rol rol)
        {
            this.memberName = salesMember.firtName + " " + salesMember.lastName;
            this.memberType = rol.type;
            this.comission = rol.comssion;
            this.earning = trialMemberships.tmSaleAmount * (this.comission / 100);
        }

        /// <param name="csm"></param>
        /// <summary>
        /// Creates a view model given the relationship beetween a salesmember in a contract with certain role.
        /// </summary>
        /// <param name="trialMemberships">The contract where the member worked.</param>
        /// <param name="salesMember">The instance of the member itslef.</param>
        /// <param name="rol">The role done by the member in the given contract.</param>
        public VMNameRolComissionTM(TrialMemberships trialMemberships, Rol rol)
        {
            this.memberName = "- -";
            this.memberType = rol.type;
            this.comission = rol.comssion;
            this.earning = trialMemberships.tmSaleAmount * (this.comission / 100);
        }
    }
}