using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class RolSalesMember
    {
        //Id de rolSalesMember
        public int rolSalesMemberID { get; set; }

        //llave foranea del tipo de rol
        public int rolID { get; set; }
        public virtual Rol rol { get; set; }

        //llave foranea del SalesMember
        public int salesMemberID { get; set; }
        public virtual SalesMember salesMember { get; set; }
    }
}