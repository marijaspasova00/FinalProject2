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
using FinalProjectAmPlansForLoans.Services.Services.Implementations;

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
                TotalAmount = loanInput.Principal,
                PaymentFrequency = loanInput.PaymentFrequency,
                NoInstallment = loanInput.NumberOfInstallments,
                Principal = loanInput.Principal,
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
                            installmentAmount = CalculateYearlyPayment(loanInput.Principal, loanInput.InterestRate, loanInput.NumberOfInstallments);
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

        private decimal CalculateYearlyPayment(decimal principal, decimal annualRate, int numberOfInstallments)
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
            decimal remainingPrincipal = loanInput.Principal;

            DateTime installmentDate = loanInput.FirstInstallmentDate; 

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
                        var annualPlan = CalculateAnnualAmortizationPlan(loanInput.Principal, (double)loanInput.InterestRate, loanInput.NumberOfInstallments);
                        installmentAmount = annualPlan[i - 1].TotalAmount;
                        var annualEntry = annualPlan[i - 1];
                        amortizationPlans.Add(new AmPlan
                        {
                            LoanInputID = loanInput.Id,
                            NoInstallment = i,
                            TotalAmount = annualEntry.TotalAmount,
                            PaymentDate = installmentDate,
                            Principal = annualEntry.Principal,
                            Interest = annualEntry.Interest,
                            RemainingAmount = annualEntry.RemainingAmount,
                            PaymentFrequency = loanInput.PaymentFrequency,
                            ClosingDate = DateTime.MinValue, 
                        });
                        installmentDate = installmentDate.AddYears(1);
                        continue;

                    default:
                        throw new ArgumentException("Invalid payment frequency.");
                }

                decimal interestRatePerPeriod = loanInput.InterestRate / 100 / (loanInput.PaymentFrequency == PaymentFrequency.Monthly ? 12 : loanInput.PaymentFrequency == PaymentFrequency.Quarterly ? 4 : 1);
                decimal interestAmount = remainingPrincipal * interestRatePerPeriod;
                decimal principalAmount = installmentAmount - interestAmount;

                remainingPrincipal -= principalAmount;

                amortizationPlans.Add(new AmPlan
                {
                    LoanInputID = loanInput.Id,
                    NoInstallment = i,
                    TotalAmount = installmentAmount,
                    PaymentDate = installmentDate,
                    Principal = principalAmount,
                    Interest = interestAmount,
                    RemainingAmount = remainingPrincipal,
                    PaymentFrequency = loanInput.PaymentFrequency,
                    ClosingDate = DateTime.MinValue, 
                });

                installmentDate = loanInput.PaymentFrequency switch
                {
                    PaymentFrequency.Monthly => installmentDate.AddMonths(1),
                    PaymentFrequency.Quarterly => installmentDate.AddMonths(3),
                    PaymentFrequency.Yearly => installmentDate.AddYears(1),
                    _ => installmentDate
                };
            }

            var finalPaymentDate = amortizationPlans.Last().PaymentDate;
            foreach (var plan in amortizationPlans)
            {
                plan.ClosingDate = finalPaymentDate;
            }

            return _mapper.Map<IEnumerable<AmPlanViewModel>>(amortizationPlans);
        }

        // Annual plan calculation method
        public List<AmPlan> CalculateAnnualAmortizationPlan(decimal principal, double annualInterestRate, int totalYears)
        {
            List<AmPlan> amortizationPlan = new List<AmPlan>();
            decimal remainingBalance = principal;
            double r = annualInterestRate / 100;
            decimal annualPayment = principal * (decimal)(r / (1 - Math.Pow(1 + r, -totalYears)));

            for (int year = 1; year <= totalYears; year++)
            {
                decimal interest = remainingBalance * (decimal)r;
                decimal principalPortion = annualPayment - interest;

                remainingBalance -= principalPortion;

                amortizationPlan.Add(new AmPlan
                {
                    NoInstallment = year,
                    PaymentDate = DateTime.Now.AddYears(year),
                    TotalAmount = annualPayment,
                    Principal = principalPortion,
                    Interest = interest,
                    RemainingAmount = remainingBalance
                });
            }

            return amortizationPlan;
        }
        public async Task<decimal> GetAdminFeeByProductIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return (decimal)(product?.AdminFee ?? 0); 
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
