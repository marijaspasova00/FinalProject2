using FinalProjectAmPlansForLoans.Domain.Models;
using FinalProjectAmPlansForLoans.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.Services.Services
{
    public interface IAmPlanService
    {
        Task AddAmortizationPlanAsync(AmPlanViewModel amortizationPlanViewModel);
        Task AddAmortizationPlansAsync(List<AmPlanViewModel> amortizationPlanViewModels);
    }
}
