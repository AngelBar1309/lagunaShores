using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class ContractSalesMember
    {
        //llave primaria 
        [Key]
        public int contractSalesMemberID { get; set; }


        //Llaves foraneas de contrato

        public int? salesContractID { get; set; }
        public virtual SalesContract salesContract { get; set; }
        //Llaves foraneas de miembro de venta
        public  int salesMemberID { get; set; }
        public virtual SalesMember salesMember { get; set; }

        //llave foranea de tipo de miembro 
        public int rolID { get; set; }
        public virtual Rol rol { get; set; }

        public ContractSalesMember(){}

        /// <summary>
        /// Creates a new instance con Contract-SalesMember pair taking each argument.
        /// </summary>
        /// <param name="salesContractID"></param>
        /// <param name="salesMemberID"></param>
        /// <param name="rolID"></param>
        public ContractSalesMember(int salesContractID, int salesMemberID, int rolID)
        {
                this.salesContractID = salesContractID;
                this.salesMemberID = salesMemberID;
                this.rolID = rolID;
        }

        public void completeContractSalesMember()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            this.salesMember = db.SalesMembers.Find(this.salesMemberID);
            this.rol = db.SalesMemberTypes.Find(this.rolID);
        }
    }
}