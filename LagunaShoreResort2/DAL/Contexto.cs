using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LagunaShoreResort2.Models;
using System.Data.Entity;


namespace LagunaShoreResort2.DAL
{
    public class Contexto : DbContext
    {
        public Contexto()
            : base("ConexionLagunaShoreResort")
        {

        }
        //Definicion de tablas apartir de las entidades
        public DbSet<Client> clients { get; set; }
        public DbSet<Condo> condos { get; set; }
        public DbSet<ContractSalesMember> ContractSalesMembers { get; set; }
        public DbSet<Deposit> deposits { get; set; }
        public DbSet<TrialMemberships> trialMemberships { get; set; }

        //No es necesario, el plan de pagos se genera al vuelo cada vez que se desea consultar
        //public DbSet<Deposit_Payment> Deposit_Payment { get; set; }
        //public DbSet<Payment> payments { get; set; }

        public DbSet<SalesContract> salesContracts { get; set; }
        public DbSet<SalesMember> salesMembers { get; set; }
        public DbSet<Rol> salesMemberTypes { get; set; }

    }
}