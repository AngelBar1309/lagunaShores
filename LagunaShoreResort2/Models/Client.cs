using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LagunaShoreResort2.Models
{
    public class Client
    {
        public int clientID {get; set;}//llave primaria
        
        [DisplayName("Client Name")] //DispalyName es para que se muestre el nombre en la vista como tu gustes
        public string firstName {get; set;}

        [DisplayName("Middle Name")]
        public string middleName {get; set;}
        
        [DisplayName("Last Name")]
        public string lastName {get; set;}

        [DisplayName("Legal Name")]
        public string legalName {get; set;}
       
        [DisplayName("Email Addres")]
        public string emailAddress {get; set;}
        
        [DisplayName("Primary Phone Number")]
        public string primaryPhoneNumber {get; set;}

        
        [DisplayName("Primary Residence Address")]
        public string primaryResidenceAddress {get; set;}

        [DisplayName("First Name 2")]
        public string secondFirstName {get; set;}

        [DisplayName("Middle Name 2")]
        public string secondMiddleName {get; set;}

        [DisplayName("Last Name 2")]
        public string secondLastName {get; set;}

        [DisplayName("Legal Name 2")]
        public string secondLegalName {get; set;}

        [DisplayName("Email 2")]
        public string secondEmailAddress  {get; set;}

        [DisplayName("Phone Number 2")]
        public string secondPhoneNumber  {get; set;}
       
        [DisplayName("City")]
        public string city {get; set;}
       
        [DisplayName("State")]
        public string state { get; set;}
        
        [DisplayName("Zip Code")]
        public string zipCode {get; set;}
        
        [DisplayName("Type Of ID")]
        [Required]
        public string typeOfID { get; set; }

        [Required]
        [DisplayName("Nationality")]
        public string nationality { get; set; }

        [DisplayName("verified by admin")]
        public bool verifiedByAdmin { get; set; }

        //Un cliente puede estar en varios contratos
        public virtual ICollection<SalesContract> salesContracts { get; set; }

        //un cliente puede estar en varios TM
        public virtual ICollection<TrialMemberships> trialMemberships { get; set; }

        //Un cliente tener varios terrenos como propiedad
        [InverseProperty("clientAssigned")]
        public virtual ICollection<RealStateContract> realsStatesOwning { get; set; }

        //Un cliente puede haber vendido varios terrenos
        [InverseProperty("clientAssignor")]
        public virtual ICollection<RealStateContract> realStatesTransfered { get; set; }
 
    }
}