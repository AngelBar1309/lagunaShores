namespace LagunaShoreResort2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        clientID = c.Int(nullable: false, identity: true),
                        firstName = c.String(),
                        middleName = c.String(),
                        lastName = c.String(),
                        legalName = c.String(),
                        emailAddress = c.String(),
                        primaryPhoneNumber = c.String(),
                        primaryResidenceAddress = c.String(),
                        secondFirstName = c.String(),
                        secondMiddleName = c.String(),
                        secondLastName = c.String(),
                        secondLegalName = c.String(),
                        secondEmailAddress = c.String(),
                        secondPhoneNumber = c.String(),
                        city = c.String(),
                        state = c.String(),
                        zipCode = c.String(),
                        typeOfID = c.String(nullable: false),
                        nationality = c.String(nullable: false),
                        verifiedByAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.clientID);
            
            CreateTable(
                "dbo.RealStateContracts",
                c => new
                    {
                        realStateContractID = c.Int(nullable: false, identity: true),
                        type = c.String(nullable: false),
                        MLS = c.String(nullable: false),
                        ownershipHeld = c.String(nullable: false),
                        saleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        saleDate = c.DateTime(nullable: false),
                        closingDate = c.DateTime(nullable: false),
                        verifiedByAdmin = c.Boolean(nullable: false),
                        requestToAccountant = c.Boolean(nullable: false),
                        canceledContract = c.Boolean(nullable: false),
                        commissionPaid = c.Boolean(nullable: false),
                        salesMemberID = c.Int(nullable: false),
                        condoID = c.Int(nullable: false),
                        closingCost = c.Double(nullable: false),
                        paymentsQuantity = c.Int(nullable: false),
                        NumberofDownPayments = c.Int(nullable: false),
                        interestRate = c.Double(nullable: false),
                        currency = c.String(nullable: false),
                        InitialHOAMonth = c.DateTime(nullable: false),
                        HOAYearlyPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        deposit = c.Decimal(precision: 18, scale: 2),
                        clientAssignedID = c.Int(nullable: false),
                        clientAssignorID = c.Int(),
                    })
                .PrimaryKey(t => t.realStateContractID)
                .ForeignKey("dbo.Condoes", t => t.condoID, cascadeDelete: true)
                .ForeignKey("dbo.SalesMembers", t => t.salesMemberID, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.clientAssignedID, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.clientAssignorID)
                .Index(t => t.salesMemberID)
                .Index(t => t.condoID)
                .Index(t => t.clientAssignedID)
                .Index(t => t.clientAssignorID);
            
            CreateTable(
                "dbo.Condoes",
                c => new
                    {
                        condoID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        phase = c.Double(nullable: false),
                        block = c.String(),
                        lot = c.String(),
                        bedrooms = c.Int(nullable: false),
                        description = c.String(),
                        building = c.String(),
                        sqft = c.String(),
                        sqmt = c.String(),
                        usedFA = c.Int(nullable: false),
                        evenFA = c.Int(nullable: false),
                        oddFA = c.Int(nullable: false),
                        listpriceMin = c.Double(nullable: false),
                        listpriceMax = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.condoID);
            
            CreateTable(
                "dbo.SalesContracts",
                c => new
                    {
                        salesContractID = c.Int(nullable: false, identity: true),
                        contractNumber = c.String(),
                        contractType = c.String(),
                        typeOfFraction = c.String(),
                        contractDate = c.DateTime(nullable: false),
                        saleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        closingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        listPrice = c.Double(nullable: false),
                        deposit = c.Decimal(precision: 18, scale: 2),
                        tradeInValue = c.Decimal(precision: 18, scale: 2),
                        previousBalance = c.Decimal(precision: 18, scale: 2),
                        newUnitPrice = c.Decimal(precision: 18, scale: 2),
                        upgrade = c.Boolean(nullable: false),
                        upgraded = c.Boolean(nullable: false),
                        previousTMID = c.Int(),
                        previousFCID = c.Int(),
                        interestRate = c.Double(nullable: false),
                        verifiedByAdmin = c.Boolean(nullable: false),
                        verificationDate = c.DateTime(),
                        requestToAccountant = c.Boolean(nullable: false),
                        requestToAccountantDate = c.DateTime(),
                        comments = c.String(),
                        canceledContract = c.Boolean(nullable: false),
                        canceledDate = c.DateTime(),
                        NumberofDownPayments = c.Int(nullable: false),
                        qualification = c.String(),
                        attachmentsSpanish = c.String(),
                        attachmentsEnglish = c.String(),
                        currency = c.String(),
                        deedWeeks = c.Int(nullable: false),
                        bonusWeeks = c.Double(nullable: false),
                        bonusWeeksExp = c.DateTime(nullable: false),
                        advantageWeeks = c.Double(nullable: false),
                        advantageWeeksExp = c.DateTime(nullable: false),
                        commissionPaid = c.Boolean(nullable: false),
                        commissionPaidDate = c.DateTime(),
                        userCreateContract = c.String(),
                        priceInWordSpanish = c.String(),
                        priceInWordInglish = c.String(),
                        Concord = c.String(),
                        yearEven = c.Boolean(nullable: false),
                        csToOwner = c.Boolean(nullable: false),
                        csToOwnerDate = c.DateTime(),
                        csToConcord = c.Boolean(nullable: false),
                        csToConcordDate = c.DateTime(),
                        paymentsQuantity = c.Int(nullable: false),
                        downPaymentCompleted = c.Boolean(nullable: false),
                        InitialHOAMonth = c.DateTime(nullable: false),
                        HOAYearlyPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        condoID = c.Int(nullable: false),
                        clientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.salesContractID)
                .ForeignKey("dbo.Clients", t => t.clientID, cascadeDelete: true)
                .ForeignKey("dbo.Condoes", t => t.condoID, cascadeDelete: true)
                .Index(t => t.condoID)
                .Index(t => t.clientID);
            
            CreateTable(
                "dbo.ContractSalesMembers",
                c => new
                    {
                        contractSalesMemberID = c.Int(nullable: false, identity: true),
                        salesContractID = c.Int(),
                        salesMemberID = c.Int(nullable: false),
                        rolID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.contractSalesMemberID)
                .ForeignKey("dbo.Rols", t => t.rolID, cascadeDelete: true)
                .ForeignKey("dbo.SalesMembers", t => t.salesMemberID, cascadeDelete: true)
                .ForeignKey("dbo.SalesContracts", t => t.salesContractID)
                .Index(t => t.salesContractID)
                .Index(t => t.salesMemberID)
                .Index(t => t.rolID);
            
            CreateTable(
                "dbo.Rols",
                c => new
                    {
                        rolID = c.Int(nullable: false, identity: true),
                        type = c.String(),
                        comssion = c.Double(nullable: false),
                        mandatory = c.Boolean(nullable: false),
                        mandatoryTrialManager = c.Boolean(nullable: false),
                        inEveryContract = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.rolID);
            
            CreateTable(
                "dbo.RolSalesMembers",
                c => new
                    {
                        rolSalesMemberID = c.Int(nullable: false, identity: true),
                        rolID = c.Int(nullable: false),
                        salesMemberID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.rolSalesMemberID)
                .ForeignKey("dbo.Rols", t => t.rolID, cascadeDelete: true)
                .ForeignKey("dbo.SalesMembers", t => t.salesMemberID, cascadeDelete: true)
                .Index(t => t.rolID)
                .Index(t => t.salesMemberID);
            
            CreateTable(
                "dbo.SalesMembers",
                c => new
                    {
                        salesMemberID = c.Int(nullable: false, identity: true),
                        firtName = c.String(nullable: false),
                        lastName = c.String(nullable: false),
                        countryOfResidence = c.String(nullable: false),
                        countryOfOrigin = c.String(nullable: false),
                        address = c.String(nullable: false),
                        telephone = c.String(nullable: false),
                        email = c.String(nullable: false),
                        dateOfBirth = c.DateTime(nullable: false),
                        dateOfHire = c.DateTime(nullable: false),
                        taxID = c.String(nullable: false),
                        emergencyContact = c.String(nullable: false),
                        emergencyContactName = c.String(nullable: false),
                        socialMedia = c.String(nullable: false),
                        droppedOutDate = c.DateTime(),
                        active = c.Boolean(),
                    })
                .PrimaryKey(t => t.salesMemberID);
            
            CreateTable(
                "dbo.TrialSalesMembers",
                c => new
                    {
                        trialSalesMemberID = c.Int(nullable: false, identity: true),
                        trialMembershipID = c.Int(),
                        salesMemberID = c.Int(nullable: false),
                        rolID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.trialSalesMemberID)
                .ForeignKey("dbo.Rols", t => t.rolID, cascadeDelete: true)
                .ForeignKey("dbo.SalesMembers", t => t.salesMemberID, cascadeDelete: true)
                .ForeignKey("dbo.TrialMemberships", t => t.trialMembershipID)
                .Index(t => t.trialMembershipID)
                .Index(t => t.salesMemberID)
                .Index(t => t.rolID);
            
            CreateTable(
                "dbo.TrialMemberships",
                c => new
                    {
                        trialMembershipID = c.Int(nullable: false, identity: true),
                        contractNumberTM = c.String(),
                        contractType = c.String(),
                        tmContractDate = c.DateTime(nullable: false),
                        tmSaleAmount = c.Double(nullable: false),
                        deposit = c.Decimal(precision: 18, scale: 2),
                        upgraded = c.Boolean(nullable: false),
                        tmInterestRate = c.Double(nullable: false),
                        tmVerifiedByAdmin = c.Boolean(nullable: false),
                        tmVerificationDate = c.DateTime(),
                        tmRequestToAccountat = c.Boolean(nullable: false),
                        tmRequestToAccountantDate = c.DateTime(),
                        trialNights = c.Double(),
                        weekendNights = c.Double(),
                        weekdayNights = c.Double(),
                        referralNights = c.Double(),
                        trialExpDate = c.DateTime(),
                        advantageWeeks = c.Double(),
                        advantageExpDate = c.DateTime(),
                        quickWeeksExpDate = c.DateTime(),
                        tmCanceledContract = c.Boolean(nullable: false),
                        tmCanceledContractDate = c.DateTime(),
                        tmPaymentsStartDate = c.DateTime(nullable: false),
                        tmNumberofDownPayments = c.Int(nullable: false),
                        tmNumberPayments = c.Int(nullable: false),
                        tmQualification = c.String(),
                        tmCurrency = c.String(),
                        tmListPrice = c.Double(nullable: false),
                        tmCommissionPaid = c.Boolean(nullable: false),
                        tmCommissionPaidDate = c.DateTime(),
                        tmRoomType = c.String(),
                        tmComments = c.String(),
                        userCreateContract = c.String(),
                        Concord = c.String(),
                        csToConcord = c.Boolean(nullable: false),
                        csToConcordDate = c.DateTime(),
                        clientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.trialMembershipID)
                .ForeignKey("dbo.Clients", t => t.clientID, cascadeDelete: true)
                .Index(t => t.clientID);
            
            CreateTable(
                "dbo.Deposits",
                c => new
                    {
                        noFolio = c.Int(nullable: false, identity: true),
                        DepositDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrencyType = c.String(nullable: false),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RefNumber = c.String(nullable: false),
                        PayMethod = c.String(nullable: false),
                        Refund = c.Boolean(nullable: false),
                        depositComments = c.String(),
                        verification = c.Boolean(nullable: false),
                        salesContractID = c.Int(),
                        trialMembershipID = c.Int(),
                        realStateContractID = c.Int(),
                        salesContractID_HOA = c.Int(),
                        realStateContractID_HOA = c.Int(),
                    })
                .PrimaryKey(t => t.noFolio)
                .ForeignKey("dbo.TrialMemberships", t => t.trialMembershipID)
                .ForeignKey("dbo.SalesContracts", t => t.salesContractID)
                .ForeignKey("dbo.SalesContracts", t => t.salesContractID_HOA)
                .ForeignKey("dbo.RealStateContracts", t => t.realStateContractID)
                .ForeignKey("dbo.RealStateContracts", t => t.realStateContractID_HOA)
                .Index(t => t.salesContractID)
                .Index(t => t.trialMembershipID)
                .Index(t => t.realStateContractID)
                .Index(t => t.salesContractID_HOA)
                .Index(t => t.realStateContractID_HOA);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.RealStateContracts", "clientAssignorID", "dbo.Clients");
            DropForeignKey("dbo.RealStateContracts", "clientAssignedID", "dbo.Clients");
            DropForeignKey("dbo.RealStateContracts", "salesMemberID", "dbo.SalesMembers");
            DropForeignKey("dbo.Deposits", "realStateContractID_HOA", "dbo.RealStateContracts");
            DropForeignKey("dbo.Deposits", "realStateContractID", "dbo.RealStateContracts");
            DropForeignKey("dbo.Deposits", "salesContractID_HOA", "dbo.SalesContracts");
            DropForeignKey("dbo.Deposits", "salesContractID", "dbo.SalesContracts");
            DropForeignKey("dbo.ContractSalesMembers", "salesContractID", "dbo.SalesContracts");
            DropForeignKey("dbo.TrialSalesMembers", "trialMembershipID", "dbo.TrialMemberships");
            DropForeignKey("dbo.Deposits", "trialMembershipID", "dbo.TrialMemberships");
            DropForeignKey("dbo.TrialMemberships", "clientID", "dbo.Clients");
            DropForeignKey("dbo.TrialSalesMembers", "salesMemberID", "dbo.SalesMembers");
            DropForeignKey("dbo.TrialSalesMembers", "rolID", "dbo.Rols");
            DropForeignKey("dbo.RolSalesMembers", "salesMemberID", "dbo.SalesMembers");
            DropForeignKey("dbo.ContractSalesMembers", "salesMemberID", "dbo.SalesMembers");
            DropForeignKey("dbo.RolSalesMembers", "rolID", "dbo.Rols");
            DropForeignKey("dbo.ContractSalesMembers", "rolID", "dbo.Rols");
            DropForeignKey("dbo.SalesContracts", "condoID", "dbo.Condoes");
            DropForeignKey("dbo.SalesContracts", "clientID", "dbo.Clients");
            DropForeignKey("dbo.RealStateContracts", "condoID", "dbo.Condoes");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Deposits", new[] { "realStateContractID_HOA" });
            DropIndex("dbo.Deposits", new[] { "salesContractID_HOA" });
            DropIndex("dbo.Deposits", new[] { "realStateContractID" });
            DropIndex("dbo.Deposits", new[] { "trialMembershipID" });
            DropIndex("dbo.Deposits", new[] { "salesContractID" });
            DropIndex("dbo.TrialMemberships", new[] { "clientID" });
            DropIndex("dbo.TrialSalesMembers", new[] { "rolID" });
            DropIndex("dbo.TrialSalesMembers", new[] { "salesMemberID" });
            DropIndex("dbo.TrialSalesMembers", new[] { "trialMembershipID" });
            DropIndex("dbo.RolSalesMembers", new[] { "salesMemberID" });
            DropIndex("dbo.RolSalesMembers", new[] { "rolID" });
            DropIndex("dbo.ContractSalesMembers", new[] { "rolID" });
            DropIndex("dbo.ContractSalesMembers", new[] { "salesMemberID" });
            DropIndex("dbo.ContractSalesMembers", new[] { "salesContractID" });
            DropIndex("dbo.SalesContracts", new[] { "clientID" });
            DropIndex("dbo.SalesContracts", new[] { "condoID" });
            DropIndex("dbo.RealStateContracts", new[] { "clientAssignorID" });
            DropIndex("dbo.RealStateContracts", new[] { "clientAssignedID" });
            DropIndex("dbo.RealStateContracts", new[] { "condoID" });
            DropIndex("dbo.RealStateContracts", new[] { "salesMemberID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Deposits");
            DropTable("dbo.TrialMemberships");
            DropTable("dbo.TrialSalesMembers");
            DropTable("dbo.SalesMembers");
            DropTable("dbo.RolSalesMembers");
            DropTable("dbo.Rols");
            DropTable("dbo.ContractSalesMembers");
            DropTable("dbo.SalesContracts");
            DropTable("dbo.Condoes");
            DropTable("dbo.RealStateContracts");
            DropTable("dbo.Clients");
        }
    }
}
