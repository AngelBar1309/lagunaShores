using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LagunaShoreResort2.Models;
//Implementation of MVC CheckBoxList Extension http://mvccbl.com/#documentation
namespace LagunaShoreResort2.Models.ViewModels
{
    public class VMMemberQualificationSelection
    {
        public List<VMQualificationMemberOption> availableMemberTypes { get; set; }
        public List<VMQualificationMemberOption> selectedMemberTypes { get; set; }
        public PostedMemberTypes postedMemberTypes { get; set; }

        public void fillWithMemberTypesDB(){
            this.availableMemberTypes = new List<VMQualificationMemberOption>();
            ApplicationDbContext db = new ApplicationDbContext();

            var roles = db.SalesMemberTypes.ToList();
            foreach(Rol rol in roles)
                this.availableMemberTypes.Add(new VMQualificationMemberOption
                {
                    typeID = rol.rolID, 
                    check = false, 
                    memberType = rol.type 
                });
        }


        public void selectMemberTypes(List<RolSalesMember> salesMememberRoles)
        {
            this.selectedMemberTypes = new List<VMQualificationMemberOption>();
            var roles = from saleMemeberRol in salesMememberRoles select saleMemeberRol.rol;
            foreach (Rol rol in roles)
                this.selectedMemberTypes.Add(new VMQualificationMemberOption
                {
                    typeID = rol.rolID,
                    check = false,
                    memberType = rol.type
                });
        }
    }

    public class VMQualificationMemberOption
    {
        public int typeID { get; set; }
        public Boolean check { get; set; }
        public String memberType { get; set; }
    }

    // Helper class to make posting back selected values easier
    public class PostedMemberTypes
    {
        public int[] members { get; set; }
    }

}