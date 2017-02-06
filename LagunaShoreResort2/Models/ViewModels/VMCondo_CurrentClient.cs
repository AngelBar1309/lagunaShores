using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models.ViewModels
{
    //ViewModel to show Condo Data related with current and actual contract
    //where is being sold or was completely paid.
    public class VMCondo_CurrentClient
    {
        //Condo Data
        public int condoID { get; set; }
        public String condoName { get; set; }

        //Id from current contract and owner client
        public int salesContractID { get; set; }
        public int clientID { get; set; }

        //Client Data
        public String legalName1 { get; set; }
        public String legalName2 { get; set; }

        public VMCondo_CurrentClient() { }

        public VMCondo_CurrentClient(int condoID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var condos = db.Condoes.Include("salesContracts");
            Condo condo = condos.SingleOrDefault(c => c.condoID == condoID);
            this.condoID = condoID;
            //If condo is owned by a client
            if (condo != null && condo.salesContracts.Count>0)
            {
                //Look for the current contract done for the indicated condo
                SalesContract contract = condo.salesContracts.OrderByDescending(sc => sc.contractDate).FirstOrDefault();
                this.salesContractID = contract.salesContractID;
                this.clientID = contract.clientID;
                this.condoName = condo.name;
                this.legalName1 = contract.client.legalName;
                this.legalName2 = contract.client.secondLegalName;
            }
            //If condo has not been bought
            else
            {
                this.salesContractID = 0;
                this.clientID = 0;
                this.legalName1 = "No Owner";
                this.legalName2 = "";
            }
        }
    }
}