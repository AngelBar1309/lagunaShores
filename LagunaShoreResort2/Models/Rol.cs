using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class Rol
    {
        [Key]
        public int rolID { get; set; }//llave primaria 

        [DisplayName("Member Role")]
        public string type { get; set; }
        [DisplayName("Comission (%)")]
        public decimal comssion { get; set; }
        [DisplayName("Mandatory Rol")]
        public bool mandatory { get; set; }

        [DisplayName("Mandatory Rol")]
        public bool mandatoryTrialManager { get; set; }
        
        [DisplayName("In Every Contract")]
        public Boolean inEveryContract { get; set; }

        //Un rol puede tener varios empleados
        public ICollection<RolSalesMember> rolSalesMembers { get; set; }

        //Un Rol puede ser adoptado por diferentes SalesMember en cada Contract
        public virtual ICollection<ContractSalesMember> contractSalesMembers { get; set; }
        //Un Rol puede ser adoptado por diferentes SalesMember en cada TM
        public virtual ICollection<TrialSalesMembers> trialSalesMembers { get; set; }

        public Rol(){}

        public Rol(int rolID, String memberType)
        {
            this.rolID = rolID;
            this.type = memberType;
        }

        /// <summary>
        /// Retrieve all the default sales members for every contract
        /// </summary>
        /// <returns>A list of roles for every instance of the sales members by default.</returns>
        public static List<Rol> getDefaultSalesMembersTypesTM()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Rol> defaultSalesMembers = db.SalesMemberTypes.Include("rolSalesMembers").Where(
                rol => rol.type == "Trial Assistant" || rol.type == "Trial Manager" ||
                    rol.type == "OPC IN HOUSE" || rol.type == "OPC OUT HOUSE" ||
                    rol.type == "VLO").ToList();

            return defaultSalesMembers;
        }
        //Default validation error messages
        public static class SalesMembersTypesNames
        {
            public const string LINER1 = "LINER 1";
            public const string LINER2 = "LINER 2";
            public const string CLOSER1 = "CLOSER 1";
            public const string CLOSER2 = "CLOSER 2";
            public const string OPC_IN_HOUSE = "OPC IN HOUSE";
            public const string OPC_OUT_HOUSE = "OPC OUT HOUSE";
            public const string VLO = "VLO";
            public const string MANAGER1 = "MANAGER1";
            public const string MANAGER2 = "MANAGER2";
            public const string MANAGER3 = "MANAGER3";
            public const string MANAGER4 = "MANAGER4";
            public const string SALES_DIRECTOR = "SALES DIRECTOR";
            public const string MARKETING_MANAGER = "MARKETING MANAGER";
            public const string TRIAL_ASSISTANT = "TRIAL ASSISTANT";
            public const string TRIAL_MANAGER = "TRIAL MANAGER";
        }
    }

}