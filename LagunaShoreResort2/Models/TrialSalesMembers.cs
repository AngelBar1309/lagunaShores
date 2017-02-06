using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class TrialSalesMembers
    {
        //llave primaria 
        [Key]
        public int trialSalesMemberID { get; set; }

        //Llaves foraneas de contrato

        public int? trialMembershipID { get; set; }
        public virtual TrialMemberships trialMemberships { get; set; }
        //Llaves foraneas de miembro de venta
        public int salesMemberID { get; set; }
        public virtual SalesMember salesMember { get; set; }

        //llave foranea de tipo de miembro 
        public int rolID { get; set; }
        public virtual Rol rol { get; set; }

        public TrialSalesMembers(){}
        /// <summary>
        /// Creates a new instance con Contract-SalesMember pair taking each argument.
        /// </summary>
        /// <param name="trialContractID"></param>
        /// <param name="salesMemberID"></param>
        /// <param name="rolID"></param>
        public TrialSalesMembers(int trialContractID, int salesMemberID, int rolID)
        {
            this.trialMembershipID = trialContractID;
            this.salesMemberID = salesMemberID;
            this.rolID = rolID;
        }
    }
}