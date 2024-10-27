//using FinalProjectAmPlansForLoans.Domain.Enums;
//using FinalProjectAmPlansForLoans.Domain.Models;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FinalProjectAmPlansForLoans.DataAccess.DataContext
//{
//    public class AmPlansDbContext : DbContext
//    {
//        public AmPlansDbContext(DbContextOptions<AmPlansDbContext> options)
//            : base(options)
//        {
//        }
//        public DbSet<Product> Products { get; set; }
//        public DbSet<LoanInput> LoanInputs { get; set; }
//        public DbSet<AmPlan> AmPlans { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // Configure LoanInput and its relationship
//            modelBuilder.Entity<LoanInput>()
//                .ToTable("LoanInputs")
//                .HasKey(li => li.Id);

//            modelBuilder.Entity<LoanInput>()
//                .HasOne(li => li.Product)
//                .WithMany(p => p.LoanInputs)
//                .HasForeignKey(li => li.ProductID)
//                .OnDelete(DeleteBehavior.Restrict); // Disable cascade delete here

//            // Configure Product entity
//            modelBuilder.Entity<Product>()
//                .ToTable("Products")
//                .HasKey(p => p.Id);

//            // Configure AmPlan entity
//            modelBuilder.Entity<AmPlan>()
//                .ToTable("AmPlans")
//                .HasKey(ap => ap.Id);

//            // Configure the relationship between AmPlan and LoanInput
//            modelBuilder.Entity<AmPlan>()
//                .HasOne(ap => ap.LoanInput)
//                .WithMany(li => li.AmortizationPlans)
//                .HasForeignKey(ap => ap.LoanInputID)
//                .OnDelete(DeleteBehavior.Cascade); // Cascade delete only if necessary

//            // Configure the relationship between AmPlan and Product
//            modelBuilder.Entity<AmPlan>()
//                .HasOne(ap => ap.Product)
//                .WithMany(p => p.AmortizationPlans)
//                .HasForeignKey(ap => ap.ProductID)
//                .OnDelete(DeleteBehavior.Restrict); // Disable cascade delete to prevent multiple paths

//            modelBuilder.Entity<Product>().HasData(
//               new Product { Id = 1, ProductName = "Home Loan", Status = ProductStatus.Active, Description = "Standard home financing", MinAmount = 10000.00, MaxAmount = 100000.00, MinInterestRate = 2.00, MaxInterestRate = 5.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 0.00 },
//               new Product { Id = 2, ProductName = "Agro Loan", Status = ProductStatus.Active, Description = "Standard Agro financing", MinAmount = 10000.00, MaxAmount = 500000.00, MinInterestRate = 3.00, MaxInterestRate = 4.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 1500.00 },
//               new Product { Id = 3, ProductName = "Auto Loan", Status = ProductStatus.Active, Description = "Financing for vehicles", MinAmount = 2000.00, MaxAmount = 50000.00, MinInterestRate = 2.50, MaxInterestRate = 4.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 2000.00 },
//               new Product { Id = 4, ProductName = "Education Loan", Status = ProductStatus.Active, Description = "Financing for Education", MinAmount = 5000.00, MaxAmount = 15000.00, MinInterestRate = 2.00, MaxInterestRate = 3.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 500.00 },
//               new Product { Id = 5, ProductName = "Personal Loan", Status = ProductStatus.Active, Description = "Unsecured personal financing", MinAmount = 1500.00, MaxAmount = 7000.00, MinInterestRate = 2.00, MaxInterestRate = 5.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 600.00 },
//               new Product { Id = 6, ProductName = "Business Loan", Status = ProductStatus.Active, Description = "Standard business financing", MinAmount = 30000.00, MaxAmount = 500000.00, MinInterestRate = 3.50, MaxInterestRate = 7.00, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 3500.00 }
//          );

//        }

//    }
//}

using FinalProjectAmPlansForLoans.Domain.Enums;
using FinalProjectAmPlansForLoans.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectAmPlansForLoans.DataAccess.DataContext
{
    public class AmPlansDbContext : DbContext
    {
        public AmPlansDbContext(DbContextOptions<AmPlansDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<LoanInput> LoanInputs { get; set; }
        public DbSet<AmPlan> AmPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasKey(p => p.Id);

            // Configure LoanInput entity
            modelBuilder.Entity<LoanInput>()
                .ToTable("LoanInputs")
                .HasKey(li => li.Id);

            // Set up relationship between LoanInput and Product
            modelBuilder.Entity<LoanInput>()
                .HasOne(li => li.Product) // Each LoanInput has one Product
                .WithMany() // Assuming no navigation property in Product for LoanInputs
                .HasForeignKey(li => li.ProductID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure AmPlan entity
            modelBuilder.Entity<AmPlan>()
                .ToTable("AmPlans")
                .HasKey(ap => ap.Id);

            // Set up relationship between AmPlan and LoanInput
            modelBuilder.Entity<AmPlan>()
                .HasOne(ap => ap.LoanInput) // Each AmPlan has one LoanInput
                .WithMany(li => li.AmortizationPlans) // LoanInput has many AmPlans
                .HasForeignKey(ap => ap.LoanInputID)
                .OnDelete(DeleteBehavior.Cascade); // If a LoanInput is deleted, delete its AmPlans

            // Set up relationship between AmPlan and Product
            modelBuilder.Entity<AmPlan>()
                .HasOne(ap => ap.Product) // Each AmPlan has one Product
                .WithMany() // Assuming no navigation property in Product for AmPlans
                .HasForeignKey(ap => ap.ProductID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Seed initial data for Product
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, ProductName = "Home Loan", Status = ProductStatus.Active, Description = "Standard home financing", MinAmount = 10000.00, MaxAmount = 100000.00, MinInterestRate = 2.00, MaxInterestRate = 5.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 0.00 },
                new Product { Id = 2, ProductName = "Agro Loan", Status = ProductStatus.Active, Description = "Standard Agro financing", MinAmount = 10000.00, MaxAmount = 500000.00, MinInterestRate = 3.00, MaxInterestRate = 4.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 1500.00 },
                new Product { Id = 3, ProductName = "Auto Loan", Status = ProductStatus.Active, Description = "Financing for vehicles", MinAmount = 2000.00, MaxAmount = 50000.00, MinInterestRate = 2.50, MaxInterestRate = 4.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 2000.00 },
                new Product { Id = 4, ProductName = "Education Loan", Status = ProductStatus.Active, Description = "Financing for Education", MinAmount = 5000.00, MaxAmount = 15000.00, MinInterestRate = 2.00, MaxInterestRate = 3.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 500.00 },
                new Product { Id = 5, ProductName = "Personal Loan", Status = ProductStatus.Active, Description = "Unsecured personal financing", MinAmount = 1500.00, MaxAmount = 7000.00, MinInterestRate = 2.00, MaxInterestRate = 5.50, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 600.00 },
                new Product { Id = 6, ProductName = "Business Loan", Status = ProductStatus.Active, Description = "Standard business financing", MinAmount = 30000.00, MaxAmount = 500000.00, MinInterestRate = 3.50, MaxInterestRate = 7.00, MinNumberOfInstallments = 10, MaxNumberOfInstallments = 60, AdminFee = 3500.00 }
            );
        }

    }
}

