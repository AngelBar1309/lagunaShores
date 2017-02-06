using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMComissionRequestTM
    {
        
        [DisplayName("ID")]
        public int trialMemberhsipID { get; set; }

        [DisplayName("Request Date")]
        public DateTime requestDate { get; set; }

        [DisplayName("Contract Number")]
        public String contractNumber { get; set; }

        [DisplayName("Client Name")]
        public String clientName { get; set; }

        [DisplayName("Second Client Name")]
        public String secondClientName { get; set; }

        [DisplayName("Sale Amount")]
        public double saleAmount { get; set; }

        [DisplayName("Currency")]
        public String currency { get; set; }

        [DisplayName("Commission Requested")]
        public bool commissionRequested { get; set; }

        [DisplayName("Commission Requested")]
        public bool? commissionPaid { get; set; }

        [DisplayName("Verified By Contract Manager")]
        public bool? verifiedByAdmin { get; set; } //Modificable

        /*Roles and their commissions*/
        public List<VMNameRolComissionTM> membersAndComissions { get; set; }

        /// <summary>
        /// Creates a view model for a Commission Resquest based from a given SalesContact
        /// </summary>
        /// <param name="c"></param>
        public VMComissionRequestTM(TrialMemberships c)
        {
            this.trialMemberhsipID = c.trialMembershipID;
            this.requestDate = DateTime.Now;
            this.contractNumber = c.contractNumberTM;
            this.clientName = c.client.legalName;
            this.secondClientName = c.client.secondLegalName;
            this.saleAmount = c.tmSaleAmount;
            this.currency = c.tmCurrency;
            this.commissionRequested = c.tmRequestToAccountat;
            this.verifiedByAdmin = c.tmVerifiedByAdmin;
            this.commissionPaid = c.tmCommissionPaid;

            //Fill sales member array with Contract-SalesMember association
            membersAndComissions = new List<VMNameRolComissionTM>();
            fillMemberAndComission(c);
        }

        private void fillMemberAndComission(TrialMemberships c)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            VMNameRolComissionTM memberComission;
            //Create a list with view models for every sales member related to the contract
            foreach (TrialSalesMembers csm in c.trialSalesMembers)
            {
                memberComission = new VMNameRolComissionTM(csm);
                membersAndComissions.Add(memberComission);
            }

            //Get from database the roles that has to receive commission from every contract
            var everyContractSalesMember = db.SalesMemberTypes.Where(memb => memb.inEveryContract).ToList();
            foreach (Rol rol in everyContractSalesMember)
            {
                //If a everyContractSalesMember was registered when the contract was created
                var salesMemberInEveryContractRegistered = membersAndComissions.Where(csm => csm.memberType == rol.type);
                if (salesMemberInEveryContractRegistered.Count() > 0)
                {
                    //Its added to the commission request with name censored
                    membersAndComissions.RemoveAll(csm => csm.memberType == rol.type);
                    foreach (TrialSalesMembers csm in c.trialSalesMembers)
                    {
                        memberComission = new VMNameRolComissionTM(csm);
                        membersAndComissions.Add(memberComission);
                    }
                    //memberComission = new VMNameRolComissionTM(c, rol);
                    //membersAndComissions.Add(memberComission);
                }
                else {
                    //For each rol, gets from database wich sales members have this rol
                    var rolSalesMember = db.SalesMemberTypes.Include("rolSalesMembers").FirstOrDefault(smt => smt.rolID == rol.rolID).
                        rolSalesMembers;

                    if (rolSalesMember.Count() > 0)
                    {
                        memberComission = new VMNameRolComissionTM(c, rol);
                        membersAndComissions.Add(memberComission);
                    }
                }
            }
        }
    }
}