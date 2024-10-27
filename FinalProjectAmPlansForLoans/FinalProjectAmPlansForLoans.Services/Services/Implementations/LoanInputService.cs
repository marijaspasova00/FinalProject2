using AmortizationPlansForLoansFinalProject.Services.Services;
using AutoMapper;
using FinalProjectAmPlansForLoans.DataAccess.Repositories.Implementations;
using FinalProjectAmPlansForLoans.DataAccess.Repositories;
using FinalProjectAmPlansForLoans.Domain.Enums;
using FinalProjectAmPlansForLoans.Domain.Models;
using FinalProjectAmPlansForLoans.Services.Services;
using FinalProjectAmPlansForLoans.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinalProjectAmPlansForLoans.DataAccess.DataContext;

namespace AmortizationPlansForLoansFinalProject.Services.Services.Implementations
{
    public class LoanInputService : ILoanInputService
    {
        private readonly ILoanInputRepository _loanInputRepository;
        private readonly IAmPlanRepository _amortizationPlanRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly AmPlansDbContext _context;

        public LoanInputService(
            ILoanInputRepository loanInputRepository,
            IAmPlanRepository amortizationPlanRepository,
            IProductRepository productRepository,
            IMapper mapper, AmPlansDbContext context)
        {
            _loanInputRepository = loanInputRepository;
            _amortizationPlanRepository = amortizationPlanRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task AddLoanInputAsync(LoanInputViewModel loanInputViewModel)
        {
            var loanInput = _mapper.Map<LoanInput>(loanInputViewModel);
            await ValidateLoanInput(loanInput);
            await _loanInputRepository.AddLoanInputIntoDbAsync(loanInput);

            var amortizationPlan = CalculateAmortizationPlan(loanInput);
            await _amortizationPlanRepository.AddAmPlanIntoDbAsync(amortizationPlan);
        }

        private async Task ValidateLoanInput(LoanInput loanInput)
        {
            var product = await _productRepository.GetByIdAsync(loanInput.ProductID);

            if (product == null)
            {
                throw new ArgumentException("Invalid product ID.");
            }
            if (loanInput.Principal < (decimal)product.MinAmount || loanInput.Principal > (decimal)product.MaxAmount)
            {
                throw new ArgumentException($"Principal amount must be between {product.MinAmount} and {product.MaxAmount}.");
            }

            if (loanInput.InterestRate < (decimal)product.MinInterestRate || loanInput.InterestRate > (decimal)product.MaxInterestRate)
            {
                throw new ArgumentException($"Interest rate must be between {product.MinInterestRate} and {product.MaxInterestRate}.");
            }

            if (loanInput.NumberOfInstallments < product.MinNumberOfInstallments || loanInput.NumberOfInstallments > product.MaxNumberOfInstallments)
            {
                throw new ArgumentException($"Number of installments must be between {product.MinNumberOfInstallments} and {product.MaxNumberOfInstallments}.");
            }

            if (loanInput.AdminFee < (decimal)product.AdminFee)
            {
                throw new ArgumentException($"Admin fee must be at least {product.AdminFee}.");
            }

        }

        private AmPlan CalculateAmortizationPlan(LoanInput loanInput)
        {
            var amortizationPlan = new AmPlan
            {
                LoanInputID = loanInput.Id,
                TotalAmount = loanInput.Principal + loanInput.AdminFee,
                PaymentFrequency = loanInput.PaymentFrequency,
                NoInstallment = loanInput.NumberOfInstallments,
                Principal = loanInput.Principal,
                FirstInstallmentDate = loanInput.FirstInstallmentDate,
                ClosingDate = loanInput.ClosingDate,
                Installments = new List<decimal>()
            };

            for (int i = 1; i <= loanInput.NumberOfInstallments; i++)
            {
                decimal installmentAmount = 0;
                switch (loanInput.PaymentFrequency)
                {
                    case PaymentFrequency.Monthly:
                        installmentAmount = CalculateMonthlyPayment(loanInput.Principal, loanInput.InterestRate, loanInput.NumberOfInstallments);
                        break;
                    case PaymentFrequency.Quarterly:
                        installmentAmount = CalculateQuarterlyPayment(loanInput.Principal, loanInput.InterestRate, loanInput.NumberOfInstallments);
                        break;
                    case PaymentFrequency.Yearly:
                        installmentAmount = CalculateYearlyPayment(loanInput.Principal, loanInput.InterestRate);
                        break;
                    default:
                        throw new ArgumentException("Invalid payment frequency.");
                }

                amortizationPlan.Installments.Add(installmentAmount); 
            }

            amortizationPlan.Interest = amortizationPlan.Installments.Sum() - loanInput.Principal; 
            return amortizationPlan;
        }

        private decimal CalculateMonthlyPayment(decimal principal, decimal annualRate, int totalPayments)
        {
            decimal monthlyRate = annualRate / 100 / 12;
            return principal * monthlyRate / (1 - (decimal)Math.Pow((double)(1 + monthlyRate), -totalPayments));
        }

        private decimal CalculateQuarterlyPayment(decimal principal, decimal annualRate, int totalPayments)
        {
            decimal quarterlyRate = annualRate / 100 / 4;
            return principal * quarterlyRate / (1 - (decimal)Math.Pow((double)(1 + quarterlyRate), -totalPayments));
        }

        private decimal CalculateYearlyPayment(decimal principal, decimal annualRate)
        {
            decimal yearlyRate = annualRate / 100;
            return principal * yearlyRate;
        }

        public async Task<IEnumerable<AmPlanViewModel>> GetAmortizationPlansByLoanInputAsync(int loanInputId)
        {
            var loanInput = await _loanInputRepository.GetLoanInputByIdAsync(loanInputId); 
            if (loanInput == null)
                throw new ArgumentException("Invalid loan input ID.");

            var amortizationPlans = new List<AmPlan>();

            for (int i = 1; i <= loanInput.NumberOfInstallments; i++)
            {
                decimal installmentAmount = 0;
                DateTime installmentDate = loanInput.FirstInstallmentDate;

                switch (loanInput.PaymentFrequency)
                {
                    case PaymentFrequency.Monthly:
                        installmentAmount = CalculateMonthlyPayment(loanInput.Principal, loanInput.InterestRate, loanInput.NumberOfInstallments);
                        installmentDate = loanInput.FirstInstallmentDate.AddMonths(i - 1); // Monthly 
                        break;
                    case PaymentFrequency.Quarterly:
                        installmentAmount = CalculateQuarterlyPayment(loanInput.Principal, loanInput.InterestRate, loanInput.NumberOfInstallments);
                        installmentDate = loanInput.FirstInstallmentDate.AddMonths(3 * (i - 1)); // Quarterly 
                        break;
                    case PaymentFrequency.Yearly:
                        installmentAmount = CalculateYearlyPayment(loanInput.Principal, loanInput.InterestRate);
                        installmentDate = loanInput.FirstInstallmentDate.AddYears(i - 1); // Yearly 
                        break;
                    default:
                        throw new ArgumentException("Invalid payment frequency.");
                }

                var amortizationPlan = new AmPlan
                {
                    LoanInputID = loanInput.Id,
                    NoInstallment = i,
                    TotalAmount = installmentAmount,
                    PaymentDate = installmentDate,
                    Principal = loanInput.Principal,
                    Interest = loanInput.InterestRate,
                    PaymentFrequency = loanInput.PaymentFrequency,
                    DateFrom = loanInput.FirstInstallmentDate.Date,
                    DateTo = installmentDate,
                    ClosingDate = loanInput.ClosingDate,
                };

                amortizationPlans.Add(amortizationPlan);
            }


            return _mapper.Map<IEnumerable<AmPlanViewModel>>(amortizationPlans);
        }

        public async Task AddLoanInputAsync(LoanInput loanInput)
        {
            if (loanInput == null)
            {
                throw new ArgumentNullException(nameof(loanInput), "LoanInput cannot be null");
            }
            await _context.LoanInputs.AddAsync(loanInput);

            await _context.SaveChangesAsync();
        }
    }
}
