using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProjectAmPlansForLoans.DataAccess.DataContext;
using FinalProjectAmPlansForLoans.Domain.Enums;
using FinalProjectAmPlansForLoans.Domain.Models;
using FinalProjectAmPlansForLoans.Services.Services;
using FinalProjectAmPlansForLoans.ViewModels;

public class LoanInputController : Controller
{
    private readonly AmPlansDbContext _context;
    private readonly ILoanInputService _loanInputService;

    public LoanInputController(AmPlansDbContext context, ILoanInputService loanInputService)
    {
        _context = context;
        _loanInputService = loanInputService;
    }

    // GET: LoanInput
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

    // POST: LoanInput/CalculateAmortization
    [HttpPost]
    public async Task<IActionResult> CalculateAmortization(LoanInputViewModel model)
    {
        // Log all products to check what's in the database
        var products = await _context.Products.ToListAsync();
        foreach (var p in products)
        {
            Console.WriteLine($"Product ID: {p.Id}, Product Name: {p.ProductName}");
        }
        Console.WriteLine("Selected Product ID: " + model.ProductID); // Updated from SelectedProductId
        Console.WriteLine("Principal: " + model.Principal);
        Console.WriteLine("Interest Rate: " + model.InterestRate);
        Console.WriteLine("Number of Installments: " + model.NumberOfInstallments);

        // Validate ProductID before fetching the product
        if (model.ProductID <= 0) // Change SelectedProductId to ProductID
        {
            ModelState.AddModelError(nameof(model.ProductID), "Please select a valid product."); // Updated error message
            return await ReloadIndexView(model);
        }

        // Validate the selected product
        var product = await _context.Products.FindAsync(model.ProductID); // Updated from SelectedProductId
        if (product == null)
        {
            ModelState.AddModelError(nameof(model.ProductID), "Invalid product selected.");
            return await ReloadIndexView(model);
        }

        // Validate loan input
        await ValidateLoanInput(model, product);
        if (!ModelState.IsValid) // Uncommented to ensure validation before proceeding
        {
            return await ReloadIndexView(model);
        }

        // Create LoanInput object from ViewModel
        var loanInput = new LoanInput // Changed to LoanInput instead of LoanInputViewModel
        {
            ProductID = model.ProductID, // Ensure ProductID is set correctly
            AgreementDate = model.AgreementDate,
            Principal = model.Principal,
            InterestRate = model.InterestRate,
            PaymentFrequency = (PaymentFrequency)model.SelectedPaymentFrequency, // Assuming you have this in your ViewModel
            AdminFee = model.AdminFee,
            FirstInstallmentDate = model.FirstInstallmentDate,
            NumberOfInstallments = model.NumberOfInstallments,
            ClosingDate = model.ClosingDate
        };

        // Check if model is null
        if (loanInput == null) // Changed from model to loanInput
        {
            throw new ArgumentNullException(nameof(loanInput), "LoanInput is null");
        }

        // Add loan input and get amortization plans
        await _loanInputService.AddLoanInputAsync(loanInput);
        var amortizationPlans = await _loanInputService.GetAmortizationPlansByLoanInputAsync(loanInput.Id);

        if (amortizationPlans == null || !amortizationPlans.Any())
        {
            ModelState.AddModelError("", "No amortization plans were generated. Please check your inputs.");
        }

        model.AmortizationPlans = amortizationPlans; // Ensure this property exists in your ViewModel
        return View("Index", model);
    }


    private async Task<IActionResult> ReloadIndexView(LoanInputViewModel model)
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

        return View("Index", model);
    }

    private async Task ValidateLoanInput(LoanInputViewModel model, Product product)
    {

        // Check if the Product is null, which would indicate an invalid product ID
        if (product == null)
        {
            throw new ArgumentException("Invalid product ID.");
        }
        if (model.Principal < (decimal)product.MinAmount || model.Principal > (decimal)product.MaxAmount)
        {
            ModelState.AddModelError(nameof(model.Principal), $"Amount must be between {product.MinAmount} and {product.MaxAmount}.");
        }
        if (model.InterestRate < (decimal)product.MinInterestRate || model.InterestRate > (decimal)product.MaxInterestRate)
        {
            ModelState.AddModelError(nameof(model.InterestRate), $"Interest Rate must be between {product.MinInterestRate} and {product.MaxInterestRate}.");
        }
        if (model.NumberOfInstallments < product.MinNumberOfInstallments || model.NumberOfInstallments > product.MaxNumberOfInstallments)
        {
            ModelState.AddModelError(nameof(model.NumberOfInstallments), $"Number of Installments must be between {product.MinNumberOfInstallments} and {product.MaxNumberOfInstallments}.");
        }
    }
}
