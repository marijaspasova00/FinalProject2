﻿using Microsoft.AspNetCore.Mvc;
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
using FinalProjectAmPlansForLoans.DataAccess.Repositories;

public class LoanInputController : Controller
{
    private readonly AmPlansDbContext _context;
    private readonly ILoanInputService _loanInputService;
    private readonly IProductRepository _productRepository;
    private readonly IAmPlanService _amPlanService;

    public LoanInputController(AmPlansDbContext context, ILoanInputService loanInputService, IProductRepository productRepository, IAmPlanService amPlanService)
    {
        _context = context;
        _loanInputService = loanInputService;
        _productRepository = productRepository;
        _amPlanService = amPlanService;
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
        var selectedProduct = await _productRepository.GetByIdAsync(model.SelectedProductId);

        if (selectedProduct != null)
        {
            model.AdminFee = (decimal)selectedProduct.AdminFee;
        }
        var product = await _context.Products
            .Where(p => p.Id == model.SelectedProductId)
            .FirstOrDefaultAsync();

        ModelState.Clear();

        if (product != null)
        {
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

        if (!ModelState.IsValid)
        {
            await PopulateCombo(model);

            return View("Index", model);
        }

        var loanInput = new LoanInput
        {
            ProductID = model.SelectedProductId,
            Principal = model.Principal,
            InterestRate = model.InterestRate,
            NumberOfInstallments = model.NumberOfInstallments,
            PaymentFrequency = (PaymentFrequency)model.SelectedPaymentFrequency,
            AdminFee = (decimal)(product?.AdminFee ?? 0),
            FirstInstallmentDate = DateTime.Now,
            ClosingDate = DateTime.Now.AddMonths(model.NumberOfInstallments)
        };

        await _loanInputService.AddLoanInputAsync(loanInput);
        var amortizationPlans = await _loanInputService.GetAmortizationPlansByLoanInputAsync(loanInput.Id);

        if (amortizationPlans == null || !amortizationPlans.Any())
        {
            ModelState.AddModelError("", "No amortization plans were generated. Please check your inputs.");

            await PopulateCombo(model);

            return View("Index", model);
        }
        foreach (var amPlan in amortizationPlans)
        {
            amPlan.ProductID = loanInput.ProductID;
            await _amPlanService.AddAmortizationPlanAsync(amPlan);
        }

        model.AmortizationPlans = amortizationPlans;
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

    [HttpGet]
    public async Task<JsonResult> GetAdminFee(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return Json(new { adminFee = 0.00 });
        }
        return Json(new { adminFee = product.AdminFee });
    }

}
