using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LagunaShoreResort2.Models.ViewModels;

namespace LagunaShoreResort2.Models
{
    public class SalesMember
    {
        public int salesMemberID {get; set;}//Llave primaria

        [Required]
        [DisplayName("First Name")]
        public string firtName {get; set;}

        [Required]
        [DisplayName("Last Name")]
        public string lastName {get; set;}

        [Required]
        [DisplayName("Country Of Residence")]
        public string countryOfResidence {get; set;}

        [Required]
        [DisplayName("Country Of Origin")]
        public string countryOfOrigin {get; set;}

        [Required]
        [DisplayName("Address")]
        public string address {get; set;}

        [Required]
        [DisplayName("Telephone")]
        public string telephone {get; set;}

        [Required]
        [DisplayName("Email")]
        public string email {get; set;}

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Of Birth")]
        public DateTime dateOfBirth {get; set;}

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Of Hire")]
        public DateTime dateOfHire {get; set;}

        [Required]
        public string taxID { get; set; }

        [Required]
        [DisplayName("Emergency Contact Phone")]
        public string emergencyContact { get; set; }

        [Required]
        [DisplayName("Emergency Contact Name")]
        public string emergencyContactName { get; set; }

        [Required]
        [DisplayName("Social Media")]
        public string socialMedia { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Dropped Out Date")]
        [DataType(DataType.Date)]
        public DateTime? droppedOutDate  { get; set; }//Modificable
        
        [DisplayName("Active")]
        public bool? active { get; set; }

        //Un sale member puede participar en varios contratos con diferentes roles
        public virtual ICollection<ContractSalesMember> contractSalesMembers { get; set; }
        //Un sale member puede participar en varios trial memberships con diferentes roles 
        public virtual ICollection<TrialSalesMembers> trialSalesMembers { get; set; }

        //Un empleado puede tener varios Roles
        public virtual ICollection<RolSalesMember> rolSalesMembers { get; set; }


        public List<RolSalesMember> generateRolSalesMemberList(VMMemberQualificationSelection selectedQualifications)
        {
            List<RolSalesMember> qualifications = new List<RolSalesMember>();
            //Qualifications for new sales member are registered based on checkboxlist selection
            foreach (int typeID in selectedQualifications.postedMemberTypes.members)
            {
                RolSalesMember rolSalesMember = new RolSalesMember();
                rolSalesMember.rolID = typeID;
                rolSalesMember.salesMemberID = this.salesMemberID;
                qualifications.Add(rolSalesMember);
            }
            return qualifications;
        }
    }
}