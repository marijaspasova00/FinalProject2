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

            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasKey(p => p.Id);

            modelBuilder.Entity<LoanInput>()
                .ToTable("LoanInputs")
                .HasKey(li => li.Id);

            modelBuilder.Entity<LoanInput>()
                .HasOne(li => li.Product) 
                .WithMany() 
                .HasForeignKey(li => li.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AmPlan>()
                .ToTable("AmPlans")
                .HasKey(ap => ap.Id);

            modelBuilder.Entity<AmPlan>()
                .HasOne(ap => ap.LoanInput) 
                .WithMany(li => li.AmortizationPlans)
                .HasForeignKey(ap => ap.LoanInputID)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<AmPlan>()
                .HasOne(ap => ap.Product) 
                .WithMany() 
                .HasForeignKey(ap => ap.ProductID)
                .OnDelete(DeleteBehavior.Restrict); 

            // Seed Data
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

