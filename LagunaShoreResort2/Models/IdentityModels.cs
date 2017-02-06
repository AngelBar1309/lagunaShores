using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LagunaShoreResort2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ConexionLagunaShoreResort", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.Client> Clients { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.Condo> Condoes { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.ContractSalesMember> ContractSalesMembers { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.SalesContract> SalesContracts { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.SalesMember> SalesMembers { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.Rol> SalesMemberTypes { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.Deposit> Deposits { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.TrialSalesMembers> TrialSalesMembers { get; set; }

        public DbSet<TrialMemberships> trialMemberships { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.RolSalesMember> RolSalesMembers { get; set; }

        public System.Data.Entity.DbSet<LagunaShoreResort2.Models.RealStateContract> RealStateContracts { get; set; }
    }
}