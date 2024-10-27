using FinalProjectAmPlansForLoans.DataAccess.DataContext;
using FinalProjectAmPlansForLoans.DataAccess.Repositories.Implementations;
using FinalProjectAmPlansForLoans.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FinalProjectAmPlansForLoans.Services.Services;
using FinalProjectAmPlansForLoans.Services.Services.Implementations;
using AmortizationPlansForLoansFinalProject.Services.Services.Implementations;

namespace FinalProjectAmPlansForLoans.Helper
{
    public static class DependencyInjectionHelper
    {
        public static void InjectDbContext(this IServiceCollection services)
        {
            services.AddDbContext<AmPlansDbContext>(options =>
            options.UseSqlServer(@"Data Source=(localdb)\SEDCLocalDb;Database=FinalProjectAmPlasForLoans;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));
        }
        public static void InjectRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ILoanInputRepository, LoanInputRepository>();
            services.AddScoped<IAmPlanRepository, AmPlanRepository>();
        }

        public static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ILoanInputService, LoanInputService>();
            services.AddScoped<IAmPlanService, AmPlanService>();
        }
    }
}
