using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMComissionRequest
    {
        [DisplayName("ID")]
        public int salesContractID { get; set; }

        [DisplayName("Request Date")]
        [DisplayFormat(DataFormatString="{0:d}")]
        public DateTime requestDate { get; set; }

        [DisplayName("Contract Number")]
        public String contractNumber { get; set; }

        [DisplayName("Client Name")]
        public String clientName { get; set; }

        [DisplayName("Second Client Name")]
        public String secondClientName { get; set; }

        [DisplayName("Sale Amount")]
        public decimal saleAmount { get; set; }

        [DisplayName("Closing Cost")]
        public decimal closigCost { get; set; }

        [DisplayName("Currency")]
        public String currency { get; set; }

        [DisplayName("Commission Requested")]
        public bool commissionRequested { get; set; }

        [DisplayName("Commission Requested")]
        public bool commissionPaid { get; set; }

        [DisplayName("Verified By Contract Manager")]
        public bool verifiedByAdmin { get; set; } //Modificable

        /*Roles and their commissions*/
        public List<VMNameRolComission> membersAndComissions { get; set; }

        /// <summary>
        /// Creates a view model for a Commission Resquest based from a given SalesContact
        /// </summary>
        /// <param name="c"></param>
        public VMComissionRequest(SalesContract c)
        {
            this.salesContractID = c.contractID;
            this.requestDate = DateTime.Now;
            this.contractNumber = c.contractNumber;
            this.clientName = c.client.legalName;
            this.secondClientName = c.client.secondLegalName;
            this.saleAmount = c.saleAmount;
            this.currency = c.currency;
            this.commissionRequested = c.requestToAccountant;
            this.verifiedByAdmin = c.verifiedByAdmin;
            this.commissionPaid = c.commissionPaid;
            this.closigCost = c.closingCost; 

            //Fill sales member array with Contract-SalesMember association
            membersAndComissions = new List<VMNameRolComission>();
            fillMemberAndComission(c);
        }

        private void fillMemberAndComission(SalesContract c)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            VMNameRolComission memberComission;
            //Create a list with view models for every sales member related to the contract
            foreach (ContractSalesMember csm in c.contractSalesMembers)
            {
                memberComission = new VMNameRolComission(csm);
                membersAndComissions.Add(memberComission);
            }

            //Get from database the roles that has to receive commission from every contract
            var everyContractSalesMember = db.SalesMemberTypes.Where(memb => memb.inEveryContract).ToList();
            foreach (Rol rol in everyContractSalesMember)
            {
                //If a everyContractSalesMember was registered when the contract was created
                var salesMemberInEveryContractRegistered = membersAndComissions.Where(csm => csm.memberType == rol.type);
                if (salesMemberInEveryContractRegistered.Count()>0)
                {
                    //Its added to the commission request with name censored
                    membersAndComissions.RemoveAll(csm => csm.memberType == rol.type);
                    //foreach (ContractSalesMember csm in c.contractSalesMembers)
                    //{
                    //    memberComission = new VMNameRolComission(csm);
                    //    membersAndComissions.Add(memberComission);
                    //}
                    memberComission = new VMNameRolComission(c, rol);
                    membersAndComissions.Add(memberComission);
                }
                else { 
                    //For each rol, gets from database wich sales members have this rol
                    var rolSalesMember = db.SalesMemberTypes.Include("rolSalesMembers").FirstOrDefault(smt => smt.rolID == rol.rolID).
                        rolSalesMembers;

                    if (rolSalesMember.Count() > 0) {
                        //foreach (ContractSalesMember csm in c.contractSalesMembers)
                        //{
                        //    memberComission = new VMNameRolComission(csm);
                        //    membersAndComissions.Add(memberComission);
                        //}
                        memberComission = new VMNameRolComission(c, rol);
                        membersAndComissions.Add(memberComission);
                    }
                }
            }

        }
    }
}