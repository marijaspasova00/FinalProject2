using AutoMapper;
using FinalProjectAmPlansForLoans.DataAccess.Repositories;
using FinalProjectAmPlansForLoans.Domain.Models;
using FinalProjectAmPlansForLoans.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.Services.Services.Implementations
{
    public class AmPlanService : IAmPlanService
    {
        private readonly IAmPlanRepository _amPlanRepository;
        private readonly IMapper _mapper;

        public AmPlanService(IAmPlanRepository amPlanRepository, IMapper mapper)
        {
            _amPlanRepository = amPlanRepository;
            _mapper = mapper;
        }

        public async Task AddAmortizationPlanAsync(AmPlanViewModel amortizationPlanViewModel)
        {
            var amortizationPlan = _mapper.Map<AmPlan>(amortizationPlanViewModel);
            await _amPlanRepository.AddAmPlanIntoDbAsync(amortizationPlan);
        }

        public async Task AddAmortizationPlansAsync(List<AmPlanViewModel> amortizationPlanViewModels)
        {
            var amortizationPlans = _mapper.Map<List<AmPlan>>(amortizationPlanViewModels);
            await _amPlanRepository.AddRangeAsync(amortizationPlans);
        }
    }

}
