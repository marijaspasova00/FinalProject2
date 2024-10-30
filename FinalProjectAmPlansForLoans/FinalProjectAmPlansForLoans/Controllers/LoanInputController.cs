using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AmortizationPlansForLoansFinalProject.Services.Services;
using FinalProjectAmPlansForLoans.DataAccess.DataContext;
using FinalProjectAmPlansForLoans.Domain.Enums;
using FinalProjectAmPlansForLoans.Domain.Models;
using FinalProjectAmPlansForLoans.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using FinalProjectAmPlansForLoans.Services.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class LoanInputController : Controller
{
    private readonly AmPlansDbContext _context;
    private readonly ILoanInputService _loanInputService;

    public LoanInputController(AmPlansDbContext context, ILoanInputService loanInputService)
    {
        _context = context;
        _loanInputService = loanInputService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new LoanInputViewModel
        {
            Products = await _context.Products.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.ProductName
            }).ToListAsync(),
            PaymentFrequencies = Enum.GetValues(typeof(PaymentFrequency))
                                      .Cast<PaymentFrequency>()
                                      .Select(pf => new SelectListItem
                                      {
                                          Value = ((int)pf).ToString(),
                                          Text = pf.ToString()
                                      }).ToList(),
            SelectedPaymentFrequency = (int)PaymentFrequency.Monthly
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CalculateAmortization(LoanInputViewModel model)
    {
        // Retrieve product details for validation
        var product = await _context.Products
            .Where(p => p.Id == model.SelectedProductId)
            .FirstOrDefaultAsync();

        ModelState.Clear();

        if (product != null)
        {
            // Validate Amount
            if (model.Principal < (decimal)product.MinAmount || model.Principal > (decimal)product.MaxAmount)
            {
                ModelState.AddModelError(nameof(model.Principal), $"Amount must be between {product.MinAmount} and {product.MaxAmount}.");
            }

            // Validate Interest Rate
            if (model.InterestRate < (decimal)product.MinInterestRate || model.InterestRate > (decimal)product.MaxInterestRate)
            {
                ModelState.AddModelError(nameof(model.InterestRate), $"Interest Rate must be between {product.MinInterestRate} and {product.MaxInterestRate}.");
            }

            // Validate Number of Installments
            if (model.NumberOfInstallments < product.MinNumberOfInstallments || model.NumberOfInstallments > product.MaxNumberOfInstallments)
            {
                ModelState.AddModelError(nameof(model.NumberOfInstallments), $"Number of Installments must be between {product.MinNumberOfInstallments} and {product.MaxNumberOfInstallments}.");
            }
        }

        // Check if ModelState is valid
        if (!ModelState.IsValid)
        {
            await PopulateCombo(model);

            return View("Index", model);
        }

        // Create and save loan input
        var loanInput = new LoanInput
        {
            ProductID = model.SelectedProductId,
            Principal = model.Principal,
            InterestRate = model.InterestRate,
            NumberOfInstallments = model.NumberOfInstallments,
            PaymentFrequency = (PaymentFrequency)model.SelectedPaymentFrequency,
            AdminFee = 0,
            FirstInstallmentDate = DateTime.Now,
            ClosingDate = DateTime.Now.AddMonths(model.NumberOfInstallments)
        };

        await _loanInputService.AddLoanInputAsync(loanInput);
        var amortizationPlans = await _loanInputService.GetAmortizationPlansByLoanInputAsync(loanInput.Id);

        // Check if amortization plans were generated
        if (amortizationPlans == null || !amortizationPlans.Any())
        {
            ModelState.AddModelError("", "No amortization plans were generated. Please check your inputs.");

            await PopulateCombo(model);

            return View("Index", model);
        }

        // Assign amortization plans to the model for display
        model.AmortizationPlans = amortizationPlans;

        await PopulateCombo(model);

        return View("Index", model);
    }

    private async Task PopulateCombo(LoanInputViewModel model)
    {
            model.Products = await _context.Products.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.ProductName
            }).ToListAsync();

        model.PaymentFrequencies = Enum.GetValues(typeof(PaymentFrequency))
                                      .Cast<PaymentFrequency>()
                                      .Select(pf => new SelectListItem
                                      {
                                          Value = ((int)pf).ToString(),
                                          Text = pf.ToString()
                                      }).ToList();
    }
}
